using UnityEngine;
using UnityEngine.UI;

namespace MiniKingdom.UI
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private Image skillIcon;
        [SerializeField] private Image cooldownFill;
        [SerializeField] private Button button;
        
        private float _cooldownTimer = 0f;
        private float _maxCooldown = 1f;

        private void Update()
        {
            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                cooldownFill.fillAmount = _cooldownTimer / _maxCooldown;
                button.interactable = false;

                if (_cooldownTimer <= 0)
                {
                    cooldownFill.fillAmount = 0;
                    button.interactable = true;
                    UIAnimations.PunchScale(transform);
                }
            }
        }

        public void StartCooldown(float duration)
        {
            _maxCooldown = duration;
            _cooldownTimer = duration;
            cooldownFill.fillAmount = 1f;
            button.interactable = false;
        }

        public void SetIcon(Sprite icon)
        {
            skillIcon.sprite = icon;
        }
    }
}
