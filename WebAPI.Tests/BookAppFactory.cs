using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebAPI.Data;

namespace WebAPI.Tests;
public class BookAppFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", "not-used-in-tests");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<BookAppContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<BookAppContext>>();

            services.AddDbContext<BookAppContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}