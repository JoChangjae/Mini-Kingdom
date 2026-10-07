using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        private bool _isSuccess;

        public void SetupResult(bool success)
        {
            _isSuccess = success;
        }

        protected override void OnScreenShow()
        {
            successTitle.SetActive(_isSuccess);
            failureTitle.SetActive(!_isSuccess);

            returnToKingdomBtn.onClick.AddListener(OnReturnToKingdom);
            tryAgainBtn.onClick.AddListener(OnTryAgain);
            watchAdBtn.onClick.AddListener(OnWatchAd);

            ShowRewards();
        }

        protected override void OnScreenHide()
        {
            returnToKingdomBtn.onClick.RemoveAllListeners();
            tryAgainBtn.onClick.RemoveAllListeners();
            watchAdBtn.onClick.RemoveAllListeners();

            foreach (Transform t in rewardContainer) Destroy(t.gameObject);
        }

        protected override void OnScreenUpdate() {}

        private void ShowRewards()
        {
            // 애니메이션으로 자원들이 차오르는 연출 (임시)
            dailyBonusText.text = "x1.5 일일 보너스 적용!";
            UIAnimations.PunchScale(dailyBonusText.transform);

            statsSummaryText.text = "방 통과: 12 | 몹 처치: 45 | 최대 콤보: 30";
            
            // Contribution bar update
            contributionBar.value = 0.7f;
            contributionText.text = "왕국 기여도 70%";
        }

        private void OnReturnToKingdom()
        {
            UIManager.Instance.Show(ScreenType.Kingdom, false);
        }

        private void OnTryAgain()
        {
            UIManager.Instance.Show(ScreenType.DungeonSelect, false);
        }

        private void OnWatchAd()
        {
            PopupManager.Instance.ShowToast("광고 시청 성공! 보상 2배 지급.");
            watchAdBtn.interactable = false;
        }
    }
}
