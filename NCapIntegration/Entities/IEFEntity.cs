namespace NCapIntegration.Entities
{
    public interface IEFEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IEFSoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
