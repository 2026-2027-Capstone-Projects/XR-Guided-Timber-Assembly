using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class XRInspectableObject : MonoBehaviour
{
    [SerializeField] private LocalizedString localizedInfoTextHeader;
    [SerializeField] private LocalizedString localizedInfoTextBody;
    [Tooltip("Auto-detected on the same GameObject if left empty.")]
    [SerializeField] private XRHoverOutline hoverOutline;

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (hoverOutline == null)
        {
            hoverOutline = GetComponent<XRHoverOutline>();
        }
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
        interactable.activated.AddListener(OnActivate);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
        interactable.activated.RemoveListener(OnActivate);
    }

    public void OnSelect(SelectEnterEventArgs args)
    {
        TriggerPopup();
    }

    public void OnActivate(ActivateEventArgs args)
    {
        TriggerPopup();
    }

    private void TriggerPopup()
    {
        if (PopupUIManager.Instance != null)
        {
            PopupUIManager.Instance.ShowPopup(localizedInfoTextHeader, localizedInfoTextBody, hoverOutline);
        }
    }
}