using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Kingdom;
using MiniKingdom.Player;
using MiniKingdom.Core;
using MiniKingdom.Data;

namespace MiniKingdom.UI
{
    /// <summary>
    /// In-dungeon merchant Goldie shop screen.
    /// Allows buying healing potions, stat upgrades, or relics using collected run gold.
    /// </summary>
    public class DungeonShopScreen : ScreenBase
    {
        [Header("Merchant Dialog")]
        [SerializeField] private TextMeshProUGUI merchantText;
        [SerializeField] private TextMeshProUGUI goldBalanceText;

        [Header("Item Slots")]
        [SerializeField] private Button potionBuyButton;
        [SerializeField] private TextMeshProUGUI potionPriceText;

        [SerializeField] private Button atkBuffBuyButton;
        [SerializeField] private TextMeshProUGUI atkBuffPriceText;

        [SerializeField] private Button relicBuyButton;
        [SerializeField] private TextMeshProUGUI relicPriceText;

        [Header("Leave")]
        [SerializeField] private Button leaveButton;

        private const int PotionCost = 35;
        private const int AtkBuffCost = 60;
        private const int RelicCost = 120;

        protected override void OnScreenShow()
        {
            UpdateGoldDisplay();
            if (merchantText != null)
                merchantText.text = "\"어서 오시오, 폐하! 던전 깊은 곳까지 진귀한 물건들을 공수해왔지요.\"";

            if (potionPriceText != null) potionPriceText.text = $"{PotionCost} G";
            if (atkBuffPriceText != null) atkBuffPriceText.text = $"{AtkBuffCost} G";
            if (relicPriceText != null) relicPriceText.text = $"{RelicCost} G";

            if (potionBuyButton != null) potionBuyButton.onClick.AddListener(BuyPotion);
            if (atkBuffBuyButton != null) atkBuffBuyButton.onClick.AddListener(BuyAtkBuff);
            if (relicBuyButton != null) relicBuyButton.onClick.AddListener(BuyRelic);
            if (leaveButton != null) leaveButton.onClick.AddListener(OnLeaveClicked);
        }

        protected override void OnScreenHide()
        {
            if (potionBuyButton != null) potionBuyButton.onClick.RemoveAllListeners();
            if (atkBuffBuyButton != null) atkBuffBuyButton.onClick.RemoveAllListeners();
            if (relicBuyButton != null) relicBuyButton.onClick.RemoveAllListeners();
            if (leaveButton != null) leaveButton.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() { }

        private void UpdateGoldDisplay()
        {
            int currentGold = ResourceManager.Instance != null 
                ? ResourceManager.Instance.GetAmount(ResourceType.Gold) 
                : 100;
            if (goldBalanceText != null) goldBalanceText.text = $"보유 골드: {currentGold} G";
        }

        private void BuyPotion()
        {
            if (ResourceManager.Instance != null && !ResourceManager.Instance.Spend(ResourceType.Gold, PotionCost))
            {
                if (merchantText != null) merchantText.text = "\"골드가 부족합니다, 폐하! 던전을 좀 더 털어보시지요.\"";
                return;
            }

            var player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                float maxHp = player.CalculateFinalStat(MiniKingdom.Player.StatType.MaxHP);
                player.Heal(maxHp * 0.5f);
            }

            if (potionBuyButton != null) potionBuyButton.interactable = false;
            UpdateGoldDisplay();
            if (merchantText != null) merchantText.text = "\"특제 회복 물약입니다! 상처가 씻은 듯이 나을 겁니다!\"";
        }

        private void BuyAtkBuff()
        {
            if (ResourceManager.Instance != null && !ResourceManager.Instance.Spend(ResourceType.Gold, AtkBuffCost))
            {
                if (merchantText != null) merchantText.text = "\"골드가 부족합니다, 폐하!\"";
                return;
            }

            var player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                player.AddModifier(new StatModifier
                {
                    statType = MiniKingdom.Data.StatType.ATK,
                    value = 0.2f,
                    isPercentage = true
                });
            }

            if (atkBuffBuyButton != null) atkBuffBuyButton.interactable = false;
            UpdateGoldDisplay();
            if (merchantText != null) merchantText.text = "\"숫돌로 칼날을 바짝 갈아드렸습니다! (공격력 +20%)\"";
        }

        private void BuyRelic()
        {
            if (ResourceManager.Instance != null && !ResourceManager.Instance.Spend(ResourceType.Gold, RelicCost))
            {
                if (merchantText != null) merchantText.text = "\"골드가 부족합니다, 폐하! 유물은 귀한 몸이라 비싸지요.\"";
                return;
            }

            var player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                player.AddModifier(new StatModifier
                {
                    statType = MiniKingdom.Data.StatType.HP,
                    value = 50f,
                    isPercentage = false
                });
                player.Heal(50f);
            }

            if (relicBuyButton != null) relicBuyButton.interactable = false;
            UpdateGoldDisplay();
            if (merchantText != null) merchantText.text = "\"고대 왕국의 거인의 심장을 획득하셨습니다! (최대체력 +50)\"";
        }

        private void OnLeaveClicked()
        {
            UIManager.Instance?.CloseCurrent();
            EventBus.Publish(new RoomClearedEvent(0));
        }
    }
}
