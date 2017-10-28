using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Expressions_IQuerable
{
    class ConvertVisitor : ExpressionVisitor
    {
        public Expression Visit(Expression<Func<int, int, int,int>> exp, Dictionary<string, int> dict)
        {
            var lambda = GetLambda(exp);
            var replaced = Replace(lambda, dict);
            return base.Visit(replaced);
        }

        private Expression Replace(LambdaExpression lambda, Dictionary<string, int> arguments)
        {
            var replacer = new ReplacerVisitor(lambda.Parameters, arguments);
            return replacer.Replace(lambda.Body);
        }

        private LambdaExpression GetLambda(Expression expression)
        {
            var finder = new FieldLambdaFinder();
            return (LambdaExpression)finder.Find(expression);
        }

        class FieldLambdaFinder : ExpressionVisitor
        {
            protected override Expression VisitMember(MemberExpression node)
            {
                var constantExpression = (ConstantExpression)node.Expression;
                var info = (FieldInfo)node.Member;
                var fieldValue = (Expression)info.GetValue(constantExpression.Value);
                return fieldValue;
            }

            public Expression Find(Expression expression)
            {
                return Visit(expression);
            }
        }
    }

    
}
