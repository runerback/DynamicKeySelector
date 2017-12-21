using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DynamicKeySelector
{
	public class DynamicGroupKey : IEqualityComparer<DynamicGroupKey>
	{
		public DynamicGroupKey(object source, IEnumerable<PropertyInfo> props)
		{
			this.map = props.ToDictionary(prop => prop.Name, prop => prop.GetValue(source, null));
			this.backingTypes = props.Select(prop => prop.PropertyType);
		}

		private IDictionary<string, object> map;
		private IEnumerable<Type> backingTypes;

		public object this[string name]
		{
			get
			{
				if (string.IsNullOrEmpty(name))
					return null;
				object value;
				if (this.map.TryGetValue(name, out value))
					return value;
				return null;
			}
		}

		public object this[int index]
		{
			get { return this.map.Values.ElementAtOrDefault(index); }
		}

		public bool Equals(DynamicGroupKey x, DynamicGroupKey y)
		{
			if (x == null)
				return y == null;
			else if (y == null) return false;

			if (!x.map.Keys.SequenceEqual(y.map.Keys))
				return false;
			if (!x.backingTypes.SequenceEqual(y.backingTypes))
				return false;
			//compare values by their default EqualityComparer
			using (var e1 = x.map.Values.GetEnumerator())
			using (var e2 = y.map.Values.GetEnumerator())
			using (var e3 = x.backingTypes.GetEnumerator())
			{
				bool areEquals = true;
				while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
				{
					var propertyType = e3.Current;
					var comparer = typeof(EqualityComparer<>)
						.MakeGenericType(propertyType)
						.GetProperty("Default", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
						.GetValue(null, null);
					areEquals &= (bool)typeof(IEqualityComparer<>)
						.MakeGenericType(propertyType)
						.GetMethod("Equals", new Type[] { propertyType, propertyType })
						.Invoke(comparer, new object[] { e1.Current, e2.Current });
				}
				return areEquals;
			}
		}

		public int GetHashCode(DynamicGroupKey obj)
		{
			if (obj == null) return 0;
			///TODO: get each hash code by their default EqualityComparer then compare two sequence
			///but it seems that GetHashCode was not used in Enumerable.GroupBy methods
			throw new NotImplementedException();
		}
	}

	public class DynamicGroupKey<TElement> : DynamicGroupKey
	{
		public DynamicGroupKey(TElement source, IEnumerable<PropertyInfo> props)
			: base(source, props)
		{
			///TODO: add some pre conditions here
		}
	}
}
