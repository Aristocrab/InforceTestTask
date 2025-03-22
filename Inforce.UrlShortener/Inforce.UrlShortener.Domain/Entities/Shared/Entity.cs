namespace Inforce.UrlShortener.Domain.Entities.Shared;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;
}