using Bw.Core.Persistence;
using Bw.Persistence.Mongo.Abstraction;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Bw.Persistence.Mongo;

public class MongoDbContext : IMongoDbContext, ITxDbContextExecution
{
    public IClientSessionHandle? Session { get; set; }
    public IMongoDatabase Database { get; set; }
    public IMongoClient MongoClient { get; set; }

    protected readonly IList<Func<Task>> _commands;

    public MongoDbContext(IOptions<MongoOptions> options)
    {
        // Set Guid to CSharp style (with dash -)
        //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        RegisterConventions();

        MongoClient = new MongoClient(options.Value.ConnectionString);
        var databaseName = options.Value.DatabaseName;
        Database = MongoClient.GetDatabase(databaseName);

        // Every command will be stored and it'll be processed at SaveChanges
        _commands = new List<Func<Task>>();
    }

    private void RegisterConventions()
    {
        ConventionRegistry.Register("conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreIfDefaultConvention(false),
                new ImmutablePocoConvention()
            }, _ => true);
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Session = await MongoClient.StartSessionAsync(null, cancellationToken);
        Session.StartTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Session is { IsInTransaction: true })
        {
            await Session.CommitTransactionAsync(cancellationToken);
            Session.Dispose();
            Session = null;
        }
    }

    public void Dispose()
    {
        if (Session is { IsInTransaction: true })
        {
            Session.AbortTransaction();
        }

        Session?.Dispose();
        Session = null;

        GC.SuppressFinalize(this);
    }

    public async Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await action();
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
        finally
        {
            Session?.Dispose();
            Session = null;
        }
    }

    public async Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransaction(cancellationToken);
            throw;
        }
        finally
        {
            Session?.Dispose();
            Session = null;
        }
    }


    public IMongoCollection<T> GetCollection<T>(string? name = null)
        => Database.GetCollection<T>(name ?? typeof(T).Name.ToLower());



    public async Task RollbackTransaction(CancellationToken cancellationToken = default)
    {
        if (Session is { IsInTransaction: true })
        {
            await Session.AbortTransactionAsync(cancellationToken);
        }
    }


    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = _commands.Count;
        if (Session is null)
        {
            Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        }

        if (!Session.IsInTransaction)
        {
            Session.StartTransaction();
        }



        try
        {
            var commandTasks = _commands.Select(c => c());

            await Task.WhenAll(commandTasks);

            await Session.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await Session.AbortTransactionAsync(cancellationToken);
            _commands.Clear();
            throw;
        }


        _commands.Clear();
        return result;
    }

    public IClientSessionHandle? GetSession()
    {
        return Session;
    }
}
