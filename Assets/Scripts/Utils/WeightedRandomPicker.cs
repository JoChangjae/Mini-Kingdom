using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniKingdom.Utils
{
    /// <summary>
    /// A generic class for picking items based on their assigned weights.
    /// </summary>
    /// <typeparam name="T">Type of the item.</typeparam>
    public class WeightedRandomPicker<T>
    {
        private class Entry
        {
            public T Item;
            public float Weight;
        }

        private List<Entry> _entries = new List<Entry>();
        private Random _rng = new Random();

        public void AddItem(T item, float weight)
        {
            if (weight <= 0) return;
            _entries.Add(new Entry { Item = item, Weight = weight });
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public T Pick()
        {
            if (_entries.Count == 0) return default;

            float totalWeight = _entries.Sum(e => e.Weight);
            float randomVal = (float)_rng.NextDouble() * totalWeight;

            float currentWeight = 0;
            foreach (var entry in _entries)
            {
                currentWeight += entry.Weight;
                if (randomVal <= currentWeight)
                {
                    return entry.Item;
                }
            }

            return _entries.Last().Item;
        }

        /// <summary>
        /// Picks multiple items without repeating them (non-repeat picks).
        /// </summary>
        public List<T> PickMultiple(int count)
        {
            List<T> results = new List<T>();
            if (count <= 0 || _entries.Count == 0) return results;

            // 임시 복사본 생성하여 뽑힌 항목 제거하면서 진행
            List<Entry> tempEntries = new List<Entry>(_entries);

            int iterations = Math.Min(count, tempEntries.Count);
            for (int i = 0; i < iterations; i++)
            {
                float totalWeight = tempEntries.Sum(e => e.Weight);
                float randomVal = (float)_rng.NextDouble() * totalWeight;

                float currentWeight = 0;
                for (int j = 0; j < tempEntries.Count; j++)
                {
                    currentWeight += tempEntries[j].Weight;
                    if (randomVal <= currentWeight)
                    {
                        results.Add(tempEntries[j].Item);
                        tempEntries.RemoveAt(j); // 중복 방지
                        break;
                    }
                }
            }

            return results;
        }
    }
}
