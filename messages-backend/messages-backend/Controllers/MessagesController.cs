
using messages_backend.Data;
using messages_backend.Helpers;
using messages_backend.Models.DTO;
using messages_backend.Models.Entities;
using messages_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace messages_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : BaseController
    {
		private readonly IEnumerable<IConnectionFactory> _connectionFactories;
		private readonly IMessagesService _messagesService;
        private readonly ICryptoService _cryptoService;
        private readonly Random _random;
        private readonly ApplicationDbContext _context;


        public MessagesController(IMessagesService messagesService,
            ICryptoService cryptoService,
            IEnumerable<IConnectionFactory> connectionFactories,
            ApplicationDbContext context
            )
        {
            _messagesService = messagesService;
            _cryptoService = cryptoService;
            _connectionFactories = connectionFactories;
            _random = new Random();   
            _context = context;
        }

        [HttpGet("all-messages")]
        public ActionResult<List<Message>> GetAll()
        {
            var result = _context.Message.ToList();
            return Ok(result);
        }

        [HttpPost("send-message")]
        public IActionResult SendMessage(SendMessage payload)
        {
            // message will be divided and encrypted with RSA
            var messageParts = _messagesService.DivideAndEncrypt(payload.Text, payload.ReceiverId, Account.Id);

            foreach (var part in messageParts)
            {
				// choose one random connection to send message
				var factory = _connectionFactories.ElementAt(_random.Next(_connectionFactories.Count()));


				using var connection = factory.CreateConnection();
				using var channel = connection.CreateModel();

				channel.QueueDeclare(queue: "my_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
				var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(part));

                channel.BasicPublish(exchange: "", routingKey: "my_queue", basicProperties: null, body: body);
            }
			// todo: send this parts to M servers
			return Ok();
        }

        [HttpGet("chat-messages/{id}")]
        public ActionResult<List<Message>> GetChatMessages(Guid id)
        {
            var result = _context
                .Message
                .Where(x => x.SenderId == Account.Id && x.RecieverId == id
                || x.SenderId == id && x.RecieverId == Account.Id)
                .OrderBy(x => x.CreatedAt)
                .ToList();
            return Ok(result);
        }

        [HttpGet("number-of-messages/{id}")]
        public ActionResult<int> GetNumberOfMessages(Guid id)
        {
            var result = _context
                .Message
				.Where(x => x.SenderId == Account.Id && x.RecieverId == id)
                .ToList().Count();  
            return Ok(result);
		}
	}
}
