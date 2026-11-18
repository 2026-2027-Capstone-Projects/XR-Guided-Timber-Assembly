using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

[Serializable]
public struct SideDimension
{
    [Tooltip("Text shown in the popup, e.g. Side A")]
    public string label;
    [Min(0f)] public float feet;
    [Min(0f)] public float inches;
}



[CreateAssetMenu(fileName = "PartShapeDimensionData", menuName = "Timber Frame/Part Shape Dimensions")]
public class PartShapeDimensionData : ScriptableObject
{
    [Header("Component Identification")]
    [SerializeField] private PartInfo.ID partId = PartInfo.ID.None;

    [Header("Side Lengths (feet + inches), shown top to bottom")]
    [SerializeField] private List<SideDimension> sides = new List<SideDimension>();

    [Header("Thickness (feet + inches)")]
    [SerializeField, Min(0f)] private float thicknessFeet;
    [SerializeField, Min(0f)] private float thicknessInches;

    public PartInfo.ID PartId => partId;
    public IReadOnlyList<SideDimension> Sides => sides;
    public float ThicknessFeet => thicknessFeet;
    public float ThicknessInches => thicknessInches;
}
