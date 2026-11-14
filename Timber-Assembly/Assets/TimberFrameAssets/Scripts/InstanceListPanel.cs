using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.Localization;

public class InstanceListPanel : MonoBehaviour
{

    [SerializeField] private InstanceEntryUI entryPrefab;
    [SerializeField] private Transform entryContainer;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject emptyMessage;

    private LocalizedString currentTitle;

    public event Action<XRInspectablePart> InstanceSelected;

    public void Show(PartInfo.ID pID)
    {
        Clear();

        int count = 0;
        foreach (XRInspectablePart part in PartCatalog.GetAllMatchingParts(pID).OrderBy(p => p.UniqueID, StringComparer.Ordinal))
        {
            InstanceEntryUI entry = Instantiate(entryPrefab, entryContainer);
            entry.Setup(part);
            entry.Selected += selected => InstanceSelected?.Invoke(selected);
            count++;
        }

        if (emptyMessage != null)
        {
            emptyMessage.SetActive(count == 0);
        }

        if (titleText != null)
        {
            currentTitle = PartInfo.GetName(pID);
            currentTitle.StringChanged += UpdateTitle;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        Clear();
        gameObject.SetActive(false);
    }

    private void UpdateTitle(string value) => titleText.text = value;

    private void Clear()
    {
        if (currentTitle != null)
        {
            currentTitle.StringChanged -= UpdateTitle;
            currentTitle = null;
        }

        foreach (Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy() => Clear();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
