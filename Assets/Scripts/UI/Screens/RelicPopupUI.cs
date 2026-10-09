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
            // 유물 획득 처리 (Process taking the relic)
            Debug.Log("Relic Taken!");
            UIManager.Instance.CloseCurrent();
        }

        private void OnSkipClicked()
        {
            // 유물 포기 처리 (Process skipping the relic)
            Debug.Log("Relic Skipped.");
            UIManager.Instance.CloseCurrent();
        }
    }
}
