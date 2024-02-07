using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Bw.Persistence.EFCore.Abstraction;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
