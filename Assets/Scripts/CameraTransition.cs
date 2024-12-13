using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private Image blackScreenPanel; // Image for the black screen
    [SerializeField] private float transitionDuration = 1f; // Duration of the fade

    private void Start()
    {
        // Ensure the black screen is transparent at the start
        if (blackScreenPanel != null)
        {
            Color color = blackScreenPanel.color;
            color.a = 0;
            blackScreenPanel.color = color;
            blackScreenPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Black Screen Panel is not assigned.");
        }
    }

    public void FadeToBlack()
    {
        if (blackScreenPanel != null)
        {
            blackScreenPanel.gameObject.SetActive(true);
            StartCoroutine(FadePanel(blackScreenPanel, 0, 1));
        }
    }

    public void FadeFromBlack()
    {
        if (blackScreenPanel != null)
        {
            StartCoroutine(FadePanel(blackScreenPanel, 1, 0, () =>
            {
                blackScreenPanel.gameObject.SetActive(false);
            }));
        }
    }

    private IEnumerator FadePanel(Image panel, float startAlpha, float endAlpha, System.Action onComplete = null)
    {
        float elapsedTime = 0;
        Color color = panel.color;

        while (elapsedTime < transitionDuration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);
            panel.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        panel.color = color;
        onComplete?.Invoke();
    }
}

