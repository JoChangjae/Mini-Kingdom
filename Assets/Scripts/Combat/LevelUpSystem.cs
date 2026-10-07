using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Utils;

namespace MiniKingdom.Combat
{
    /// <summary>
    /// In-run level up and upgrade choice system.
    /// </summary>
    public class LevelUpSystem : MonoBehaviour
    {
        [SerializeField] private int maxLevel = 15;
        [SerializeField] private List<UpgradeData> allUpgradesPool;
        
        private int _currentLevel = 1;
        private float _currentXP = 0;
        private float _xpToNextLevel = 100f;

        private Dictionary<string, int> _synergyCounts = new Dictionary<string, int>();
        private List<UpgradeData> _activeUpgrades = new List<UpgradeData>();

        public void AddXP(float amount)
        {
            if (_currentLevel >= maxLevel) return;

            _currentXP += amount;
            if (_currentXP >= _xpToNextLevel)
            {
                _currentXP -= _xpToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            _currentLevel++;
            _xpToNextLevel *= 1.2f; // XP requirement scales up

            // Pause game
            Time.timeScale = 0f;

            // Generate choices
            int choiceCount = HasLibraryBuff() ? 4 : 3;
            var choices = GenerateChoices(choiceCount);

            // TODO: UI에 choices를 전달하고 유저 선택 대기
            // EventBus.Publish(new LevelUpEvent(choices));
        }

        private List<UpgradeData> GenerateChoices(int count)
        {
            var available = allUpgradesPool.Where(u => 
                u.RequiredLevel <= _currentLevel && 
                (u.IsStackable || !_activeUpgrades.Contains(u))
            ).ToList();

            var choices = new List<UpgradeData>();
            var picker = new WeightedRandomPicker<UpgradeData>();
            foreach (var u in available) picker.Add(u, u.Weight);

            for (int i = 0; i < count; i++)
            {
                if (available.Count == 0) break;
                var picked = picker.PickRandom();
                choices.Add(picked);
                picker.Remove(picked); // 중복 선택 방지
            }

            return choices;
        }

        public void SelectUpgrade(UpgradeData upgrade)
        {
            _activeUpgrades.Add(upgrade);

            // Apply stats
            var pStats = FindObjectOfType<Player.PlayerStats>();
            if (pStats != null)
            {
                foreach (var mod in upgrade.Modifiers) pStats.AddModifier(mod);
            }

            // 시너지 카운트
            if (!string.IsNullOrEmpty(upgrade.SynergyTag))
            {
                if (!_synergyCounts.ContainsKey(upgrade.SynergyTag)) _synergyCounts[upgrade.SynergyTag] = 0;
                _synergyCounts[upgrade.SynergyTag]++;
                
                CheckSynergies(upgrade.SynergyTag);
            }

            Time.timeScale = 1f; // Resume
        }

        private void CheckSynergies(string tag)
        {
            // 특정 태그 카운트가 임계치에 도달하면 시너지 효과 발동
            // SynergyData 필요
        }

        private bool HasLibraryBuff()
        {
            // 왕립도서관 효과 체크
            return false;
        }
    }
}
