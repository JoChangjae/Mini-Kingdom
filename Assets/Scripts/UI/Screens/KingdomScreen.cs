using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Kingdom;
using MiniKingdom.Core; // Assuming EventBus is here

namespace MiniKingdom.UI.Screens
{
    public class KingdomScreen : ScreenBase
    {
        [Header("Top Bar")]
        [SerializeField] private TextMeshProUGUI kingdomLevelText;
        [SerializeField] private ResourceBar goldBar;
        [SerializeField] private ResourceBar woodBar;
        [SerializeField] private ResourceBar stoneBar;
        
        [Header("Weather System")]
        [SerializeField] private Image weatherIcon;

        [Header("Kingdom View")]
        [SerializeField] private Transform buildingContainer;
        [SerializeField] private ScrollRect kingdomScrollView;

        [Header("Treasury / Tax")]
        [SerializeField] private Button treasuryButton;
        [SerializeField] private TextMeshProUGUI pendingGoldText;
        private int _pendingGold = 0;

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
            treasuryButton.onClick.AddListener(OnTreasuryClicked);
            
            // 날씨 이벤트 구독 (Subscribe to weather events)
            EventBus.Subscribe<WeatherChangedEvent>(OnWeatherChanged);
            
            // 오프라인 골드 임시 생성 (Generate fake offline gold for testing)
            _pendingGold = Random.Range(100, 500);
            UpdateTreasuryUI();
        }

        protected override void OnScreenHide()
        {
            exploreTabBtn.onClick.RemoveAllListeners();
            discoveryTabBtn.onClick.RemoveAllListeners();
            shopTabBtn.onClick.RemoveAllListeners();
            buildTabBtn.onClick.RemoveAllListeners();
            treasuryButton.onClick.RemoveAllListeners();
            
            EventBus.Unsubscribe<WeatherChangedEvent>(OnWeatherChanged);
        }

        protected override void OnScreenUpdate()
        {
            // Update defense countdown etc.
        }

        private void UpdateKingdomUI()
        {
            if (ResourceManager.Instance != null)
            {
                if (goldBar != null) goldBar.SetAmount(ResourceManager.Instance.GetResource(Data.ResourceType.Gold));
                if (woodBar != null) woodBar.SetAmount(ResourceManager.Instance.GetResource(Data.ResourceType.Wood));
                if (stoneBar != null) stoneBar.SetAmount(ResourceManager.Instance.GetResource(Data.ResourceType.Stone));
            }
            else
            {
                if (goldBar != null) goldBar.SetAmount(1500);
                if (woodBar != null) woodBar.SetAmount(300);
                if (stoneBar != null) stoneBar.SetAmount(150);
            }

            if (BuildingManager.Instance != null && kingdomLevelText != null)
            {
                int lvl = Mathf.Max(1, BuildingManager.Instance.GetTotalKingdomLevel());
                kingdomLevelText.text = $"Lv. {lvl}";
            }
            else if (kingdomLevelText != null)
            {
                kingdomLevelText.text = "Lv. 1";
            }
            
            if (dailyBonusBadge != null) dailyBonusBadge.SetActive(true);
            if (activeDecreeText != null) activeDecreeText.text = "풍년: 골드 획득 +10%";
        }

        private void UpdateTreasuryUI()
        {
            if (pendingGoldText != null)
                pendingGoldText.text = _pendingGold > 0 ? $"+{_pendingGold} Gold" : "No Taxes";
        }

        private void OnTreasuryClicked()
        {
            if (_pendingGold > 0)
            {
                // 세금 징수 (Collect taxes)
                PopupManager.Instance?.ShowToast($"+{_pendingGold} 골드 획득!");
                ResourceManager.Instance?.AddResource(Data.ResourceType.Gold, _pendingGold);
                SaveManager.Instance?.SaveGame();

                _pendingGold = 0;
                UpdateTreasuryUI();
                UpdateKingdomUI();
            }
        }
        
        private void OnWeatherChanged(WeatherChangedEvent evt)
        {
            // 날씨 아이콘 변경 (Change weather icon)
            if (weatherIcon != null && evt.WeatherSprite != null)
            {
                weatherIcon.sprite = evt.WeatherSprite;
            }
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
