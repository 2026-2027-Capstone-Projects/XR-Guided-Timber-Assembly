using UnityEngine;
using System;

public class PartCatalogPanel : MonoBehaviour
{
    [SerializeField] private PartTypeEntry[] entries;
    [SerializeField] private CatalogEntryUI entryPrefab;
    [SerializeField] private Transform entryContainer;

    // Raised when an entry is selected
    public event Action<PartInfo.ID> EntrySelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (PartTypeEntry entry in entries)
        {
            if (entry == null || entry.PartId == PartInfo.ID.None) continue;

            CatalogEntryUI ui = Instantiate(entryPrefab, entryContainer);
            ui.Setup(entry);
            ui.Selected += id => EntrySelected?.Invoke(id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
