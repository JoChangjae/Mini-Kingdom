using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class DungeonSelectScreen : ScreenBase
    {
        [Header("Dungeon List")]
        [SerializeField] private ScrollRect dungeonScrollRect;
        [SerializeField] private Transform dungeonCardContainer;
        [SerializeField] private GameObject dungeonCardPrefab;

        [Header("Details")]
        [SerializeField] private TextMeshProUGUI statsSummaryText;
        [SerializeField] private TextMeshProUGUI dailyBonusText;
        [SerializeField] private Button departButton;

        protected override void OnScreenShow()
        {
            PopulateDungeons();
            UpdateStats();
            
            departButton.onClick.AddListener(OnDepartClicked);
        }

        protected override void OnScreenHide()
        {
            departButton.onClick.RemoveAllListeners();
            // Clear cards
            foreach (Transform child in dungeonCardContainer)
            {
                Destroy(child.gameObject);
            }
        }

        protected override void OnScreenUpdate()
        {
        }

        private void PopulateDungeons()
        {
            // Dummy logic: 생성할 던전 카드들
            for (int i = 0; i < 3; i++)
            {
                var card = Instantiate(dungeonCardPrefab, dungeonCardContainer);
                // Initialize card data
            }
        }

        private void UpdateStats()
        {
            statsSummaryText.text = "ATK: 120 | DEF: 45 | HP: 500";
            dailyBonusText.text = "일일 보너스: 3/5 남음";
        }

        private void OnDepartClicked()
        {
            // 던전 진입
            UIManager.Instance.Show(ScreenType.InDungeon_HUD, false);
            // DungeonRunManager.Instance.StartRun(selectedDungeonId);
        }
    }
}
