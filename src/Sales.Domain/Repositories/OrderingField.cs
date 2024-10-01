namespace Sales.Domain.Repositories;

public class OrderingField
{
    public string FieldName { get; set; } = string.Empty;
    public bool Ascending { get; set; } = false;
}