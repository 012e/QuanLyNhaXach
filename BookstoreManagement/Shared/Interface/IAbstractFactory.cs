namespace BookstoreManagement.Core.Interface;

public interface IAbstractFactory<T>
where T : class
{
    T Create();
}
