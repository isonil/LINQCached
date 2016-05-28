# About
IEnumerable extension which allows easy frame-based caching.

LinqCache.FramePassed() must be called each frame.

LINQCached caches IEnumerable elements and stores them in a list which it then keeps returning for the next X frames instead of enumerating over IEnumerable elements over again. Useful in cases like this:

```C#
foreach( var elem in collection
  .Where(x => ExpensiveCheck(x))
  .Cached(60, 0)
  .Where(x => QuickPostCacheCheck(x)) )
{
  // ...
}
```

It can also be used to cache additional info about elements:

```C#
foreach( var cachedElemInfo in collection
  .Cached(60, 0, x => return new CachedInfo(x)) )
{
  // ...
}
```

or

```C#
foreach( var cachedElemInfo in collection
  .Cached(60, 0, (Foo foo, CachedFoo cachedFoo) => cachedFoo.attribute = ExpensiveFooAttributeCalculation(foo)) )
{
  // ...
}
```

LINQCached tries to minimize memory allocations.
