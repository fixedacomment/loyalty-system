using ButtonChallenge.Database;
using ButtonChallenge.BusinessRules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ButtonChallenge
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // These are my only changes on this file, using default dependency injection to setup EF and my DB layer.

            // Comment this line and uncomment the next one to use sqlite.
            services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("UserDatabase"));
            //services.AddDbContext<UserContext>(opt => opt.UseSqlite("Data Source=buttonchallenge.db"));
            services.AddTransient<LoyaltyManager>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
