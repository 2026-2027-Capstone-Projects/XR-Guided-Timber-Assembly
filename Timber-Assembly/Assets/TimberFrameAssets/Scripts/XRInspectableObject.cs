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

    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
    }

    public void OnSelect(SelectEnterEventArgs args)
    {
        if (PopupUIManager.Instance != null)
        {
            PopupUIManager.Instance.ShowPopup(localizedInfoTextHeader, localizedInfoTextBody);
        }
    }
}