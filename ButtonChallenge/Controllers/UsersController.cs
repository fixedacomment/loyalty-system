using ButtonChallenge.BusinessRules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ButtonChallenge.Controllers
{
    /// <summary>
    /// Controller for both users and transactions.
    /// </summary>
    /// <remarks>
    /// Right now, transactions are accessible through a user
    /// (e.g. api/v1/users/1/transactions). To illustrate this
    /// more clearly, I decided to keep both on the same controller.
    /// We might want to separate them later on if there starts
    /// to be many more endpoints, for better readability.
    /// 
    /// There is no security at all, which would be pretty nice
    /// to have in reality.
    /// 
    /// There is a version added to the endpoint path, simply to
    /// illustrate that there could be a story around versioning of
    /// the API. There are many solutions for versioning, but having
    /// it in the URL is quick and fairly standard.
    /// </remarks>
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly LoyaltyManager _database;

        public UsersController(LoyaltyManager database)
        {
            _database = database;
        }

        /// <summary>
        /// List all users.
        /// </summary>
        /// <returns>All users</returns>
        // GET api/v1/users
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_database
                .EnumerateUsers()
                .Select(u => new
                {
                    Id = u.UserId,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Points
                }));
        }

        /// <summary>
        /// Get a specific user.
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>User with matching ID</returns>
        // GET api/v1/users/5
        [HttpGet("{id}")]
        public IActionResult GetUser(long id)
        {
            try
            {
                var user = _database.GetUser(id);
                return Ok(new
                {
                    Id = user.UserId,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Points
                });
            }
            catch (InvalidOperationException)
            {
                return NotFound("User not found.");
            }
        }

        /// <summary>
        /// Get all transfers for a user ID.
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>All transfers for corresponding user</returns>
        // GET api/v1/users/5/transfers
        [HttpGet("{id}/transfers")]
        public IActionResult GetTransfers(long id)
        {
            try
            {
                return Ok(_database
                    .EnumerateTransfers(id)
                    .Select(t => new
                    {
                        Id = t.TransferId,
                        t.UserId,
                        t.Amount
                    }));
            }
            catch(InvalidOperationException)
            {
                return NotFound("User not found.");
            }
        }
        
        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="userInfo">Information for a new user</param>
        /// <returns>Information for the new user</returns>
        // POST api/v1/users
        [HttpPost]
        public object PostUser([FromBody] JObject userInfo)
        {
            if(!IsValidString(userInfo["firstName"])
                || !IsValidString(userInfo["lastName"])
                || !IsValidEmail(userInfo["email"]))
            {
                return BadRequest("Invalid information for a new user.");
            }
            string firstName = userInfo["firstName"].ToString();
            string lastName = userInfo["lastName"].ToString();
            string email = userInfo["email"].ToString();

            var user = _database.CreateUser(firstName, lastName, email);
            return Ok(new
            {
                Id = user.UserId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Points
            });
        }

        /// <summary>
        /// Add a transfer to a user.
        /// </summary>
        /// <param name="id">ID of user to whom points are added or substracted</param>
        /// <param name="transferInfo">Information for a new transfer</param>
        /// <returns>New transfer</returns>
        // POST api/v1/users/5/transfers
        [HttpPost("{id}/transfers")]
        public object PostTransfer(long id, [FromBody] JObject transferInfo)
        {
            if (!IsValidInt(transferInfo["amount"]))
            {
                return BadRequest("Invalid information for a new transaction.");
            }
            int transferAmount = transferInfo["amount"].ToObject<int>();
            try
            {
                var transfer = _database.TransferPoints(id, transferAmount);
                return new
                {
                    Id = transfer.TransferId,
                    transfer.UserId,
                    transfer.Amount
                };
            }
            catch(InsufficientPointsException)
            {
                return BadRequest("There is not enough points for this user.");
            }
            catch (InvalidOperationException)
            {
                return NotFound("User not found.");
            }
        }

        /// <summary>
        /// Verifies if a JSON token is an int.
        /// </summary>
        /// <param name="value">JSON token</param>
        /// <returns>True if it is a string</returns>
        private bool IsValidInt(JToken value)
        {
            return value != null && value.Type == JTokenType.Integer;
        }

        /// <summary>
        /// Verifies if a JSON token is a string.
        /// </summary>
        /// <param name="value">JSON token</param>
        /// <returns>True if it is a string</returns>
        private bool IsValidString(JToken value)
        {
            return value != null && value.Type == JTokenType.String && !string.IsNullOrWhiteSpace(value.ToString());
        }

        /// <summary>
        /// Verifies if a JSON token is an email.
        /// </summary>
        /// <remarks>
        /// This is pretend, validating an email is hard.
        /// </remarks>
        /// <param name="value">JSON token</param>
        /// <returns>True if it is an email</returns>
        private bool IsValidEmail(JToken value)
        {
            return IsValidString(value) && value.ToString().Contains("@");
        }
    }
}
