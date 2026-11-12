using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Manages the inspection view of a component, including rotation, enlargement (scaling), and reset functionality.
/// Stores original transform state to allow non-destructive inspection of the component.
/// </summary>
public class InspectionViewController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField, Min(0.1f)] private float rotationSensitivity = 2f;
    [SerializeField] private Vector2 rotationSpeedRange = new Vector2(30f, 120f);

    [Header("Mouse Rotation (editor/desktop testing)")]
    [SerializeField, Min(0.01f)] private float mouseDegreesPerPixel = 0.3f;

    [Header("Quest Rotation")]
    [SerializeField, Min(0.01f)] private float controllerDegreesPerMeter = 180f;

    [Header("Desktop Shortcuts (editor/desktop testing)")]
    [SerializeField] private Key enlargeKey = Key.Digit3;
    [SerializeField] private Key resetKey = Key.R;

    [Header("Enlargement Settings")]
    [SerializeField, Min(0.1f)] private float normalScale = 1f;
    [SerializeField, Min(0.1f)] private float enlargedScale = 1.5f;
    [SerializeField, Min(0.01f)] private float scaleTransitionDuration = 0.3f;

    [Header("UI Feedback")]
    [SerializeField] private Transform enlargedIndicatorUI; // Optional: UI element to show enlarged state

    // Same object XRInspectablePart moves (the parent holding the visible mesh).
    private Transform inspectedTransform;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;

    private bool isInspecting;
    private bool isEnlargedMode;
    private float scaleTransitionTimer;
    private Vector3 scaleTransitionStart;
    private Vector3 targetScale;
    private Vector3 currentScaleVelocity;

    private Coroutine rotationCoroutine;
    private Vector3 currentAngularVelocity;
    private bool hasControllerDragPosition;
    private Vector3 previousControllerPosition;

    public event System.Action EnlargementChanged;

    private void Awake()
    {
        inspectedTransform = transform.parent != null ? transform.parent : transform;
        CacheOriginalTransform();
    }

    private void OnEnable()
    {
        if (enlargedIndicatorUI != null)
        {
            enlargedIndicatorUI.gameObject.SetActive(isEnlargedMode);
        }
    }

    private void Update()
    {
        if (isInspecting)
        {
            RotateWithMouseDrag();
            HandleKeyboardShortcuts();
            HandleQuestControls();
        }
    }

    private void HandleKeyboardShortcuts()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard[enlargeKey].wasPressedThisFrame)
        {
            ToggleEnlargement();
        }
        if (keyboard[resetKey].wasPressedThisFrame)
        {
            ResetInspectionView();
        }
    }

    private void LateUpdate()
    {
        UpdateScaleTransition();
    }

    /// <summary>
    /// Enables or disables mouse-drag rotation (true while the part is being inspected).
    /// </summary>
    public void SetInspecting(bool inspecting)
    {
        isInspecting = inspecting;
        hasControllerDragPosition = false;
    }

    // Hold the left mouse button and drag; rotates around the camera's axes so it feels natural from any orientation.
    private void RotateWithMouseDrag()
    {
        Mouse mouse = Mouse.current;
        Camera cam = Camera.main;
        if (mouse == null || cam == null || !mouse.leftButton.isPressed)
        {
            return;
        }

        Vector2 delta = mouse.delta.ReadValue() * mouseDegreesPerPixel;
        if (delta == Vector2.zero)
        {
            return;
        }

        RotateFromDrag(cam, delta);
    }

    private void HandleQuestControls()
    {
        if (!QuestInspectionInput.IsAvailable || !QuestInspectionInput.HasHandedness)
        {
            hasControllerDragPosition = false;
            return;
        }

        Camera cam = Camera.main;
        if (!QuestInspectionInput.Grip.IsPressed() || !QuestInspectionInput.Tracked.IsPressed() || cam == null)
        {
            hasControllerDragPosition = false;
        }
        else
        {
            Vector3 position = QuestInspectionInput.Position.ReadValue<Vector3>();
            if (hasControllerDragPosition)
            {
                Vector3 delta = position - previousControllerPosition;
                Transform trackingOrigin = cam.transform.parent;
                Vector3 worldDelta = trackingOrigin != null ? trackingOrigin.TransformVector(delta) : delta;
                RotateFromDrag(cam, new Vector2(
                    Vector3.Dot(worldDelta, cam.transform.right),
                    Vector3.Dot(worldDelta, cam.transform.up)) * controllerDegreesPerMeter);
            }

            previousControllerPosition = position;
            hasControllerDragPosition = true;
        }

        if (QuestInspectionInput.Enlarge.WasPressedThisFrame())
        {
            ToggleEnlargement();
        }
        if (QuestInspectionInput.Reset.WasPressedThisFrame())
        {
            ResetInspectionView();
        }
    }

    private void RotateFromDrag(Camera cam, Vector2 delta)
    {
        inspectedTransform.Rotate(cam.transform.up, -delta.x, Space.World);
        inspectedTransform.Rotate(cam.transform.right, delta.y, Space.World);
    }

    /// <summary>
    /// Stores the current transform state so it can be restored during reset.
    /// </summary>
    private void CacheOriginalTransform()
    {
        originalLocalPosition = inspectedTransform.localPosition;
        originalLocalRotation = inspectedTransform.localRotation;
        originalLocalScale = inspectedTransform.localScale;

        scaleTransitionStart = originalLocalScale;
        targetScale = originalLocalScale;
        scaleTransitionTimer = scaleTransitionDuration;
        isEnlargedMode = false;
    }

    /// <summary>
    /// Toggles between normal and enlarged inspection mode.
    /// </summary>
    public void ToggleEnlargement()
    {
        SetEnlargedMode(!isEnlargedMode);
    }

    /// <summary>
    /// Sets the enlarged mode state and triggers scale transition.
    /// </summary>
    public void SetEnlargedMode(bool enlarged)
    {
        if (isEnlargedMode == enlarged)
            return;

        isEnlargedMode = enlarged;
        targetScale = isEnlargedMode ? 
            originalLocalScale * enlargedScale : 
            originalLocalScale * normalScale;

        scaleTransitionStart = inspectedTransform.localScale;
        scaleTransitionTimer = 0f;

        if (enlargedIndicatorUI != null)
        {
            enlargedIndicatorUI.gameObject.SetActive(isEnlargedMode);
        }
        EnlargementChanged?.Invoke();
    }

    /// <summary>
    /// Smoothly transitions the component's scale to the target scale.
    /// </summary>
    private void UpdateScaleTransition()
    {
        if (scaleTransitionDuration <= 0.001f)
        {
            inspectedTransform.localScale = targetScale;
            scaleTransitionTimer = 0f;
            return;
        }

        if (scaleTransitionTimer < scaleTransitionDuration)
        {
            scaleTransitionTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(scaleTransitionTimer / scaleTransitionDuration);
            progress = Mathf.SmoothStep(0f, 1f, progress);

            inspectedTransform.localScale = Vector3.Lerp(
                scaleTransitionStart,
                targetScale,
                progress
            );
        }
        else
        {
            inspectedTransform.localScale = targetScale;
        }
    }

    /// <summary>
    /// Initiates rotation of the component around its local Y-axis using angular velocity.
    /// </summary>
    /// <param name="direction">Rotation direction: 1 for clockwise, -1 for counter-clockwise</param>
    public void StartAutoRotation(int direction)
    {
        float speed = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y);
        currentAngularVelocity = Vector3.up * speed * direction;
    }

    /// <summary>
    /// Stops the automatic rotation immediately.
    /// </summary>
    public void StopAutoRotation()
    {
        currentAngularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Applies a discrete rotation to the component around the Y-axis (local).
    /// Used for button-controlled rotation increments.
    /// </summary>
    /// <param name="degrees">Amount to rotate in degrees (positive = counter-clockwise when viewed from above)</param>
    public void RotateByDegrees(float degrees)
    {
        inspectedTransform.Rotate(0f, degrees, 0f, Space.Self);
    }

    /// <summary>
    /// Rotates the component based on continuous input (e.g., analog stick or drag motion).
    /// </summary>
    /// <param name="inputDelta">Normalized input value (-1 to 1)</param>
    public void RotateByInput(float inputDelta)
    {
        float rotationAmount = inputDelta * rotationSensitivity * Time.deltaTime;
        inspectedTransform.Rotate(0f, rotationAmount, 0f, Space.Self);
    }

    /// <summary>
    /// Resets rotation and scale only; position is owned by XRInspectablePart.
    /// </summary>
    public void ResetInspectionView()
    {
        inspectedTransform.localRotation = originalLocalRotation;
        inspectedTransform.localScale = originalLocalScale * normalScale;

        targetScale = originalLocalScale * normalScale;
        scaleTransitionStart = targetScale;
        isEnlargedMode = false;
        scaleTransitionTimer = 0f;
        currentAngularVelocity = Vector3.zero;

        if (enlargedIndicatorUI != null)
        {
            enlargedIndicatorUI.gameObject.SetActive(false);
        }
        EnlargementChanged?.Invoke();
    }

    /// <summary>
    /// Returns whether the inspection view is currently in enlarged mode.
    /// </summary>
    public bool IsEnlarged()
    {
        return isEnlargedMode;
    }

    /// <summary>
    /// Gets the current scale factor relative to the normal scale.
    /// </summary>
    public float GetCurrentScaleFactor()
    {
        return inspectedTransform.localScale.x / originalLocalScale.x;
    }

    /// <summary>
    /// Gets the original local position of the component before inspection.
    /// </summary>
    public Vector3 GetOriginalPosition()
    {
        return originalLocalPosition;
    }

    /// <summary>
    /// Gets the original local rotation of the component before inspection.
    /// </summary>
    public Quaternion GetOriginalRotation()
    {
        return originalLocalRotation;
    }

    /// <summary>
    /// Gets the original local scale of the component before inspection.
    /// </summary>
    public Vector3 GetOriginalScale()
    {
        return originalLocalScale;
    }
}

internal static class QuestInspectionInput
{
    private static InputActionAsset actions;

    internal static InputAction Enlarge { get; private set; }
    internal static InputAction Reset { get; private set; }
    internal static InputAction Grip { get; private set; }
    internal static InputAction Position { get; private set; }
    internal static InputAction Tracked { get; private set; }
    internal static InputAction Close { get; private set; }
    internal static InputAction ChooseLeft { get; private set; }
    internal static InputAction ChooseRight { get; private set; }
    internal static bool IsAvailable => actions != null;
    internal static bool HasHandedness { get; private set; }
    internal static bool IsLeftHanded { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetSession()
    {
        HasHandedness = false;
        IsLeftHanded = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        if (actions != null)
        {
            actions.Disable();
        }

        actions = Resources.Load<InputActionAsset>("QuestInspectionControls");
        if (actions == null)
        {
            Debug.LogError("Quest inspection input actions could not be loaded.");
            return;
        }

        Enlarge = actions.FindAction("Inspection/Enlarge", true);
        Reset = actions.FindAction("Inspection/Reset", true);
        Grip = actions.FindAction("Inspection/Grip", true);
        Position = actions.FindAction("Inspection/Position", true);
        Tracked = actions.FindAction("Inspection/Tracked", true);
        Close = actions.FindAction("Inspection/Close", true);
        ChooseLeft = actions.FindAction("Inspection/ChooseLeft", true);
        ChooseRight = actions.FindAction("Inspection/ChooseRight", true);
        Enlarge.bindingMask = null;
        Reset.bindingMask = null;
        Grip.bindingMask = null;
        Position.bindingMask = null;
        Tracked.bindingMask = null;
        Close.bindingMask = null;
        actions.Enable();
    }

    internal static void SetHandedness(bool leftHanded)
    {
        string dominant = leftHanded ? "Left" : "Right";
        string offHand = leftHanded ? "Right" : "Left";
        Enlarge.bindingMask = InputBinding.MaskByGroup(dominant);
        Reset.bindingMask = InputBinding.MaskByGroup(dominant);
        Grip.bindingMask = InputBinding.MaskByGroup(dominant);
        Position.bindingMask = InputBinding.MaskByGroup(dominant);
        Tracked.bindingMask = InputBinding.MaskByGroup(dominant);
        Close.bindingMask = InputBinding.MaskByGroup(offHand);
        IsLeftHanded = leftHanded;
        HasHandedness = true;
    }
}
