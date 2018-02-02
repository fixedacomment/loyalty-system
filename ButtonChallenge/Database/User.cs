using System.Collections.Generic;

namespace ButtonChallenge.Database
{
    /// <summary>
    /// User entity in the DB.
    /// </summary>
    public class User
    {
        public long UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public List<Transfer> Transfers { get; set; }

        public int Points { get; set; }

        /// <summary>
        /// Concurrency token for the entity. If a change was made on a concurrent
        /// thread, EF will throw an exception if we try to save over.
        /// </summary>
        public byte[] RowVersion { get; set; }
    }
}