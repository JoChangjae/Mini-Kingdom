using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
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
        
        // 의존성 주입 또는 싱글톤을 통해 가져올 플레이어 스탯 (Player stats reference)
        [SerializeField] private PlayerStats _playerStats;

        public IReadOnlyList<RelicData> CollectedRelics => _collectedRelics;

        /// <summary>
        /// 새로운 유물을 추가하고 스탯을 적용합니다. (Adds a relic and applies its stat modifiers)
        /// </summary>
        /// <param name="relicData">추가할 유물 데이터 (Relic data to add)</param>
        public void AddRelic(RelicData relicData)
        {
            if (relicData == null) return;

            _collectedRelics.Add(relicData);

            // 유물의 스탯 보너스를 플레이어 스탯에 적용 (Apply stat modifiers)
            if (_playerStats != null)
            {
                _playerStats.MaxHp += relicData.HpBonus;
                _playerStats.PhysicalDamageBonus += relicData.PhysicalDamageBonus;
                _playerStats.MagicDamageBonus += relicData.MagicDamageBonus;
                _playerStats.NatureDamageBonus += relicData.NatureDamageBonus;
                // 기타 스탯 적용...
                
                // 플레이어 스탯 갱신 처리
                _playerStats.ApplyModifiers();
            }

            // 유물 획득 이벤트 발행
            EventBus.Publish(new RelicAddedEvent { AddedRelic = relicData });
        }
        
        /// <summary>
        /// 유물에 의한 특별 효과 발동이 필요할 때 호출 (Triggers special effects if needed)
        /// </summary>
        public void TriggerRelicEffects(string triggerType)
        {
            // 특정 시점(예: 전투 시작, 피격 시)에 유물 효과를 발동하는 로직
        }
    }
}
