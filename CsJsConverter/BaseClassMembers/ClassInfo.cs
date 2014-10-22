using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CsJsConversion.BaseClassMembers
{
    public class ClassInfo<TTarget>
    {
        public HtmlString GetFieldName(Expression<Func<TTarget, object>> field)
        {
            var pi = GetPropertyInfo(field);
            return new HtmlString(pi.Name);
        }

        protected PropertyInfo GetPropertyInfo(Expression<Func<TTarget, object>> propertyLambda)
        {
            Type type = typeof(TTarget);

            var member = propertyLambda.Body as MemberExpression;

            if (member == null)
            {
                if (propertyLambda.Body.NodeType == ExpressionType.Convert ||
                    propertyLambda.Body.NodeType == ExpressionType.Convert)
                {
                    member = ((UnaryExpression)propertyLambda.Body).Operand as MemberExpression;
                }
            }

            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                                                          propertyLambda));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",
                                                          propertyLambda));
            }

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda,
                    type));
            }

            return propInfo;
        }
    }
}
