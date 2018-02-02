using ButtonChallenge.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ButtonChallenge.BusinessRules
{
    /// <summary>
    /// Applies the changes to the entities.
    /// </summary>
    public class LoyaltyManager
    {
        private readonly UserContext _context;

        public LoyaltyManager(UserContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lists all users.
        /// </summary>
        /// <remarks>
        /// In reality with millions of users, there would be pagination.
        /// </remarks>
        /// <returns>Users</returns>
        public IEnumerable<User> EnumerateUsers()
        {
            return _context.Users;
        }

        /// <summary>
        /// Gets a user from its ID
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns>User entity</returns>
        public User GetUser(long id)
        {
            return _context.Users
                .Where((u) => u.UserId == id)
                .First();
        }

        /// <summary>
        /// Lists the transfers for a specified user ID.
        /// </summary>
        /// <remarks>
        /// In reality, there would be pagination.
        /// </remarks>
        /// <param name="userId">ID of the corresponding user</param>
        /// <returns>Corresponding transfers</returns>
        public IEnumerable<Transfer> EnumerateTransfers(long userId)
        {
            var user = _context.Users
                .Include((u) => u.Transfers)
                .Where((u) => u.UserId == userId)
                .First();
            return user.Transfers;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="email">User's email</param>
        /// <returns>User entity saved in the DB</returns>
        public User CreateUser(string firstName, string lastName, string email)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Points = 0
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        /// <summary>
        /// Adds or remove points. Uses optimistic concurrency to ensure
        /// transfers are not executed if there is not enough points.
        /// </summary>
        /// <remarks>
        /// I decided to throw exceptions if there is not enough points,
        /// that the caller will need to handle. The reason for this is
        /// there is many different reasons for rejecting transfers (e.g. user
        /// not found). If this method was returning success or failure, it would
        /// be less clear to the caller which case returns and which throws.
        /// </remarks>
        /// <param name="userId">User ID</param>
        /// <param name="transferAmount">Amount to add or substract</param>
        /// <returns>Transfer entity saved in the DB</returns>
        public Transfer TransferPoints(long userId, int transferAmount)
        {
            var transfer = new Transfer
            {
                Amount = transferAmount
            };

            bool success = false;
            int attempts = 3;
            do
            {
                var user = _context.Users
                    .Where(u => u.UserId == userId)
                    .Include(u => u.Transfers)
                    .First();
                user.Points += transferAmount;
                if (user.Points < 0)
                {
                    throw new InsufficientPointsException($"User with ID {userId} doesn't have enough points.");
                }
                user.Transfers.Add(transfer);
                try
                {
                    // Tries to save the change in points and the new transfer.
                    _context.SaveChanges();
                    success = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Reload the user information and try again if the user
                    // was updated on a concurrent thread.
                    attempts--;
                }
            } while (!success && attempts > 0);

            return transfer;
        }
    }
}