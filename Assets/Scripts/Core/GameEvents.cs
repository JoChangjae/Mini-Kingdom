using UnityEngine;

namespace MiniKingdom.Core
{
    // 리소스 타입
    public enum ResourceType { Gold, Wood, Stone, Gem }
    // 시너지 타입
    public enum SynergyType { None, FireAttack, FastMovement }
    // 왕국 칙령 타입
    public enum DecreeType { None, WoodDay, WarriorDay, MerchantDay, ScholarDay, BuilderDay, HealerDay, ExplorerDay, FortuneDay }

    public struct ResourceChangedEvent
    {
        public ResourceType ResourceType;
        public int OldAmount;
        public int NewAmount;
    }

    public struct BuildingBuiltEvent
    {
        public string BuildingData;
        public int Level;
    }

    public struct BuildingUpgradedEvent
    {
        public string BuildingData;
        public int OldLevel;
        public int NewLevel;
    }

    public struct DungeonEnteredEvent
    {
        public string DungeonData;
    }

    public struct RoomEnteredEvent
    {
        public string RoomType;
        public int RoomIndex;
    }

    public struct EnemyKilledEvent
    {
        public string EnemyData;
        public Vector3 Position;
    }

    public struct BossKilledEvent
    {
        public string EnemyData;
    }

    public struct PlayerLevelUpEvent
    {
        public int NewLevel;
        public string[] Choices;
    }

    public struct UpgradeChosenEvent
    {
        public string UpgradeData;
    }

    public struct SynergyActivatedEvent
    {
        public SynergyType SynergyType;
    }

    public struct PlayerDamagedEvent
    {
        public float Damage;
        public float RemainingHP;
    }

    public struct PlayerHealedEvent
    {
        public float Amount;
        public float CurrentHP;
    }

    public struct PerfectDodgeEvent { }

    public struct RunCompletedEvent
    {
        public bool Success;
        public string Stats;
    }

    public struct DailyBonusRunEvent
    {
        public int RunNumber;
        public bool IsBonus;
    }

    public struct RoyalDecreeChosenEvent
    {
        public DecreeType DecreeType;
    }

    public struct DiscoveryUnlockedEvent
    {
        public string EntryType;
        public string EntryId;
    }

    public struct KingdomDefenseEvent
    {
        public bool Success;
        public int Score;
    }
    public struct WeatherChangedEvent
    {
        public MiniKingdom.Data.WeatherType WeatherType;
    }

    public struct TaxCollectedEvent
    {
        public int GoldAmount;
        public int ResourceAmount;
    }

    public struct RelicAcquiredEvent
    {
        public MiniKingdom.Data.RelicData RelicData;
    }

    public struct FishCaughtEvent
    {
        public string FishType;
        public bool IsSuccess;
    }

    public struct SkillFusionEvent
    {
        public MiniKingdom.Data.SkillFusionData FusionData;
    }
}
