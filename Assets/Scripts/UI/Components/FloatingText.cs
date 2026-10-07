using UnityEngine;
using TMPro;
using MiniKingdom.Utils;

namespace MiniKingdom.UI
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private float floatSpeed = 50f;
        [SerializeField] private float lifeTime = 1f;
        
        private float _timer;
        private RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void Setup(string text, Color color, Vector2 startPos)
        {
            textMesh.text = text;
            textMesh.color = color;
            _rect.anchoredPosition = startPos + new Vector2(Random.Range(-20f, 20f), Random.Range(-10f, 10f));
            _timer = lifeTime;
            gameObject.SetActive(true);
            
            UIAnimations.ScaleBounce(transform, 0.2f, 1.3f);
        }

        private void Update()
        {
            _rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;
            _timer -= Time.deltaTime;

            Color c = textMesh.color;
            c.a = _timer / lifeTime;
            textMesh.color = c;

            if (_timer <= 0)
            {
                // ObjectPool.Instance.Return(gameObject);
                gameObject.SetActive(false); // Fallback
            }
        }
    }
}
