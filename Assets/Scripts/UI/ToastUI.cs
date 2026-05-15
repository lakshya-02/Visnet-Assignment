using System.Collections;
using TMPro;
using UnityEngine;

namespace VisnetXR.UI
{
    public class ToastUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text label;
        [SerializeField] private float visibleSeconds = 2.2f;
        [SerializeField] private float fadeSeconds = 0.25f;

        private Coroutine activeRoutine;

        public void Bind(CanvasGroup group, TMP_Text text)
        {
            canvasGroup = group;
            label = text;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void Show(string message)
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(ShowRoutine(message));
        }

        private IEnumerator ShowRoutine(string message)
        {
            label.text = message;
            gameObject.SetActive(true);
            yield return Fade(1f);
            yield return new WaitForSeconds(visibleSeconds);
            yield return Fade(0f);
            gameObject.SetActive(false);
        }

        private IEnumerator Fade(float target)
        {
            float start = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeSeconds)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeSeconds);
                yield return null;
            }

            canvasGroup.alpha = target;
        }
    }
}
