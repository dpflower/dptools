using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Data;
using System.Web;

namespace DP.Data.Common
{
    public class ExpressionTree
    {
        private Expression DynamicCreateLambda(ParameterExpression r, Type t)
        {
            PropertyInfo[] properties = t.GetProperties();
            List<MemberBinding> bindings = new List<MemberBinding>();

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType.Namespace == "System")
                {
                    MethodCallExpression propertyValue = Expression.Call(typeof(DataRecordExtend).GetMethod("Field").MakeGenericMethod(property.PropertyType), r, Expression.Constant(property.Name));
                    MemberBinding binding = Expression.Bind(property, propertyValue);
                    bindings.Add(binding);
                }
                else
                {
                    Expression expression = DynamicCreateLambda(r, property.PropertyType);
                    MemberBinding binding = Expression.Bind(property, expression);
                    bindings.Add(binding);
                }
            }
            Expression initializer = Expression.MemberInit(Expression.New(t), bindings);
            return initializer;
        }

        public Func<IDataRecord, T> IDataRecordToEntity<T>()
        {
            ParameterExpression r = Expression.Parameter(typeof(IDataRecord), "r");
            Expression initializer = DynamicCreateLambda(r, typeof(T));
            Expression<Func<IDataRecord, T>> lambda = Expression.Lambda<Func<IDataRecord, T>>(initializer, r);
            return lambda.Compile();

        }

        public static Func<IDataRecord, T> IDataRecordToEntityFactory<T>()
        {
            string key = typeof(T).FullName + "_IDataRecordToEntity";
            Func<IDataRecord, T> func = HttpRuntime.Cache.Get(key) as Func<IDataRecord, T>;
            if (func == null)
            {
                ExpressionTree expressionTree = new ExpressionTree();
                func = expressionTree.IDataRecordToEntity<T>();
                HttpRuntime.Cache.Insert(key, func);
            }
            return func;
        }

    }
}
