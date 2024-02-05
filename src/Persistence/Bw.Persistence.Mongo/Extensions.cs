using Bw.Extensions.Microsoft.DependencyInjection;
using Bw.Persistence.Mongo.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Bw.Persistence.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services)
        where TContext : MongoDbContext, IMongoDbContext
    {
        services.AddValidatedOptions<MongoOptions>(nameof(MongoOptions));

        BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));


        services.AddScoped(typeof(TContext));
        services.AddScoped<IMongoDbContext>(sp => sp.GetRequiredService<TContext>()!);

        services.AddTransient(typeof(IMongoRepository<,>), typeof(MongoRepository<,>));
        services.AddTransient(typeof(IMongoUnitOfWork<>), typeof(MongoUnitOfWork<>));

        return services;

    }




    private static void RegisterConventions()
    {
        ConventionRegistry.Register("conventions",
            new ConventionPack
            {
                  new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreIfDefaultConvention(false),
                new ImmutablePocoConvention(),
            },
            _ => true
            );
    }
}
