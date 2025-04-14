using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuestCrafter.DTO;
using QuestCrafter.Models;

namespace QuestCrafter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        public readonly UserManager<User> _userManager;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;
        public AuthController(UserManager<User> userManager, IConfiguration configuration, IMapper mapper) {_userManager = userManager; _configuration = configuration; _mapper = mapper;}


        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(CreateUserDTO userDTO)
        {
            if (!ModelState.IsValid) {return BadRequest(modelState: ModelState);}
            
            var user = _mapper.Map<User>(userDTO);

            var result = await _userManager.CreateAsync(user, userDTO.Password);

            if (result.Succeeded)
            {
                return Ok( new {msg = "Account Created", UserId = user.Id});
            }
            return BadRequest(result.Errors);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid) {return BadRequest(ModelState);}

            // var user = !string.IsNullOrEmpty(model.username)
            //     ? await _userManager.FindByNameAsync(model.username)
            //     : await _userManager.FindByEmailAsync(model.email);

            var user = !string.IsNullOrEmpty(model.username)
                ? await _userManager.FindByNameAsync(model.username)
                : !string.IsNullOrEmpty(model.email) ?
                    await _userManager.FindByEmailAsync(model.email)
                    : null;
            
            if (user == null || ! await _userManager.CheckPasswordAsync(user, model.password)) {return Unauthorized("Invalid credentials");}
            var token = jwtGenerator(user);
            return Ok( new {msg = "you can log in with this token", token = token});
        }



        private string jwtGenerator(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? throw new InvalidOperationException("User ID is null")),
                new Claim(ClaimTypes.Name, user.UserName ?? throw new InvalidOperationException("Username is null"))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"] ?? throw new ArgumentNullException("JWT Key is missing in config")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddHours(1.9)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}