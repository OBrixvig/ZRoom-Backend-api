using ZRoomLibrary;
using ZRoomBackendApi.Services;
using ZRoomLoginLibrary.Repositories;

namespace ZRoomBackendApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var loginServerConnectionString = builder.Configuration.GetConnectionString("loginDB");

            // Add services to the container.
            if (loginServerConnectionString != null)
            {
                builder.Services.AddSingleton<IUserRepository>(new UserRepositoryDB(loginServerConnectionString));
                builder.Services.AddSingleton<AvailableBookingRepository>(new AvailableBookingRepository(loginServerConnectionString));
                builder.Services.AddSingleton<BookingRepository>(new BookingRepository(loginServerConnectionString));
                builder.Services.AddSingleton<AvailableBookingsDatabaseUpdater>(new AvailableBookingsDatabaseUpdater(loginServerConnectionString));
            }
            else
            {
                throw new InvalidOperationException("Connection string 'loginDB' is missing in the configuration.");
            }

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<JwtTokenGenerator>();
            builder.Services.AddHostedService<BookingRotationService>();
            

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                builder => builder.AllowAnyOrigin().
                AllowAnyMethod().
               AllowAnyHeader()
                );
                options.AddPolicy("AllowOnlyGetPut",
                builder => builder.AllowAnyOrigin().
                WithMethods("GET", "PUT").
               AllowAnyHeader()
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.UseCors("AllowAny");

            app.MapControllers();

            app.Run();
        }
    }
}
