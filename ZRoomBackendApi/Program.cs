

using ZRoomLibrary;
using ZRoomBackendApi.Services;
using ZRoomLoginLibrary.Repositories;
using ZRoomLibrary.Services;

namespace ZRoomBackendApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var loginServerConnectionString = builder.Configuration.GetConnectionString("loginDB");

            if (string.IsNullOrEmpty(loginServerConnectionString))
            {
                throw new InvalidOperationException("Connection string 'loginDB' is not configured.");
            }

            // Add services to the container.
            builder.Services.AddSingleton<IUserRepository>(new UserRepositoryDB(loginServerConnectionString));
            builder.Services.AddSingleton<AvailableBookingRepository>(new AvailableBookingRepository(loginServerConnectionString));
            builder.Services.AddSingleton<RoomRepository>(new RoomRepository(loginServerConnectionString));
            builder.Services.AddSingleton<BookingRepository>(sp =>
                new BookingRepository(
                    loginServerConnectionString,
                    sp.GetRequiredService<ZRoomLibrary.Services.EmailHandlerService>()
                )
            );

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<JwtTokenGenerator>();
            builder.Services.AddSingleton<ZRoomLibrary.Services.EmailHandlerService>();
            builder.Services.AddSingleton<ZRoomLibrary.Services.PinCodeService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SupportNonNullableReferenceTypes();
            });

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
