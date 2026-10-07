using UnityEngine;
using System.Collections;
using System;

namespace MiniKingdom.UI
{
    /// <summary>
    /// Utility class for UI animations.
    /// Using coroutines for demonstration, typically DOTween is used here.
    /// </summary>
    public static class UIAnimations
    {
        public static void FadeIn(CanvasGroup cg, float duration, Action onComplete = null)
        {
            StaticCoroutineRunner.Instance.StartCoroutine(FadeCoroutine(cg, cg.alpha, 1f, duration, onComplete));
        }

        public static void FadeOut(CanvasGroup cg, float duration, Action onComplete = null)
        {
            StaticCoroutineRunner.Instance.StartCoroutine(FadeCoroutine(cg, cg.alpha, 0f, duration, onComplete));
        }

        private static IEnumerator FadeCoroutine(CanvasGroup cg, float start, float end, float duration, Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (cg != null)
                {
                    cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
                }
                yield return null;
            }
            if (cg != null) cg.alpha = end;
            onComplete?.Invoke();
        }

        public static void ScaleBounce(Transform t, float duration = 0.3f, float maxScale = 1.1f)
        {
            StaticCoroutineRunner.Instance.StartCoroutine(ScaleBounceCoroutine(t, duration, maxScale));
        }

        private static IEnumerator ScaleBounceCoroutine(Transform t, float duration, float maxScale)
        {
            Vector3 originalScale = t.localScale;
            float halfDuration = duration / 2f;
            
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                if (t != null)
                    t.localScale = Vector3.Lerp(originalScale, originalScale * maxScale, elapsed / halfDuration);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                if (t != null)
                    t.localScale = Vector3.Lerp(originalScale * maxScale, originalScale, elapsed / halfDuration);
                yield return null;
            }
            
            if (t != null) t.localScale = originalScale;
        }

        public static void PunchScale(Transform t, float amount = 1.2f, float duration = 0.2f)
        {
            ScaleBounce(t, duration, amount);
        }
    }

    /// <summary>
    /// Helper to run coroutines from static contexts.
    /// </summary>
    public class StaticCoroutineRunner : MonoBehaviour
    {
        private static StaticCoroutineRunner _instance;
        public static StaticCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("StaticCoroutineRunner");
                    _instance = go.AddComponent<StaticCoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
    }
}
