using boxtureAssignment.DAL;
using boxtureAssignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace boxtureAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly AppDbContext _context;
        public LoginController(IConfiguration configuration, AppDbContext context)
        {
            _config = configuration;
            _context = context;
        }

        #region AuthenticateUser
        private UserAuth AuthenticateUser(inputsAuth userAuth)
        {
            UserAuth _userAuth = null;
            var user = _context.Users.Where(user => user.username == userAuth.userAuth && user.password == userAuth.passAuth).FirstOrDefault();
            if (user != null)
            {
                _userAuth = new UserAuth { userAuth = user.username, userid = user.Id };
            }
            return _userAuth;
        }
        #endregion

        #region TokenGeneration
        private string GenerateToken(UserAuth userAuth)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userAuth.userAuth),
                    new Claim(ClaimTypes.Sid, userAuth.userid.ToString()),
                    new Claim(ClaimTypes.Role, "User"),

            };
            var user = _context.Users.Where(user => user.username == userAuth.userAuth).First();

            if (user.isAdmin)
            {
                claims = new List<Claim> {
                     new Claim(ClaimTypes.Name,userAuth.userAuth),
                     new Claim(ClaimTypes.Sid,userAuth.userid.ToString()),
                     new Claim(ClaimTypes.Role, "Admin"),
                     new Claim(ClaimTypes.Role, "User"),
                  };
            }
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region LoginUser
        [AllowAnonymous]
        [HttpPost]
        public IActionResult LoginUser(inputsAuth auth)
        {
            IActionResult response = Unauthorized();
            var userAuth = AuthenticateUser(auth);
            if (userAuth != null)
            {
                var token = GenerateToken(userAuth);
                response = Ok(new { token = token });
            }
            return response;
        } 
        #endregion

    }
}
