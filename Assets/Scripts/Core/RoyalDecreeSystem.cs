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

        private List<DecreeType> _cachedOptions = new List<DecreeType>();

        private void Start()
        {
            LoadData();
            CheckDailyReset();
        }

        /// <summary>
        /// Generates or returns the 3 random decrees for today.
        /// </summary>
        public List<DecreeType> GetDailyDecreeOptions()
        {
            CheckDailyReset();

            if (_cachedOptions != null && _cachedOptions.Count >= 3)
            {
                return _cachedOptions;
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

            _cachedOptions = picker.PickMultiple(3);
            if (_cachedOptions == null || _cachedOptions.Count < 3)
            {
                _cachedOptions = new List<DecreeType> { DecreeType.WarriorDay, DecreeType.MerchantDay, DecreeType.WoodDay };
            }

            // If an active decree was already selected but not in the 3 options, ensure it's in the list
            if (ActiveDecree != DecreeType.None && !_cachedOptions.Contains(ActiveDecree))
            {
                _cachedOptions[0] = ActiveDecree;
            }

            SaveData();
            return _cachedOptions;
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
                _cachedOptions.Clear();
                PlayerPrefs.DeleteKey("DecreeDailyOptions");
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

            string optsStr = PlayerPrefs.GetString("DecreeDailyOptions", string.Empty);
            if (!string.IsNullOrEmpty(optsStr))
            {
                _cachedOptions = new List<DecreeType>();
                string[] parts = optsStr.Split(',');
                foreach (var p in parts)
                {
                    if (int.TryParse(p, out int val) && Enum.IsDefined(typeof(DecreeType), val))
                    {
                        _cachedOptions.Add((DecreeType)val);
                    }
                }
            }
        }

        private void SaveData()
        {
            PlayerPrefs.SetInt("ActiveDecree", (int)ActiveDecree);
            PlayerPrefs.SetString("DecreeLastResetDate", _lastResetDate.ToString("o"));
            if (_cachedOptions != null && _cachedOptions.Count > 0)
            {
                var intList = new List<string>();
                foreach (var o in _cachedOptions) intList.Add(((int)o).ToString());
                PlayerPrefs.SetString("DecreeDailyOptions", string.Join(",", intList));
            }
            PlayerPrefs.Save();
        }
    }
}
