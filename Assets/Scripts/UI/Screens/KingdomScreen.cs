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
            // 왕국 레벨 및 자원 업데이트 (임시 데이터 연동)
            kingdomLevelText.text = "Lv. 5";
            goldBar.SetAmount(1500);
            woodBar.SetAmount(300);
            stoneBar.SetAmount(150);
            
            dailyBonusBadge.SetActive(true); // Example logic
            activeDecreeText.text = "풍년: 골드 획득 +10%";
        }

        private void UpdateTreasuryUI()
        {
            pendingGoldText.text = _pendingGold > 0 ? $"+{_pendingGold} Gold" : "No Taxes";
        }

        private void OnTreasuryClicked()
        {
            if (_pendingGold > 0)
            {
                // 세금 징수 (Collect taxes)
                PopupManager.Instance.ShowToast($"+{_pendingGold} 골드 획득!");
                // Here we would add to actual player resources
                _pendingGold = 0;
                UpdateTreasuryUI();
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
