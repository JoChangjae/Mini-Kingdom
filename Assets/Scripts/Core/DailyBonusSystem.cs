using System;
using UnityEngine;

namespace MiniKingdom.Core
{
    /// <summary>
    /// Manages the daily bonus runs logic.
    /// Tracks daily run count and resets at 5 AM.
    /// </summary>
    public class DailyBonusSystem : MonoBehaviour
    {
        public event Action<int, bool> OnDailyBonusRunCompleted;
        public event Action OnDailyBonusExpired;

        private int _dailyRunCount;
        private DateTime _lastResetDate;

        private int MaxBonusRuns = 5;
        
        private void Start()
        {
            LoadData();
            CheckDailyReset();
            
            // 런 완료 이벤트 구독
            EventBus.Subscribe<RunCompletedEvent>(OnRunCompleted);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<RunCompletedEvent>(OnRunCompleted);
        }

        private void OnRunCompleted(RunCompletedEvent evt)
        {
            CheckDailyReset();
            
            _dailyRunCount++;
            bool isBonus = _dailyRunCount <= MaxBonusRuns;

            // 일일 보너스 이벤트 발행
            EventBus.Publish(new DailyBonusRunEvent 
            { 
                RunNumber = _dailyRunCount, 
                IsBonus = isBonus 
            });
            
            OnDailyBonusRunCompleted?.Invoke(_dailyRunCount, isBonus);

            if (_dailyRunCount == MaxBonusRuns)
            {
                OnDailyBonusExpired?.Invoke();
                Debug.Log("[DailyBonusSystem] 오늘의 보너스 런을 모두 소진했습니다.");
            }

            SaveData();
        }

        private void CheckDailyReset()
        {
            DateTime now = DateTime.Now;
            // 리셋 기준 시간: 오늘 오전 5시
            DateTime resetTimeToday = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0);
            
            if (now < resetTimeToday)
            {
                // 현재 시간이 새벽 5시 이전이면, 리셋 기준일은 어제
                resetTimeToday = resetTimeToday.AddDays(-1);
            }

            if (_lastResetDate < resetTimeToday)
            {
                // 리셋 시간 지남
                _dailyRunCount = 0;
                _lastResetDate = resetTimeToday;
                Debug.Log("[DailyBonusSystem] 일일 보너스가 리셋되었습니다.");
                SaveData();
            }
        }

        private void LoadData()
        {
            _dailyRunCount = PlayerPrefs.GetInt("DailyRunCount", 0);
            
            string dateStr = PlayerPrefs.GetString("LastResetDate", string.Empty);
            if (string.IsNullOrEmpty(dateStr) || !DateTime.TryParse(dateStr, out _lastResetDate))
            {
                _lastResetDate = DateTime.MinValue;
            }
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("DailyRunCount", _dailyRunCount);
            PlayerPrefs.SetString("LastResetDate", _lastResetDate.ToString("o"));
            PlayerPrefs.Save();
        }
    }
}
