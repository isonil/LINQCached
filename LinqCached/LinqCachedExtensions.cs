using System;
using System.Collections.Generic;

namespace LinqCached
{

public static class LinqCachedExtensions
{
	public static IEnumerable<T> Cached<T, TKey>(this IEnumerable<T> source, int cacheDuration, TKey key) where T : new()
	{
		if( source == null )
			throw new ArgumentNullException("source");

		if( key == null )
			throw new ArgumentNullException("key");

		return LinqCache<T, TKey, T>.GetCachedEntryList(source, key, cacheDuration);
	}

	public static IEnumerable<TCached> Cached<T, TKey, TCached>(this IEnumerable<T> source, int cacheDuration, TKey key, Func<T, TCached> selector) where TCached : new()
	{
		if( source == null )
			throw new ArgumentNullException("source");

		if( key == null )
			throw new ArgumentNullException("key");

		if( selector == null )
			throw new ArgumentNullException("selector");

		return LinqCache<T, TKey, TCached>.GetCachedEntryList(source, key, cacheDuration, selector);
	}

	public static IEnumerable<TCached> Cached<T, TKey, TCached>(this IEnumerable<T> source, int cacheDuration, TKey key, Action<T, TCached> updater) where TCached : new()
	{
		if( source == null )
			throw new ArgumentNullException("source");

		if( key == null )
			throw new ArgumentNullException("key");

		if( updater == null )
			throw new ArgumentNullException("updater");

		return LinqCache<T, TKey, TCached>.GetCachedEntryList(source, key, cacheDuration, updater);
	}
}

}
