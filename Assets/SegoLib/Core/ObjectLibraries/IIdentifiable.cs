namespace SegoLib
{
    /// <summary>
    /// A simple interface for objects that should have a unique Id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIdentifiable<T>
    {
        public T Id { get; }
    } 
}
