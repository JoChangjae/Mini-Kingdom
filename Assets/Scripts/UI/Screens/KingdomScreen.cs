using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Kingdom;

namespace MiniKingdom.UI
{
    public class KingdomScreen : ScreenBase
    {
        [Header("Top Bar")]
        [SerializeField] private TextMeshProUGUI kingdomLevelText;
        [SerializeField] private ResourceBar goldBar;
        [SerializeField] private ResourceBar woodBar;
        [SerializeField] private ResourceBar stoneBar;

        [Header("Kingdom View")]
        [SerializeField] private Transform buildingContainer;
        [SerializeField] private ScrollRect kingdomScrollView;

        [Header("Bottom Tabs")]
        [SerializeField] private Button buildTabBtn;
        [SerializeField] private Button exploreTabBtn;
        [SerializeField] private Button residentsTabBtn;
        [SerializeField] private Button shopTabBtn;
        [SerializeField] private Button discoveryTabBtn;

        [Header("Indicators")]
        [SerializeField] private GameObject dailyBonusBadge;
        [SerializeField] private TextMeshProUGUI activeDecreeText;
        [SerializeField] private GameObject defenseCountdownPanel;
        [SerializeField] private TextMeshProUGUI defenseCountdownText;

        protected override void OnScreenShow()
        {
            UpdateKingdomUI();
            
            // Register listeners
            exploreTabBtn.onClick.AddListener(OnExploreClicked);
            discoveryTabBtn.onClick.AddListener(OnDiscoveryClicked);
            shopTabBtn.onClick.AddListener(OnShopClicked);
            buildTabBtn.onClick.AddListener(OnBuildClicked);
        }

        protected override void OnScreenHide()
        {
            exploreTabBtn.onClick.RemoveAllListeners();
            discoveryTabBtn.onClick.RemoveAllListeners();
            shopTabBtn.onClick.RemoveAllListeners();
            buildTabBtn.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate()
        {
            // Update defense countdown etc.
        }

        private void UpdateKingdomUI()
        {
            // 왕국 레벨 및 자원 업데이트 (임시 데이터 연동)
            kingdomLevelText.text = "Lv. 5";
            goldBar.SetAmount(1500);
            woodBar.SetAmount(300);
            stoneBar.SetAmount(150);
            
            dailyBonusBadge.SetActive(true); // Example logic
            activeDecreeText.text = "풍년: 골드 획득 +10%";
        }

        private void OnExploreClicked()
        {
            UIManager.Instance.Show(ScreenType.DungeonSelect);
        }

        private void OnDiscoveryClicked()
        {
            UIManager.Instance.Show(ScreenType.DiscoveryBook);
        }

        private void OnShopClicked()
        {
            UIManager.Instance.Show(ScreenType.Shop);
        }

        private void OnBuildClicked()
        {
            // UIManager.Instance.Show(ScreenType.BuildingList);
            PopupManager.Instance.ShowToast("건설 메뉴 오픈!");
        }

        public void OnBuildingTapped(int buildingId)
        {
            // Show BuildingDetailScreen with buildingId
            UIManager.Instance.Show(ScreenType.Building);
        }
    }
}
