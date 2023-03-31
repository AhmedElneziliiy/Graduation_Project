using Graduation.Data;
using Graduation.Interfaces;
using Graduation.Services;
using Microsoft.EntityFrameworkCore;

namespace Graduation.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<DataContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            
            services.AddScoped<ITokenService, TokenService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
