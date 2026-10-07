using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniKingdom.Utils
{
    /// <summary>
    /// Useful extension methods for Unity and C#.
    /// </summary>
    public static class Extensions
    {
        private static System.Random _rng = new System.Random();

        /// <summary>
        /// Gets a random element from the list.
        /// </summary>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[_rng.Next(list.Count)];
        }

        /// <summary>
        /// Shuffles the list in place using Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = _rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }
        }

        /// <summary>
        /// Destroys all children of the transform.
        /// </summary>
        public static void DestroyAllChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Remaps a float value from one range to another.
        /// </summary>
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Returns a random point in a circle with the given radius.
        /// </summary>
        public static Vector2 RandomPointInCircle(this Vector2 center, float radius)
        {
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle;
            return center + randomDir * radius;
        }

        /// <summary>
        /// Selects an item based on weighted random probability.
        /// </summary>
        public static T WeightedRandom<T>(this IEnumerable<T> items, Func<T, float> weightSelector)
        {
            float totalWeight = items.Sum(weightSelector);
            float randomValue = (float)_rng.NextDouble() * totalWeight;

            float currentWeight = 0;
            foreach (var item in items)
            {
                currentWeight += weightSelector(item);
                if (randomValue <= currentWeight)
                {
                    return item;
                }
            }

            return items.FirstOrDefault();
        }

        /// <summary>
        /// Gets a component of the given type, or adds it if it doesn't exist.
        /// </summary>
        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }
    }
}
