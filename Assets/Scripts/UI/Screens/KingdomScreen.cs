using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

        private bool _isTransitioning = false;

        protected override void OnScreenShow()
        {
            _isTransitioning = false;
            UpdateKingdomUI();
            
            // Register listeners safely
            if (exploreTabBtn != null)
            {
                exploreTabBtn.onClick.RemoveListener(OnExploreClicked);
                exploreTabBtn.onClick.AddListener(OnExploreClicked);
            }
            if (discoveryTabBtn != null)
            {
                discoveryTabBtn.onClick.RemoveListener(OnDiscoveryClicked);
                discoveryTabBtn.onClick.AddListener(OnDiscoveryClicked);
            }
            if (shopTabBtn != null)
            {
                shopTabBtn.onClick.RemoveListener(OnShopClicked);
                shopTabBtn.onClick.AddListener(OnShopClicked);
            }
            if (buildTabBtn != null)
            {
                buildTabBtn.onClick.RemoveListener(OnBuildClicked);
                buildTabBtn.onClick.AddListener(OnBuildClicked);
            }
            if (treasuryButton != null)
            {
                treasuryButton.onClick.RemoveListener(OnTreasuryClicked);
                treasuryButton.onClick.AddListener(OnTreasuryClicked);
            }
            
            // 날씨 이벤트 구독 (Subscribe to weather events)
            EventBus.Subscribe<WeatherChangedEvent>(OnWeatherChanged);
            
            // 오프라인 골드 임시 생성 (Generate fake offline gold for testing)
            _pendingGold = Random.Range(100, 500);
            UpdateTreasuryUI();
        }

        protected override void OnScreenHide()
        {
            if (exploreTabBtn != null) exploreTabBtn.onClick.RemoveAllListeners();
            if (discoveryTabBtn != null) discoveryTabBtn.onClick.RemoveAllListeners();
            if (shopTabBtn != null) shopTabBtn.onClick.RemoveAllListeners();
            if (buildTabBtn != null) buildTabBtn.onClick.RemoveAllListeners();
            if (treasuryButton != null) treasuryButton.onClick.RemoveAllListeners();
            
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

        public void OnExploreClicked()
        {
            if (_isTransitioning) return;
            _isTransitioning = true;

            Debug.Log("[KingdomScreen] ⚔️ 던전 출정하기 클릭! Dungeon 씬으로 이동합니다.");
            if (Application.CanStreamedLevelBeLoaded("Dungeon"))
            {
                SceneManager.LoadScene("Dungeon");
            }
            else if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(ScreenType.DungeonSelect);
            }
            else
            {
                Debug.LogWarning("[KingdomScreen] Dungeon 씬을 로드할 수 없습니다.");
                _isTransitioning = false;
            }
        }

        private void OnDiscoveryClicked()
        {
            UIManager.Instance?.Show(ScreenType.DiscoveryBook);
        }

        private void OnShopClicked()
        {
            UIManager.Instance?.Show(ScreenType.Shop);
        }

        private void OnBuildClicked()
        {
            // UIManager.Instance?.Show(ScreenType.BuildingList);
            PopupManager.Instance?.ShowToast("건설 메뉴 오픈!");
        }

        public void OnBuildingTapped(int buildingId)
        {
            // Show BuildingDetailScreen with buildingId
            UIManager.Instance?.Show(ScreenType.Building);
        }
    }
}
