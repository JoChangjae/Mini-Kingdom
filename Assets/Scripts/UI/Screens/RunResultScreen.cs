using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MiniKingdom.Core;
using MiniKingdom.Dungeon;

namespace MiniKingdom.UI
{
    public class RunResultScreen : ScreenBase
    {
        [Header("Titles")]
        [SerializeField] private GameObject successTitle;
        [SerializeField] private GameObject failureTitle;

        [Header("Rewards")]
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private GameObject rewardItemPrefab;
        [SerializeField] private TextMeshProUGUI dailyBonusText;

        [Header("Contribution")]
        [SerializeField] private Slider contributionBar;
        [SerializeField] private TextMeshProUGUI contributionText;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI statsSummaryText;

        [Header("Buttons")]
        [SerializeField] private Button returnToKingdomBtn;
        [SerializeField] private Button tryAgainBtn;
        [SerializeField] private Button watchAdBtn;

        private bool _isSuccess = true;
        private RunResult _latestResult;

        public void SetupResult(bool success)
        {
            _isSuccess = success;
        }

        protected override void OnScreenShow()
        {
            if (DungeonRunManager.Instance != null && DungeonRunManager.Instance.CurrentResult != null)
            {
                _latestResult = DungeonRunManager.Instance.CurrentResult;
                _isSuccess = _latestResult.IsVictory;
            }

            if (successTitle != null) successTitle.SetActive(_isSuccess);
            if (failureTitle != null) failureTitle.SetActive(!_isSuccess);

            if (returnToKingdomBtn != null) returnToKingdomBtn.onClick.AddListener(OnReturnToKingdom);
            if (tryAgainBtn != null) tryAgainBtn.onClick.AddListener(OnTryAgain);
            if (watchAdBtn != null) watchAdBtn.onClick.AddListener(OnWatchAd);

            ShowRewards();
        }

        protected override void OnScreenHide()
        {
            if (returnToKingdomBtn != null) returnToKingdomBtn.onClick.RemoveAllListeners();
            if (tryAgainBtn != null) tryAgainBtn.onClick.RemoveAllListeners();
            if (watchAdBtn != null) watchAdBtn.onClick.RemoveAllListeners();

            if (rewardContainer != null)
            {
                foreach (Transform t in rewardContainer) Destroy(t.gameObject);
            }
        }

        protected override void OnScreenUpdate() {}

        private void ShowRewards()
        {
            if (dailyBonusText != null)
            {
                dailyBonusText.text = "x1.5 일일 보너스 적용!";
                UIAnimations.PunchScale(dailyBonusText.transform);
            }

            int rooms = _latestResult != null ? _latestResult.RoomsCleared : 10;
            int kills = _latestResult != null ? _latestResult.EnemiesKilled : 35;
            int gold = _latestResult != null ? _latestResult.GoldReward : 200;
            int wood = _latestResult != null ? _latestResult.WoodReward : 45;

            if (statsSummaryText != null)
            {
                statsSummaryText.text = $"방 클리어: {rooms}개 | 몬스터 처치: {kills}마리\n획득 골드: +{gold} | 획득 목재: +{wood}";
            }
            
            if (contributionBar != null) contributionBar.value = _isSuccess ? 1.0f : 0.7f;
            if (contributionText != null) contributionText.text = _isSuccess ? "왕국 기여도 100% 달성!" : "왕국 기여도 70% 유지 (보존)";
        }

        private void OnReturnToKingdom()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Kingdom);
            }

            if (Application.CanStreamedLevelBeLoaded("Kingdom"))
            {
                SceneManager.LoadScene("Kingdom");
            }
            else if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(ScreenType.Kingdom, false);
            }
        }

        private void OnTryAgain()
        {
            if (Application.CanStreamedLevelBeLoaded("Dungeon"))
            {
                SceneManager.LoadScene("Dungeon");
            }
            else if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(ScreenType.DungeonSelect, false);
            }
        }

        private void OnWatchAd()
        {
            PopupManager.Instance?.ShowToast("광고 시청 성공! 보상 2배 지급.");
            if (watchAdBtn != null) watchAdBtn.interactable = false;

            if (Kingdom.ResourceManager.Instance != null && _latestResult != null)
            {
                Kingdom.ResourceManager.Instance.AddResource(Data.ResourceType.Gold, _latestResult.GoldReward);
                Kingdom.ResourceManager.Instance.AddResource(Data.ResourceType.Wood, _latestResult.WoodReward);
            }
        }
    }
}
