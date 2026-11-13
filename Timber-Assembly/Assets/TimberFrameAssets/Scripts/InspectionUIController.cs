using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the UI for the inspection view, providing buttons and visual feedback
/// for rotation, enlargement, and reset functionality.
/// </summary>
public class InspectionUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InspectionViewController inspectionViewController;
    [SerializeField] private PopupUIManager popupUIManager;

    [Header("UI Elements")]
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button enlargeButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button closeButton;

    [Header("Button Rotation")]
    [SerializeField, Min(0f)] private float buttonRotationDegrees = 45f;

    [Header("Enlargement Indicator")]
    [SerializeField] private TextMeshProUGUI enlargedStatusText;
    [SerializeField] private Color enlargedStatusColor = new Color(1f, 0.8f, 0f, 1f); // Yellow
    [SerializeField] private Color normalStatusColor = new Color(1f, 1f, 1f, 1f); // White

    [Header("Audio/Visual Feedback")]
    [SerializeField] private float buttonPressScale = 0.95f;
    [SerializeField, Min(0.01f)] private float buttonScaleRecoveryDuration = 0.2f;

    private bool isInspectionMode;

    private void Awake()
    {
        if (inspectionViewController == null)
        {
            inspectionViewController = GetComponentInParent<InspectionViewController>();
        }

        if (popupUIManager == null)
        {
            popupUIManager = PopupUIManager.Instance;
        }
    }

    private void OnEnable()
    {
        RegisterButtonListeners();
        if (inspectionViewController != null)
            inspectionViewController.EnlargementChanged += UpdateEnlargedStatusIndicator;
        UpdateEnlargedStatusIndicator();
    }

    private void OnDisable()
    {
        UnregisterButtonListeners();
        if (inspectionViewController != null)
            inspectionViewController.EnlargementChanged -= UpdateEnlargedStatusIndicator;
    }

    private void RegisterButtonListeners()
    {
        if (rotateLeftButton != null)
            rotateLeftButton.onClick.AddListener(OnRotateLeftPressed);
        if (rotateRightButton != null)
            rotateRightButton.onClick.AddListener(OnRotateRightPressed);
        if (enlargeButton != null)
            enlargeButton.onClick.AddListener(OnEnlargePressed);
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetPressed);
        if (closeButton != null)
            closeButton.onClick.AddListener(OnClosePressed);
    }

    private void UnregisterButtonListeners()
    {
        if (rotateLeftButton != null)
            rotateLeftButton.onClick.RemoveListener(OnRotateLeftPressed);
        if (rotateRightButton != null)
            rotateRightButton.onClick.RemoveListener(OnRotateRightPressed);
        if (enlargeButton != null)
            enlargeButton.onClick.RemoveListener(OnEnlargePressed);
        if (resetButton != null)
            resetButton.onClick.RemoveListener(OnResetPressed);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnClosePressed);
    }

    private void OnRotateLeftPressed()
    {
        if (inspectionViewController != null)
        {
            inspectionViewController.RotateByDegrees(-buttonRotationDegrees);
            PlayButtonPressAnimation(rotateLeftButton);
        }
    }

    private void OnRotateRightPressed()
    {
        if (inspectionViewController != null)
        {
            inspectionViewController.RotateByDegrees(buttonRotationDegrees);
            PlayButtonPressAnimation(rotateRightButton);
        }
    }

    private void OnEnlargePressed()
    {
        if (inspectionViewController != null)
        {
            inspectionViewController.ToggleEnlargement();
            PlayButtonPressAnimation(enlargeButton);
        }
    }

    private void OnResetPressed()
    {
        if (inspectionViewController != null)
        {
            inspectionViewController.ResetInspectionView();
            PlayButtonPressAnimation(resetButton);
        }
    }

    private void OnClosePressed()
    {
        if (popupUIManager != null)
        {
            popupUIManager.ClosePopup();
        }
        PlayButtonPressAnimation(closeButton);
    }

    private void UpdateEnlargedStatusIndicator()
    {
        if (inspectionViewController != null && enlargedStatusText != null)
        {
            bool isEnlarged = inspectionViewController.IsEnlarged();
            enlargedStatusText.text = isEnlarged ? "ENLARGED" : "NORMAL";
            enlargedStatusText.color = isEnlarged ? enlargedStatusColor : normalStatusColor;
        }
    }

    private void PlayButtonPressAnimation(Button button)
    {
        if (button == null)
            return;

        StartCoroutine(AnimateButtonPress(button.transform));
    }

    private System.Collections.IEnumerator AnimateButtonPress(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 pressedScale = originalScale * buttonPressScale;
        float elapsedTime = 0f;

        // Scale down
        while (elapsedTime < buttonScaleRecoveryDuration * 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / (buttonScaleRecoveryDuration * 0.5f));
            buttonTransform.localScale = Vector3.Lerp(originalScale, pressedScale, progress);
            yield return null;
        }

        elapsedTime = 0f;

        // Scale back up
        while (elapsedTime < buttonScaleRecoveryDuration * 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / (buttonScaleRecoveryDuration * 0.5f));
            buttonTransform.localScale = Vector3.Lerp(pressedScale, originalScale, progress);
            yield return null;
        }

        buttonTransform.localScale = originalScale;
    }

    public void SetInspectionMode(bool enabled)
    {
        isInspectionMode = enabled;
        gameObject.SetActive(enabled);
    }

    public bool IsInInspectionMode()
    {
        return isInspectionMode;
    }
}
