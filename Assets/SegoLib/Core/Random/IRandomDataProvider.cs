using SegoLib.DataProviders;


namespace SegoLib.Random
{
    public interface IRandomDataProvider<T> : IDataProvider<T>, IProbabilityMassProvider<T>
    {
    } 
}
