using UnityEngine;

[CreateAssetMenu(fileName = "PartDimensionData", menuName = "Timber Frame/Part Dimensions")]
public class PartDimensionData : ScriptableObject
{
    [Header("Component Identification")]
    [SerializeField] private PartInfo.ID partId = PartInfo.ID.None;

    /* [Header("Documented Dimensions (meters)")]
    [SerializeField, Min(0f)] private float widthMeters;
    [SerializeField, Min(0f)] private float heightMeters;
    [SerializeField, Min(0f)] private float lengthMeters; */

    [Header("Documented Dimensions (feet + inches)")]
    [SerializeField, Min(0f)] private float widthFeet;
    [SerializeField, Min(0f)] private float widthInches;
    [SerializeField, Min(0f)] private float heightFeet;
    [SerializeField, Min(0f)] private float heightInches;
    [SerializeField, Min(0f)] private float lengthFeet;
    [SerializeField, Min(0f)] private float lengthInches;

    public PartInfo.ID PartId => partId;
    //public float WidthMeters => widthMeters;
    //public float HeightMeters => heightMeters;
    //public float LengthMeters => lengthMeters;

    public float WidthFeet => widthFeet;
    public float WidthInches => widthInches;
    public float HeightFeet => heightFeet;
    public float HeightInches => heightInches;
    public float LengthFeet => lengthFeet;
    public float LengthInches => lengthInches;




}
