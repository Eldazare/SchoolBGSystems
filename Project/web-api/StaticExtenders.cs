using Microsoft.Extensions.DependencyInjection;

namespace web_api
{
    public static class StaticExtenders
    {
        public static IServiceCollection UseRepository(this IServiceCollection services){
            return services.AddSingleton<MongoRep>();
        }

        public static IServiceCollection UseGameProcessor(this IServiceCollection services){
            return services.AddSingleton<GameProcessor>();
        }
    }
}