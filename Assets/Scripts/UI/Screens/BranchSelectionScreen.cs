using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Dungeon;
using MiniKingdom.Core;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Branch selection popup when the current room offers multiple paths (e.g. Danger Combat vs Rest/Shop).
    /// </summary>
    public class BranchSelectionScreen : ScreenBase
    {
        [Header("Path A UI")]
        [SerializeField] private Button pathAButton;
        [SerializeField] private TextMeshProUGUI pathATitle;
        [SerializeField] private TextMeshProUGUI pathADesc;
        [SerializeField] private Image pathAIcon;

        [Header("Path B UI")]
        [SerializeField] private Button pathBButton;
        [SerializeField] private TextMeshProUGUI pathBTitle;
        [SerializeField] private TextMeshProUGUI pathBDesc;
        [SerializeField] private Image pathBIcon;

        private RoomConfig _roomA;
        private RoomConfig _roomB;

        public void SetupBranches(RoomConfig roomA, RoomConfig roomB)
        {
            _roomA = roomA;
            _roomB = roomB;

            if (pathATitle != null) pathATitle.text = GetRoomDisplayName(roomA?.Type ?? RoomType.Combat);
            if (pathADesc != null) pathADesc.text = GetRoomDescription(roomA?.Type ?? RoomType.Combat);

            if (pathBTitle != null) pathBTitle.text = GetRoomDisplayName(roomB?.Type ?? RoomType.Rest);
            if (pathBDesc != null) pathBDesc.text = GetRoomDescription(roomB?.Type ?? RoomType.Rest);
        }

        protected override void OnScreenShow()
        {
            Time.timeScale = 0f; // 일시 정지 후 선택 대기
            if (pathAButton != null) pathAButton.onClick.AddListener(OnChoosePathA);
            if (pathBButton != null) pathBButton.onClick.AddListener(OnChoosePathB);
        }

        protected override void OnScreenHide()
        {
            Time.timeScale = 1f;
            if (pathAButton != null) pathAButton.onClick.RemoveAllListeners();
            if (pathBButton != null) pathBButton.onClick.RemoveAllListeners();
        }

        protected override void OnScreenUpdate() { }

        private void OnChoosePathA()
        {
            UIManager.Instance.CloseCurrent();
            DungeonRunManager.Instance?.MoveToNextRoom(false);
        }

        private void OnChoosePathB()
        {
            UIManager.Instance.CloseCurrent();
            DungeonRunManager.Instance?.MoveToNextRoom(true);
        }

        private string GetRoomDisplayName(RoomType type)
        {
            switch (type)
            {
                case RoomType.Combat: return "⚔️ 일반 전투방";
                case RoomType.Rest: return "❤️ 평화로운 휴식방";
                case RoomType.Shop: return "🛒 방랑 상인 골디";
                case RoomType.Treasure: return "💎 비밀 보물방";
                case RoomType.Event: return "❓ 미지의 이벤트방";
                case RoomType.Boss: return "👑 보스방";
                default: return "방 진입";
            }
        }

        private string GetRoomDescription(RoomType type)
        {
            switch (type)
            {
                case RoomType.Combat: return "몬스터를 처치하고 골드와 스킬을 획득합니다.";
                case RoomType.Rest: return "HP를 40% 회복하거나 모닥불 낚시를 진행합니다.";
                case RoomType.Shop: return "골드로 즉석 물약, 스탯 강화 또는 유물을 구매합니다.";
                case RoomType.Treasure: return "희귀 보물 상자에서 강력한 유물을 얻습니다.";
                case RoomType.Event: return "위험과 행운이 공존하는 랜덤 이벤트를 만납니다.";
                case RoomType.Boss: return "던전의 지배자와 최후의 결전을 벌입니다.";
                default: return "다음 지역으로 이동합니다.";
            }
        }
    }
}
