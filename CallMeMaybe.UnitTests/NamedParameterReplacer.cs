using System.Linq;
using System.Linq.Expressions;

namespace CallMeMaybe.UnitTests
{
    public class NamedParameterReplacer : ExpressionVisitor
    {
        private readonly string _parameterName;
        private readonly Expression _replacement;

        public NamedParameterReplacer(string parameterName, Expression replacement)
        {
            _parameterName = parameterName;
            _replacement = replacement;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var newArguments = node.Method.GetParameters().Zip(node.Arguments,
                (p, a) => p.Name == _parameterName ? _replacement : a);
            var newNode = Expression.Call(node.Object, node.Method, newArguments);
            return base.VisitMethodCall(newNode);
        }
    }
}