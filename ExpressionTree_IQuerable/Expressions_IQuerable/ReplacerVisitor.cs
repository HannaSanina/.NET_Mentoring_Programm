using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expressions_IQuerable
{
    class ReplacerVisitor : ExpressionVisitor
    {
        private Dictionary<ParameterExpression, int> _replacementList = new Dictionary<ParameterExpression, int>();

        public ReplacerVisitor(IEnumerable<ParameterExpression> what, Dictionary<string, int> with)
        {
            foreach (var item in what)
            {
                var x = with.First(param => param.Key == item.Name).Value;
                _replacementList.Add(item, x);
            }
        }

        public Expression Replace(Expression body)
        {
            return Visit(body);
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            int value;
            if (node.Name == "a")
            {
                return _replacementList.TryGetValue(node, out value) ? Expression.Constant(value) : base.VisitParameter(node);
            }
            if (node.Name == "b")
            {
                return _replacementList.TryGetValue(node, out value) ? Expression.Constant(value) : base.VisitParameter(node);
            }
            if (node.Name == "c")
            {
                return _replacementList.TryGetValue(node, out value) ? Expression.Constant(value) : base.VisitParameter(node);
            }

            return base.VisitParameter(node);
        }
    }
}
