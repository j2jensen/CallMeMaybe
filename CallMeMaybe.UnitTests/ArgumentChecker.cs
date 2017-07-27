using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace CallMeMaybe.UnitTests
{
    public class ArgumentChecker
    {
        // Convenience methods to leverage implicit generic typing.
        public static ArgumentChecker<T> For<T>(T reusableInstance)
        {
            return new ArgumentChecker<T>(reusableInstance);
        }

        public static ArgumentChecker<T> For<T>(Func<T> instanceFactory)
        {
            return new ArgumentChecker<T>(instanceFactory);
        }
    }

    public class ArgumentChecker<T>
    {
        private readonly Func<T> _instanceFactory;

        public ArgumentChecker(Func<T> instanceFactory)
        {
            _instanceFactory = instanceFactory;
            // todo: if T is disposable, we should dispose any created instances once we're done.
        }

        public ArgumentChecker(T reusableInstance)
        {
            _instanceFactory = () => reusableInstance;
        }

        public void AssertReferenceParametersDisallowNullValues(Expression<Action<T>> normalCall)
        {
            if (normalCall.Body.NodeType != ExpressionType.Call)
            {
                throw new InvalidOperationException(String.Format(
                    "The provided normal method call expression should have been a method call, but " +
                    "was a {0} instead: {1}", normalCall.Body.NodeType, normalCall));
            }
            try
            {
                normalCall.Compile().Invoke(GetInstance()); // No exception from a regular call.
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    "The provided normal method call expression should not produce an exception, but " +
                    "it did.", e);
            }
            var method = ((MethodCallExpression) normalCall.Body).Method;
            var referenceParameters = method.GetParameters().Where(p => p.ParameterType.GetTypeInfo().IsClass);
            foreach (var parameter in referenceParameters)
            {
                AssertParameterDisallowsNullValue(normalCall, parameter);
            }
        }

        private T GetInstance()
        {
            try
            {
                return _instanceFactory();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(string.Format(
                    "An exception was thrown while trying to instantiate a {0}", typeof (T)),
                    e);
            }
        }

        private void AssertParameterDisallowsNullValue(Expression<Action<T>> normalCall, ParameterInfo parameter)
        {
            var callWithNullParameter =
                (Expression<Action<T>>)
                    new NamedParameterReplacer(parameter.Name,
                        Expression.Constant(null, parameter.ParameterType))
                        .Visit(normalCall);
            try
            {
                callWithNullParameter.Compile().Invoke(GetInstance());
                Assert.True(false, $"Passing a null value for parameter \"{parameter.Name}\" failed to produce an exception, in expression: {callWithNullParameter}");
            }
            catch (ArgumentNullException e)
            {
                if (e.ParamName != parameter.Name)
                {
                    Assert.True(false,
                        $"Passing a null value for parameter \"{parameter.Name}\" threw an ArgumentNullException, but the reported parameter name was \"{e.ParamName}\", in expression: {callWithNullParameter}");
                }
            }
            catch (Exception e)
            {
                Assert.True(false, $"Passing a null value for parameter \"{parameter.Name}\" threw an exception of type {e.GetType()} instead of an ArgumentNullException, in expression: {callWithNullParameter}. Thrown exception: {e}");
            }
        }
    }
}