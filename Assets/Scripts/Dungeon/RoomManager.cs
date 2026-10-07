using UnityEngine;
using MiniKingdom.Core;

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
                    // Spawn objects, doors closed
                    ChangeState(RoomState.Active);
                    break;
                case RoomState.Active:
                    if (_currentConfig.Type == RoomType.Combat)
                    {
                        var spawner = GetComponent<Enemy.EnemySpawner>();
                        if (spawner) spawner.StartSpawning(null, _currentConfig.Difficulty);
                    }
                    else
                    {
                        // Non-combat room is instantly cleared
                        ChangeState(RoomState.Clear);
                    }
                    break;
                case RoomState.Clear:
                    // Open doors
                    ChangeState(RoomState.Reward);
                    break;
                case RoomState.Reward:
                    // Spawn chest
                    break;
                case RoomState.Exit:
                    // Transition to next room handled by DungeonRunManager
                    break;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.RoomClearedEvent>(OnRoomCleared);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.RoomClearedEvent>(OnRoomCleared);
        }

        private void OnRoomCleared(GameEvents.RoomClearedEvent e)
        {
            if (_state == RoomState.Active)
            {
                ChangeState(RoomState.Clear);
            }
        }
    }
}
