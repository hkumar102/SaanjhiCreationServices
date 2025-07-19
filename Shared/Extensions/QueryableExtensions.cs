using System;
using System.Linq;
using System.Linq.Expressions;

namespace Shared.Extensions;


public static class QueryableExtensions
{
    public static IQueryable<T> WhereContainsIgnoreCase<T>(
        this IQueryable<T> source,
        Expression<Func<T, string>> selector,
        string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return source;

        var param = Expression.Parameter(typeof(T), "x");

        // x => x.Field.ToLower().Contains(value.ToLower())
        var propertyAccess = Expression.Invoke(selector, param);
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);

        var loweredProperty = Expression.Call(propertyAccess, toLowerMethod!);
        var loweredValue = Expression.Constant(value.ToLower());

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
        var containsCall = Expression.Call(loweredProperty, containsMethod!, loweredValue);

        var lambda = Expression.Lambda<Func<T, bool>>(containsCall, param);

        return source.Where(lambda);
    }

    public static IQueryable<T> WhereEqualsIgnoreCase<T>(
        this IQueryable<T> source,
        Expression<Func<T, string>> selector,
        string value)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Invoke(selector, param);
        var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

        var left = Expression.Call(property, toLower);
        var right = Expression.Constant(value.ToLower());

        var equals = Expression.Equal(left, right);
        var lambda = Expression.Lambda<Func<T, bool>>(equals, param);

        return source.Where(lambda);
    }
}