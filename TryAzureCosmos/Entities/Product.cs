namespace TryAzureCosmos.Entities
{
    public record Product
    (
        string id,
        string categoryId,
        string name,
        decimal price,
        string[] tags
    );
}
