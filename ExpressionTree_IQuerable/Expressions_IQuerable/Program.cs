using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Expressions_IQuerable
{
    class Program
    {
        private static void Main(string[] args)
        {
            Dictionary<string, int> valueForReplace = new Dictionary<string, int> {{"a", 5}, {"b", 10}, {"c", -10}};

            Expression<Func<int, int>> source = (a) => a - (1 + a + 1) * (a + 5 - 1) - (a - 1);
            Expression<Func<int, int, int, int>> sourceReplace = (a, b, c) => c / a * b;

            var replaceResult = sourceReplace.Convert(valueForReplace);
            var transformResult = (new ExpressionTransformer().VisitAndConvert(source, ""));

            Console.WriteLine(source + " " + source.Compile().Invoke(44));
            Console.WriteLine(transformResult + " " + transformResult.Compile().Invoke(44));
            Console.WriteLine(sourceReplace);
            Console.WriteLine(replaceResult);

            Console.ReadLine();
        }
    }

    static class ExpressionExtender
    {
        public static Expression Convert(this Expression<Func<int, int, int,int>> expression, Dictionary<string, int> dict)
        {
            return new ConvertVisitor().Visit(expression, dict);
        }
    }
}
