using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Player;
using MiniKingdom.Core;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Rest room screen where player can choose to rest (heal HP), fish for supplies, or meditate (ATK buff).
    /// </summary>
    public class RestRoomScreen : ScreenBase
    {
        [Header("Option Buttons")]
        [SerializeField] private Button restHealButton;
        [SerializeField] private Button fishingButton;
        [SerializeField] private Button meditateButton;
        [SerializeField] private Button leaveButton;

        [Header("Descriptions")]
        [SerializeField] private TextMeshProUGUI statusMessageText;

        private bool _actionTaken = false;

        protected override void OnScreenShow()
        {
            _actionTaken = false;
            if (statusMessageText != null) statusMessageText.text = "모닥불 가에서 휴식을 취합니다. 무엇을 하시겠습니까?";

            if (restHealButton != null)
            {
                restHealButton.interactable = true;
                restHealButton.onClick.AddListener(OnRestHealClicked);
            }
            if (fishingButton != null)
            {
                fishingButton.interactable = true;
                fishingButton.onClick.AddListener(OnFishingClicked);
            }
            if (meditateButton != null)
            {
                meditateButton.interactable = true;
                meditateButton.onClick.AddListener(OnMeditateClicked);
            }
            if (leaveButton != null)
            {
                leaveButton.onClick.AddListener(OnLeaveClicked);
            }
        }

        protected override void OnScreenHide()
        {
            if (restHealButton != null) restHealButton.onClick.RemoveAllListeners();
            if (fishingButton != null) fishingButton.onClick.RemoveAllListeners();
            if (meditateButton != null) meditateButton.onClick.RemoveAllListeners();
            if (leaveButton != null) leaveButton.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() { }

        private void OnRestHealClicked()
        {
            if (_actionTaken) return;
            _actionTaken = true;

            var player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                float maxHp = player.CalculateFinalStat(MiniKingdom.Player.StatType.MaxHP);
                player.Heal(maxHp * 0.45f);
            }

            if (statusMessageText != null)
                statusMessageText.text = "<color=#2ecc71>따스한 모닥불 온기에 체력을 45% 회복했습니다!</color>";

            DisableActions();
        }

        private void OnFishingClicked()
        {
            if (_actionTaken) return;
            _actionTaken = true;

            // 낚시 미니게임 열기
            UIManager.Instance?.Show(ScreenType.FishingMinigame);
            if (statusMessageText != null)
                statusMessageText.text = "강가에서 낚시를 시작합니다!";

            DisableActions();
        }

        private void OnMeditateClicked()
        {
            if (_actionTaken) return;
            _actionTaken = true;

            var player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                player.AddModifier(new Data.StatModifier
                {
                    statType = Data.StatType.ATK,
                    value = 0.15f,
                    isPercentage = true
                });
            }

            if (statusMessageText != null)
                statusMessageText.text = "<color=#f39c12>명상을 통해 정신을 집중하여 공격력이 15% 상승했습니다!</color>";

            DisableActions();
        }

        private void DisableActions()
        {
            if (restHealButton != null) restHealButton.interactable = false;
            if (fishingButton != null) fishingButton.interactable = false;
            if (meditateButton != null) meditateButton.interactable = false;
        }

        private void OnLeaveClicked()
        {
            if (UIManager.Instance != null) UIManager.Instance.CloseCurrent();
            else gameObject.SetActive(false);

            // 다음 방으로 진행
            var roomMgr = FindAnyObjectByType<MiniKingdom.Dungeon.RoomManager>();
            if (roomMgr != null)
            {
                EventBus.Publish(new RoomClearedEvent(0));
            }
            else
            {
                MiniKingdom.Dungeon.DungeonRunManager.Instance?.MoveToNextRoom(false);
            }
        }
    }
}
