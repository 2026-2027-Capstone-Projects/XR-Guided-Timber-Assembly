using UnityEngine;
using System;
using System.Collections.Generic;

public class CatalogSelectionController : MonoBehaviour
{

    [SerializeField] private PartCatalogPanel typePanel;
    [SerializeField] private InstanceListPanel instancePanel;

    private readonly List<XRHoverOutline> highlighted = new List<XRHoverOutline>();

    private XRInspectablePart inspectedFromCatalog;

    // Raised when the learner selects one instance from the list
    public event Action<XRInspectablePart> InstanceSelected;

    private void OnEnable()
    {
        Debug.Log("[Controller] OnEnable (subscribing)");
        typePanel.EntrySelected += OnTypeSelected;
        instancePanel.InstanceSelected += OnInstanceSelected;
    }

    private void OnDisable()
    {
        Debug.Log("[Controller] OnDisable (unsubscribing)");
        typePanel.EntrySelected -= OnTypeSelected;
        instancePanel.InstanceSelected -= OnInstanceSelected;
        ClearHighlight();
    }

    private void OnTypeSelected(PartInfo.ID pID)
    {
        EndCatalogInspection();
        ClearHighlight();

        foreach (XRInspectablePart part in PartCatalog.GetAllMatchingParts(pID))
        {
            XRHoverOutline outline = part.HoverOutline;
            if (outline == null)
            {
                Debug.LogWarning($"{part.UniqueID} has no XRHoverOutline, so it can't be highlighted.", part);
                continue;
            }

            outline.SetSelectedOutline(true);
            highlighted.Add(outline);
        }

        instancePanel.Show(pID);
    }

    private void OnInstanceSelected(XRInspectablePart part)
    {
        InstanceSelected?.Invoke(part);
        inspectedFromCatalog = part;
        part.Inspect();
    }

    // Clears the highlight and hides the instance list
    public void ClearSelection()
    {
        EndCatalogInspection();
        ClearHighlight();
        instancePanel.Hide();
    }

    private void ClearHighlight()
    {
        foreach (XRHoverOutline outline in highlighted)
        {
            if (outline != null) outline.SetSelectedOutline(false);
        }
        highlighted.Clear();
    }

    private PopupUIManager popupManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // PopupUIManager sets itself to awake
        popupManager = PopupUIManager.Instance;
        if (popupManager != null)
        {
            popupManager.PopupClosed += ReapplyHighlight;
        }
    }

    private void OnDestroy()
    {
        if (popupManager != null)
        {
            popupManager.PopupClosed -= ReapplyHighlight;
        }
    }

    // PopupUIManager clears the selected outline of the part it is showing. Put the catalogs highlight back
    private void ReapplyHighlight()
    {
        foreach (XRHoverOutline outline in highlighted)
        {
            if (outline != null) 
            {
                outline.SetSelectedOutline(true);
            }
        }
    }

    // Closing the popup also sends the inspected part back to its original spot
    private void EndCatalogInspection()
    {
        if (popupManager != null && inspectedFromCatalog != null && popupManager.IsInspecting(inspectedFromCatalog))
        {
            popupManager.ClosePopup();
        }
        inspectedFromCatalog = null;
    }

}
