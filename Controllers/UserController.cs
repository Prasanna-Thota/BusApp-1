using BusApplication.Data;
using BusApplication.Models;
using BusApplication.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Linq;

namespace BusApplication.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("signup")]
        public ActionResult<UserDetailsEntity> CreateUser([FromBody] UserDetails user)
        {
            if (user == null)
            {
                return BadRequest("Invalid request data.");
            }

            // Check if username already exists
            var existingUser = _db.User_Details.FirstOrDefault(x => x.UserName == user.UserName);

            if (existingUser != null)
            {
                return BadRequest("Username is already in use.");
            }

            // Create new user details entity
            UserDetailsEntity userDetails = new UserDetailsEntity
            {
                UserName = user.UserName,
                Email = user.Email,
                City = user.City,
                Password = user.Password,
                TermsAccepted = user.TermsAccepted,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            // Attempt to save to database
            try
            {
                _db.User_Details.Add(userDetails);
                _db.SaveChanges();
                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user.");
            }
        }

        [HttpGet("signin")]
        public IActionResult SignIn([FromQuery] string username, [FromQuery] string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            var user = _db.User_Details.FirstOrDefault(u => u.UserName == username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Password != password)
            {
                return Unauthorized("Invalid password.");
            }

            // You might want to return a token or other user details here
            return Ok(new { Message = "Sign-in successful", Username = user.UserName });
        }

        [HttpGet("getDetails/{username}")] // Endpoint for fetching details by username
        public ActionResult GetBookingDetails(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be null or empty.");
            }

            try
            {
                var bookings = _db.Booking_Details
                   .Include(b => b.Contact) // Include ContactDetails
                   .Include(b => b.Passengers) // Include Passengers
                   .Where(b => b.UserName == username)
                   .ToList();

                if (bookings == null || bookings.Count == 0)
                {
                    return NotFound($"No bookings found for username '{username}'.");
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
                // Handle specific exceptions as needed
            }
        }

        [HttpPost]
        [Route("contact")]
        public IActionResult ContactUs([FromBody] ContactUsDetails details)
        {
            if (details == null)
            {
                return BadRequest("Invalid data.");
            }

            // Simulating saving data to a database or sending an email.
            // You should replace this with actual data handling logic.
            var d = new ContactUsDetailsEntity
            {
                Name = details.Name,
                Email = details.Email,
                Message = details.Message,
            };

            _db.ContactUs_Details.Add(d);
            _db.SaveChanges();


            // Return a success response.
            return Ok("Message Sent Successfully");
        }

        [HttpGet("getContact")]
        public ActionResult GetContactUsDetails()
        {
            try
            {
                var details = _db.ContactUs_Details.ToList(); // Assuming ContactUs_Details is a DbSet
                return Ok(details);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, ex.Message); // Return only the exception message in the response
            }
        }

        [HttpGet("getBookingDetails")]
        public ActionResult getBookingDetails()
        {
            try
            {
                var d = _db.Booking_Details
                    .Include(b => b.Contact)
                   .Include(b => b.Passengers)
                   .ToList();
                return Ok(d);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("getUser/{username}")]
        public ActionResult getUserDetails(string username)
        {
            try
            {
                var user = _db.User_Details.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("getallUsers")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _db.User_Details.ToList();

                if (users.Count == 0)
                {
                    return NotFound("No users found.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("updateUser/{username}")]
        public ActionResult UpdateUserDetails(string username, [FromBody] UserDetails updatedUser)
        {
            try
            {
                var user = _db.User_Details.FirstOrDefault(u => u.UserName == username);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                user.Id = user.Id;
                user.UserName = updatedUser.UserName;
                user.Email = updatedUser.Email;
                user.City = updatedUser.City;
                user.Password = updatedUser.Password;
                user.TermsAccepted = user.TermsAccepted;
                user.CreatedDate = user.CreatedDate;
                user.UpdatedDate = DateTime.Now;

                _db.SaveChanges();

                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }

        [HttpGet("verifyuser/{username}/{email}")]
        public ActionResult verifyUser(string username, string email)
        {
            try
            {
                var userByUsername = _db.User_Details.FirstOrDefault(u => u.UserName == username);
                var userByEmail = _db.User_Details.FirstOrDefault(u => u.Email == email);

                if (userByUsername == null)
                {
                    // Username is invalid
                    return StatusCode(404, "Username is invalid");
                }

                if (userByEmail == null)
                {
                    // Email is invalid
                    return StatusCode(400, "Email is invalid");
                }

                // Both username and email are valid, return the user details
                return Ok(userByUsername);
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
