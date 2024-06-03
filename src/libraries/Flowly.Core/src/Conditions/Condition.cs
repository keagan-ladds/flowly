namespace Flowly.Core.Conditions
{
    public abstract class Condition : ICondition
    {
        public abstract bool Evaluate();

        public static readonly ICondition True = new AlwaysTrueCondition();
        public static readonly ICondition False = new AlwaysFalseCondition();
        public static ICondition operator &(Condition lhs, Condition rhs) => new AndCondition(lhs, rhs);
        public static ICondition operator +(Condition lhs, Condition rhs) => new AndCondition(lhs, rhs);
        public static ICondition operator |(Condition lhs, Condition rhs) => new OrCondition(lhs, rhs);
    }
}
