
using messages_backend.Helpers;
using messages_backend.Models.DTO;
using messages_backend.Models.Entities;
using messages_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace messages_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : BaseController
    {

        private readonly IMessagesService _messagesService;
        private readonly ICryptoService _cryptoService;

        public MessagesController(IMessagesService messagesService, 
            ICryptoService cryptoService)
        {
            _messagesService = messagesService;
            _cryptoService= cryptoService;
        }

        [HttpPost("send-message")]
        public IActionResult SendMessage(SendMessage payload) 
        {
            // message will be divided and encrypted with RSA
           
            
            // todo: send this parts to M servers
            return Ok();
        }

        [HttpGet("test-divide")]
        public ActionResult<List<MessagePartition>> DivideMessage()
        {
            return Ok(_messagesService.DivideAndEncrypt("test chat message", Account.Id));
        }


        [HttpGet("test-compose")]
        public ActionResult<string> ComposeMessage() 
        {
            return _messagesService
                .ComposeMessage(_messagesService.
                DivideMessage("test chat message"), 
                new Guid(), new Guid()).Text;
        }

        [HttpGet("messages-for-chat/{id}")]
        public ActionResult<List<Message>> MessagesForChat(Guid id)
        {
            // todo: get messages for chat by user id
            return Ok();
        }

        [HttpPost("one-message")]
        public ActionResult<Message> TestMessage(SendMessage payload)
        {
            var divided = _messagesService.DivideAndEncrypt("jebem vam majku svima pocevsi od ....", payload.ReceiverId);
            return Ok(_messagesService.ComposeMessage(divided, payload.ReceiverId, Account.Id));
        }
       
    }
}
