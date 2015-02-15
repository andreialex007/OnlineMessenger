using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineMessenger.Domain.Extensions
{
    public static class CommonExtensions
    {
        public static T Next<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            bool flag = false;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (flag) return enumerator.Current;

                    if (predicate(enumerator.Current))
                    {
                        flag = true;
                    }
                }
            }
            return source.FirstOrDefault();
        }
    }
}
