using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading.Channels;
using System.Text;
using messages_backend.Services;
using messages_backend.Data;
using messages_backend.Models.DTO;
using Newtonsoft.Json;
using messages_backend.Helpers;
using messages_backend.Models.Entities;

namespace messages_backend.RabbitMQ
{
	public class RabbitMQConsumer : BackgroundService
	{
		private readonly IEnumerable<IConnectionFactory> _connectionFactories;
	    private readonly IServiceScopeFactory _serviceScopeFactory;
		private Dictionary<Guid, List<MessagePartition>> receivedMessages = new Dictionary<Guid, List<MessagePartition>>();

		public RabbitMQConsumer(IEnumerable<IConnectionFactory> connectionFactories,
			IServiceScopeFactory serviceScopeFactory
			)
		{
			_connectionFactories = new List<IConnectionFactory>
			{
				new ConnectionFactory()
				{
					HostName = "rabbitmq-1",
					Port = 5672,
					UserName = "sergej",
					Password = "NewPassword123",
					DispatchConsumersAsync = true
				},
				new ConnectionFactory()
				{
					HostName = "rabbitmq-2",
					Port = 5672,
					UserName = "sergej",
					Password = "NewPassword123",
					DispatchConsumersAsync = true
				},
				new ConnectionFactory()
				{
					HostName = "rabbitmq-3",
					Port = 5672,
					UserName = "sergej",
					Password = "NewPassword123",
					DispatchConsumersAsync = true
				},
			};

		    _serviceScopeFactory = serviceScopeFactory;	
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			foreach (var factory in _connectionFactories)
			{
				using (var connection = factory.CreateConnection())
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(queue: "my_queue",
										 durable: false,
										 exclusive: false,
										 autoDelete: false,
										 arguments: null);

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += async (model, ea) =>
					{
						var body = ea.Body.ToArray();
						var message = System.Text.Encoding.UTF8.GetString(body);
						MessagePartition partition = JsonConvert.DeserializeObject<MessagePartition>(message);
						if (partition == null)
							throw new AppException("Partition is not deserialized!");
						if (receivedMessages.ContainsKey(partition.Id))
						{
							receivedMessages[partition.Id].Add(partition);
						}
						else 
						{
							receivedMessages[partition.Id] = new List<MessagePartition>() { partition };
						}	

						checkReceivedMessages(partition);


					};
					channel.BasicConsume(queue: "my_queue",
										 autoAck: true,
										 consumer: consumer);

					while (!stoppingToken.IsCancellationRequested)
					{
						await Task.Delay(1000, stoppingToken);
					}
				}
			}
		}

		private void checkReceivedMessages(MessagePartition partition)
		{
			
			int totalParts = partition.TotalParts;
			if (receivedMessages[partition.Id].Count == totalParts)
			{
				using (var scope = _serviceScopeFactory.CreateScope())
				{
					var messagesService = scope.ServiceProvider.GetRequiredService<IMessagesService>();
					Message message = messagesService.ComposeMessage(receivedMessages[partition.Id], partition.ReceiverId, partition.SenderId);
				    messagesService.SaveMessage(message);
				}
			}
		}
	}
}