namespace Base.Contracts;

public interface IDomainEntityMeta
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}