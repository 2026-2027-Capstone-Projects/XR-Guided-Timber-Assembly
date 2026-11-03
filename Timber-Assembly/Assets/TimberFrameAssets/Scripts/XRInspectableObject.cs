using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class XRInspectablePart : MonoBehaviour
{
    [Tooltip("The ID of this part, used for fetching data.")]
    [SerializeField] private PartInfo.ID partIdentifier = PartInfo.ID.None;
    [Tooltip("Auto-detected on the same GameObject if left empty.")]
    [SerializeField] private XRHoverOutline hoverOutline;
    [Header("Selection Placement")]
    [SerializeField, Min(0.1f)] private float cameraDistance = 1.5f;

    private XRSimpleInteractable interactable;
    private Transform partTransform;
    private Vector3 originalPartPosition;
    private bool hasOriginalPartPosition;
    private LocalizedString localizedInfoTextHeader;
    private LocalizedString localizedInfoTextBody;

    private void Awake()
    {
        // Cache reference to UX interface
        interactable = GetComponent<XRSimpleInteractable>();
        partTransform = transform.parent != null ? transform.parent : transform;

        // Cache the LocalizedStrings
        localizedInfoTextHeader = PartInfo.GetName(partIdentifier);
        localizedInfoTextBody = PartInfo.GetDescription(partIdentifier);

        // Set up object outlining, if available
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
        if (PopupUIManager.Instance == null)
        {
            return;
        }

        PopupUIManager.Instance.ShowPopup(localizedInfoTextHeader, localizedInfoTextBody, partIdentifier, hoverOutline, this);
        MovePartInFrontOfCamera();
    }

    private void MovePartInFrontOfCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Cannot move selected part because no MainCamera was found.", this);
            return;
        }

        if (!hasOriginalPartPosition)
        {
            originalPartPosition = partTransform.position;
            hasOriginalPartPosition = true;
        }

        partTransform.position = mainCamera.transform.position + mainCamera.transform.forward * cameraDistance;
    }

    public void RestoreOriginalPosition()
    {
        if (!hasOriginalPartPosition) return;

        partTransform.position = originalPartPosition;
        hasOriginalPartPosition = false;
    }
}
