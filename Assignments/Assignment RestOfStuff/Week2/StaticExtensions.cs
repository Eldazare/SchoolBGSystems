using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Week2
{
    public static class StaticExtensions
    {
        public static IServiceCollection UsePlayersProcessor(this IServiceCollection services){
            return services.AddSingleton<PlayersProcessor>();
        }

        public static IServiceCollection UseRepository(this IServiceCollection services, IRepository rep){
            return services.AddSingleton<IRepository>(rep);
        }

        public static IServiceCollection UseItemsProcessor(this IServiceCollection services){
            return services.AddSingleton<ItemsProcessor>();
        }
    }
}