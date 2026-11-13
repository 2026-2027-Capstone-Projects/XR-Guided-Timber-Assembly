using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

// TODO: When there's no time crunch, refactor XRInspectablePart and PartInfo into Part to align with this implementation.

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(MultiObjectSmoothMover))]
public class Connection : MonoBehaviour
{
    #region Constants
    public const int ArchetypeBitfield = 0b11100000;
    public const string NamesTable = "ConnectionNames";
    public const string DescriptionsTable = "ConnectionDescriptions";
    #endregion

    #region Serialized Fields
    [SerializeField] private ID _uniqueID;
    [SerializeField] private Participant[] _participants;

    [Header("Selection Placement")]
    [SerializeField, Min(0.1f)] private float cameraDistance = 1.5f;
    [SerializeField, Min(0.1f)] private float moveDuration = 0.4f;
    [SerializeField, Min(0.05f)] private float inspectionScale = 1.0f;

    [Header("Auto-Configurable Data")]
    [SerializeField] private GameObject _timberFrame = null;
    [SerializeField] private XRHoverOutline _hoverOutline = null;
    [SerializeField] private Collider _selectionCollider = null;
    [SerializeField] private XRSimpleInteractable _interactable = null;
    [SerializeField] private MultiObjectSmoothMover _explosionMover = null;
    #endregion

    #region Enums
    /// <summary>ConnectionVariant Supertypes<para/>Stored within a 3-bit MSB bitfield of each <see cref="VariantCode"/></summary>
    public enum ArchetypeCode : byte
    {
        // Not a Connection / Unset
        None = 0,
        // Archetypes
        CONN_TPIN = 0b00100000, // SlatColumnTPin
        CONN_SLOT = 0b01000000, // RafterPurlinSlot
        CONN_DOWEL = 0b10000000  // TimberDowel
    }

    public enum VariantCode : byte
    {
        // Not a Connection / Unset
        None = 0,
        // CONN_TPIN Variants
        CONN_TPIN_WP_EC1 = 1 | (int)ArchetypeCode.CONN_TPIN, // { Wall Panel, EC1, T-Pin }
        CONN_TPIN_WP_EC2 = 2 | (int)ArchetypeCode.CONN_TPIN, // { Wall Panel, EC2, T-Pin }
        CONN_TPIN_2WP_MC = 3 | (int)ArchetypeCode.CONN_TPIN, // { Wall Panel, Wall Panel, MC, T-Pin, T-Pin }
        // CONN_SLOT Variants
        CONN_SLOT_RP_SAM = 1 | (int)ArchetypeCode.CONN_SLOT,
        CONN_SLOT_RP_RB = 2 | (int)ArchetypeCode.CONN_SLOT,
        CONN_SLOT_RP_TAM = 3 | (int)ArchetypeCode.CONN_SLOT,
        // CONN_DOWEL Variants
        CONN_DOWEL_EC1_SAM = 1 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_EC2_SAM = 2 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_MC_SAM = 3 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_SAM_RB = 4 | (int)ArchetypeCode.CONN_DOWEL,
        CONN_DOWEL_TAM_RB = 5 | (int)ArchetypeCode.CONN_DOWEL
    }
    #endregion

    #region Data Structures
    [Serializable]
    public struct ID
    {
        // Note: Unity cannot serialize 'readonly' fields. Use a private serialized backing field with a readonly getter.
        [SerializeField] private VariantCode _variantPrefix;
        private byte _uniqueIDSuffix; // Set dynamically at runtime; not serialized

        public readonly VariantCode VariantPrefix => _variantPrefix;
        public readonly byte UniqueIDSuffix => _uniqueIDSuffix;

        public ID(VariantCode variantPrefix, byte uniqueIDSuffix)
        {
            _variantPrefix = variantPrefix;
            _uniqueIDSuffix = uniqueIDSuffix;
        }

        public override readonly string ToString() => $"{_variantPrefix}-{_uniqueIDSuffix.ToUniqueIDSuffix()}";
    }

    [Serializable]
    public struct Participant
    {
        [Tooltip("Hashable XRInspectablePart reference")]
        //public XRInspectablePart.ID partUniqueID;
        public XRInspectablePart part;

        [Tooltip("Local offset relative to the Connection when exploded.")]
        //"The offset should assume the Connection is on the frame's local right, facing its local front.\n" +
        //"If it's not there, the Connection will automatically alter the offset to orient / match its mirrored nature.")]
        public Vector3 explodedLocalOffset;
    }

    private struct OriginalTransformState
    {
        public Transform parent;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }
    #endregion



    #region Private State
    private (Vector3 Position, Quaternion Rotation) _initialWorldRigidTransform;
    private OriginalTransformState _connectionOriginalLocalState;
    private OriginalTransformState[] _partOriginalStates;
    private MultiObjectSmoothMover.MoveTarget[] _moverTargets;

    private Coroutine _placementRoutine;
    #endregion

    #region Public Properties
    public VariantCode Variant => _uniqueID.VariantPrefix;
    public ArchetypeCode Archetype => Variant.ToArchetype();
    public ID UniqueID => _uniqueID;

    public LocalizedString Name => new LocalizedString(NamesTable, Variant.ToString());
    public LocalizedString Description => new LocalizedString(DescriptionsTable, Archetype.ToString());
    public XRHoverOutline HoverOutline => _hoverOutline;

    public bool IsInspecting { get; private set; }
    public bool IsExploded => _explosionMover != null && !_explosionMover.IsAtStateA;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (_selectionCollider == null)
            _selectionCollider = GetComponentInChildren<SphereCollider>();
        if (_interactable == null)
            _interactable = GetComponentInChildren<XRSimpleInteractable>();
        if (_explosionMover == null)
            _explosionMover = GetComponentInChildren<MultiObjectSmoothMover>();
        if (_explosionMover == null)
            _explosionMover.Duration = moveDuration;
        if (_hoverOutline == null)
            _hoverOutline = GetComponentInChildren<XRHoverOutline>();

        // Enforce uniform scale to prevent mesh shearing when reparenting parts
        transform.localScale = Vector3.one;

        // Standard Unity null check (avoids UnityEngine.Object fake-null bypass with ??=)
        if (_timberFrame == null)
        {
            PartCatalog catalog = GetComponentInParent<PartCatalog>();
            if (catalog != null)
            {
                _timberFrame = catalog.gameObject;
            }
        }

        if (_timberFrame != null)
        {
            transform.rotation = _timberFrame.transform.rotation; // Match rotation

            // If on the Timber Frame's local left side, flip explosions about the X-axis to match mirror symmetry
            /*
            bool isMirrored = _timberFrame.transform.InverseTransformPoint(transform.position).x < 0f;
            if (isMirrored)
            {
                for (int i = 0; i < _participants.Length; i++)
                {
                    _participants[i].explodedLocalOffset.x *= -1f;
                }
            }
            */
        }

        // Save initialized world & local transforms for drift-free restoration
        _initialWorldRigidTransform = (transform.position, transform.rotation);
        _connectionOriginalLocalState = CaptureState(transform);

        _partOriginalStates = new OriginalTransformState[_participants.Length];
        _moverTargets = new MultiObjectSmoothMover.MoveTarget[_participants.Length];
    }

    private void OnEnable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.AddListener(OnSelectEntered);
            _interactable.activated.AddListener(OnActivated);
            //_interactable.selectExited.AddListener(OnSelectExited);
        }


    }

    private void OnDisable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(OnSelectEntered);
            _interactable.activated.RemoveListener(OnActivated);
            //_interactable.selectExited.RemoveListener(OnSelectExited);
        }


    }
    #endregion

    #region Public API
    public List<XRInspectablePart> GetParts()
    {
        List<XRInspectablePart> parts = new List<XRInspectablePart>(_participants.Length);
        for (int i = 0; i < _participants.Length; i++)
        {
            //parts.Add(PartCatalog.GetPart(_participants[i].partUniqueID));
            if (_participants[i].part != null)
                parts.Add(_participants[i].part);
        }
        return parts;
    }

    /// <summary>
    /// Assigns the runtime Unique ID suffix while preserving the serialized VariantPrefix.
    /// </summary>
    public void SetUniqueIDSuffix(byte suffix)
    {
        _uniqueID = new ID(_uniqueID.VariantPrefix, suffix);
    }

    /// <summary>
    /// Groups participating XRInspectableParts under this Connection, computes local LineRenderer trail vectors,
    /// and moves the Connection in front of the active camera (or explicit target pose).
    /// </summary>
    public void BeginInspection(Transform referenceCamera = null)
    {
        if (IsInspecting) return;
        IsInspecting = true;
        //_selectionCollider.enabled = false;

        if (referenceCamera == null && Camera.main != null)
        {
            referenceCamera = Camera.main.transform;
        }

        // 1. Reparent participating parts & precompute local explosion + LineRenderer trail vectors
        for (int i = 0; i < _participants.Length; i++)
        {
            XRInspectablePart inspectable = _participants[i].part;
            if (inspectable == null) continue;

            // Tell parts to ignore interaction while they're part of a connection view, hook their interactable up to this one
            inspectable.DisableSelecting = true;
            if (inspectable.interactable != null)
                inspectable.interactable.parentInteractable = _interactable;

            Transform pt = inspectable.PartTransform;
            _partOriginalStates[i] = CaptureState(pt);

            // Reparent to Connection while keeping world position intact
            pt.SetParent(this.transform, true);

            Vector3 assembledLocalPos = pt.localPosition;
            Vector3 explodedLocalPos = assembledLocalPos + _participants[i].explodedLocalOffset;

            LineRenderer[] lines = inspectable.ExplodeLineRenderers;
            Vector3[] lineEndPoints = null;

            if (lines != null && lines.Length > 0)
            {
                lineEndPoints = new Vector3[lines.Length];
                Vector3 worldDisplacement = transform.TransformVector(_participants[i].explodedLocalOffset);

                for (int j = 0; j < lines.Length; j++)
                {
                    if (lines[j] != null)
                    {
                        // Express the inverse displacement in the LineRenderer's local space so Point 0 stays at (0,0,0)
                        // on the moving part and Point 1 stretches back to the assembled origin.
                        lineEndPoints[j] = lines[j].transform.InverseTransformVector(-worldDisplacement);
                    }
                }
            }

            _moverTargets[i] = new MultiObjectSmoothMover.MoveTarget
            {
                targetTransform = pt,
                positionA = assembledLocalPos,
                positionB = explodedLocalPos,
                lineRenderers = lines,
                fullExplosionLineEndPoints = lineEndPoints
            };

            XRHoverOutline hoverOutline = inspectable.HoverOutline;
            if (hoverOutline != null)
            {
                _hoverOutline.ManagedOutlines?.Add(hoverOutline);
            }
        }

        _explosionMover.Duration = moveDuration;
        _explosionMover.ConfigureTargets(_moverTargets, resetProgress: true);

        // 2. Calculate placement in front of the user's camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Cannot move selected Connection because no MainCamera was found.", this);
            return;
        }

        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * cameraDistance;

        StartPlacementRoutine(MoveConnectionRoutine(targetPosition, Vector3.one * inspectionScale));
    }

    /// <summary>
    /// Smoothly transitions between Assembled (false) and Exploded (true) views using MultiObjectSmoothMover.
    /// </summary>
    public void SetExploded(bool explode)
    {
        if (!IsInspecting) return;
        _explosionMover.TransitionToStateB(explode);
    }

    /// <summary>
    /// Toggles between Assembled and Exploded views (reversible mid-flight).
    /// </summary>
    public void ToggleExploded()
    {
        if (!IsInspecting) return;
        _explosionMover.ToggleState();
    }

    /// <summary>
    /// Collapses any active explosion, returns the Connection to its initial frame transform, and unparents all parts.
    /// </summary>
    public void EndInspection()
    {
        if (!IsInspecting) return;

        if (!_explosionMover.IsAtStateA)
        {
            _explosionMover.TransitionToStateB(false, OnCollapseCompleteBeforeReturn);
        }
        else
        {
            OnCollapseCompleteBeforeReturn();
        }
    }
    #endregion

    #region XR Interaction Handlers
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!IsInspecting)
        {
            BeginInspection();
            TriggerPopup();
        }
        else
        {
            ToggleExploded();
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (IsInspecting)
        {
            ToggleExploded();
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        _hoverOutline.SetSelectedOutline(false);
        EndInspection();
    }
    #endregion



    #region Internal Coroutines & Helpers
    private void OnCollapseCompleteBeforeReturn()
    {
        StartPlacementRoutine(ReturnAndUnparentRoutine());
    }

    private void TriggerPopup()
    {
        if (PopupUIManager.Instance == null)
        {
            Debug.Log("PopupUIManager.Instance is null!");
            return;
        }

        PopupUIManager.Instance.ShowPopup(Name, Description, UniqueID.ToString());
    }

    private IEnumerator MoveConnectionRoutine(Vector3 targetWorldPos, Vector3 targetScale)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / moveDuration);

            transform.position = Vector3.Lerp(startPos, targetWorldPos, t);
            //transform.rotation = Quaternion.Slerp(startRot, targetWorldRot, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.position = targetWorldPos;
        //transform.rotation = targetWorldRot;
        transform.localScale = targetScale;
    }

    private IEnumerator ReturnAndUnparentRoutine()
    {
        // Use the live TimberFrame transform if it moved, otherwise use _initialWorldRigidTransform
        Vector3 returnWorldPos = (_connectionOriginalLocalState.parent != null)
            ? _connectionOriginalLocalState.parent.TransformPoint(_connectionOriginalLocalState.localPosition)
            : _initialWorldRigidTransform.Position;

        Quaternion returnWorldRot = (_connectionOriginalLocalState.parent != null)
            ? _connectionOriginalLocalState.parent.rotation * _connectionOriginalLocalState.localRotation
            : _initialWorldRigidTransform.Rotation;

        yield return MoveConnectionRoutine(returnWorldPos, Vector3.one);

        // Snap Connection to exact original local state
        RestoreState(transform, _connectionOriginalLocalState);

        // Unparent all participating parts and restore exact cached local transforms (zero drift)
        for (int i = 0; i < _participants.Length; i++)
        {
            XRHoverOutline hoverOutline = _participants[i].part?.HoverOutline;
            if (hoverOutline != null)
            {
                hoverOutline.SetSelectedOutline(false);
            }

            if (_participants[i].part != null)
            {
                RestoreState(_participants[i].part.transform, _partOriginalStates[i]);
                _participants[i].part.interactable.parentInteractable = null;
            }
        }

        _selectionCollider.enabled = true;
        IsInspecting = false;
    }

    private void StartPlacementRoutine(IEnumerator routine)
    {
        if (_placementRoutine != null) StopCoroutine(_placementRoutine);
        _placementRoutine = StartCoroutine(routine);
    }

    private static OriginalTransformState CaptureState(Transform t)
    {
        return new OriginalTransformState
        {
            parent = t.parent,
            localPosition = t.localPosition,
            localRotation = t.localRotation,
            localScale = t.localScale
        };
    }

    private static void RestoreState(Transform t, OriginalTransformState state)
    {
        t.SetParent(state.parent, false);
        t.localPosition = state.localPosition;
        t.localRotation = state.localRotation;
        t.localScale = state.localScale;
    }
    #endregion
}

#region Extensions
public static class ConnectionExtensions
{
    /// <summary>Extract the ArchetypeCode enum stored within the VariantCode MSB bitfield</summary>
    public static Connection.ArchetypeCode ToArchetype(this Connection.VariantCode connectionVariant)
        => (Connection.ArchetypeCode)((int)connectionVariant & Connection.ArchetypeBitfield);
}
#endregion
