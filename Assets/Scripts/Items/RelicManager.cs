using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Player;

namespace MiniKingdom.Items
{
    /// <summary>
    /// Event published when a new relic is added.
    /// </summary>
    public struct RelicAddedEvent
    {
        public RelicData AddedRelic;
    }

    /// <summary>
    /// 런 중 획득한 유물들을 관리합니다. (Manages relics collected during a run)
    /// </summary>
    public class RelicManager : MonoBehaviour
    {
        private List<RelicData> _collectedRelics = new List<RelicData>();
        
        [SerializeField] private PlayerStats _playerStats;

        public IReadOnlyList<RelicData> CollectedRelics => _collectedRelics;

        private void Awake()
        {
            if (_playerStats == null)
            {
                _playerStats = FindObjectOfType<PlayerStats>();
            }
        }

        /// <summary>
        /// 새로운 유물을 추가하고 스탯을 적용합니다. (Adds a relic and applies its stat modifiers)
        /// </summary>
        public void AddRelic(RelicData relicData)
        {
            if (relicData == null) return;

            _collectedRelics.Add(relicData);

            // 유물의 스탯 보너스를 플레이어 스탯에 적용
            if (_playerStats != null && relicData.effects != null)
            {
                foreach (var mod in relicData.effects)
                {
                    if (mod != null) _playerStats.AddModifier(mod);
                }
            }

            // 유물 획득 이벤트 발행
            EventBus.Publish(new GameEvents.RelicAcquiredEvent { RelicData = relicData });
            Debug.Log($"[RelicManager] 유물 획득: {relicData.relicName}");
        }

        public void ClearRelics()
        {
            _collectedRelics.Clear();
        }
    }
}
