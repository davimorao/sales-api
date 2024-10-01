namespace Sales.Domain.Repositories;

public sealed class OrderingField
{
    public string FieldName { get; set; } = string.Empty;
    public bool Ascending { get; set; } = false;
}