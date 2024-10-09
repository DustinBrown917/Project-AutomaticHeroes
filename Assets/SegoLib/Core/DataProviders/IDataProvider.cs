namespace SegoLib.DataProviders
{
    public interface IDataProvider<T>
    {
        public T GetValue();
    }
}