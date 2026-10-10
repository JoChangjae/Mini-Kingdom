using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;
using MiniKingdom.Utils;

namespace MiniKingdom.Kingdom
{
    [Serializable]
    public class BuildingSaveEntry
    {
        public string buildingId;
        public int level;
    }

    [Serializable]
    public class BuildingSaveDataContainer
    {
        public List<BuildingSaveEntry> list = new List<BuildingSaveEntry>();
    }

    public class BuildingInstance
    {
        public BuildingData Data;
        public int Level;
        public bool IsConstructing;
        public float ConstructionFinishTime;

        public string GetEffectDescription()
        {
            if (Data == null) return string.Empty;
            return Data.buildingId switch
            {
                "building_blacksmith" => $"플레이어 공격력 +{5 * Level} (다음 Lv: +{5 * (Level + 1)})",
                "building_barracks" => $"플레이어 체력 +{25 * Level}, 방어력 +{2 * Level} (다음 Lv: +{25 * (Level + 1)} / +{2 * (Level + 1)})",
                "building_magic_tower" => $"플레이어 공격 속도 +{10 * Level}% (다음 Lv: +{10 * (Level + 1)}%)",
                "building_farm" => $"던전 체력 +{15 * Level}, 방치 세금 생산 증가",
                _ => $"왕국 버프 효과 적용 중 (Lv.{Level})"
            };
        }
    }

    /// <summary>
    /// Manages kingdom buildings, upgrades, persistent saving, and dungeon buffs.
    /// </summary>
    public class BuildingManager : Singleton<BuildingManager>
    {
        private List<BuildingInstance> _buildings = new List<BuildingInstance>();

        public IReadOnlyList<BuildingInstance> Buildings => _buildings;
        public event Action OnBuildingsUpdated;

        private void Start()
        {
            InitializeBuildings();
        }

        public void InitializeBuildings()
        {
            if (_buildings.Count > 0) return;

            // Load from Resources/Data or create robust defaults
            var blacksmith = CreateOrLoadData("building_blacksmith", "대장간", "무기를 제련하여 플레이어의 공격력을 영구 강화합니다.", BuildingCategory.Military,
                new StatModifier { statType = StatType.ATK, value = 5f, isPercentage = false, source = ModifierSource.Building });

            var barracks = CreateOrLoadData("building_barracks", "훈련소", "기초 체력과 방어력을 훈련하여 생존력을 영구 강화합니다.", BuildingCategory.Military,
                new StatModifier { statType = StatType.HP, value = 25f, isPercentage = false, source = ModifierSource.Building },
                new StatModifier { statType = StatType.DEF, value = 2f, isPercentage = false, source = ModifierSource.Building });

            var magicTower = CreateOrLoadData("building_magic_tower", "마법탑", "비전 마법을 연구하여 공격 속도를 영구 상승시킵니다.", BuildingCategory.Magic,
                new StatModifier { statType = StatType.SPD, value = 0.10f, isPercentage = true, source = ModifierSource.Building });

            var farm = CreateOrLoadData("building_farm", "농장", "식량을 공급하여 체력을 보강하고 왕국의 경제력을 증대시킵니다.", BuildingCategory.Production,
                new StatModifier { statType = StatType.HP, value = 15f, isPercentage = false, source = ModifierSource.Building });

            _buildings.Add(new BuildingInstance { Data = blacksmith, Level = 1 });
            _buildings.Add(new BuildingInstance { Data = barracks, Level = 1 });
            _buildings.Add(new BuildingInstance { Data = magicTower, Level = 1 });
            _buildings.Add(new BuildingInstance { Data = farm, Level = 1 });

            LoadSavedLevels();
            OnBuildingsUpdated?.Invoke();
        }

        private BuildingData CreateOrLoadData(string id, string name, string desc, BuildingCategory cat, params StatModifier[] mods)
        {
            var loaded = Resources.Load<BuildingData>($"Buildings/{id}");
            if (loaded != null) return loaded;

            var data = ScriptableObject.CreateInstance<BuildingData>();
            data.buildingId = id;
            data.buildingName = name;
            data.description = desc;
            data.category = cat;
            data.maxLevel = 5;
            data.BuffModifiers = new List<StatModifier>(mods);
            return data;
        }

        public int GetTotalKingdomLevel()
        {
            return Mathf.Max(1, _buildings.Sum(b => b.Level));
        }

        public BuildingInstance GetBuilding(string buildingId)
        {
            return _buildings.FirstOrDefault(b => b.Data != null && b.Data.buildingId == buildingId);
        }

        public int GetGoldCost(BuildingInstance inst) => 150 * inst.Level;
        public int GetSecondaryCost(BuildingInstance inst) => 50 * inst.Level;
        public ResourceType GetSecondaryType(BuildingInstance inst)
        {
            if (inst.Data != null && (inst.Data.buildingId.Contains("blacksmith") || inst.Data.buildingId.Contains("barracks")))
            {
                return ResourceType.Stone;
            }
            return ResourceType.Wood;
        }

        public bool TryUpgradeBuilding(BuildingInstance inst, out string message)
        {
            if (inst == null)
            {
                message = "유효하지 않은 건물입니다.";
                return false;
            }

            if (inst.Level >= inst.Data.MaxLevel)
            {
                message = $"{inst.Data.buildingName}은(는) 이미 최고 레벨(Lv.{inst.Data.MaxLevel})입니다!";
                return false;
            }

            int goldCost = GetGoldCost(inst);
            int secCost = GetSecondaryCost(inst);
            ResourceType secType = GetSecondaryType(inst);
            string secName = secType == ResourceType.Stone ? "석재" : "목재";

            if (ResourceManager.Instance != null)
            {
                if (!ResourceManager.Instance.HasResource(ResourceType.Gold, goldCost))
                {
                    message = $"골드가 부족합니다! (필요: {goldCost} 골드)";
                    return false;
                }

                if (!ResourceManager.Instance.HasResource(secType, secCost))
                {
                    message = $"{secName}가 부족합니다! (필요: {secCost} {secName})";
                    return false;
                }

                // 자원 소모
                ResourceManager.Instance.ConsumeResource(ResourceType.Gold, goldCost);
                ResourceManager.Instance.ConsumeResource(secType, secCost);
            }

            // 즉시 레벨업
            inst.Level++;
            SaveBuildings();
            OnBuildingsUpdated?.Invoke();

            message = $"🎉 {inst.Data.buildingName} Lv.{inst.Level} 업그레이드 완료!\n{inst.GetEffectDescription()}";
            Debug.Log($"[BuildingManager] {message}");
            return true;
        }

        public List<StatModifier> GetAllBuildingBuffs()
        {
            List<StatModifier> buffs = new List<StatModifier>();
            foreach (var b in _buildings)
            {
                if (b.Level > 0 && b.Data != null && b.Data.BuffModifiers != null)
                {
                    foreach (var mod in b.Data.BuffModifiers)
                    {
                        var scaledMod = new StatModifier
                        {
                            statType = mod.statType,
                            value = mod.value * b.Level,
                            isPercentage = mod.isPercentage,
                            source = ModifierSource.Building
                        };
                        buffs.Add(scaledMod);
                    }
                }
            }
            return buffs;
        }

        public void SaveBuildings()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.CurrentSaveData == null) return;

            var container = new BuildingSaveDataContainer();
            foreach (var b in _buildings)
            {
                if (b.Data != null)
                {
                    container.list.Add(new BuildingSaveEntry { buildingId = b.Data.buildingId, level = b.Level });
                }
            }

            SaveManager.Instance.CurrentSaveData.KingdomData = JsonUtility.ToJson(container);
            SaveManager.Instance.SaveGame();
        }

        private void LoadSavedLevels()
        {
            if (SaveManager.Instance == null || SaveManager.Instance.CurrentSaveData == null) return;
            string json = SaveManager.Instance.CurrentSaveData.KingdomData;
            if (string.IsNullOrEmpty(json) || json == "{}") return;

            try
            {
                var container = JsonUtility.FromJson<BuildingSaveDataContainer>(json);
                if (container != null && container.list != null)
                {
                    foreach (var entry in container.list)
                    {
                        var target = GetBuilding(entry.buildingId);
                        if (target != null && entry.level > 0)
                        {
                            target.Level = entry.level;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[BuildingManager] 건물 데이터 로드 중 오류: {e.Message}");
            }
        }
    }
}
