using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatMash
{
    public static class AsyncLinq
    {
        public static async IAsyncEnumerable<TResult> Select<TSource, TResult>(this IAsyncEnumerable<TSource> sequence, Func<TSource, TResult> selector)
        {
            await foreach(TSource elt in sequence)
            {
                TResult item = selector(elt);
                yield return item;
            }
        }

        public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> sequence)
        {
            List<T> result = new List<T>();

            await foreach(T elt in sequence)
                result.Add(elt);

            return result;
        }
    }
}