namespace ButtonChallenge.Database
{
    /// <summary>
    /// Transfer entity in the DB.
    /// </summary>
    /// <remarks>
    /// There is no concurrency token, because transfers are never modified.
    /// </remarks>
    public class Transfer
    {
        public long TransferId { get; set; }

        public long UserId { get; set; }

        public int Amount { get; set; }
    }
}