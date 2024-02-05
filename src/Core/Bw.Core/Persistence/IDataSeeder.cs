namespace Bw.Core.Persistence;

public interface IDataSeeder
{
    Task SeedAllAsync();
    int Order { get; }

}
