using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telligent.Core.Infrastructure.Database;

namespace Telligent.Tag.Database;

public static class DbContextExtension
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, string connection)
    {
        services.AddDbContext<TagDbContext>(options =>
        {
            options.UseMySql(connection, ServerVersion.AutoDetect(connection));
        });

        return services;
    }

    public static void RegisterDbContexts(this ContainerBuilder builder)
    {
        builder.RegisterType<TagDbContext>().As<BaseDbContext>().InstancePerLifetimeScope();
    }
}