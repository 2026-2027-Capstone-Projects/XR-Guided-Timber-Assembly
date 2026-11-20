using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

/// <summary>
/// Displays temporary messages in front of the main camera, including in XR.
/// Add this component to the camera to configure its durations, or let ShowOnMainCamera create it.
/// Later messages replace the current message and restart the fade.
/// </summary>
public class CameraNotification : MonoBehaviour
{
    [SerializeField, Min(0f)] private float displayDuration = 2f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private TMP_Text messageText;
    private Coroutine fadeCoroutine;
    private LocalizedString currentMessage;

    /// <summary>Shows localized text on the camera tagged MainCamera.</summary>
    public static void ShowOnMainCamera(LocalizedString message)
    {
        CameraNotification notification = GetMainCameraNotification();
        if (notification != null)
        {
            notification.Show(message);
        }
    }

    /// <summary>Shows plain text on the camera tagged MainCamera.</summary>
    public static void ShowOnMainCamera(string message)
    {
        CameraNotification notification = GetMainCameraNotification();
        if (notification != null)
        {
            notification.Show(message);
        }
    }

    private static CameraNotification GetMainCameraNotification()
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogWarning("Cannot show camera notification because no MainCamera was found.");
            return null;
        }

        CameraNotification notification = camera.GetComponent<CameraNotification>();
        if (notification == null)
        {
            notification = camera.gameObject.AddComponent<CameraNotification>();
        }

        return notification;
    }

    public void Show(LocalizedString message)
    {
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning("Cannot show a camera notification on a disabled component.", this);
            return;
        }
        if (message == null || message.IsEmpty)
        {
            Debug.LogWarning("Cannot show an empty localized camera notification.", this);
            return;
        }

        ClearMessage();
        currentMessage = message;
        currentMessage.StringChanged += DisplayMessage;
    }

    public void Show(string message)
    {
        if (!isActiveAndEnabled)
        {
            Debug.LogWarning("Cannot show a camera notification on a disabled component.", this);
            return;
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            Debug.LogWarning("Cannot show an empty camera notification.", this);
            return;
        }

        ClearMessage();
        DisplayMessage(message);
    }

    private void DisplayMessage(string message)
    {
        if (canvasGroup == null)
        {
            CreateCanvas();
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        messageText.text = message;
        canvasGroup.alpha = 1f;
        fadeCoroutine = StartCoroutine(FadeAfterDelay());
    }

    private IEnumerator FadeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(displayDuration);
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        fadeCoroutine = null;
        Unsubscribe();
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Camera Notification", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        canvasObject.transform.SetParent(transform, false);
        canvasObject.transform.localPosition = new Vector3(0f, -0.3f, 1.5f);
        canvasObject.transform.localScale = Vector3.one * 0.001f;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(800f, 120f);
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = GetComponent<Camera>();
        canvas.sortingOrder = 20;

        canvasGroup = canvasObject.GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Image background = canvasObject.AddComponent<Image>();
        background.color = new Color(0.08f, 0.1f, 0.12f, 0.9f);
        background.raycastTarget = false;

        GameObject textObject = new GameObject("Notification Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20f, 10f);
        textRect.offsetMax = new Vector2(-20f, -10f);
        messageText = textObject.GetComponent<TMP_Text>();
        messageText.fontSize = 36f;
        messageText.enableAutoSizing = true;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.white;
        messageText.raycastTarget = false;
    }

    private void ClearMessage()
    {
        Unsubscribe();
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void Unsubscribe()
    {
        if (currentMessage != null)
        {
            currentMessage.StringChanged -= DisplayMessage;
            currentMessage = null;
        }
    }

    private void OnDisable()
    {
        ClearMessage();
    }

    private void OnDestroy()
    {
        if (canvasGroup != null)
        {
            Destroy(canvasGroup.gameObject);
        }
    }
}
