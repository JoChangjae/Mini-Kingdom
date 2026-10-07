using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MiniKingdom.UI
{
    public class ResourceBar : MonoBehaviour
    {
        [SerializeField] private Image resourceIcon;
        [SerializeField] private TextMeshProUGUI amountText;
        
        private int _currentAmount;

        public void SetAmount(int amount, bool animate = true)
        {
            if (animate && amount != _currentAmount)
            {
                UIAnimations.PunchScale(amountText.transform);
                // Can add NumberRoll animation here
            }
            
            _currentAmount = amount;
            amountText.text = _currentAmount.ToString("N0");
        }

        public void Flash()
        {
            UIAnimations.ScaleBounce(transform);
        }
    }
}
