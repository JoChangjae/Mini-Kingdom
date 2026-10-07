using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Kingdom
{
    /// <summary>
    /// Encyclopedia system tracking discoveries.
    /// </summary>
    public class DiscoveryBookManager : Singleton<DiscoveryBookManager>
    {
        private HashSet<string> _unlockedEntries = new HashSet<string>();

        public void UnlockEntry(DiscoveryBookEntry entry)
        {
            if (entry != null && !_unlockedEntries.Contains(entry.Id))
            {
                _unlockedEntries.Add(entry.Id);
                CheckMilestones();
                EventBus.Publish(new GameEvents.DiscoveryUnlockedEvent(entry));
            }
        }

        private void CheckMilestones()
        {
            int count = _unlockedEntries.Count;
            // 특정 개수 도달 시 보너스 적용 로직
            if (count % 10 == 0)
            {
                Debug.Log("Milestone reached! Apply global stat bonus.");
            }
        }

        public float GetCompletionPercentage(int totalEntries)
        {
            if (totalEntries == 0) return 0f;
            return (float)_unlockedEntries.Count / totalEntries * 100f;
        }
    }
}
