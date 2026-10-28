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
    [SerializeField] private Button closeButton;

    private LocalizedString currentLocalizedStringHeader;
    private LocalizedString currentLocalizedStringBody;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); // Uncomment this line if we want the PopupUIManager to persist across scenes

        closeButton.onClick.AddListener(ClosePopup);
        popupPanel.SetActive(false);
    }

    public void ShowPopup(LocalizedString header, LocalizedString body)
    {
        if (header == null || header.IsEmpty)
        {
            Debug.LogWarning("Assigned header LocalizedString is null or empty.");
            return;
        }
        else if (body == null || body.IsEmpty)
        {
            Debug.LogWarning("Assigned body LocalizedString is null or empty.");
            return;
        }

        UnsubscribeCurrentString();


        // Subscribing to StringChanged automatically triggers an asynchronous fetch 
        // and invokes UpdateText immediately upon completion, as well as whenever the active Locale changes.

        // Header
        currentLocalizedStringHeader = header;
        currentLocalizedStringHeader.StringChanged += UpdateHeader;

        // Body
        currentLocalizedStringBody = body;
        currentLocalizedStringBody.StringChanged += UpdateBody;

        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        UnsubscribeCurrentString();
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

    private void UnsubscribeCurrentString()
    {
        if (currentLocalizedStringHeader != null)
        {
            currentLocalizedStringHeader.StringChanged -= UpdateHeader;
            currentLocalizedStringHeader = null;
        }
        if (currentLocalizedStringBody != null)
        {
            currentLocalizedStringBody.StringChanged -= UpdateBody;
            currentLocalizedStringBody = null;
        }
    }
}