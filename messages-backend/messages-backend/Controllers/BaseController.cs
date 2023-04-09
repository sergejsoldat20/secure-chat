using messages_backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace messages_backend.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];
    }
}
