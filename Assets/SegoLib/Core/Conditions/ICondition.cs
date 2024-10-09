
namespace SegoLib
{
    public interface ICondition<T>
    {
        bool Evaluate(T ctx);
    }
}