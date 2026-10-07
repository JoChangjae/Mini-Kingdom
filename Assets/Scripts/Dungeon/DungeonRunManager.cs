using UnityEngine;
using MiniKingdom.Core;

namespace MiniKingdom.Dungeon
{
    public class RunResult
    {
        public bool IsVictory;
        public int RoomsCleared;
        public int EnemiesKilled;
        // Resources...
    }

    /// <summary>
    /// Manages the full lifecycle of a dungeon run.
    /// </summary>
    public class DungeonRunManager : Singleton<DungeonRunManager>
    {
        private DungeonGenerator _generator;
        private RoomConfig _currentRoom;
        private RunResult _result;

        private void Start()
        {
            _generator = GetComponent<DungeonGenerator>();
            StartRun();
        }

        public void StartRun()
        {
            _result = new RunResult();
            
            // Apply kingdom buffs
            ApplyPreRunBuffs();

            // Generate layout
            _currentRoom = _generator.Generate();
            LoadRoom(_currentRoom);
        }

        private void ApplyPreRunBuffs()
        {
            // 영지 건물 버프, 칙령 버프 적용
        }

        private void LoadRoom(RoomConfig room)
        {
            // 방 씬 로드 및 초기화
            var roomManager = FindObjectOfType<RoomManager>();
            if (roomManager != null) roomManager.Initialize(room);
        }

        public void MoveToNextRoom(bool choosePathB = false)
        {
            _result.RoomsCleared++;

            if (_currentRoom.Type == RoomType.Boss)
            {
                EndRun(true);
                return;
            }

            _currentRoom = choosePathB && _currentRoom.IsBranch ? _currentRoom.NextB : _currentRoom.NextA;
            LoadRoom(_currentRoom);
        }

        public void EndRun(bool isVictory)
        {
            _result.IsVictory = isVictory;
            
            // Calculate final rewards with fail retention (70%) if lost
            float rewardMultiplier = isVictory ? 1.0f : 0.7f;
            
            // Save stats
            // SaveManager.Save...

            // Show UI
            EventBus.Publish(new GameEvents.RunEndedEvent(_result));
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnPlayerDied(GameEvents.PlayerDiedEvent e)
        {
            EndRun(false);
        }
    }
}
