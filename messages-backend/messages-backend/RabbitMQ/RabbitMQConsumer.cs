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
		private readonly IConnection _connection1;
		private readonly IModel _channel1;
		private readonly IConnection _connection2;
		private readonly IModel _channel2;
		private readonly IConnection _connection3;
		private readonly IModel _channel3;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private Dictionary<Guid, List<MessagePartition>> receivedMessages = new Dictionary<Guid, List<MessagePartition>>();

		public RabbitMQConsumer(IServiceScopeFactory serviceScopeFactory)
		{
			var factory1 = new ConnectionFactory() 
			{
				HostName = "rabbitmq-1",
				Port = 5672,
				UserName = "sergej",
				Password = "NewPassword123"
			};
			_connection1 = factory1.CreateConnection();
			_channel1 = _connection1.CreateModel();
			_channel1.QueueDeclare(queue: "my_queue",
									durable: false,
									exclusive: false,
									autoDelete: false,
									arguments: null);

			var factory2 = new ConnectionFactory()
			{
				HostName = "rabbitmq-2",
				Port = 5672,
				UserName = "sergej",
				Password = "NewPassword123"
			};
			_connection2 = factory2.CreateConnection();
			_channel2 = _connection2.CreateModel();
			_channel2.QueueDeclare(queue: "my_queue",
									durable: false,
									exclusive: false,
									autoDelete: false,
									arguments: null);

			var factory3 = new ConnectionFactory()
			{
				HostName = "rabbitmq-3",
				Port = 5672,
				UserName = "sergej",
				Password = "NewPassword123"
			};
			_connection3 = factory3.CreateConnection();
			_channel3 = _connection3.CreateModel();
			_channel3.QueueDeclare(queue: "my_queue",
									durable: false,
									exclusive: false,
									autoDelete: false,
									arguments: null);
			_serviceScopeFactory = serviceScopeFactory;	
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var consumer1 = new EventingBasicConsumer(_channel1);
			consumer1.Received += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"Received message from queue1: {message}");
				MessagePartition partition = JsonConvert.DeserializeObject<MessagePartition>(message);
				if (partition == null)
					throw new AppException("Partition is not deserialized!");
				lock (receivedMessages)
				{
					if (receivedMessages.ContainsKey(partition.Id))
					{
						receivedMessages[partition.Id].Add(partition);
					}
					else
					{
						receivedMessages[partition.Id] = new List<MessagePartition>() { partition };
					}
				}
				checkReceivedMessages(partition);
			};
			_channel1.BasicConsume(queue: "my_queue",
									autoAck: true,
									consumer: consumer1);

			var consumer2 = new EventingBasicConsumer(_channel2);
			consumer2.Received += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"Received message from queue2: {message}");
				MessagePartition partition = JsonConvert.DeserializeObject<MessagePartition>(message);
				if (partition == null)
					throw new AppException("Partition is not deserialized!");
				lock (receivedMessages)
				{
					if (receivedMessages.ContainsKey(partition.Id))
					{
						receivedMessages[partition.Id].Add(partition);
					}
					else
					{
						receivedMessages[partition.Id] = new List<MessagePartition>() { partition };
					}
				}

				checkReceivedMessages(partition);
			};
			_channel2.BasicConsume(queue: "my_queue",
									autoAck: true,
									consumer: consumer2);

			var consumer3 = new EventingBasicConsumer(_channel3);
			consumer3.Received += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"Received message from queue3: {message}");
				MessagePartition partition = JsonConvert.DeserializeObject<MessagePartition>(message);
				if (partition == null)
					throw new AppException("Partition is not deserialized!");
				lock (receivedMessages)
				{
					if (receivedMessages.ContainsKey(partition.Id))
					{
						receivedMessages[partition.Id].Add(partition);
					}
					else
					{
						receivedMessages[partition.Id] = new List<MessagePartition>() { partition };
					}
				}
				checkReceivedMessages(partition);
			};
			_channel3.BasicConsume(queue: "my_queue",
									autoAck: true,
									consumer: consumer3);

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(1000, stoppingToken);
			}
		}

		public override void Dispose()
		{
			_channel1.Dispose();
			_connection1.Dispose();
			_channel2.Dispose();
			_connection2.Dispose();
			_channel3.Dispose();
			_connection3.Dispose();
			base.Dispose();
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