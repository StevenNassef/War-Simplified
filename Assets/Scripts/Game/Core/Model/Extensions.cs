using System;
using System.Collections.Generic;

namespace Game.Core.Model
{
    public static class Extensions
    {
        public static int GetUniqueHighestItemByIndex<T>(this IReadOnlyList<T> items) where T : IComparable<T>
        {
            return GetUniqueHighestItemByIndex(items, out _);
        }

        public static int GetUniqueHighestItemByIndex<T>(this IReadOnlyList<T> items, out T highestItem) where T : IComparable<T>
        {
            if (items == null || items.Count == 0)
            {
                highestItem = default;
                return -1;
            }
                
            highestItem = items[0];
            var highestIndex = 0;
            var highestValueCount = 1;
            
            for (var i = 1; i < items.Count; i++)
            {
                var compareResult = items[i].CompareTo(items[highestIndex]);
                switch (compareResult)
                {
                    case > 0:
                        highestIndex = i;
                        highestValueCount = 1;
                        break;
                    case 0:
                        highestValueCount++;
                        break;
                }
            }

            if (highestValueCount > 1)
            {
                highestIndex = -1;
                highestItem = default;
            }
            else
            {
                highestItem = items[highestIndex];
            }
            
            return highestIndex;
        }

        public static int GetUniqueHighestItemByIndex<T>(this IReadOnlyList<T> items, IComparer<T> comparer)
        {
            return GetUniqueHighestItemByIndex(items, comparer, out _);
        }

        public static int GetUniqueHighestItemByIndex<T>(this IReadOnlyList<T> items, IComparer<T> comparer, out T highestItem)
        {
            if (items == null || items.Count == 0)
            {
                highestItem = default;
                return -1;
            }
                
            highestItem = items[0];
            var highestIndex = 0;
            var highestValueCount = 1;
            
            for (var i = 1; i < items.Count; i++)
            {
                var compareResult = comparer.Compare(items[i], items[highestIndex]);
                switch (compareResult)
                {
                    case > 0:
                        highestIndex = i;
                        highestValueCount = 1;
                        break;
                    case 0:
                        highestValueCount++;
                        break;
                }
            }

            if (highestValueCount > 1)
            {
                highestIndex = -1;
                highestItem = default;
            }
            else
            {
                highestItem = items[highestIndex];
            }
            
            return highestIndex;
        }
    }
}