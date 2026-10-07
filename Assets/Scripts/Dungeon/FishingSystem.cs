using System;
using UnityEngine;
using MiniKingdom.Core;

namespace MiniKingdom.Dungeon
{
    /// <summary>
    /// Event published when a fish is caught.
    /// </summary>
    public struct FishCaughtEvent
    {
        public string FishId;
        public int Quality;
    }

    /// <summary>
    /// 휴식방에서 진행되는 낚시 미니게임을 관리하는 시스템 (Manages fishing minigame in rest rooms).
    /// </summary>
    public class FishingSystem : MonoBehaviour
    {
        public enum FishingState
        {
            Idle,
            Casting,
            Waiting,
            Biting,
            Reeling,
            Caught,
            Failed
        }

        private FishingState _currentState = FishingState.Idle;
        private float _stateTimer = 0f;
        
        [SerializeField] private float _minWaitTime = 2f;
        [SerializeField] private float _maxWaitTime = 5f;
        [SerializeField] private float _biteDuration = 1.5f;
        [SerializeField] private float _reelingDuration = 3f;

        public FishingState CurrentState => _currentState;

        private void Update()
        {
            if (_currentState == FishingState.Waiting)
            {
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0)
                {
                    ChangeState(FishingState.Biting);
                }
            }
            else if (_currentState == FishingState.Biting)
            {
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0)
                {
                    // 미끼를 물었으나 반응하지 않아 실패 (Missed the bite)
                    ChangeState(FishingState.Failed);
                }
            }
            else if (_currentState == FishingState.Reeling)
            {
                _stateTimer -= Time.deltaTime;
                if (_stateTimer <= 0)
                {
                    // 릴링 시간 초과로 실패 (Reeling timed out)
                    ChangeState(FishingState.Failed);
                }
            }
        }

        /// <summary>
        /// 낚싯대를 던집니다. (Casts the fishing line)
        /// </summary>
        public void CastLine()
        {
            if (_currentState != FishingState.Idle && _currentState != FishingState.Failed && _currentState != FishingState.Caught)
                return;
                
            ChangeState(FishingState.Casting);
            // 애니메이션 등 처리 (Animation logic here)
            
            _stateTimer = UnityEngine.Random.Range(_minWaitTime, _maxWaitTime);
            ChangeState(FishingState.Waiting);
        }

        /// <summary>
        /// 화면 터치/클릭 등의 입력을 처리합니다. (Handles input for reeling)
        /// </summary>
        public void Interact()
        {
            switch (_currentState)
            {
                case FishingState.Biting:
                    // 타이밍 바가 나타나는 릴링 단계로 진입
                    ChangeState(FishingState.Reeling);
                    _stateTimer = _reelingDuration;
                    break;
                case FishingState.Reeling:
                    // 여기서 타이밍 바 미니게임 로직 처리 (Timing bar minigame check)
                    // 임시로 성공 처리
                    ProcessReelingSuccess();
                    break;
                case FishingState.Idle:
                case FishingState.Failed:
                case FishingState.Caught:
                    CastLine();
                    break;
            }
        }

        private void ProcessReelingSuccess()
        {
            ChangeState(FishingState.Caught);
            // 물고기 획득 이벤트 발행
            EventBus.Publish(new FishCaughtEvent { FishId = "basic_fish", Quality = 1 });
            
            // 일정 시간 후 Idle로 복귀
            Invoke(nameof(ResetToIdle), 2f);
        }

        private void ResetToIdle()
        {
            ChangeState(FishingState.Idle);
        }

        private void ChangeState(FishingState newState)
        {
            _currentState = newState;
            // 상태 변경에 따른 UI 업데이트나 이펙트 처리 (UI update or effects)
        }
    }
}
