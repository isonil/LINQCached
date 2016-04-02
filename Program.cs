using System;
using System.Linq;
using System.Collections.Generic;

namespace LinqCached
{

public class CachedIntInfo
{
	public int value;
	public bool even;
	public bool negative;
}

public class Program
{
	private static Random random = new Random();

	private static IEnumerable<int> RandomValues(int count)
	{
		for( int i = 0; i < count; ++i )
		{
			yield return random.Next(-100, 100);
		}
	}

	public static void Main(string[] args)
	{
		Console.WriteLine("--- Cached random value ---");
		Console.WriteLine();

		for( int i = 0; i < 10; ++i )
		{
			foreach( var elem in RandomValues(1).Cached(3, 0) )
			{
				Console.WriteLine(elem);
			}

			LinqCache.FramePassed();
		}

		Console.WriteLine();
		Console.WriteLine("--- Cached ints info ---");
		Console.WriteLine();

		for( int i = 0; i < 7; ++i )
		{
			foreach( var elem in RandomValues(2).Cached(3, 1, (int value, CachedIntInfo cached) =>
				{
					Console.WriteLine("(caching int)");
					cached.value = value;
					cached.even = value % 2 == 0;
					cached.negative = value < 0;
				}))
			{
				Console.WriteLine(elem.value + ", even=" + elem.even + ", negative=" + elem.negative);
			}

			Console.WriteLine();

			LinqCache.FramePassed();
		}
	}
}

}
