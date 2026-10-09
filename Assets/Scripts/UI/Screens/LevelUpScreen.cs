using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using MiniKingdom.Combat;

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
            if (titleObj != null) UIAnimations.ScaleBounce(titleObj.transform);

            PopulateCards();

            _autoSelectTimer = 15f;
            _isTimerActive = true;
        }

        protected override void OnScreenHide()
        {
            Time.timeScale = 1f; // Resume game
            if (cardContainer != null)
            {
                foreach (Transform child in cardContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        protected override void OnScreenUpdate()
        {
            if (_isTimerActive)
            {
                _autoSelectTimer -= Time.unscaledDeltaTime;
                if (timerText != null) timerText.text = Mathf.CeilToInt(_autoSelectTimer).ToString();

                if (_autoSelectTimer <= 0)
                {
                    AutoSelect();
                }
            }
        }

        private void PopulateCards()
        {
            if (synergyHintText != null)
                synergyHintText.text = "공격 3개 선택 시 광전사 시너지! 융합 스킬 해금에 도전하세요.";

            var choices = LevelUpSystem.Instance != null 
                ? LevelUpSystem.Instance.GenerateUpgradeChoices() 
                : new List<Data.SkillData>();

            int count = choices != null && choices.Count > 0 ? choices.Count : 3;

            for (int i = 0; i < count; i++)
            {
                int index = i;
                GameObject card = null;

                if (cardPrefab != null && cardContainer != null)
                {
                    card = Instantiate(cardPrefab, cardContainer);
                }
                else if (cardContainer != null)
                {
                    card = new GameObject($"Card_{i}");
                    card.transform.SetParent(cardContainer, false);
                    card.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.3f, 0.9f);
                    card.AddComponent<Button>();
                }

                if (card != null)
                {
                    var btn = card.GetComponent<Button>();
                    if (btn == null) btn = card.AddComponent<Button>();
                    btn.onClick.AddListener(() => OnCardSelected(index));

                    // 카드 텍스트 설정
                    var textMesh = card.GetComponentInChildren<TextMeshProUGUI>();
                    if (textMesh != null && choices != null && i < choices.Count && choices[i] != null)
                    {
                        var skill = choices[i];
                        string title = skill.IsFusion ? $"🔥 {skill.skillName} (융합)" : skill.skillName;
                        textMesh.text = $"<b>{title}</b>\n{skill.description}\n[Lv.{skill.CurrentLevel}]";
                    }
                }
            }
        }

        private void OnCardSelected(int index)
        {
            _isTimerActive = false;
            if (LevelUpSystem.Instance != null)
            {
                LevelUpSystem.Instance.ApplyUpgradeByIndex(index);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Pop();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void AutoSelect()
        {
            OnCardSelected(Random.Range(0, 3));
        }
    }
}
