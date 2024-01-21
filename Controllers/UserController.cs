using boxtureAssignment.Controllers.Entity;
using boxtureAssignment.DAL;
using boxtureAssignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace boxtureAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        #region GetAllUsers
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllusers()
        {
            try
            {
                var users = _context.Users.ToList();
                if (users.Count == 0)
                {
                    return NotFound("Users Not available.");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetUserDetails
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        [Authorize(Roles = "User")]
        public IActionResult GetUserDetails(Guid id)
        {
            if (!IsValidUuid(id))
            {
                return StatusCode(400, "Invalid userId. Please provide a valid UUID.");
            }
            try
            {
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return NotFound($"User Details Not Found with id {id}.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        private bool IsValidUuid(Guid uuid)
        {
            // Use a regular expression to check if the string representation of the Guid is a valid UUID
            string uuidPattern = @"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$";
            return Regex.IsMatch(uuid.ToString(), uuidPattern);
        }
        #endregion

        #region AddNewUser
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult PostUser(userEntity model)
        {
            if (model.age == null || model.age <= 0)
            {
                return BadRequest(new { message = "Invalid Age Please provide a valid Age." });
            }
            if (model.username == null || model.password == null || model.isAdmin == null || model.age == null)
            {
                return StatusCode(400, "Please fill all the required fields");
            }
            try
            {
                User user = new User();
                user.username = model.username;
                user.password = model.password;
                user.isAdmin = model.isAdmin;
                user.age = model.age;
                user.hobbies = model.hobbies.Length > 0 ? string.Join(",", model.hobbies) : "";
                _context.Add(user);
                _context.SaveChanges();
                return StatusCode(201, "New User Created.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region UpdateUserDetails
        [HttpPut]
        [Authorize(Roles = "Admin,User")]
        [Authorize(Roles = "User")]
        public IActionResult PutUser(userEntity model)
        {
            if (model == null || model.Id == null)
            {
                return BadRequest("User data is invalid");
            }
            else if (!IsValidUuid(model.Id))
            {
                return BadRequest(new { message = "Invalid userId. Please provide a valid UUID." });
            }
            try
            {
                var user = _context.Users.Find(model.Id);
                if (user == null)
                {
                    return NotFound($"User not found with id {model.Id}.");
                }

                var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var sid = HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
                Guid originalId = new Guid(sid);

                if (checkIsAdmin(originalId) || originalId == model.Id)
                {
                    user.username = model.username;
                    user.password = model.password;
                    user.isAdmin = model.isAdmin;
                    user.age = model.age;
                    user.hobbies = model.hobbies.Length > 0 ? string.Join(",", model.hobbies) : "";
                    _context.SaveChanges();
                    return Ok("User Details updated successfully.");
                }
                return StatusCode(405, "Not Allowed to perform this operation.");

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet("userinfo")]
        public IActionResult GetUserInfo()
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var sid = HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
            return Ok(new { UserId = new Guid(sid), UserName = userName });
        }
        #endregion

        #region DeleteUser
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            if (!IsValidUuid(id))
            {
                return BadRequest(new { message = "Invalid userId. Please provide a valid UUID." });
            }
            try
            {
                var user = _context.Users.Find(id);
                {
                    if (user == null)
                    {
                        return NotFound($"User not found with id {id}.");
                    }
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return StatusCode(204, "User Record found and Deleted Successfully.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region checkIsadmin
        private bool checkIsAdmin(Guid id)
        {
            if (_context.Users.Any(user => user.Id == id && user.isAdmin == true))
            {
                return true;
            }
            return false;
        } 
        #endregion

    }
}
