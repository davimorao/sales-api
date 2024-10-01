namespace Sales.Domain.Repositories
{
    public interface ISpecification<T>
    {
        public string ToSqlQuery();
        public object Parameters { get; }
    }
}
