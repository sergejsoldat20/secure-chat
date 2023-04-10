using AutoMapper;
using messages_backend.Data;
using messages_backend.Models.DTO;
using messages_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace messages_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AccountController(IAccountService accountService,
            IMapper mapper, ApplicationDbContext context)
        {
            _accountService = accountService;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthResponse>> Authenticate(AuthenticateRequest request)
        {
            var response =  _accountService.Authenticate(request);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = _accountService.Register(request);
            if (response == RegisterEnum.Success)
            {
                return Ok(new { message = "User registered successfuly." });
            }
            else if (response == RegisterEnum.ExistingEmail)
            {
                return BadRequest(new { message = "Email already exists!" });
            }
            else
            {
                return BadRequest(new { message = "Error" });
            }
        }
    }
}
