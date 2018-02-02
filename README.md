Assumptions
====================================================================================

Endpoints

* There is no security on the endpoints, and anyone will get full access to anyone.
* There is "v1" in the url to simulate versioning of api. There are many ways to do
versioning, and url is a very simple way to do it (though controversial for some).
* I designed the endpoints so that transactions are "sub-resources" of users, and
with no verbs like "getUsers". Instead, we "GET" on "users".
* There is no paging of requests, but we could imagine it as a great way of limiting
needless work and data transfer when we want to list, for example, all transfers
for a user.
* There's a bit of validation of data on the controller's perspective.
* The endpoints return relevant status codes by catching exceptions. It may be
technically slower, but it is easier to understand semantically.
* For pretend security reasons, the endpoints only send relevant information back,
and not just what comes from the database.

Database

* The project is configured to work locally as easily as possible, and is memory by
default.
* For real persistence of data, I'm using sqlite. Again, it's just because it's
easy to install use (as it is embedded in the app). It is ACID compliant, which would
be a real-life nice-to-have for this challenge.
* I'm using Entity Framework as an ORM. This would simplify security (no injection)
and manages transformation of entities so I only need to deal with POCO.
* Optimistic concurrency is easiest when using EF to handle the point deduction
scenario.

Architecture

* The architecture is minimalist, but I'm separating controller from DB access via
the LoyaltyManager. The LoyaltyManager is responsible for trying to apply the
relevant business rules (e.g. adding/subtracting points) to entities according to
the controllers asks. 
* The controller will only validate basic data, and relies on the LoyaltyManager
for any validation on the database. It then catches the default exceptions and
returns status codes (OK, BadRequest, NotFound).

Unit tests

* I added a few tests on the loyalty manager via an in-memory database. There are
more tests I would've loved to add with more time, like a functional test for
concurrently adding/subtracting points to ensure concurrency is accurate.
* The tests were run only via VS2017.



Running the project
====================================================================================

Tested on Windows, with VS2017 (latest update) and .Net Core.

With VS (if you already have VS2017)
1. Open ButtonChallenge.sln
2. F5 to run the project
3. The tests work with VS

With .Net Core
0. Install .Net Core from https://www.microsoft.com/net/learn/get-started/windows
1. Open the command line and "cd" to ButtonChallenge.csproj
	cd ButtonChallenge/ButtonChallenge.csproj
2. Run the project
	dotnet run



The endpoints
====================================================================================

Use your favorite REST client (tried with Postman).
The default URL is different between .Net Core and VS
VS: http://localhost:55234/
dotnet: http://localhost:55235/

GET api/v1/users
List all users.

POST api/v1/users
Create a new user

GET api/v1/users/5
Get a specific user

GET api/v1/users/5/transfers
Get all transfers for a user ID

POST api/v1/users/5/transfers
Add a transfer to a user.

Examples

To list all users via Visual Studio, I went to:
GET http://localhost:55234/api/v1/users

To list all users via command line (dotnet run), I went to:
GET http://localhost:55235/api/v1/users



The database
====================================================================================

I configured the project to use a runtime database to make it work out of the box.
This can be modified to run a real database with a few changes.

Note: This was tested for .Net Core only.

1. In Startup.cs:
	Comment out this line:
		services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("UserDatabase"));
	And uncomment this line:
		services.AddDbContext<UserContext>(opt => opt.UseSqlite("Data Source=buttonchallenge.db"));
2. The project is configured to install Entity Framework CLI tool. In command line,
	run the following:
		dotnet restore
3. Now that EF CLI tool is installed, you can set up the initial database migrations.
	In command line:
		dotnet ef database update
4. Run the project as before, your users and transactions should now be preserved!
