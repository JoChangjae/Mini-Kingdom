using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Kingdom;
using MiniKingdom.Data;

namespace MiniKingdom.UI
{
    /// <summary>
    /// UI Card for individual kingdom buildings allowing direct inspection and instant upgrades.
    /// </summary>
    public class BuildingCardUI : MonoBehaviour
    {
        [SerializeField] private string buildingId;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI effectText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button upgradeBtn;
        [SerializeField] private TextMeshProUGUI upgradeBtnText;

        public string BuildingId => buildingId;

        public void SetReferences(string id, TextMeshProUGUI title, TextMeshProUGUI effect, TextMeshProUGUI cost, Button btn, TextMeshProUGUI btnTxt)
        {
            buildingId = id;
            titleText = title;
            effectText = effect;
            costText = cost;
            upgradeBtn = btn;
            upgradeBtnText = btnTxt;

            if (upgradeBtn != null)
            {
                upgradeBtn.onClick.RemoveListener(OnClickUpgrade);
                upgradeBtn.onClick.AddListener(OnClickUpgrade);
            }
            Refresh();
        }

        private void Start()
        {
            if (upgradeBtn != null)
            {
                upgradeBtn.onClick.RemoveListener(OnClickUpgrade);
                upgradeBtn.onClick.AddListener(OnClickUpgrade);
            }
            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (!Application.isPlaying) return;
            if (BuildingManager.Instance == null) return;
            var inst = BuildingManager.Instance.GetBuilding(buildingId);
            if (inst == null || inst.Data == null) return;

            if (titleText != null)
            {
                titleText.text = $"{inst.Data.buildingName}  Lv.{inst.Level}";
            }

            if (effectText != null)
            {
                effectText.text = inst.GetEffectDescription();
            }

            if (inst.Level >= inst.Data.MaxLevel)
            {
                if (costText != null) costText.text = "최고 레벨 (MAX)";
                if (upgradeBtn != null) upgradeBtn.interactable = false;
                if (upgradeBtnText != null) upgradeBtnText.text = "MAX";
            }
            else
            {
                int gold = BuildingManager.Instance.GetGoldCost(inst);
                int sec = BuildingManager.Instance.GetSecondaryCost(inst);
                ResourceType secType = BuildingManager.Instance.GetSecondaryType(inst);
                string secName = secType == ResourceType.Stone ? "석재" : "목재";

                if (costText != null)
                {
                    costText.text = $"비용: 골드 {gold:N0} | {secName} {sec:N0}";
                }

                if (upgradeBtn != null)
                {
                    bool canAfford = true;
                    if (ResourceManager.Instance != null)
                    {
                        canAfford = ResourceManager.Instance.HasResource(ResourceType.Gold, gold) &&
                                    ResourceManager.Instance.HasResource(secType, sec);
                    }
                    upgradeBtn.interactable = canAfford;
                }

                if (upgradeBtnText != null)
                {
                    upgradeBtnText.text = "강화";
                }
            }
        }

        public void OnClickUpgrade()
        {
            if (BuildingManager.Instance == null) return;
            var inst = BuildingManager.Instance.GetBuilding(buildingId);
            if (inst == null) return;

            if (BuildingManager.Instance.TryUpgradeBuilding(inst, out string msg))
            {
                PopupManager.Instance?.ShowToast(msg);
                Refresh();
            }
            else
            {
                PopupManager.Instance?.ShowToast(msg);
            }
        }
    }
}
