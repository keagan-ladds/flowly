namespace Flowly.Core.Conditions
{
    public sealed class AlwaysFalseCondition : ICondition
    {
        public bool Evaluate()
        {
            return false;
        }
    }
}
