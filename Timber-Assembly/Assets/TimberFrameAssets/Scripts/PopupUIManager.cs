using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class PopupUIManager : MonoBehaviour
{
    public static PopupUIManager Instance { get; private set; }
    public event System.Action PopupClosed;

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
    public bool HasActiveInspection => currentInspectablePart != null;

    public bool IsInspecting(XRInspectablePart part)
    {
        return currentInspectablePart != null && currentInspectablePart == part;
    }

    private GameObject handednessPrompt;
    private TMP_Text handednessPromptText;
    private LocalizedString handednessPromptString;
    private readonly List<Behaviour> suspendedJumpProviders = new List<Behaviour>();
    private readonly List<Behaviour> suspendedGrabProviders = new List<Behaviour>();
    private bool warnedAboutMissingCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (QuestInspectionInput.HasHandedness)
        {
            AttachToOffHand();
        }
        closeButton.onClick.AddListener(ClosePopup);
        popupPanel.SetActive(false);
    }

    private void Start()
    {
        if (!QuestInspectionInput.HasHandedness)
        {
            SuspendLocomotion(suspendedJumpProviders, false);
            EnsureHandednessPrompt();
        }
    }

    private void Update()
    {
        if (!QuestInspectionInput.HasHandedness)
        {
            EnsureHandednessPrompt();
            if (!QuestInspectionInput.IsAvailable)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;
            bool keyboardLeft = keyboard != null && keyboard.lKey.wasPressedThisFrame;
            bool keyboardRight = keyboard != null && keyboard.rKey.wasPressedThisFrame;
            if (TryChooseHand(keyboardLeft, keyboardRight,
                QuestInspectionInput.ChooseLeft.WasPressedThisFrame(),
                QuestInspectionInput.ChooseRight.WasPressedThisFrame(), out bool leftHanded))
            {
                QuestInspectionInput.SetHandedness(leftHanded);
                HideHandednessPrompt();
                AttachToOffHand();
            }
            return;
        }

        if (!HasActiveInspection && QuestInspectionInput.IsAvailable &&
            !QuestInspectionInput.ChooseLeft.IsPressed() && !QuestInspectionInput.ChooseRight.IsPressed())
        {
            RestoreLocomotion(suspendedJumpProviders);
        }
        if (!HasActiveInspection && QuestInspectionInput.IsAvailable && !QuestInspectionInput.Grip.IsPressed())
        {
            RestoreLocomotion(suspendedGrabProviders);
        }

        if (HasActiveInspection && QuestInspectionInput.IsAvailable && QuestInspectionInput.Close.WasPressedThisFrame())
        {
            ClosePopup();
            return;
        }

        // Desktop/editor shortcut for closing the popup and returning to the structure
        if (popupPanel.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePopup();
        }
    }

    private static bool TryChooseHand(bool keyboardLeft, bool keyboardRight,
        bool controllerLeft, bool controllerRight, out bool leftHanded)
    {
        if (keyboardLeft != keyboardRight)
        {
            leftHanded = keyboardLeft;
            return true;
        }

        if (!keyboardLeft && !keyboardRight && controllerLeft != controllerRight)
        {
            leftHanded = controllerLeft;
            return true;
        }

        leftHanded = false;
        return false;
    }

    private void AttachToOffHand()
    {
        transform.localScale = Vector3.one * popupScale;

        string controllerName = QuestInspectionInput.IsLeftHanded ? "Right Controller" : "Left Controller";
        GameObject controller = GameObject.Find(controllerName);
        if (controller == null)
        {
            Debug.LogWarning($"Cannot anchor popup because no {controllerName} was found.", this);
            return;
        }

        transform.SetParent(controller.transform, false);
        Vector3 position = leftControllerLocalPosition;
        if (QuestInspectionInput.IsLeftHanded)
        {
            position.x = -position.x;
        }
        transform.localPosition = position;
        transform.localRotation = Quaternion.identity;
    }

    private void EnsureHandednessPrompt()
    {
        if (handednessPrompt != null)
        {
            return;
        }

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            if (!warnedAboutMissingCamera)
            {
                Debug.LogError("Cannot display the handedness prompt because no MainCamera was found.", this);
                warnedAboutMissingCamera = true;
            }
            return;
        }

        SuspendLocomotion(suspendedJumpProviders, false);
        handednessPrompt = new GameObject("Handedness Prompt", typeof(RectTransform), typeof(Canvas), typeof(Image));
        handednessPrompt.transform.SetParent(mainCamera.transform, false);
        handednessPrompt.transform.localPosition = new Vector3(0f, 0f, 1.3f);
        handednessPrompt.transform.localScale = Vector3.one * 0.0014f;
        handednessPrompt.GetComponent<RectTransform>().sizeDelta = new Vector2(800f, 300f);
        Canvas canvas = handednessPrompt.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        handednessPrompt.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.12f, 0.95f);
        handednessPrompt.GetComponent<Image>().raycastTarget = false;

        GameObject textObject = new GameObject("Prompt Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(handednessPrompt.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(20f, 20f);
        textRect.offsetMax = new Vector2(-20f, -20f);
        handednessPromptText = textObject.GetComponent<TextMeshProUGUI>();
        handednessPromptText.fontSize = 34f;
        handednessPromptText.alignment = TextAlignmentOptions.Center;
        handednessPromptText.raycastTarget = false;

        handednessPromptString = new LocalizedString(PartInfo.NamesTable, "HandednessPrompt");
        handednessPromptString.StringChanged += UpdateHandednessPrompt;
    }

    private void UpdateHandednessPrompt(string text)
    {
        if (handednessPromptText != null)
        {
            handednessPromptText.text = text;
        }
    }

    private void HideHandednessPrompt()
    {
        if (handednessPromptString != null)
        {
            handednessPromptString.StringChanged -= UpdateHandednessPrompt;
            handednessPromptString = null;
        }
        if (handednessPrompt != null)
        {
            Destroy(handednessPrompt);
            handednessPrompt = null;
            handednessPromptText = null;
        }
    }

    private void SuspendLocomotion(List<Behaviour> suspended, bool grab)
    {
        if (suspended.Count != 0)
        {
            return;
        }
        Camera mainCamera = Camera.main;
        XROrigin origin = mainCamera != null ? mainCamera.GetComponentInParent<XROrigin>() : null;
        if (origin == null)
        {
            return;
        }
        if (grab)
        {
            foreach (GrabMoveProvider provider in origin.GetComponentsInChildren<GrabMoveProvider>(true))
                SuspendProvider(provider, suspended);
            foreach (TwoHandedGrabMoveProvider provider in origin.GetComponentsInChildren<TwoHandedGrabMoveProvider>(true))
                SuspendProvider(provider, suspended);
        }
        else
        {
            foreach (JumpProvider provider in origin.GetComponentsInChildren<JumpProvider>(true))
                SuspendProvider(provider, suspended);
        }
    }

    private static void SuspendProvider(Behaviour provider, List<Behaviour> suspended)
    {
        if (provider.isActiveAndEnabled)
        {
            suspended.Add(provider);
            provider.enabled = false;
        }
    }

    private static void RestoreLocomotion(List<Behaviour> suspended)
    {
        foreach (Behaviour provider in suspended)
        {
            if (provider != null)
                provider.enabled = true;
        }
        suspended.Clear();
    }

    public void ShowPopup(LocalizedString header, LocalizedString body, PartInfo.ID identifier = PartInfo.ID.None, XRHoverOutline sourceOutline = null, XRInspectablePart sourcePart = null)
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
        SuspendLocomotion(suspendedJumpProviders, false);
        SuspendLocomotion(suspendedGrabProviders, true);

        currentActiveOutline = sourceOutline;
        if (currentActiveOutline != null)
        {
            currentActiveOutline.SetSelectedOutline(true);
        }
        currentInspectablePart = sourcePart;

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
        if (currentInspectablePart != null)
        {
            currentInspectablePart.EndInspection();
            currentInspectablePart = null;
        }
        // Lets listeners (e.g. the dimensions panel) hide when the part is closed or another is selected
        PopupClosed?.Invoke();
    }

    private void OnDisable()
    {
        if (HasActiveInspection)
        {
            ClosePopup();
        }
        HideHandednessPrompt();
        RestoreLocomotion(suspendedJumpProviders);
        RestoreLocomotion(suspendedGrabProviders);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ClosePopup);
        }
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
