using System;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Utils;

namespace MiniKingdom.Core
{
    /// <summary>
    /// Manages the daily royal decree selection and effects.
    /// </summary>
    public class RoyalDecreeSystem : MonoBehaviour
    {
        public DecreeType ActiveDecree { get; private set; } = DecreeType.None;
        
        private DateTime _lastResetDate;

        private void Start()
        {
            LoadData();
            CheckDailyReset();
        }

        /// <summary>
        /// Generates 3 random decrees for the player to choose from today.
        /// </summary>
        public List<DecreeType> GetDailyDecreeOptions()
        {
            CheckDailyReset();

            if (ActiveDecree != DecreeType.None)
            {
                Debug.LogWarning("[RoyalDecreeSystem] 이미 오늘의 칙령이 선택되었습니다.");
                return new List<DecreeType> { ActiveDecree };
            }

            var picker = new WeightedRandomPicker<DecreeType>();
            // 모든 칙령 타입에 동일한 가중치 부여 (None 제외)
            foreach (DecreeType type in Enum.GetValues(typeof(DecreeType)))
            {
                if (type != DecreeType.None)
                {
                    picker.AddItem(type, 1.0f);
                }
            }

            return picker.PickMultiple(3);
        }

        /// <summary>
        /// Chooses a decree for the day.
        /// </summary>
        public void ChooseDecree(DecreeType decree)
        {
            if (ActiveDecree != DecreeType.None) return;

            ActiveDecree = decree;
            Debug.Log($"[RoyalDecreeSystem] 오늘의 칙령 선택됨: {ActiveDecree}");

            // 칙령 선택 이벤트 발행
            EventBus.Publish(new RoyalDecreeChosenEvent { DecreeType = ActiveDecree });

            SaveData();
        }

        private void CheckDailyReset()
        {
            DateTime now = DateTime.Now;
            // 리셋 기준 시간: 오전 5시
            DateTime resetTimeToday = new DateTime(now.Year, now.Month, now.Day, 5, 0, 0);
            
            if (now < resetTimeToday)
            {
                resetTimeToday = resetTimeToday.AddDays(-1);
            }

            if (_lastResetDate < resetTimeToday)
            {
                ActiveDecree = DecreeType.None;
                _lastResetDate = resetTimeToday;
                Debug.Log("[RoyalDecreeSystem] 왕국 칙령 시스템이 일일 리셋되었습니다.");
                SaveData();
            }
        }

        private void LoadData()
        {
            ActiveDecree = (DecreeType)PlayerPrefs.GetInt("ActiveDecree", (int)DecreeType.None);
            
            string dateStr = PlayerPrefs.GetString("DecreeLastResetDate", string.Empty);
            if (string.IsNullOrEmpty(dateStr) || !DateTime.TryParse(dateStr, out _lastResetDate))
            {
                _lastResetDate = DateTime.MinValue;
            }
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("ActiveDecree", (int)ActiveDecree);
            PlayerPrefs.SetString("DecreeLastResetDate", _lastResetDate.ToString("o"));
            PlayerPrefs.Save();
        }
    }
}
