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
                    if (_currentConfig == null)
                    {
                        ChangeState(RoomState.Clear);
                        break;
                    }

                    if (_currentConfig.Type == RoomType.Combat || _currentConfig.Type == RoomType.Boss)
                    {
                        var spawner = GetComponent<Enemy.EnemySpawner>();
                        if (spawner != null)
                        {
                            spawner.StartSpawning(null, _currentConfig.Difficulty);
                        }
                        else
                        {
                            Invoke(nameof(ForceClear), 3f);
                        }
                    }
                    else if (_currentConfig.Type == RoomType.Rest)
                    {
                        if (UIManager.Instance != null)
                        {
                            UIManager.Instance.Show(ScreenType.RestRoom);
                        }
                        else
                        {
                            ChangeState(RoomState.Clear);
                        }
                    }
                    else if (_currentConfig.Type == RoomType.Shop)
                    {
                        if (UIManager.Instance != null)
                        {
                            UIManager.Instance.Show(ScreenType.DungeonShop);
                        }
                        else
                        {
                            ChangeState(RoomState.Clear);
                        }
                    }
                    else if (_currentConfig.Type == RoomType.Treasure)
                    {
                        if (UIManager.Instance != null)
                        {
                            var relicPopup = FindObjectOfType<RelicPopupUI>(true);
                            if (relicPopup != null)
                            {
                                relicPopup.Setup(null, "황금 왕관 유물", "고대 왕의 권능이 깃든 보물", "골드 획득량 +20%");
                            }
                            UIManager.Instance.Show(ScreenType.RelicPopup);
                        }
                        GrantRoomRewards();
                        Invoke(nameof(AdvanceToNextRoom), 2f);
                    }
                    else
                    {
                        ChangeState(RoomState.Clear);
                    }
                    break;

                case RoomState.Clear:
                    Debug.Log("[RoomManager] 방 클리어!");
                    ChangeState(RoomState.Reward);
                    break;

                case RoomState.Reward:
                    GrantRoomRewards();
                    // 보상 획득 및 레벨업 체크 후 다음 방 또는 분기 선택으로 이동
                    Invoke(nameof(AdvanceToNextRoom), 1.5f);
                    break;

                case RoomState.Exit:
                    if (_currentConfig != null && _currentConfig.IsBranch && _currentConfig.NextB != null && UIManager.Instance != null)
                    {
                        // 2갈래 길 선택 화면 띄우기
                        var branchScreen = FindObjectOfType<BranchSelectionScreen>(true);
                        if (branchScreen != null)
                        {
                            branchScreen.SetupBranches(_currentConfig.NextA, _currentConfig.NextB);
                        }
                        UIManager.Instance.Show(ScreenType.BranchSelection);
                    }
                    else if (DungeonRunManager.Instance != null)
                    {
                        DungeonRunManager.Instance.MoveToNextRoom(false);
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
            EventBus.Subscribe<RoomClearedEvent>(OnRoomCleared);
            EventBus.Subscribe<BossKilledEvent>(OnBossKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RoomClearedEvent>(OnRoomCleared);
            EventBus.Unsubscribe<BossKilledEvent>(OnBossKilled);
        }

        private void OnRoomCleared(RoomClearedEvent e)
        {
            if (_state == RoomState.Active)
            {
                ChangeState(RoomState.Clear);
            }
        }

        private void OnBossKilled(BossKilledEvent e)
        {
            if (_state == RoomState.Active)
            {
                ChangeState(RoomState.Clear);
            }
        }
    }
}
