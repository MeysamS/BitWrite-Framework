using Bw.Core.Persistence;

namespace Bw.Persistence.Mongo.Abstraction;

public interface IMongoUnitOfWork : IUnitOfWork<IMongoDbContext>    
{ }