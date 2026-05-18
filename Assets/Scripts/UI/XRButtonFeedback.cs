using UnityEngine;
using UnityEngine.EventSystems;

namespace VisnetXR.UI
{
    /// <summary>
    /// Adds subtle hover and press scaling for controller-targeted UI buttons.
    /// </summary>
    public class XRButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float hoverScale = 1.025f;
        [SerializeField] private float pressScale = 0.985f;

        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = originalScale * hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = originalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = originalScale * pressScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = originalScale * hoverScale;
        }
    }
}
