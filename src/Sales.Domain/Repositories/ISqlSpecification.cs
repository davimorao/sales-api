namespace Sales.Domain.Repositories
{
    public interface ISqlSpecification<T>
    {
        public string ToSqlQuery();
        public object Parameters { get; }
    }
}
