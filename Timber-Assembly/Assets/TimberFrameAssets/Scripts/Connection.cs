using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

// TODO: When there's no time crunch, refactor XRInspectablePart and PartInfo into Part to align with this implementation.
/*
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
public class Connection : MonoBehaviour
{
    #region Constants
    public const int ArchetypeBitfield = 0b11100000;
    public const string NamesTable = "ConnectionNames";
    public const string DescriptionsTable = "ConnectionDescriptions";
    #endregion


    #region Enums
    /// <summary>ConnectionVariant Supertypes<para/>Stored within a 3-bit MSB bitfield of each <see cref="ConnectionVariant"/></summary>
    public enum ArchetypeCode : byte
    {
        // Not a Connection / Unset
        None = 0,
        // Archetypes
        CONN_TPIN  = 0b00100000, // SlatColumnTPin,
        CONN_SLOT  = 0b01000000, // RafterPurlinSlot,
        CONN_DOWEL = 0b10000000 // TimberDowel
    }
    public enum VariantCode : byte
    {
        // Not a Connection / Unset
        None = 0,
        // CONN_TPIN Variants
        CONN_TPIN_WP_EC1   = 1 | (int)ArchetypeCode.CONN_TPIN, // { Wall Panel, EC1, T-Pin }
        CONN_TPIN_WP_EC2   = 2 | (int)ArchetypeCode.CONN_TPIN, // { Wall Panel, EC2, T-Pin }
        CONN_TPIN_2WP_MC   = 3 | (int)ArchetypeCode.CONN_TPIN, // { Wall Panel, Wall Panel, MC, T-Pin, T-Pin }
        // CONN_SLOT Variants
        CONN_SLOT_RP_SAM   = 1 | (int)ArchetypeCode.CONN_SLOT,
        CONN_SLOT_RP_RB    = 2 | (int)ArchetypeCode.CONN_SLOT,
        CONN_SLOT_RP_TAM   = 3 | (int)ArchetypeCode.CONN_SLOT,
        // CONN_DOWEL Variants
        CONN_DOWEL_EC1_SAM = 1 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_EC2_SAM = 2 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_MC_SAM  = 3 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_SAM_RB  = 4 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_TAM_RB  = 5 | (int)ArchetypeCode.CONN_DOWEL
    }
    #endregion


    #region Data Structures
    [Serializable]
    public readonly struct ID
    {
        [SerializeField]public readonly VariantCode VariantPrefix;
        private readonly byte _uniqueIDSuffix; // This is set dynamically, don't make it a SerializeField
        public byte UniqueIDSuffix => _uniqueIDSuffix;
        ID(VariantCode variantPrefix, byte uniqueIDSuffix)
        {
            VariantPrefix = variantPrefix;
            _uniqueIDSuffix = uniqueIDSuffix;
        }
        string ToString() => VariantPrefix.ToString()+"-"+UniqueIDSuffix.ToUniqueIDSuffix();
    }

    [Serializable]
    public struct Participant
    {
        [Tooltip("Hashable XRInspectablePart UniqueID")]
        //public XRInspectablePart.ID partUniqueID;
        public XRInspectablePart part;
        [Tooltip("Local offset relative to the Connection when exploded.\n" +
                 "The offset should assume the Connection is on the frame's local right, facing its local front.\n" +
                 "If it's not there, the Connection will automatically alter the offset to orient / match its mirrored nature.")]
        public Vector3 explodedLocalOffset;
        //[Tooltip("Optional child mesh/highlight object for Issue 4.3 interface")]
        //public XRHoverOutline hoverOutline;
    }
    #endregion

    [SerializeField] private ID _uniqueID;
    [SerializeField] private Participant[] _participants;
    

    [Header("Selection Placement")]
    [SerializeField, Min(0.1f)] private float cameraDistance = 1.5f;
    [SerializeField, Min(0.1f)] private float moveDuration = 0.4f;

    [Header("Auto-Configurable Data")]
    [SerializeField] private GameObject _timberFrame;

    private (Vector3 Position, Quaternion Rotation) _initialWorldRigidTransform;

    public VariantCode Variant => _uniqueID.VariantPrefix;
    public ArchetypeCode Archetype => Variant.ToArchetype();
    public ID UniqueID => _uniqueID; 

    public LocalizedString Name => new LocalizedString(NamesTable, Variant.ToString());
    public LocalizedString Description => new LocalizedString(DescriptionsTable, Archetype.ToString());


    public List<XRInspectablePart> GetParts()
    {
        List<XRInspectablePart> parts = new List<XRInspectablePart>();
        for (int i = 0; i < _participants.Length; i++)
        {
            //parts.Add(PartCatalog.GetPart(_participants[i].partUid));
            parts.Add(_participants[i].part);
        }
        return parts;
    }



    private void Awake()
    {
        // If unset, attempt to get the closest ancestor PartCatalog since the frame root should be the part manager.
        _timberFrame ??= GetComponentInParent<PartCatalog>().gameObject;

        if(_timberFrame != null)
        {
            transform.rotation = _timberFrame.transform.rotation; // Match rotation

            // If on the Timber Frame's local left side, flip explosions about the X-axis to match the Timberframe's mirror symmetry
            bool isMirrored = _timberFrame.transform.InverseTransformPoint(transform.position).x < 0f;

            if (isMirrored)
            {
                for (int i = 0; i < _participants.Length; i++)
                {
                    _participants[i].explodedLocalOffset.x *= -1;
                }
            }
        }

        _initialWorldRigidTransform = (transform.position, transform.rotation); // Save the initialized transform
    }
}


#region Extensions
public static class ConnectionExtensions
{

    /// <summary>Extract the ConnectionArchetype enum stored within the ConnectionArchetype enum</summary>
    public static Connection.ArchetypeCode ToArchetype(this Connection.VariantCode connectionVariant) => (Connection.ArchetypeCode)((int)connectionVariant & Connection.ArchetypeBitfield);
    public static string ToString(this Connection.VariantCode connectionVariant) => connectionVariant.ToString();
}
#endregion
*/
