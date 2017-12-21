using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DynamicKeySelector
{
	public static class DynamicEnumerableExtension
	{
		public static IEnumerable<IGrouping<DynamicGroupKey, TElement>> GroupBy<TSource, TElement>(this IEnumerable<TSource> source, IEnumerable<string> propertyNames, Func<TSource, TElement> elementSelector)
		{
			Type sourceType = typeof(TSource);
			return GroupBy(source, propertyNames.Select(prop => sourceType.GetProperty(prop)), elementSelector);
		}

		public static IEnumerable<IGrouping<DynamicGroupKey, TSource>> GroupBy<TSource>(this IEnumerable<TSource> source, IEnumerable<string> propertyNames)
		{
			Type sourceType = typeof(TSource);
			return GroupBy(source, propertyNames.Select(prop => sourceType.GetProperty(prop)));
		}

		public static IEnumerable<IGrouping<DynamicGroupKey, TSource>> GroupBy<TSource>(this IEnumerable<TSource> source, IEnumerable<PropertyInfo> properties)
		{
			return source.GroupBy(item => new DynamicGroupKey(item, properties));
		}

		public static IEnumerable<IGrouping<DynamicGroupKey, TElement>> GroupBy<TSource, TElement>(this IEnumerable<TSource> source, IEnumerable<PropertyInfo> properties, Func<TSource, TElement> elementSelector)
		{
			return source.GroupBy(item => new DynamicGroupKey(item, properties), elementSelector);
		}
	}
}
