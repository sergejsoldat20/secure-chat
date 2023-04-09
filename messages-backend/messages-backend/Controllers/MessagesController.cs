
using messages_backend.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace messages_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : BaseController
    {

        [HttpGet("messages")]
        public List<string> GetMessages() 
        {
            var list = new List<string>();
            list.Add("prva poruka");
            list.Add("druga poruka");
            return list;
        }

    }
}
