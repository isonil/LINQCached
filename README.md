# About
IEnumerable extension methods which provide easy frame-based object caching.

LinqCache.FramePassed() must be called every frame.

LINQCached caches IEnumerable elements and stores them in a temporary list which it then keeps returning for the next X frames instead of enumerating over the IEnumerable elements over again. This can be useful in the following scenarios:

```C#
foreach( var elem in collection
  .Where(x => ExpensiveCheck(x))
  .Cached(60, 0)
  .Where(x => QuickPostCacheCheck(x)) )
{
  // ...
}
```

It can also be used to cache additional info about the elements:

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
