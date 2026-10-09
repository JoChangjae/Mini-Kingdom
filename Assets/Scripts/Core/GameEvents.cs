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

        public ResourceChangedEvent(ResourceType type, int newAmount, int oldAmount = 0)
        {
            ResourceType = type;
            NewAmount = newAmount;
            OldAmount = oldAmount;
        }

        public ResourceChangedEvent(MiniKingdom.Data.ResourceType type, int newAmount, int oldAmount = 0)
        {
            ResourceType = (ResourceType)(int)type;
            NewAmount = newAmount;
            OldAmount = oldAmount;
        }
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

    public struct RoomClearedEvent { }

    public struct PlayerDiedEvent { }

    public struct EnemyKilledEvent
    {
        public string EnemyData;
        public Vector3 Position;
        public Enemy.EnemyController EnemyController;

        public EnemyKilledEvent(Enemy.EnemyController enemy)
        {
            EnemyController = enemy;
            EnemyData = enemy != null ? enemy.name : string.Empty;
            Position = enemy != null ? enemy.transform.position : Vector3.zero;
        }

        public EnemyKilledEvent(string enemyData, Vector3 position)
        {
            EnemyController = null;
            EnemyData = enemyData;
            Position = position;
        }
    }

    public struct BossKilledEvent
    {
        public string EnemyData;
        public BossKilledEvent(string enemyData = "")
        {
            EnemyData = enemyData;
        }
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

    public struct RunEndedEvent
    {
        public MiniKingdom.Dungeon.RunResult Result;
        public RunEndedEvent(MiniKingdom.Dungeon.RunResult result)
        {
            Result = result;
        }
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
        public MiniKingdom.Data.DiscoveryBookEntry Entry;

        public DiscoveryUnlockedEvent(MiniKingdom.Data.DiscoveryBookEntry entry)
        {
            Entry = entry;
            EntryType = entry != null ? entry.category.ToString() : string.Empty;
            EntryId = entry != null ? entry.entryId : string.Empty;
        }

        public DiscoveryUnlockedEvent(string entryType, string entryId)
        {
            Entry = null;
            EntryType = entryType;
            EntryId = entryId;
        }
    }

    public struct KingdomDefenseEvent
    {
        public bool Success;
        public int Score;
    }

    public struct WeatherChangedEvent
    {
        public MiniKingdom.Data.WeatherType WeatherType;
        public Sprite WeatherSprite;

        public WeatherChangedEvent(MiniKingdom.Data.WeatherType type, Sprite sprite = null)
        {
            WeatherType = type;
            WeatherSprite = sprite;
        }
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
