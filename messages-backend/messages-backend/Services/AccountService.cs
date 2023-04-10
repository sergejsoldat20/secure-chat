using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using messages_backend.Helpers;
using messages_backend.Models.DTO;
using messages_backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace messages_backend.Services
{
    public interface IAccountService
    {
        AuthResponse Authenticate(AuthenticateRequest request);
        RegisterEnum Register(RegisterRequest request);
    }
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly ICryptoService _cryptoService;

        public AccountService(ApplicationDbContext context, 
            IOptions<AppSettings> appSettings, 
            IMapper mapper, 
            ICryptoService cryptoService)
        {
            this._context = context;
            this._appSettings = appSettings.Value;
            this._mapper = mapper;
            this._cryptoService = cryptoService;
        }

        public AuthResponse Authenticate(AuthenticateRequest request)
        {
            var account = _context
                .Accounts
                .FirstOrDefault(x => x.Email == request.Email);

            if (account == null)
            {
                throw new AppException("This email doesn't exist!");
            }

            if (!BC.Verify(request.Password, account.PasswordHash))
            {
                throw new AppException("Password incorrect!");
            }

            string jwtToken = GenerateJwtToken(account);
            var response = new AuthResponse();
            response.JwtToken = jwtToken;
            
            return response;

        }

        public RegisterEnum Register(RegisterRequest request)
        {
            //check email
            if (_context.Accounts.Any(x => x.Email == request.Email))
            {
                return RegisterEnum.ExistingEmail;
            }

            Account account;

            try
            {
                account = _mapper.Map<Account>(request);
            }
            catch
            {
                return RegisterEnum.Error;
            }
            //first user is Admin
            var isFirstAccount = _context.Accounts.Count() == 0;
            account.Role = isFirstAccount ? Role.Admin : Role.User;
            account.CreatedAt = DateTime.UtcNow;

            //hash password
            account.PasswordHash = BC.HashPassword(request.Password);

            //generate RSA private keys
            account.MainRSAKey = _cryptoService.GenerateRSA();
            account.ComainRSAKey= _cryptoService.GenerateRSA();

            _context.Add(account);
            _context.SaveChanges();
            return RegisterEnum.Success;
        }

        private string GenerateJwtToken(Account account) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
