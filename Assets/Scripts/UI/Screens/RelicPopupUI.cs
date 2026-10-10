using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Kingdom;
using MiniKingdom.Data;
using MiniKingdom.Core;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Displays a popup when a relic is found in a dungeon chest.
    /// Shows lore, effects, and allows taking or skipping the relic.
    /// </summary>
    public class RelicPopupUI : ScreenBase
    {
        [Header("Relic Info")]
        [SerializeField] private Image relicIcon;
        [SerializeField] private TextMeshProUGUI relicNameText;
        [SerializeField] private TextMeshProUGUI loreText;
        [SerializeField] private TextMeshProUGUI effectsText;

        [Header("Buttons")]
        [SerializeField] private Button takeButton;
        [SerializeField] private Button skipButton;

        // Temporary data class for example, in a real game use RelicData ScriptableObject
        private object _currentRelic;

        protected override void OnScreenShow()
        {
            takeButton.onClick.AddListener(OnTakeClicked);
            skipButton.onClick.AddListener(OnSkipClicked);
        }

        protected override void OnScreenHide()
        {
            takeButton.onClick.RemoveListener(OnTakeClicked);
            skipButton.onClick.RemoveListener(OnSkipClicked);
        }

        protected override void OnScreenUpdate() { }

        /// <summary>
        /// Call this before showing the screen to set up the relic details.
        /// </summary>
        public void Setup(Sprite icon, string name, string lore, string mechanics)
        {
            relicIcon.sprite = icon;
            relicNameText.text = name;
            loreText.text = $"<i>\"{lore}\"</i>";
            effectsText.text = mechanics;
        }

        private void OnTakeClicked()
        {
            var player = FindAnyObjectByType<MiniKingdom.Player.PlayerStats>();
            if (player != null)
            {
                player.AddModifier(new MiniKingdom.Data.StatModifier
                {
                    statType = MiniKingdom.Data.StatType.ATK,
                    value = 0.2f,
                    isPercentage = true
                });
            }

            MiniKingdom.UI.PopupManager.Instance?.ShowToast("👑 희귀 유물을 획득했습니다! (공격력 +20%)");

            if (UIManager.Instance != null) UIManager.Instance.CloseCurrent();
            else gameObject.SetActive(false);
        }

        private void OnSkipClicked()
        {
            if (UIManager.Instance != null) UIManager.Instance.CloseCurrent();
            else gameObject.SetActive(false);
        }
    }
}
