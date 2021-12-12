using System;
using System.Collections.Generic;

namespace BTDToolbox.Lib
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> codeToRun)
        {
            foreach (var element in enumerable)
                codeToRun(element);
        }
    }
}
