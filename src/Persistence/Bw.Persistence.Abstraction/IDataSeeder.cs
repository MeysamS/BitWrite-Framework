namespace Bw.Persistence.Abstraction;

public interface IDataSeeder
{
    Task SeedAllAsync();
    int Order { get; }

}
