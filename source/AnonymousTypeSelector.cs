using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DynamicKeySelector
{
	public static class AnonymousTypeSelector
	{
		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
		{
			//return new Tuple<T1, T2, T3>(t1, t2, t3);

			Type[] genericTypes = new Type[]{typeof(T1),typeof(T2),typeof(T3)};
			Type anoType = typeof(Tuple<,,>).MakeGenericType(genericTypes);
			var ctor = anoType.GetConstructor(genericTypes);
			var valuesExp = Expression.Parameter(typeof(object[]));
			var newExpression = Expression.New(
				ctor,
				ctor.GetParameters().Select((p, i) => Expression.Convert(
					Expression.ArrayIndex(valuesExp, Expression.Constant(i)),
					p.ParameterType)),
				anoType.GetProperty("Item1"),
				anoType.GetProperty("Item2"),
				anoType.GetProperty("Item3"));
			return Expression.Lambda<Func<object[], Tuple<T1, T2, T3>>>(
				newExpression,
				valuesExp).Compile()
				(new object[] { t1, t2, t3 });
		}
	}
}
