﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bw.Extensions.Microsoft.DependencyInjection;


public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationOptions<T>(this IServiceCollection services)
        where T : class
    {
        return services.AddConfigurationOptions<T>(typeof(T).Name);
    }

    public static IServiceCollection AddConfigurationOptions<T>(this IServiceCollection services, string key)
        where T : class
    {
        services.AddOptions<T>().BindConfiguration(key);

        return services.AddSingleton(x => x.GetRequiredService<IOptions<T>>().Value);
    }

    public static IServiceCollection AddValidatedOptions<T>(this IServiceCollection services)
        where T : class
    {
        return services.AddValidatedOptions<T>(typeof(T).Name, RequiredConfigurationValidator.Validate);
    }

    public static IServiceCollection AddValidatedOptions<T>(this IServiceCollection services, string key)
        where T : class
    {
        return services.AddValidatedOptions<T>(key, RequiredConfigurationValidator.Validate);
    }

    public static IServiceCollection AddValidatedOptions<T>(
        this IServiceCollection services,
        string key,
        Func<T, bool> validator
    )
        where T : class
    {
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options
        // https://thecodeblogger.com/2021/04/21/options-pattern-in-net-ioptions-ioptionssnapshot-ioptionsmonitor/
        // https://code-maze.com/aspnet-configuration-options/
        // https://code-maze.com/aspnet-configuration-options-validation/
        // https://dotnetdocs.ir/Post/42/difference-between-ioptions-ioptionssnapshot-and-ioptionsmonitor
        // https://andrewlock.net/adding-validation-to-strongly-typed-configuration-objects-in-dotnet-6/
        services.AddOptions<T>().BindConfiguration(key).Validate(validator);

        // IOptions itself registered as singleton
        return services.AddSingleton(x => x.GetRequiredService<IOptions<T>>().Value);
    }
}

public static class RequiredConfigurationValidator
{
    public static bool Validate<T>(T arg)
        where T : class
    {
        var requiredProperties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => Attribute.IsDefined(x, typeof(RequiredMemberAttribute)));

        foreach (var requiredProperty in requiredProperties)
        {
            var propertyValue = requiredProperty.GetValue(arg);
            if (propertyValue is null)
            {
                throw new Exception($"Required property '{requiredProperty.Name}' was null");
            }
        }

        return true;
    }
}
