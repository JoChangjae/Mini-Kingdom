using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.Kingdom
{
    public class BuildingInstance
    {
        public BuildingData Data;
        public int Level;
        public bool IsConstructing;
        public float ConstructionFinishTime;
    }

    /// <summary>
    /// Manages kingdom buildings, construction, and total kingdom level.
    /// </summary>
    public class BuildingManager : Singleton<BuildingManager>
    {
        private List<BuildingInstance> _buildings = new List<BuildingInstance>();

        public int GetTotalKingdomLevel()
        {
            return _buildings.Sum(b => b.Level);
        }

        public void Build(BuildingData data)
        {
            var inst = new BuildingInstance
            {
                Data = data,
                Level = 1, // Or 0 if it needs construction first
                IsConstructing = true,
                ConstructionFinishTime = Time.realtimeSinceStartup + data.BaseBuildTime
            };
            _buildings.Add(inst);
        }

        public void Upgrade(BuildingInstance inst)
        {
            if (!inst.IsConstructing && inst.Level < inst.Data.MaxLevel)
            {
                inst.IsConstructing = true;
                // 시간에 따른 레벨업 배율 적용
                inst.ConstructionFinishTime = Time.realtimeSinceStartup + (inst.Data.BaseBuildTime * inst.Level);
            }
        }

        private void Update()
        {
            foreach (var b in _buildings)
            {
                if (b.IsConstructing && Time.realtimeSinceStartup >= b.ConstructionFinishTime)
                {
                    b.IsConstructing = false;
                    b.Level++;
                    // EventBus.Publish(new BuildingFinishedEvent(b));
                }
            }
        }

        public List<StatModifier> GetAllBuildingBuffs()
        {
            List<StatModifier> buffs = new List<StatModifier>();
            foreach (var b in _buildings)
            {
                if (!b.IsConstructing && b.Data.BuffModifiers != null)
                {
                    // 레벨에 따른 버프 스케일링
                    foreach(var mod in b.Data.BuffModifiers)
                    {
                        var scaledMod = new StatModifier { Type = mod.Type, Value = mod.Value * b.Level, IsFlat = mod.IsFlat };
                        buffs.Add(scaledMod);
                    }
                }
            }
            return buffs;
        }
    }
}
