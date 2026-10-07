using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class BuildingDetailScreen : ScreenBase
    {
        [SerializeField] private Image buildingImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI effectsText;
        
        [SerializeField] private Button upgradeBtn;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button demolishBtn;
        [SerializeField] private Button closeBtn;

        protected override void OnScreenShow()
        {
            closeBtn.onClick.AddListener(() => UIManager.Instance.Pop());
            upgradeBtn.onClick.AddListener(OnUpgradeClicked);
            demolishBtn.onClick.AddListener(OnDemolishClicked);

            LoadBuildingData();
        }

        protected override void OnScreenHide()
        {
            closeBtn.onClick.RemoveAllListeners();
            upgradeBtn.onClick.RemoveAllListeners();
            demolishBtn.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() {}

        private void LoadBuildingData()
        {
            nameText.text = "대장간";
            levelText.text = "Lv. 2";
            descriptionText.text = "장비의 등급을 올려주는 시설입니다.";
            effectsText.text = "- 장비 강화 비용 10% 감소\n- 물리 공격력 +5 (던전 버프)";
            upgradeCostText.text = "나무 200, 골드 500";
        }

        private void OnUpgradeClicked()
        {
            PopupManager.Instance.ShowConfirmation(
                "건물 업그레이드",
                "대장간을 레벨 3으로 업그레이드 하시겠습니까?",
                () => {
                    PopupManager.Instance.ShowToast("업그레이드 시작됨!");
                    LoadBuildingData();
                }
            );
        }

        private void OnDemolishClicked()
        {
            PopupManager.Instance.ShowConfirmation(
                "건물 철거",
                "정말로 이 건물을 철거하시겠습니까? 투입된 자원의 일부만 돌려받습니다.",
                () => {
                    UIManager.Instance.Pop();
                    PopupManager.Instance.ShowToast("건물이 철거되었습니다.");
                }
            );
        }
    }
}
