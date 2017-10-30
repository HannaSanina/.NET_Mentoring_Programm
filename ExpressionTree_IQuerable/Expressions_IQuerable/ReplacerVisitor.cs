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
        private Dictionary<string, int> _replacementList = new Dictionary<string, int>();

        public ReplacerVisitor(IEnumerable<string> what, Dictionary<string, int> with)
        {
            foreach (var item in what)
            {
                int value;
                if (with.TryGetValue(item, out value))
                {
                    _replacementList.Add(item, value);
                };
            }
        }

        public Expression Replace(Expression body)
        {
            return Visit(body);
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            int value;
            if (_replacementList.TryGetValue(node.Name, out value))
            {
                return Expression.Constant(value);
            }

            return base.VisitParameter(node);
        }
    }
}
