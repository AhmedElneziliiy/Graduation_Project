using Graduation.Data;
using Graduation.Helpers;
using Graduation.Interfaces;
using Graduation.Services;
using Graduation.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Graduation.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<DataContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPhotoService, PhotoService>();

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();


            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
