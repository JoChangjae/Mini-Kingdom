using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MiniKingdom.Kingdom;
using MiniKingdom.Data;
using MiniKingdom.Core;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Fishing minigame UI overlay.
    /// Handles the tension bar logic and catch events.
    /// </summary>
    public class FishingMinigameUI : ScreenBase
    {
        [Header("Fishing UI Elements")]
        [SerializeField] private RectTransform tensionBar;
        [SerializeField] private RectTransform cursor;
        [SerializeField] private RectTransform greenZone;
        [SerializeField] private Button tapButton;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Settings")]
        [SerializeField] private float cursorSpeed = 300f; // Pixels per second
        
        private float _barWidth;
        private float _zoneWidth;
        private float _cursorPosition;
        private int _direction = 1;
        private bool _isPlaying;

        protected override void OnScreenShow()
        {
            _barWidth = tensionBar.rect.width;
            _zoneWidth = greenZone.rect.width;
            _cursorPosition = 0f;
            _direction = 1;
            _isPlaying = true;

            statusText.text = "Tap to Catch!";
            tapButton.onClick.AddListener(OnTapClicked);
        }

        protected override void OnScreenHide()
        {
            tapButton.onClick.RemoveListener(OnTapClicked);
            _isPlaying = false;
        }

        protected override void OnScreenUpdate()
        {
            if (!_isPlaying) return;

            // Move cursor back and forth
            _cursorPosition += _direction * cursorSpeed * Time.deltaTime;
            
            if (_cursorPosition > _barWidth / 2f)
            {
                _cursorPosition = _barWidth / 2f;
                _direction = -1;
            }
            else if (_cursorPosition < -_barWidth / 2f)
            {
                _cursorPosition = -_barWidth / 2f;
                _direction = 1;
            }

            cursor.anchoredPosition = new Vector2(_cursorPosition, cursor.anchoredPosition.y);
        }

        private void OnTapClicked()
        {
            if (!_isPlaying) return;

            _isPlaying = false;

            // Check if cursor is within the green zone
            // Assumes greenZone is anchored at center of tensionBar
            float halfZone = _zoneWidth / 2f;
            if (_cursorPosition >= -halfZone && _cursorPosition <= halfZone)
            {
                // 성공! (Success)
                statusText.text = "낚시 대성공!";
                statusText.color = Color.green;
                PlaySuccessAnimation();

                // 왕국 자원 추가 및 체력 25% 회복
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddResource(ResourceType.Food, 15);
                }
                var player = FindObjectOfType<Player.PlayerStats>();
                if (player != null)
                {
                    float maxHp = player.CalculateFinalStat(MiniKingdom.Player.StatType.MaxHP);
                    player.Heal(maxHp * 0.25f);
                }
            }
            else
            {
                // 실패... (Fail)
                statusText.text = "물고기를 놓쳤습니다...";
                statusText.color = Color.red;
                PlayFailAnimation();
            }

            // Close after a brief delay
            Invoke(nameof(CloseFishing), 1.5f);
        }

        private void PlaySuccessAnimation()
        {
            // TODO: Play nice particle effects or DOTween animations
            Debug.Log("Fishing Success!");
        }

        private void PlayFailAnimation()
        {
            // TODO: Play fail sound and shake effect
            Debug.Log("Fishing Failed.");
        }

        private void CloseFishing()
        {
            if (UIManager.Instance != null) UIManager.Instance.CloseCurrent();
            else gameObject.SetActive(false);
        }
    }
}
