using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicKeySelector
{
	public class CodeTimeChecker
	{
		private DateTime time = DateTime.Now;

		public void Set()
		{
			this.time = DateTime.Now;
		}

		public void Check()
		{
			DateTime now = DateTime.Now;
			var span = now - this.time;
			this.time = now;

			Console.WriteLine(span.TotalMilliseconds);
		}
	}
}
