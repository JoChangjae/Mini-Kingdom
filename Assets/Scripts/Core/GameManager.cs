using System;
using UnityEngine;
using MiniKingdom.Utils;

namespace MiniKingdom.Core
{
    public enum GameState
    {
        MainMenu,
        Kingdom,
        DungeonSelect,
        InDungeon,
        Combat,
        LevelUp,
        RunResult,
        Pause
    }

    /// <summary>
    /// Singleton game manager holding the global game state and major system references.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; }

        public event Action<GameState, GameState> OnGameStateChanged;
        public event Action OnRunStarted;
        public event Action OnRunEnded;

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance) return;
            
            // 초기 상태 설정
            CurrentState = GameState.MainMenu;
        }

        /// <summary>
        /// Changes the current game state and triggers events.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState oldState = CurrentState;
            CurrentState = newState;
            
            // 상태 변경 이벤트 발생
            OnGameStateChanged?.Invoke(oldState, newState);

            // 특수 상태 로직 처리 (예: 던전 진입/종료)
            if (newState == GameState.InDungeon && oldState != GameState.Combat && oldState != GameState.LevelUp && oldState != GameState.Pause)
            {
                StartRun();
            }
            else if (oldState == GameState.RunResult && newState == GameState.Kingdom)
            {
                EndRun();
            }
        }

        private void StartRun()
        {
            // 게임 런 시작 로직
            OnRunStarted?.Invoke();
            Debug.Log("[GameManager] Run Started!");
        }

        private void EndRun()
        {
            // 게임 런 종료 로직
            OnRunEnded?.Invoke();
            Debug.Log("[GameManager] Run Ended!");
        }
    }
}
