using UnityEngine;
using System;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;

public class CatalogEntryUI : MonoBehaviour
{

    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private LocalizedString localizedName;

    public PartInfo.ID PartId {  get; private set; }

    // Raised when the learner selects this entry
    public event Action<PartInfo.ID> Selected;

    private void Awake()
    {
        button.onClick.AddListener(() => Selected?.Invoke(PartId));
    }

    public void Setup(PartTypeEntry entry)
    {
        PartId = entry.PartId;

        previewImage.sprite = entry.Preview;
        previewImage.enabled = entry.Preview != null;

        ClearName();
        localizedName = PartInfo.GetName(PartId);
        localizedName.StringChanged += UpdateName;
    }

    private void UpdateName(string value) => nameText.text = value;

    private void ClearName()
    {
        if (localizedName != null) localizedName.StringChanged -= UpdateName;
    }

    private void OnDestroy() => ClearName();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
