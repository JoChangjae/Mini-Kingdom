using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Utils;
using System.Collections;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Abstract base class for all UI screens.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenBase : MonoBehaviour
    {
        [SerializeField] private ScreenType screenType;
        [SerializeField] private float transitionDuration = 0.3f;
        
        protected CanvasGroup canvasGroup;
        protected bool isVisible;

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public ScreenType GetScreenType() => screenType;

        /// <summary>
        /// Shows the screen with animation.
        /// </summary>
        public virtual void Show()
        {
            isVisible = true;
            OnScreenShow();
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Hides the screen with animation.
        /// </summary>
        public virtual void Hide()
        {
            isVisible = false;
            OnScreenHide();
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;
            
            // UIAnimations.ScaleBounce alternative
            transform.localScale = Vector3.one * 0.9f;
            
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                transform.localScale = Vector3.Lerp(Vector3.one * 0.9f, Vector3.one, t);
                yield return null;
            }
            canvasGroup.alpha = 1f;
            transform.localScale = Vector3.one;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (isVisible)
            {
                OnScreenUpdate();
                
                // Back button handling (Android/PC)
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnBackButtonPressed();
                }
            }
        }

        protected virtual void OnBackButtonPressed()
        {
            UIManager.Instance.Pop();
        }

        protected abstract void OnScreenShow();
        protected abstract void OnScreenHide();
        protected abstract void OnScreenUpdate();
    }
}
