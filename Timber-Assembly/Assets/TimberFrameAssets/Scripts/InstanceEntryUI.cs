using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class InstanceEntryUI : MonoBehaviour
{

    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;

    public XRInspectablePart Part
    {
        get; private set;
    }

    // Rasied when the learner selects this instance
    public event Action<XRInspectablePart> Selected;

    private void Awake()
    {
        button.onClick.AddListener(() => Selected?.Invoke(Part));
    }

    public void Setup(XRInspectablePart part)
    {
        Part = part;
        label.text = part.UniqueID;
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
