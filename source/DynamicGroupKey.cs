using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicKeySelector
{
	public class DynamicGroupKey
	{
		public DynamicGroupKey(object source, IEnumerable<PropertyInfo> props)
		{
			this.backingTypes = props.Select(prop => prop.PropertyType);
			try
			{
				this.map = props.ToDictionary(prop => prop.Name, prop => prop.GetValue(source, null));
			}
			catch(Exception e)
			{
				throw new Exception("Exception occurred while parsing properties in DynamicGroupBy call", e);
			}
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

		public static readonly IEqualityComparer<DynamicGroupKey> EqualityComparer = new DynamicGroupKeyComparer();

		private class DynamicGroupKeyComparer : IEqualityComparer<DynamicGroupKey>
		{
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
					while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
					{
						var propertyType = e3.Current;
						var comparer = typeof(EqualityComparer<>)
							.MakeGenericType(propertyType)
							.GetProperty("Default", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
							.GetValue(null, null);
						if (!(bool)typeof(IEqualityComparer<>)
							.MakeGenericType(propertyType)
							.GetMethod("Equals", new Type[] { propertyType, propertyType })
							.Invoke(comparer, new object[] { e1.Current, e2.Current }))
							return false;
					}
					return true;
				}
			}

			public int GetHashCode(DynamicGroupKey obj)
			{
				if (obj == null) return 0;
				//get each hash code by their default EqualityComparer then combine
				using (var e1 = obj.map.Values.GetEnumerator())
				using (var e2 = obj.backingTypes.GetEnumerator())
				{
					int hash = 0;
					while (e1.MoveNext() && e2.MoveNext())
					{
						var propertyType = e2.Current;
						var comparer = typeof(EqualityComparer<>)
							.MakeGenericType(propertyType)
							.GetProperty("Default", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
							.GetValue(null, null);
						hash ^= (int)typeof(IEqualityComparer<>)
							.MakeGenericType(propertyType)
							.GetMethod("GetHashCode", new Type[] { propertyType })
							.Invoke(comparer, new object[] { e1.Current });
					}
					return hash;
				}
			}
		}
	}
}
