using UnityEngine;
using System;
using System.Collections.Generic;

public class CatalogSelectionController : MonoBehaviour
{

    [SerializeField] private PartCatalogPanel typePanel;
    [SerializeField] private InstanceListPanel instancePanel;

    private readonly List<XRHoverOutline> highlighted = new List<XRHoverOutline>();

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

    private void OnInstanceSelected(XRInspectablePart part) => InstanceSelected?.Invoke(part);

    // Clears the highlight and hides the instance list
    public void ClearSelection()
    {
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
