using System;

namespace Flowly.Core.Conditions
{
    public class OrCondition : ICondition
    {
        private readonly ICondition _lhs;
        private readonly ICondition _rhs;

        public OrCondition(ICondition lhs, ICondition rhs)
        {
            _lhs = lhs ?? throw new ArgumentNullException(nameof(lhs));
            _rhs = rhs ?? throw new ArgumentNullException(nameof(_rhs));
        }

        public bool Evaluate()
        {
            return _lhs.Evaluate() || _rhs.Evaluate();
        }
    }
}
