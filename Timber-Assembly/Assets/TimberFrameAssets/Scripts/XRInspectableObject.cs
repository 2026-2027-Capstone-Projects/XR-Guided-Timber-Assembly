using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class XRInspectablePart : MonoBehaviour
{
    [Tooltip("The ID of this part type, used for fetching data.")]
    [SerializeField] private PartInfo.ID partID = PartInfo.ID.None;

    [Tooltip("Auto-detected on the same GameObject if left empty.")]
    [SerializeField] private XRHoverOutline hoverOutline;

    [Header("Selection Placement")]
    [SerializeField, Min(0.1f)] private float cameraDistance = 1.5f;

    [SerializeField, Min(0.1f)] private float moveDuration = 0.4f;
    [Header("Inspection View")]
    [SerializeField] private InspectionViewController inspectionViewController;
    [SerializeField] private InspectionUIController inspectionUIController;
    [Header("Documented Dimensions")]
    [SerializeField] private PartDimensionData documentedDimensions;

    private XRSimpleInteractable interactable;
    private Transform partTransform;
    private Vector3 originalPartPosition;
    private bool hasOriginalPartPosition;
    private Coroutine moveCoroutine;
    private LocalizedString localizedInfoTextHeader;
    private LocalizedString localizedInfoTextBody;

    /// <summary> Getter for the part identifier, used for fetching data and display in the popup. </summary>
    public PartInfo.ID PartID => partID;
    public PartInfo.ID pID => partID;

    /// <summary> Getter for the unique string, used for display in the popup. </summary>
    public string uID => PartCatalog.GetUID(this);
    /// <summary> Getter for the unique string, used for display in the popup. </summary>
    public string UniqueID => uID;

    /// <summary> Getter for the XRHoverOutline component reference used for highlighting the part. </summary>
    public XRHoverOutline HoverOutline => hoverOutline;

    /// <summary> Getter for the localized part name text, used for display in the popup. </summary>
    public LocalizedString PartName => localizedInfoTextHeader;

    /// <summary> Getter for the localized part description text, used for display in the popup. </summary>
    public LocalizedString PartDescription => localizedInfoTextBody;

    private void Awake()
    {
        // Cache reference to UX interface
        interactable = GetComponent<XRSimpleInteractable>();
        partTransform = transform.parent ?? transform;

        // Cache the LocalizedStrings
        localizedInfoTextHeader = PartInfo.GetName(partID);
        localizedInfoTextBody = PartInfo.GetDescription(partID);

        // Set up object outlining, if available
        hoverOutline ??= GetComponent<XRHoverOutline>();

        // Auto-detect inspection controllers if not assigned
        inspectionViewController ??= GetComponent<InspectionViewController>();
        inspectionUIController ??= GetComponentInChildren<InspectionUIController>();

    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
        interactable.activated.AddListener(OnActivate);
        //interactable.selectExited.AddListener(OnSelectExited);

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

    //public void OnSelectExited(SelectExitEventArgs args)
    //{
    //    RestoreOriginalPosition();
    //}

    private void TriggerPopup()
    {
        if (PopupUIManager.Instance == null || !QuestInspectionInput.HasHandedness)
        {
            return;
        }

        if (PopupUIManager.Instance.IsInspecting(this))
        {
            return;
        }

        PopupUIManager.Instance.ShowPopup(this);
        MovePartInFrontOfCamera();
        ShowDimensionsPopup();

        if (inspectionViewController != null)
        {
            inspectionViewController.SetInspecting(true);
        }

        if (inspectionUIController != null)
        {
            inspectionUIController.SetInspectionMode(true);
        }
    }

    /*pivate void ShowDimensionsPopup()
    {
        if (!sizeVerification.TryGetDimensions(partTransform, out Transform meshTransform, out Bounds meshBounds, out Vector3 dimensions))
        {
            return;
        }

        PopupDimensions dimensionsPopup = GetComponent<PopupDimensions>() ?? gameObject.AddComponent<PopupDimensions>();

        dimensionsPopup.Show(meshTransform, meshBounds, dimensions);
    }*/
    //Changing to match with sizeVerification changes
    private void ShowDimensionsPopup()
    {
        // Make sure documented dimensions have been assigned
        if (documentedDimensions == null)
        {
            Debug.LogWarning($"No documented dimensions assigned to {partID}.", this);
            return;
        }

        // Make sure the assigned data belongs to this component
        if (documentedDimensions.PartId != partID)
        {
            Debug.LogError($"Dimension data mismatch on {name}. "
                         + $"Expected {partID}, but asset contains "
                         + $"{documentedDimensions.PartId}.", this);
            return;
        }

        // Get mesh bounds only for popup positioning
        if (!sizeVerification.TryGetDimensions(partTransform, out Transform boundsTransform, out Bounds meshBounds, out Vector3 geometryDimensions))
        {
            Debug.LogWarning($"Could not calculate mesh bounds for {partTransform.name}.", this);
            return;
        }

        // Get PopupDimension component if it exists, otherwise add one.
        PopupDimensions dimensionsPopup = GetComponent<PopupDimensions>() ?? gameObject.AddComponent<PopupDimensions>();



        // Display documented dimensions
        // dimensionsPopup.Show(boundsTransform, meshBounds, documentedDimensions.WidthMeters, documentedDimensions.HeightMeters, documentedDimensions.LengthMeters);
        dimensionsPopup.Show(boundsTransform, meshBounds, documentedDimensions.WidthFeet, documentedDimensions.WidthInches,
                                                          documentedDimensions.HeightFeet, documentedDimensions.HeightInches,
                                                          documentedDimensions.LengthFeet, documentedDimensions.LengthInches);

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

        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * cameraDistance;
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MovePartToPosition(targetPosition));
    }

    private IEnumerator MovePartToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = partTransform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / moveDuration);
            partTransform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0f, 1f, progress));
            yield return null;
        }

        partTransform.position = targetPosition;
        moveCoroutine = null;
    }

    public void EndInspection()
    {
        if (inspectionViewController != null)
        {
            inspectionViewController.ResetInspectionViewOnExit();
            inspectionViewController.SetInspecting(false);
        }

        if (inspectionUIController != null)
        {
            inspectionUIController.SetInspectionMode(false);
        }

        RestoreOriginalPosition();
    }

    public void RestoreOriginalPosition()
    {
        if (!hasOriginalPartPosition) return;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        partTransform.position = originalPartPosition;
        hasOriginalPartPosition = false;
    }
}
