using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample03
{
    public class ExpressionToFTSRequestTranslator : ExpressionVisitor
    {
        StringBuilder resultString = new StringBuilder();
        List<StringBuilder> resultStringArray = new List<StringBuilder>();

        //public string Translate(Expression exp)
        //{
        //    resultString = new StringBuilder();
        //    Visit(exp);

        //    return resultString.ToString();
        //}
        public List<string> Translate(Expression exp)
        {
           // StringBuilder resultString = new StringBuilder();
            Visit(exp);
            resultStringArray.Add(resultString);

            return resultStringArray.Select(item => item.ToString()).ToList();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(String) && node.Method.Name == "Contains")
            {
                var value = node.Arguments[0];
                var field = node.Object;

                Visit(field);
                resultString.Append("(*");
                Visit(value);
                resultString.Append("*)");
                return node;
            }

            if (node.Method.DeclaringType == typeof(String) && node.Method.Name == "StartsWith")
            {
                var value = node.Arguments[0];
                var field = node.Object;

                Visit(field);
                resultString.Append("(");
                Visit(value);
                resultString.Append("*)");
                return node;
            }

            if (node.Method.DeclaringType == typeof(String) && node.Method.Name == "EndsWith")
            {
                var value = node.Arguments[0];
                var field = node.Object;

                Visit(field);
                resultString.Append("(*");
                Visit(value);
                resultString.Append(")");
                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    if (node.Left.NodeType == ExpressionType.MemberAccess && node.Right.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Left);
                        resultString.Append("(");
                        Visit(node.Right);
                        resultString.Append(")");
                    }
                    if (node.Right.NodeType == ExpressionType.MemberAccess && node.Left.NodeType == ExpressionType.Constant)
                    {
                        Visit(node.Right);
                        resultString.Append("(");
                        Visit(node.Left);
                        resultString.Append(")");
                    }
                    break;
                case ExpressionType.AndAlso:
                    Visit(node.Left);
                    resultStringArray.Add(resultString);
                    resultString = new StringBuilder();
                    Visit(node.Right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            resultString.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            resultString.Append(node.Value);

            return node;
        }
    }
}
