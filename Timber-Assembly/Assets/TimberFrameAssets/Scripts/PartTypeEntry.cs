using UnityEngine;

[CreateAssetMenu(fileName = "PartTypeEntry", menuName = "Timber Frame/PartTypeEntry")]
public class PartTypeEntry : ScriptableObject
{
    [SerializeField] private PartInfo.ID partId = PartInfo.ID.None;
    [SerializeField] private Sprite preview;

    public PartInfo.ID PartId => partId;
    public Sprite Preview => preview;
    
}
