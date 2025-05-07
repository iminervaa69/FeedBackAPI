
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using FeedBack.Data.Models;

namespace FeedBack.Controllers
{
    [Route("feedback-api/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FeedBackContext _context;

        public UserController(FeedBackContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult GetAll(string title = "", int page = 1, int size = 10)
        {
            var query = _context.users
                   .Where(p => string.IsNullOrEmpty(title) || p.Name.Contains(title));

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);
            var response = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(m => new
                {
                    id = m.Id,
                    username = m.Username,
                    name = m.Name,
                    email = m.Email,
                    role = m.Role,
                }).ToList();

            return Ok(new
            {
                data = response.Take(size),
                pagination = new
                {
                    currentPage = page,
                    pageSize = size,
                    totalItems,
                    totalPages
                }
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId}")]
        public ActionResult getId(int userId)
        {
            var data = _context.users.FirstOrDefault(i => i.Id.Equals(userId) && i.DeletedAt.Equals(null));

            if (data.Equals(null))
                return NotFound(new { message = "User not found." });

            return Ok( new {
                id = data.Id,
                username = data.Username,
                name = data.Name,
                email = data.Email,
                role = data.Role,
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("makeAdmin/{userId}")]
        public ActionResult makeAdmin(int userId)
        {
            var data = _context.users.FirstOrDefault(i => i.Id.Equals(userId) && i.DeletedAt.Equals(null));

            if (data.Equals(null))
                return NotFound(new { message = "User not found." });

            data.Role = "Admin";
            data.UpdatedAt = DateTime.Now;

            _context.Update(data);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User now an admin.",
                data = new
                {
                    id = data.Id,
                    username = data.Username,
                    name = data.Name,
                    email = data.Email,
                    role = data.Role,
                }
            });
        }

        private string hash(string pass)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(pass);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult post(ReqAddUser req)
        {
            var data = _context.users.FirstOrDefault(i => i.Email.Equals(req.email) && i.DeletedAt.Equals(null));

            if (!data.Equals(null))
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Invalid email add address(Already exist).",
                    user_id = data.Id
                });

            var add = new User
            {
                Name = req.fullname,
                Username = req.username,
                Email = req.email,
                PasswordHash = hash(req.password),
                Role = req.role.ToString()
            };

            _context.users.Add(add);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User successfully added.",
                created_at = DateTime.Now,
                data = new
                {
                    id = data.Id,
                    username = data.Username,
                    name = data.Name,
                    email = data.Email,
                    role = data.Role,
                },
            });
        }

        [Authorize(Roles = "Admin")]
        [Route("update/{userId}")]
        [HttpPost]
        public ActionResult put(ReqUpdateUser req, int userId)
        {
            var data = _context.users.FirstOrDefault(i => i.Id.Equals(userId) && i.DeletedAt.Equals(null));

            var email = _context.users.FirstOrDefault(i => i.Email.Equals(req.email) && i.DeletedAt.Equals(null));

            if (data.Equals(null))
                return NotFound(new { message = "User not found." });

            if (!email.Equals(null))
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Invalid email add address(Already exist).",
                    user_id = data.Id
                });

            data.Name = req.fullname;
            data.Username = req.username;
            data.Email = req.email;
            data.PasswordHash = hash(req.password);
            data.Role = req.role.ToString();
            data.UpdatedAt = DateTime.Now;

            _context.users.Update(data);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User sussecfully edited.",
                at = data.UpdatedAt,
                data = new
                {
                    id = data.Id,
                    username = data.Username,
                    name = data.Name,
                    email = data.Email,
                    role = data.Role,
                }
            });
        }

        [Authorize(Roles = "Admin")]
        [Route("delete/{userId}")]
        [HttpPost]
        public ActionResult Delete(ReqUpdateUser req, int userId)
        {
            var data = _context.users.FirstOrDefault(i => i.Id == userId && i.DeletedAt.Equals(null));

            if (data == null || data.DeletedAt != null)
                return NotFound(new { message = "User not found or already deleted." });

            data.DeletedAt = DateTime.Now;
            _context.users.Update(data);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User successfully deleted.",
                at = data.DeletedAt
            });
        }
    }
}
