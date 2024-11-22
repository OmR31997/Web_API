using EntertaimentLib_API.Models;
using EntertaimentLib_API.Services;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Web_API_UNET.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserRecordController : ControllerBase
    {
        private FirebaseService _context;
        public UserRecordController(FirebaseService context)
        {
            _context = context;
        }


        [HttpGet("users")]
        public async Task<List<UserRecord>> GetUsers()
        {
            List<UserRecord> users = await _context.GetUsersAsy("users");
            return users;
        }

        [HttpGet("user/E-{Email}")]
        public async Task<IActionResult> GetUserByEmail(string Email)
        {
            var users = await _context.GetUsersAsy("users");
            var user = users.Find(user => user.Email == Email);

            if (user?.UserId != null)
            {
                return Ok(user);
            }
            else
                return NotFound(new { user });
        }

        [HttpGet("user/M-{MNo}")]
        public async Task<IActionResult> GetEmail(string MNo)
        {
            var users = await _context.GetUsersAsy("users");
            var user = users.Find(u => u.MNo == MNo);

            if (user != null && user.Email != null)
            {
                // Return the email in a valid JSON object
                return Ok(new { email = user.Email });
            }

            // Return an empty email as a JSON object if no user found or email is null
            return Ok(new { email = "" });
        }


        [HttpGet("user/U-{UserId}/P-{UserPsw}")]
        public async Task<IActionResult> GetUserByUserId(string UserId, string UserPsw)
        {
            var password = await _context.GetPassword($"users/{UserId}");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(UserPsw, password);
            return Ok(isPasswordValid);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogData loginReq)
        {
            var users = await _context.GetSecured("users");

            // Find user by email or mobile number
            var user = users.FirstOrDefault(u => u.Email == loginReq.EmailOrMobile || u.MNo == loginReq.EmailOrMobile);
            
            if (user == null)
                return NotFound("User not found");

            if(user.Password == null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword("");
            }

            // Verify the password using bcrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginReq.Password, user.Password);
            if (isPasswordValid)
            {
                user.Password = user.Password != null ? "HasPassword" : "Pending";
                // Password is correct, proceed with login

            }
            else if(!isPasswordValid && loginReq.Password != null)
            {
                //Password is Set here into Realtime Database when the Firebase Authetication password  has been changed
                await _context.UserPasswordAsy($"users/{user.UserId}", loginReq.Password);
            }
            return Ok(user);
        }


        [HttpPost("user")]
        public async Task<IActionResult> PostUser([FromBody] UserRecord newUser)
        {
            string path = "users";
            var lastUserId = await _context.GetLastUser(path);
            string fLastUserId = lastUserId.Replace("UENT", "");

            if (newUser.UserId != null)
            {
                return BadRequest(new { message = "In UserId, no need to provide anything" });
            }

            int UserSeries = 1;
            if (int.TryParse(fLastUserId, out int Uid))
            {
                UserSeries = Uid + 1;
            }
            var customId = $"UENT{UserSeries:D3}";
            var result = await _context.UserPostAsy(path, newUser, customId);
            return Ok(result);  // Return the custom user ID along with the message
        }

        [HttpPatch("user/{UserId}/WatchList/{WatchId}")]
        public async Task<IActionResult> PatchWatchList(string UserId, string WatchId, [FromBody] WatchList fav)
        {
            return Ok(new {Message = await _context.WatchListPatchAsy($"users/{UserId}/WatchList/{WatchId}", fav)});
        }

        [HttpPut("user/{UserId}")]
        public async Task<IActionResult> PutUser(string UserId, [FromBody] UserRecord updUser)
        {
            return Ok(new { Message = await _context.UserPutAsy($"users/{UserId}", updUser)});
        }


        [HttpDelete("{UserId}")]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            string path = "users";
            var users = await _context.GetUsersAsy(path);

            var user = users.Find(result => result.UserId == UserId);
            if (user != null)
            {
                await _context.UserDeleteAsy(path, UserId);
                return Ok($"User has been deleted");
            }
            return BadRequest("Unfortutely data not found");

        }
    }
}
