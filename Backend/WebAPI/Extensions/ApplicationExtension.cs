using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using WebAPI.Middlewares;

namespace WebAPI.Extensions;

public static class ApplicationExtension
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }
    }

    public static void ConfigureLogging(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void AuthApp(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}