namespace SegoLib
{
    /// <summary>
    /// Evaluates an object of type <typeparamref name="T"/> and returns a value between 0 and 1.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INormalizedEvaluation<in T>
    {
        /// <summary>
        /// Returns a value between 0 and 1.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract float Evaluate(T t);
    }
}
