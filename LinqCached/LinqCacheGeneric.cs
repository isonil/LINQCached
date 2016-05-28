using System;
using System.Collections.Generic;

namespace LinqCached
{

public static class LinqCache<T, TKey, TCached> where TCached : new()
{
	private class CachedEntry
	{
		public List<TCached> list = new List<TCached>();
		public int leftToCache;
		public int previouslyUsedCacheDuration;

		public void TryRecache(IEnumerable<TCached> source, int cacheDuration)
		{
			if( leftToCache <= 0 )
			{
				list.Clear();
				list.AddRange(source);

				leftToCache = cacheDuration;
				previouslyUsedCacheDuration = cacheDuration;
			}

			if( LinqCache.AutomaticMode )
				--leftToCache;
		}

		public void TryRecache(IEnumerable<T> source, int cacheDuration, Func<T, TCached> selector)
		{
			if( leftToCache <= 0 )
			{
				list.Clear();

				foreach( var elem in source )
				{
					list.Add(selector(elem));
				}

				leftToCache = cacheDuration;
				previouslyUsedCacheDuration = cacheDuration;
			}

			if( LinqCache.AutomaticMode )
				--leftToCache;
		}

		public void TryRecache(IEnumerable<T> source, int cacheDuration, Action<T, TCached> updater)
		{
			if( leftToCache <= 0 )
			{
				int count = 0;
				int listCount = list.Count;

				foreach( var elem in source )
				{
					if( count < listCount )
						updater(elem, list[count]);
					else
					{
						var newEntry = new TCached();
						updater(elem, newEntry);
						list.Add(newEntry);
					}

					++count;
				}

				list.RemoveRange(count, list.Count - count);

				leftToCache = cacheDuration;
				previouslyUsedCacheDuration = cacheDuration;
			}

			if( LinqCache.AutomaticMode )
				--leftToCache;
		}
	}

	private static Dictionary<TKey, CachedEntry> cached = new Dictionary<TKey, CachedEntry>();
	private static List<TKey> toRemoveWorkingList = new List<TKey>();

	static LinqCache()
	{
		LinqCache.AddFramePassedAction(FramePassed);
		LinqCache.AddSetAllDirtyAction(SetAllDirty);
		LinqCache.AddClearAction(Clear);
	}

	public static void FramePassed()
	{
		foreach( var elem in cached )
		{
			--elem.Value.leftToCache;
			CheckShouldRemove(elem);
		}

		RemoveToRemove();
	}

	public static void FramePassedFor(TKey key)
	{
		if( key == null )
			throw new ArgumentNullException("key");

		CachedEntry elem;

		if( !cached.TryGetValue(key, out elem) )
			throw new InvalidOperationException("key not found");

		--elem.leftToCache;

		if( ShouldRemove(elem) )
			cached.Remove(key);
	}

	public static void FramePassedFor(Predicate<TKey> keyPredicate)
	{
		if( keyPredicate == null )
			throw new ArgumentNullException("keyPredicate");

		foreach( var elem in cached )
		{
			if( keyPredicate(elem.Key) )
			{
				--elem.Value.leftToCache;
				CheckShouldRemove(elem);
			}
		}

		RemoveToRemove();
	}

	public static void SetAllDirty()
	{
		foreach( var elem in cached )
		{
			var value = elem.Value;

			if( value.leftToCache > 0 )
				value.leftToCache = 0;
		}
	}
	
	public static void SetDirty(TKey key)
	{
		if( key == null )
			throw new ArgumentNullException("key");

		CachedEntry elem;

		if( !cached.TryGetValue(key, out elem) )
			throw new InvalidOperationException("key not found");

		if( elem.leftToCache > 0 )
			elem.leftToCache = 0;
	}

	public static void SetDirtyWhere(Predicate<TKey> keyPredicate)
	{
		if( keyPredicate == null )
			throw new ArgumentNullException("keyPredicate");

		foreach( var elem in cached )
		{
			var value = elem.Value;

			if( value.leftToCache > 0 && keyPredicate(elem.Key) )
				value.leftToCache = 0;
		}
	}

	public static void Clear()
	{
		cached.Clear();
	}

	internal static List<TCached> GetCachedEntryList(IEnumerable<TCached> source, TKey key, int cacheDuration)
	{
		CachedEntry entry;

		if( cached.TryGetValue(key, out entry) )
		{
			entry.TryRecache(source, cacheDuration);
			return entry.list;
		}

		entry = new CachedEntry();
		entry.TryRecache(source, cacheDuration);
		cached.Add(key, entry);

		return entry.list;
	}

	internal static List<TCached> GetCachedEntryList(IEnumerable<T> source, TKey key, int cacheDuration, Func<T, TCached> selector)
	{
		CachedEntry entry;

		if( cached.TryGetValue(key, out entry) )
		{
			entry.TryRecache(source, cacheDuration, selector);
			return entry.list;
		}

		entry = new CachedEntry();
		entry.TryRecache(source, cacheDuration, selector);
		cached.Add(key, entry);

		return entry.list;
	}

	internal static List<TCached> GetCachedEntryList(IEnumerable<T> source, TKey key, int cacheDuration, Action<T, TCached> updater)
	{
		CachedEntry entry;

		if( cached.TryGetValue(key, out entry) )
		{
			entry.TryRecache(source, cacheDuration, updater);
			return entry.list;
		}

		entry = new CachedEntry();
		entry.TryRecache(source, cacheDuration, updater);
		cached.Add(key, entry);

		return entry.list;
	}

	private static bool ShouldRemove(CachedEntry cachedEntry)
	{
		return cachedEntry.leftToCache < -cachedEntry.previouslyUsedCacheDuration - 1;
	}

	private static void CheckShouldRemove(KeyValuePair<TKey, CachedEntry> cachedEntry)
	{
		if( ShouldRemove(cachedEntry.Value) )
			toRemoveWorkingList.Add(cachedEntry.Key);
	}

	private static void RemoveToRemove()
	{
		int count = toRemoveWorkingList.Count;

		if( count == cached.Count )
			cached.Clear();
		else
		{
			for( int i = 0; i < count; ++i )
			{
				cached.Remove(toRemoveWorkingList[i]);
			}
		}

		toRemoveWorkingList.Clear();
	}
}

}
