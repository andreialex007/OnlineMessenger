using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineMessenger.Domain.Tests.Unit
{
    public static class LinqExtensions
    {
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            return !target.Except(source).Any();
        }

        public static IOrderedEnumerable<TSource> OrderBy<TSource, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TValue> selector,
            bool asc)
        {
            return asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
        }
    }
}
