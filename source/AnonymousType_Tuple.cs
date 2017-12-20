using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicKeySelector
{
	public class AnonymousType_Tuple<T1, T2, T3>
	{
		public AnonymousType_Tuple(T1 t1, T2 t2, T3 t3)
		{
			Item1_backstoreField = t1;
			Item2_backstoreField = t2;
			Item3_backstoreField = t3;
		}

		private T1 Item1_backstoreField;
		public T1 Item1
		{
			get { return this.Item1_backstoreField; }
		}

		private T2 Item2_backstoreField;
		public T2 Item2
		{
			get { return this.Item2_backstoreField; }
		}

		private T3 Item3_backstoreField;
		public T3 Item3
		{
			get { return this.Item3_backstoreField; }
		}

		//add some IDictionary<string, T> here Then read property by key
	}
}
