using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace MiniKingdom.UI
{
    public class LevelUpScreen : ScreenBase
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private TextMeshProUGUI synergyHintText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject titleObj;

        private float _autoSelectTimer = 15f;
        private bool _isTimerActive = false;

        protected override void OnScreenShow()
        {
            Time.timeScale = 0f; // Pause game
            UIAnimations.ScaleBounce(titleObj.transform);
            
            PopulateCards();
            
            _autoSelectTimer = 15f;
            _isTimerActive = true;
        }

        protected override void OnScreenHide()
        {
            Time.timeScale = 1f; // Resume game
            foreach (Transform child in cardContainer)
            {
                Destroy(child.gameObject);
            }
        }

        protected override void OnScreenUpdate()
        {
            if (_isTimerActive)
            {
                _autoSelectTimer -= Time.unscaledDeltaTime;
                timerText.text = Mathf.CeilToInt(_autoSelectTimer).ToString();

                if (_autoSelectTimer <= 0)
                {
                    AutoSelect();
                }
            }
        }

        private void PopulateCards()
        {
            // LevelUpSystem에서 옵션을 가져와 카드 생성
            synergyHintText.text = "공격 3개 선택 시 광전사 시너지!";
            
            for (int i = 0; i < 3; i++)
            {
                var card = Instantiate(cardPrefab, cardContainer);
                var btn = card.GetComponent<Button>();
                int index = i; // local copy for closure
                btn.onClick.AddListener(() => OnCardSelected(index));
            }
        }

        private void OnCardSelected(int index)
        {
            _isTimerActive = false;
            // CombatSystem.ApplyUpgrade(index);
            UIManager.Instance.Pop();
        }

        private void AutoSelect()
        {
            OnCardSelected(Random.Range(0, 3));
        }
    }
}
