namespace SegoLib
{
    /// <summary>
    /// Returns true if all conditions evaluate to false.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Nor<T> : CompoundCondition<T>
    {
        public static bool StaticEvaluate(T ctx, params ICondition<T>[] conditions)
        {
            if (ctx == null)
                throw new System.ArgumentNullException(nameof(ctx));

            if (conditions == null || conditions.Length == 0)
                return true;

            for (int i = 0; i < conditions.Length; i++) {
                if (conditions[i].Evaluate(ctx))
                    return false;
            }

            return true;
        }

        public Nor(params ICondition<T>[] conditions) : base(conditions) { }

        public override bool Evaluate(T ctx)
        {
            return StaticEvaluate(ctx, m_conditions);
        }
    }
}
