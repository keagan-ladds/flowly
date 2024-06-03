namespace Flowly.Core.Conditions
{
    public class AlwaysTrueCondition : ICondition
    {
        public bool Evaluate()
        {
            return true;
        }
    }
}
