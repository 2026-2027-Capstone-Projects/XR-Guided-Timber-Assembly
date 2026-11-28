using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Omit .Interactables if using XRI 2.x

// TODO: When there's no time crunch, refactor XRInspectablePart and PartInfo into Part to align with this implementation.

[RequireComponent(typeof(XRSimpleInteractable))]
[RequireComponent(typeof(MultiObjectSmoothMover))]
public class Connection : MonoBehaviour
{
    #region Constants & Static Readonly
    public const int ArchetypeBitfield = 0b11100000;
    public const string NamesTable = "ConnectionNames";
    public const string DescriptionsTable = "ConnectionDescriptions";

    public static readonly String[] ArchetypeCodeStrings = Enum.GetNames(typeof(Connection.ArchetypeCode));
    public static readonly String[] VariantCodeStrings = Enum.GetNames(typeof(Connection.VariantCode));
    public static readonly int ArchetypeCodeCount = ArchetypeCodeStrings.Count();
    public static readonly int VariantCodeCount = VariantCodeStrings.Count();
    #endregion

    #region Manager Variables
    public static GameObject[] ArchetypeSceneRoots { get; private set; } = new GameObject[ArchetypeCodeCount];
    public static XRHoverOutline[] ArchetypeHoverOutlines { get; private set; } = new XRHoverOutline[ArchetypeCodeCount];
    #endregion

    #region Connection Swapping
    public static Connection ActiveConnection { get; private set; }
    private static Connection _pendingConnection;
    private bool _isEndingInspection = false;
    #endregion

    #region Serialized Fields
    [SerializeField] private ID _uniqueID;
    [SerializeField] private Participant[] _participants;

    [Header("Selection Placement")]
    [SerializeField, Min(0.1f)] private float CameraDistance = 1.5f;
    [SerializeField, Min(0.1f)] private float MoveDuration = 0.4f;
    [SerializeField, Min(0.05f)] private float InspectionScale = 1.0f;

    [Header("Auto-Configurable Data")]
    [SerializeField] private GameObject _timberFrame = null;
    [SerializeField] private XRHoverOutline _hoverOutline = null;
    [SerializeField] private Collider _selectionCollider = null;
    [SerializeField] private XRSimpleInteractable _interactable = null;
    [SerializeField] private MultiObjectSmoothMover _explosionMover = null;
    [SerializeField] private MeshRenderer _meshRenderer = null;
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
        public static readonly VariantCode[] UsesUniqueIDAlphabeticSuffix = { VariantCode.CONN_TPIN_WP_EC1, VariantCode.CONN_TPIN_WP_EC2 };

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

    #endregion



    #region Private State
    private (Vector3 Position, Quaternion Rotation) _initialWorldRigidTransform;
    private Transform[] _partOriginalParents;
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

    public bool IsInspecting { get; private set; } = false;
    public bool IsExploded => _explosionMover != null && !_explosionMover.IsAtStateA;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (_timberFrame == null)
        {
            PartCatalog catalog = GetComponentInParent<PartCatalog>();
            if (catalog != null)
            {
                _timberFrame = catalog.gameObject;
            }
        }
        if (_timberFrame != null)
            transform.rotation = _timberFrame.transform.rotation; // Match rotation
        if (_selectionCollider == null)
            _selectionCollider = GetComponentInChildren<SphereCollider>();
        if (_interactable == null)
            _interactable = GetComponentInChildren<XRSimpleInteractable>();
        if (_explosionMover == null)
            _explosionMover = GetComponentInChildren<MultiObjectSmoothMover>();
        if (_explosionMover != null)
            _explosionMover.Duration = MoveDuration;
        if (_hoverOutline == null)
            _hoverOutline = GetComponentInChildren<XRHoverOutline>();
        if (_meshRenderer == null)
            _meshRenderer = GetComponentInChildren<MeshRenderer>();

        // Enforce uniform scale to prevent mesh shearing when reparenting parts
        transform.localScale = Vector3.one;

        // Save initialized world & local transforms for drift-free restoration
        _initialWorldRigidTransform = (transform.position, transform.rotation);

        // If on the Timber Frame's local left side, flip explosions about the X-axis to match mirror symmetry
        bool isMirrored = _timberFrame?.transform.InverseTransformPoint(transform.position).x < 0f;
        for (int i = 0; i < _participants.Length; i++)
        {
            Transform partTransform = _participants[i].part.transform;
            PartInfo.ID pID = _participants[i].part.pID;

            // Initialize to part's default exploded vectors
            _participants[i].explodedLocalOffset.Set(
                PartInfo.ExplosionVectors[pID].x,
                PartInfo.ExplosionVectors[pID].y,
                PartInfo.ExplosionVectors[pID].z
            );

            // Alter default vectors into Connection-Variant-specific vectors, if they exist.
            switch ((_uniqueID.VariantPrefix, pID))
            {
                case (VariantCode.CONN_TPIN_WP_EC2, PartInfo.ID.WP): // WP movement is based on EC1 connections, reverse it for EC2
                    _participants[i].explodedLocalOffset.z *= -1f;
                    break;
                case (VariantCode.CONN_TPIN_2WP_MC, PartInfo.ID.WP): // In MC connection, Reverse EC1's WP. This way, both WP move away from the MC
                    if (transform.InverseTransformPoint(partTransform.position).x > 0f)
                        _participants[i].explodedLocalOffset.z *= -1f;
                    if (isMirrored)
                        _participants[i].explodedLocalOffset.z *= -1f; // Fix mirrored WP issues in this connection
                    break;
                case (VariantCode.CONN_DOWEL_TAM_RB, PartInfo.ID.RB): // Roof Beam movement is normally up and away from the SAM, this is reversed (down and away from TAM)
                    _participants[i].explodedLocalOffset.x *= -1f;
                    _participants[i].explodedLocalOffset.y *= -1f;
                    break;
                case (VariantCode.CONN_SLOT_RP_RB, PartInfo.ID.RB): // All of these should stay stationary in these specific Connection Variants
                case (VariantCode.CONN_TPIN_WP_EC1, PartInfo.ID.EC1):
                case (VariantCode.CONN_TPIN_WP_EC2, PartInfo.ID.EC2):
                case (VariantCode.CONN_TPIN_2WP_MC, PartInfo.ID.MC):
                    _participants[i].explodedLocalOffset.x = 0;
                    _participants[i].explodedLocalOffset.y = 0;
                    _participants[i].explodedLocalOffset.z = 0;
                    break;
            }

            // Mirror movements on the other side of the model
            if (isMirrored)
                _participants[i].explodedLocalOffset.x *= -1f;
        }

        _moverTargets = new MultiObjectSmoothMover.MoveTarget[_participants.Length];
    }

    private void Start()
    {
        // Get parents for re-parenting when returning parts to the full model.
        _partOriginalParents = new Transform[_participants.Length];
        for (int i = 0; i < _participants.Length; i++)
        {
            _partOriginalParents[i] = _participants[i].part?.PartTransform?.parent;
        }
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

        // If another connection is still active or collapsing, wait until it finishes completely
        if (ActiveConnection != null && ActiveConnection != this)
        {
            _pendingConnection = this;
            StartPlacementRoutine(WaitForPreviousCollapseRoutine(ActiveConnection, referenceCamera));
            return; // ActiveConnection.EndInspection(instant: true);
        }

        _pendingConnection = null;
        ActiveConnection = this;
        IsInspecting = true;
        _isEndingInspection = false;
        _meshRenderer.enabled = false;
        _selectionCollider.enabled = false;

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
                inspectable.interactable.selectEntered.AddListener(OnSelectEntered);
            inspectable.interactable.activated.AddListener(OnActivated);

            Transform pt = inspectable.PartTransform;

            // Reparent to Connection while keeping world position intact
            pt.SetParent(transform, true);

            Vector3 assembledLocalPos = pt.localPosition;
            Vector3 explodedLocalPos = assembledLocalPos + Vector3.Scale(_participants[i].explodedLocalOffset, inspectable.transform.localScale);

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

        _explosionMover.Duration = MoveDuration;
        _explosionMover.ConfigureTargets(_moverTargets, resetProgress: true);

        // 2. Calculate placement in front of the user's camera
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Cannot move selected Connection because no MainCamera was found.", this);
            return;
        }

        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * CameraDistance;

        StartPlacementRoutine(MoveConnectionRoutine(targetPosition, Vector3.one * InspectionScale));
    }

    private IEnumerator WaitForPreviousCollapseRoutine(Connection previous, Transform referenceCamera)
    {
        // Trigger collapse if it hasn't already started
        previous.EndInspection(instant: true);

        // Hold here until OnCollapseCompleteBeforeReturn() sets IsInspecting = false
        while (previous != null && previous.IsInspecting)
        {
            yield return null;
        }

        // Wait one extra frame so any same-frame SelectExit / Update ticks on the old connection finish
        yield return null;

        // Abort if the user clicked a third connection while we were waiting
        if (_pendingConnection != this) yield break;

        _pendingConnection = null;
        BeginInspection(referenceCamera);
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
    public void EndInspection(bool instant = false)
    {
        if (!IsInspecting || _isEndingInspection) return;

        _isEndingInspection = true;
        _meshRenderer.enabled = true;

        if (instant || _explosionMover.IsAtStateA)
        {
            _explosionMover.StopAllCoroutines();
            _explosionMover.ConfigureTargets(_moverTargets, resetProgress: true);
            OnCollapseCompleteBeforeReturn();
        }
        else
        {
            _explosionMover.TransitionToStateB(false, OnCollapseCompleteBeforeReturn);
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
    #endregion

    #region XRHoverOutline Methods
    // This is all set up by PartCatalog, as it requires PartCatalog to finish set up and receives nodes from its object tree

    public static XRHoverOutline InitializeArchetypeSceneRoot(ArchetypeCode archetype, GameObject localSceneRoot)
    {
        int idx = archetype.ToIndex();
        if (localSceneRoot == null || ArchetypeSceneRoots[idx] != null) return null;

        ArchetypeSceneRoots[idx] = localSceneRoot;

        // Set up archetype group-outlining
        XRHoverOutline managerHoverOutline = localSceneRoot.GetComponent<XRHoverOutline>() ?? localSceneRoot.AddComponent<XRHoverOutline>();
        ArchetypeHoverOutlines[idx] = managerHoverOutline;

        return managerHoverOutline;
    }

    public static void SetAllHoverOutline(bool active)
    {
        foreach (var archetypeHoverOutline in ArchetypeHoverOutlines)
            archetypeHoverOutline.SetHoverOutline(active);
    }

    public static void SetAllSelectedOutline(bool active)
    {
        foreach (var archetypeHoverOutline in ArchetypeHoverOutlines)
            archetypeHoverOutline.SetSelectedOutline(active);
    }
    #endregion

    #region Internal Coroutines & Helpers
    private void OnCollapseCompleteBeforeReturn()
    {
        if (_placementRoutine != null)
        {
            StopCoroutine(_placementRoutine);
            _placementRoutine = null;
        }

        // Prioritize repositioning, so parts are where they belong quickly.
        transform.position = _initialWorldRigidTransform.Position;
        transform.rotation = _initialWorldRigidTransform.Rotation;
        transform.localScale = Vector3.one;

        // Unparent all participating parts and restore transforms
        for (int i = 0; i < _participants.Length; i++)
        {
            XRInspectablePart inspectable = _participants[i].part;
            if (inspectable == null)
                continue;

            XRHoverOutline hoverOutline = inspectable.HoverOutline;
            if (hoverOutline != null)
            {
                hoverOutline.SetSelectedOutline(false);
                _hoverOutline.ManagedOutlines?.Remove(hoverOutline); // TODO: Add functionality to toggle management on/off so I don't have to repeatedly add/remove (easy fix)
            }

            inspectable.SnapToInitialPosition();
            inspectable.PartTransform.SetParent(_partOriginalParents[i], true);

            inspectable.DisableSelecting = false;
            if (inspectable.interactable != null)
                inspectable.interactable.selectEntered.RemoveListener(OnSelectEntered);
            inspectable.interactable.activated.RemoveListener(OnActivated);
        }

        _selectionCollider.enabled = true;
        _isEndingInspection = false;
        IsInspecting = false;

        if (ActiveConnection == this) // Race condition?
            ActiveConnection = null;
    }

    private void TriggerPopup()
    {
        if (PopupUIManager.Instance == null)
        {
            Debug.Log("PopupUIManager.Instance is null!");
            return;
        }

        PopupUIManager.Instance.ShowPopup(this);
    }

    private IEnumerator MoveConnectionRoutine(Vector3 targetWorldPos, Vector3 targetScale)
    {
        Vector3 startPos = transform.position;
        //Quaternion startRot = transform.rotation;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < MoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / MoveDuration);

            transform.position = Vector3.Lerp(startPos, targetWorldPos, t);
            //transform.rotation = Quaternion.Slerp(startRot, targetWorldRot, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.position = targetWorldPos;
        //transform.rotation = targetWorldRot;
        transform.localScale = targetScale;
    }

    private void StartPlacementRoutine(IEnumerator routine)
    {
        if (_placementRoutine != null) StopCoroutine(_placementRoutine);
        _placementRoutine = StartCoroutine(routine);
    }
    #endregion
}

#region Extensions
public static class ConnectionExtensions
{

    /// <summary>Extract the ArchetypeCode enum stored within the VariantCode MSB bitfield</summary>
    public static Connection.ArchetypeCode ToArchetype(this Connection.VariantCode connectionVariant) => (Connection.ArchetypeCode)((int)connectionVariant & Connection.ArchetypeBitfield);

    //public static string ToString(this Connection.VariantCode variant) => Connection.VariantCodeStrings[variant.ToIndex()];
    /*

CONN_DOWEL_TAM_RB = 5 | (int)ArchetypeCode.CONN_DOWEL
     */
    public static int ToIndex(this Connection.VariantCode variant)
    {
        switch (variant)
        {
            default:
                return 0;
            case Connection.VariantCode.CONN_TPIN_WP_EC1:
                return 1;
            case Connection.VariantCode.CONN_TPIN_WP_EC2:
                return 2;
            case Connection.VariantCode.CONN_TPIN_2WP_MC:
                return 3;
            case Connection.VariantCode.CONN_SLOT_RP_SAM:
                return 4;
            case Connection.VariantCode.CONN_SLOT_RP_RB:
                return 5;
            case Connection.VariantCode.CONN_SLOT_RP_TAM:
                return 6;
            case Connection.VariantCode.CONN_DOWEL_EC1_SAM:
                return 7;
            case Connection.VariantCode.CONN_DOWEL_EC2_SAM:
                return 8;
            case Connection.VariantCode.CONN_DOWEL_MC_SAM:
                return 9;
            case Connection.VariantCode.CONN_DOWEL_SAM_RB:
                return 10;
        }
    }
    public static Connection.VariantCode IdxToVariantCode(this int idx)
    {
        switch (idx)
        {
            case 0:
                return Connection.VariantCode.CONN_TPIN_WP_EC1;
            case 1:
                return Connection.VariantCode.CONN_TPIN_WP_EC2;
            case 2:
                return Connection.VariantCode.CONN_TPIN_2WP_MC;
            case 3:
                return Connection.VariantCode.CONN_SLOT_RP_SAM;
            case 4:
                return Connection.VariantCode.CONN_SLOT_RP_RB;
            case 5:
                return Connection.VariantCode.CONN_SLOT_RP_TAM;
            case 6:
                return Connection.VariantCode.CONN_DOWEL_EC1_SAM;
            case 7:
                return Connection.VariantCode.CONN_DOWEL_EC2_SAM;
            case 8:
                return Connection.VariantCode.CONN_DOWEL_MC_SAM;
            case 9:
                return Connection.VariantCode.CONN_DOWEL_SAM_RB;
            default:
                return Connection.VariantCode.None;
        }
    }

    public static XRHoverOutline InitializeSceneRoot(this Connection.ArchetypeCode archetype, GameObject gameObject) => Connection.InitializeArchetypeSceneRoot(archetype, gameObject);
    public static void SetHoverOutline(this Connection.ArchetypeCode archetype, bool active) => Connection.ArchetypeHoverOutlines[archetype.ToIndex()].SetHoverOutline(active);
    public static void SetSelectedOutline(this Connection.ArchetypeCode archetype, bool active) => Connection.ArchetypeHoverOutlines[archetype.ToIndex()].SetSelectedOutline(active);
    public static int ToIndex(this Connection.ArchetypeCode archetype) => archetype != Connection.ArchetypeCode.CONN_TPIN
                                                                                        ? archetype != Connection.ArchetypeCode.CONN_SLOT
                                                                                            ? archetype != Connection.ArchetypeCode.CONN_DOWEL
                                                                                                ? -1 // Error/None
                                                                                                : 2 // CONN_DOWEL
                                                                                            : 1 // CONN_SLOT
                                                                                        : 0; // CONN_TPIN
}
#endregion
