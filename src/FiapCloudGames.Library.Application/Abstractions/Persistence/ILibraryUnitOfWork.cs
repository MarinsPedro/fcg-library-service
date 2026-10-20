namespace FiapCloudGames.Library.Application.Abstractions.Persistence;

public interface ILibraryUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
