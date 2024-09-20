using Microsoft.Extensions.DependencyInjection;
using UnsocNetwork.Data.Repositories;
using UnsocNetwork.Data.UoW;

namespace UnsocNetwork.Extensions
{
    public static class ServiceExtentions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddCustomRepository<TEntity, IRepository>(this IServiceCollection services)
                 where TEntity : class
                 where IRepository : class, IRepository<TEntity>
        {
            services.AddTransient<IRepository<TEntity>, IRepository>();

            return services;
        }

    }
}
