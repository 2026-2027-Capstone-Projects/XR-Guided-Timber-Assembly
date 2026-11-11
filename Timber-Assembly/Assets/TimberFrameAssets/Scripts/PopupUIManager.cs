using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;

public class PopupUIManager : MonoBehaviour
{
    public static PopupUIManager Instance { get; private set; }
    public event System.Action PopupClosed;

    [SerializeField] private GameObject _anchorObject;
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text popupTextHeader;
    [SerializeField] private TMP_Text popupTextBody;
    [SerializeField] private TMP_Text popupTextIdentifier;
    [SerializeField] private Button closeButton;
    [Header("Controller Anchor")]
    [SerializeField] private Vector3 leftControllerLocalPosition = new Vector3(0f, 0.12f, 0.2f);
    [SerializeField, Min(0.001f)] private float popupScale = 0.0025f;

    private LocalizedString currentLocalizedStringHeader;
    private LocalizedString currentLocalizedStringBody;
    private XRHoverOutline currentActiveOutline;
    private XRInspectablePart currentInspectablePart;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        AttachToLeftController();
        closeButton.onClick.AddListener(ClosePopup);
        popupPanel.SetActive(false);
    }

    private void Update()
    {
        // Desktop/editor shortcut for closing the popup and returning to the structure
        if (popupPanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePopup();
        }
    }

    private void AttachToLeftController()
    {
        transform.localScale = Vector3.one * popupScale;

        _anchorObject ??= GameObject.Find("Left Controller") ?? GameObject.Find("Right Controller");
        if (_anchorObject == null)
        {
            Debug.LogWarning("Cannot anchor popup because neither Left Controller no Right Controller was found.", this);
            return;
        }

        transform.SetParent(_anchorObject.transform, false);
        transform.localPosition = leftControllerLocalPosition;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    ///     Updates and displays the popup with <paramref name="sourcePart"/>'s name, unique ID, and description, using <see cref="XRInspectablePart"/> methods.
    /// </summary>
    /// <param name="sourcePart">
    ///     The part to display info for and to highlight while this popup is open,
    ///     <para/>
    ///     Highlighting requires an XRHoverOutline component
    /// </param>
    public void ShowPopup(XRInspectablePart sourcePart)
    {
        ClearCurrentState();

        if (sourcePart == null)
        {
            Debug.LogWarning("Cannot show popup because the provided source part is null.");
            return;
        }

        currentInspectablePart = sourcePart;
        (currentActiveOutline = sourcePart.HoverOutline)?.SetSelectedOutline(true);

        // Subscribing to StringChanged automatically triggers an asynchronous fetch 
        // and invokes UpdateText immediately upon completion, as well as whenever the active Locale changes.
        currentLocalizedStringHeader = sourcePart.PartName;
        currentLocalizedStringHeader.StringChanged += UpdateHeader;
        currentLocalizedStringBody = sourcePart.PartDescription;
        currentLocalizedStringBody.StringChanged += UpdateBody;

        // Set the part identifier text based on the part identifier and unique identifier
        popupTextIdentifier.text = sourcePart.UniqueID;
        popupPanel.SetActive(true);
    }

    /// <summary>
    ///     Updates and displays the popup with a localized <paramref name="header"/>, localized <paramref name="body"/>, and optional <paramref name="identifier"/>.
    /// </summary>
    /// <param name="header">
    ///     LocalizedString for the text to display across the top of the popup.
    /// </param>
    /// <param name="body">
    ///     LocalizedString for the text to display in the body of the popup.
    /// </param>
    /// <param name="identifier">
    ///     Optional string to display in the right-justified "identifier" text-area below the closeButton of the popup.
    /// </param>
    public void ShowPopup(LocalizedString header, LocalizedString body, string identifier = "")
    {
        ClearCurrentState();

        if (header == null)
        {
            popupTextHeader.text = "NO HEADER FOUND";
        }
        else
        {
            currentLocalizedStringHeader = header;
            currentLocalizedStringHeader.StringChanged += UpdateHeader;
        }

        if (body == null)
        {
            popupTextBody.text = "NO BODY FOUND";
        }
        else
        {
            currentLocalizedStringBody = body;
            currentLocalizedStringBody.StringChanged += UpdateBody;
        }

        popupTextIdentifier.text = identifier; // Allow identifier to be any arbitrary string, since this overload is not tied to PartInfo.ID or XRInspectablePart.

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
        if (currentInspectablePart != null)
        {
            currentInspectablePart.EndInspection();
            currentInspectablePart = null;
        }

        // Lets listeners (e.g. the dimensions panel) hide when the part is closed or another is selected
        PopupClosed?.Invoke();
    }
}
