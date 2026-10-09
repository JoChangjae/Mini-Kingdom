using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Combat;
using MiniKingdom.UI;

namespace MiniKingdom.Dungeon
{
    public enum RoomState { Enter, Active, Clear, Reward, Exit }

    /// <summary>
    /// Manages logic within a single room.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        private RoomConfig _currentConfig;
        private RoomState _state;

        public RoomState State => _state;
        public RoomConfig CurrentConfig => _currentConfig;

        public void Initialize(RoomConfig config)
        {
            _currentConfig = config;
            ChangeState(RoomState.Enter);
        }

        private void ChangeState(RoomState newState)
        {
            _state = newState;
            switch (_state)
            {
                case RoomState.Enter:
                    Debug.Log($"[RoomManager] 방 입장: {_currentConfig?.Type} (난이도: {_currentConfig?.Difficulty})");
                    ChangeState(RoomState.Active);
                    break;

                case RoomState.Active:
                    if (_currentConfig.Type == RoomType.Combat || _currentConfig.Type == RoomType.Boss)
                    {
                        var spawner = GetComponent<Enemy.EnemySpawner>();
                        if (spawner != null)
                        {
                            spawner.StartSpawning(null, _currentConfig.Difficulty);
                        }
                        else
                        {
                            // 스포너가 없는 경우 3초 후 자동 클리어
                            Invoke(nameof(ForceClear), 3f);
                        }
                    }
                    else
                    {
                        // 비전투 방(휴식, 상점, 보물 등)은 즉시 클리어
                        ChangeState(RoomState.Clear);
                    }
                    break;

                case RoomState.Clear:
                    Debug.Log("[RoomManager] 방 클리어!");
                    ChangeState(RoomState.Reward);
                    break;

                case RoomState.Reward:
                    GrantRoomRewards();
                    // 보상 획득 및 레벨업 체크 후 다음 방으로 이동
                    Invoke(nameof(AdvanceToNextRoom), 2f);
                    break;

                case RoomState.Exit:
                    if (DungeonRunManager.Instance != null)
                    {
                        DungeonRunManager.Instance.MoveToNextRoom();
                    }
                    break;
            }
        }

        private void ForceClear()
        {
            if (_state == RoomState.Active)
            {
                ChangeState(RoomState.Clear);
            }
        }

        private void GrantRoomRewards()
        {
            // 방 클리어 기본 자원 누적
            int gold = Random.Range(15, 30);
            int wood = Random.Range(5, 12);
            if (DungeonRunManager.Instance != null)
            {
                DungeonRunManager.Instance.AddCollectedResources(gold, wood, 0);
            }

            // 30% 확률로 런 내 레벨업 팝업 호출
            if (Random.value < 0.35f && UIManager.Instance != null)
            {
                UIManager.Instance.Show(ScreenType.LevelUp);
            }
        }

        private void AdvanceToNextRoom()
        {
            ChangeState(RoomState.Exit);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.RoomClearedEvent>(OnRoomCleared);
            EventBus.Subscribe<GameEvents.BossKilledEvent>(OnBossKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.RoomClearedEvent>(OnRoomCleared);
            EventBus.Unsubscribe<GameEvents.BossKilledEvent>(OnBossKilled);
        }

        private void OnRoomCleared(GameEvents.RoomClearedEvent e)
        {
            if (_state == RoomState.Active)
            {
                ChangeState(RoomState.Clear);
            }
        }

        private void OnBossKilled(GameEvents.BossKilledEvent e)
        {
            if (_state == RoomState.Active)
            {
                ChangeState(RoomState.Clear);
            }
        }
    }
}
