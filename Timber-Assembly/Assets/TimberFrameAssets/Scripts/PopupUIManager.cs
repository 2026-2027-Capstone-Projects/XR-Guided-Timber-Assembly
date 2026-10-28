using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;

public class PopupUIManager : MonoBehaviour
{
    public static PopupUIManager Instance { get; private set; }

    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text popupTextHeader;
    [SerializeField] private TMP_Text popupTextBody;
    [SerializeField] private TMP_Text popupTextIdentifier;
    [SerializeField] private Button closeButton;

    private LocalizedString currentLocalizedStringHeader;
    private LocalizedString currentLocalizedStringBody;
    private XRHoverOutline currentActiveOutline;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        closeButton.onClick.AddListener(ClosePopup);
        popupPanel.SetActive(false);
    }

    public void ShowPopup(LocalizedString header, LocalizedString body, PartInfo.ID identifier = PartInfo.ID.None, XRHoverOutline sourceOutline = null)
    {
        string errorHeader = null, errorBody = null;
        if (header == null || header.IsEmpty)
        {
            Debug.LogWarning("Assigned header LocalizedString is null or empty.");
            errorHeader = "NO HEADER FOUND";
        }
        if (body == null || body.IsEmpty)
        {
            Debug.LogWarning("Assigned body LocalizedString is null or empty.");
            errorBody = "NO BODY FOUND";
        }

        ClearCurrentState();

        currentActiveOutline = sourceOutline;
        if (currentActiveOutline != null)
        {
            currentActiveOutline.SetSelectedOutline(true);
        }

        // Subscribing to StringChanged automatically triggers an asynchronous fetch 
        // and invokes UpdateText immediately upon completion, as well as whenever the active Locale changes.

        // Header
        if (errorHeader != null)
        {
            UpdateHeader(errorHeader);
        }
        else
        {
            currentLocalizedStringHeader = header;
            currentLocalizedStringHeader.StringChanged += UpdateHeader;
        }

        // Body
        if (errorBody != null)
        {
            UpdateBody(errorBody);
        }
        else
        {
            currentLocalizedStringBody = body;
            currentLocalizedStringBody.StringChanged += UpdateBody;
        }

        // Identifier
        popupTextIdentifier.text = PartInfo.ToString(identifier);

        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        ClearCurrentState();
        popupPanel.SetActive(false);
    }

    private void UpdateHeader(string translatedValue)
    {
        popupTextHeader.text = translatedValue;
    }

    private void UpdateBody(string translatedValue)
    {
        popupTextBody.text = translatedValue;
    }

    private void ClearCurrentState()
    {
        // Unsubscribe from the header string
        if (currentLocalizedStringHeader != null)
        {
            currentLocalizedStringHeader.StringChanged -= UpdateHeader;
            currentLocalizedStringHeader = null;
        }
        // Unsubscribe from the body string
        if (currentLocalizedStringBody != null)
        {
            currentLocalizedStringBody.StringChanged -= UpdateBody;
            currentLocalizedStringBody = null;
        }
        // Clear the outline
        if (currentActiveOutline != null)
        {
            currentActiveOutline.SetSelectedOutline(false);
            currentActiveOutline = null;
        }
    }
}