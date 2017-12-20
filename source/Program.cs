﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Dynamic;

namespace DynamicKeySelector
{
	class Program
	{
		static void Main(string[] args)
		{
			CodeTimeChecker checker = new CodeTimeChecker();

			int counter = 0;
			var source = Enumerable.Range(0, 6)
				.Select(item => new Model()
				{
					Item1 = ++counter,
					Item2 = ++counter,
					Item3 = ++counter,
					Item4 = ++counter,
					Item5 = ++counter,
					Item6 = ++counter,
				})
				.ToArray();

			var result1 = source
				.Select(item => new
				{
					item.Item1,
					item.Item3,
					item.Item5
				});
			checker.Set();
			foreach (var item in result1)
			{
				Console.WriteLine("{0}, {1}, {2}", item.Item1, item.Item3, item.Item5);
			}
			checker.Check();

			Console.WriteLine("----------------------");

			var result2 = source
				.Select(item =>
				{
					dynamic newType = new ExpandoObject();
					((IDictionary<string, object>)newType)["Item1"] = item.Item1;
					newType.Item3 = item.Item3;
					newType.Item5 = item.Item5;
					return newType;
				});

			checker.Set();
			foreach (var item in result2)
			{
				Console.WriteLine("{0}, {1}, {2}", item.Item1, item.Item3, item.Item5);
			}
			checker.Check();

			Console.WriteLine("----------------------");

			var result3 = source
				.Select(item => AnonymousTypeSelector.Select(item.Item1, item.Item3, item.Item5));

			checker.Set();
			foreach (var item in result3)
			{
				Console.WriteLine("{0}, {1}, {2}", item.Item1, item.Item2, item.Item3);
			}
			checker.Check();

			Console.WriteLine();
			Console.WriteLine("done");
			Console.ReadLine();
		}
	}
}