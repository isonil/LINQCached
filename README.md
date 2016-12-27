# About
IEnumerable extension to do frame-based caching easily.

LinqCache.FramePassed() must be called each frame.

LINQCached caches IEnumerable elements and stores them in a list which it then keeps returning for the next X frames instead of enumerating over the IEnumerable elements over again. It can be useful in such situations:

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
