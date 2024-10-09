using UnityEngine;

namespace SegoLib
{
    public interface IFormatter
    {
        public string Format(object obj);
    }

    public abstract class Formatter : ScriptableObject, IFormatter
    {
        public static IFormatter Default { get; } = new DefaultFormatter();

        public abstract string Format(object obj);
    }

    public class DefaultFormatter : IFormatter
    {
        public string Format(object obj)
        {
            if (obj == null)
                return string.Empty;

            return obj.ToString();
        }
    }
}
