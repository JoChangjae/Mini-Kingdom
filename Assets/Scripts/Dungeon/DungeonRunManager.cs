using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Combat;
using MiniKingdom.Kingdom;
using MiniKingdom.Player;
using MiniKingdom.UI;
using MiniKingdom.Utils;

namespace MiniKingdom.Dungeon
{
    public class RunResult
    {
        public bool IsVictory;
        public int RoomsCleared;
        public int EnemiesKilled;
        public int GoldReward;
        public int WoodReward;
        public int StoneReward;
        public bool IsDailyBonusApplied;
    }

    /// <summary>
    /// Manages the full lifecycle of a dungeon run.
    /// </summary>
    public class DungeonRunManager : Singleton<DungeonRunManager>
    {
        private DungeonGenerator _generator;
        private RoomConfig _currentRoom;
        private RunResult _result;

        private int _accumulatedGold;
        private int _accumulatedWood;
        private int _accumulatedStone;

        public RunResult CurrentResult => _result;

        private void Start()
        {
            _generator = GetComponent<DungeonGenerator>();
            StartRun();
        }

        public void StartRun()
        {
            _result = new RunResult();
            _accumulatedGold = 0;
            _accumulatedWood = 0;
            _accumulatedStone = 0;
            
            // 리셋 런 스탯
            CombatSystem.Instance?.ResetRunStats();
            LevelUpSystem.Instance?.ResetSkillsForRun();

            // Apply kingdom and decree buffs
            ApplyPreRunBuffs();

            // Generate layout
            if (_generator != null)
            {
                _currentRoom = _generator.Generate();
                LoadRoom(_currentRoom);
            }
            else
            {
                _generator = gameObject.AddComponent<DungeonGenerator>();
                _currentRoom = _generator.Generate();
                LoadRoom(_currentRoom);
            }

            Debug.Log("[DungeonRunManager] 새로운 던전 런 시작!");
        }

        private void ApplyPreRunBuffs()
        {
            var playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null) return;

            // 1. 왕국 건물 버프 적용
            if (BuildingManager.Instance != null)
            {
                var buildingBuffs = BuildingManager.Instance.GetAllBuildingBuffs();
                foreach (var buff in buildingBuffs)
                {
                    playerStats.AddModifier(buff);
                }
            }

            // 2. 왕의 칙령 버프 적용 (예: 전사의 날 -> ATK +20%)
            var decreeSys = FindObjectOfType<RoyalDecreeSystem>();
            if (decreeSys != null && decreeSys.ActiveDecree == DecreeType.WarriorDay)
            {
                playerStats.AddModifier(new Data.StatModifier
                {
                    statType = Data.StatType.ATK,
                    value = 0.2f,
                    isPercentage = true
                });
                Debug.Log("[DungeonRunManager] 전사의 날 칙령 버프(공격력 +20%) 적용됨");
            }
        }

        public void AddCollectedResources(int gold, int wood, int stone)
        {
            _accumulatedGold += gold;
            _accumulatedWood += wood;
            _accumulatedStone += stone;
        }

        private void LoadRoom(RoomConfig room)
        {
            if (room == null) return;

            var roomManager = FindObjectOfType<RoomManager>();
            if (roomManager != null)
            {
                roomManager.Initialize(room);
            }
        }

        public void MoveToNextRoom(bool choosePathB = false)
        {
            if (_result == null) _result = new RunResult();
            _result.RoomsCleared++;

            if (_currentRoom != null && _currentRoom.Type == RoomType.Boss)
            {
                EndRun(true);
                return;
            }

            if (_currentRoom != null)
            {
                _currentRoom = choosePathB && _currentRoom.IsBranch ? _currentRoom.NextB : _currentRoom.NextA;
                if (_currentRoom != null)
                {
                    LoadRoom(_currentRoom);
                    return;
                }
            }

            // 더 이상 방이 없으면 승리 종료
            EndRun(true);
        }

        public void EndRun(bool isVictory)
        {
            if (_result == null) _result = new RunResult();
            _result.IsVictory = isVictory;
            _result.EnemiesKilled = CombatSystem.Instance != null ? CombatSystem.Instance.KillsThisRun : 0;

            // 패배 시 획득 자원의 70% 유지 (GDD 8.1 캐주얼 배려)
            float failRetention = isVictory ? 1.0f : 0.7f;

            // 일일 5회 보너스 적용 여부 (1.5배)
            float dailyBonusMultiplier = 1.5f; 
            _result.IsDailyBonusApplied = true;

            float totalMultiplier = failRetention * dailyBonusMultiplier;

            _result.GoldReward = Mathf.RoundToInt(_accumulatedGold * totalMultiplier);
            _result.WoodReward = Mathf.RoundToInt(_accumulatedWood * totalMultiplier);
            _result.StoneReward = Mathf.RoundToInt(_accumulatedStone * totalMultiplier);

            // 실제 왕국 자원 금고에 지급
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddResource(Data.ResourceType.Gold, _result.GoldReward);
                ResourceManager.Instance.AddResource(Data.ResourceType.Wood, _result.WoodReward);
                ResourceManager.Instance.AddResource(Data.ResourceType.Stone, _result.StoneReward);
            }

            // 자동 저장
            SaveManager.Instance?.SaveGame();

            // 런 완료 이벤트 발행
            EventBus.Publish(new RunCompletedEvent { Success = isVictory, Stats = $"Rooms: {_result.RoomsCleared}" });
            EventBus.Publish(new RunEndedEvent(_result));

            // 결과 화면 표시
            if (UIManager.Instance != null)
            {
                var resultScreen = FindObjectOfType<RunResultScreen>(true);
                if (resultScreen != null)
                {
                    resultScreen.SetupResult(isVictory);
                }
                UIManager.Instance.Show(ScreenType.RunResult, false);
            }

            Debug.Log($"[DungeonRunManager] 런 종료! 승리: {isVictory}, 골드: {_result.GoldReward}, 목재: {_result.WoodReward}");
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            EndRun(false);
        }
    }
}
