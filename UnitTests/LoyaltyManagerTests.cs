using ButtonChallenge.BusinessRules;
using ButtonChallenge.Controllers;
using ButtonChallenge.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class LoyaltyManagerTests
    {
        private UserContext _context;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "buttonchallenge")
                .Options;
            _context = new UserContext(options);
        }

        [TestMethod]
        public void GetUsersTest()
        {
            var loyalty = new LoyaltyManager(_context);

            var users = loyalty.EnumerateUsers();
            Assert.AreEqual(0, users.Count());

            loyalty.CreateUser("George", "Washington", "george.washington@abc.def");

            users = loyalty.EnumerateUsers();
            Assert.AreEqual(1, users.Count());
        }

        [TestMethod]
        public void GetSpecificUserTest()
        {
            var loyalty = new LoyaltyManager(_context);
            var createdUser = loyalty.CreateUser("George", "Bush", "george.bush@abc.def");

            var queriedUser = loyalty.GetUser(createdUser.UserId);
            Assert.AreEqual("George", queriedUser.FirstName);
            Assert.AreEqual("Bush", queriedUser.LastName);
            Assert.AreEqual("george.bush@abc.def", queriedUser.Email);
        }

        [TestMethod]
        public void TransferPointsTest()
        {
            var loyalty = new LoyaltyManager(_context);
            var user = loyalty.CreateUser("George", "Orwell", "george.orwell@abc.def");
            Assert.AreEqual(0, user.Points);

            var transfer = loyalty.TransferPoints(user.UserId, 10);
            var queriedUser = loyalty.GetUser(user.UserId);
            Assert.AreEqual(10, user.Points);
            Assert.AreEqual(10, transfer.Amount);

            var queriedTransfer = loyalty.EnumerateTransfers(user.UserId).Last();
            Assert.AreEqual(transfer.TransferId, queriedTransfer.TransferId);
            Assert.AreEqual(10, queriedTransfer.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientPointsException))]
        public void TransferPointsInsufficientPointsTest()
        {
            var loyalty = new LoyaltyManager(_context);
            var user = loyalty.CreateUser("George", "Clooney", "george.clooney@abc.def");
            Assert.AreEqual(0, user.Points);

            loyalty.TransferPoints(user.UserId, -10);
            var queriedUser = loyalty.GetUser(user.UserId);
            Assert.AreEqual(10, user.Points);
        }
    }
}
