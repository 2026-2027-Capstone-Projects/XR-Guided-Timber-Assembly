using UnityEngine;

[CreateAssetMenu(fileName = "PartDimensionData", menuName = "Timber Frame/Part Dimensions")]
public class PartDimensionData : ScriptableObject
{
    [Header("Component Identification")]
    [SerializeField] private PartInfo.ID partId = PartInfo.ID.None;

    [Header("Documented Dimensions (meters)")]
    [SerializeField, Min(0f)] private float widthMeters;
    [SerializeField, Min(0f)] private float heightMeters;
    [SerializeField, Min(0f)] private float lengthMeters;

    public PartInfo.ID PartId => partId;
    public float WidthMeters => widthMeters;
    public float HeightMeters => heightMeters;
    public float LengthMeters => lengthMeters;

}
