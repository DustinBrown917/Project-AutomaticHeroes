namespace SegoLib
{
    public abstract class CompoundCondition<T> : ICondition<T>
    {
        protected ICondition<T>[] m_conditions;

        public CompoundCondition(params ICondition<T>[] conditions)
        {
            m_conditions = conditions;
        }

        public abstract bool Evaluate(T ctx);
    }
}
