using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserManagementService.DataContexts;
using UserManagementService.Middlewares;
using UserManagementService.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<UserContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddScoped<IValidationService, ValidationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddMemoryCache();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<UserContext>();
            await context.Database.MigrateAsync();
            await SeedDatabaseAsync(context, connectionString);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static async Task SeedDatabaseAsync(UserContext dbContext, string connectionString)
    {
        const string script = @"
            SET NOCOUNT ON;
            DELETE FROM [dbo].[Users];
            DECLARE @i INT = 1;
            WHILE @i <= 50
            BEGIN
                INSERT INTO [dbo].[Users] 
                    (LastName, FirstName, MiddleName, DateOfBirth, PassportNumber, BirthPlace, Phone, Email, RegistrationAddress, ResidentialAddress)
                VALUES
                    (
                        CONCAT('LastName', @i),
                        CONCAT('FirstName', @i),
                        CONCAT('MiddleName', @i),
                        DATEADD(DAY, -RAND() * 3650, GETDATE()),
                        CONCAT(FLOOR(RAND() * 10000), ' ', FLOOR(RAND() * 1000000)),
                        CONCAT('City', @i),
                        CAST((70000000000 + @i) as varchar),
                        CONCAT('user', @i, '@example.com'),
                        CONCAT('Registration Address ', @i),
                        CONCAT('Residential Address ', @i)
                    );
                SET @i = @i + 1;
            END;";

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }
}