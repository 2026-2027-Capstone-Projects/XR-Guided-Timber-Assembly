using System.Collections;
using UnityEngine;

public class ExplodableObject : MonoBehaviour
{
    private bool _InitializeComponentRefd = false;
    private XRInspectablePart _inspectablePartComponent;
    [Tooltip("The duration of the explosion animation in seconds.")]
    [SerializeField, Min(0.1f)] private float movementDuration = 1.0f;
    [SerializeField] private LineTextureMode lineTextureMode = LineTextureMode.Tile;
    [SerializeField] private Material[] lineMaterials;
    [SerializeField] private XRInspectablePart inspectionComponent;
    [SerializeField] private LineRenderer lineRenderer;

    private bool exploded = false;
    private Vector3 explosionVector;

    /// <summary>
    ///     Reference to the <see cref="XRInspectablePart"/> component attached to this object if it exists.<para/>
    ///     Will be <see langword="null"/> if attached to an object without said component.
    /// </summary>
    public XRInspectablePart InspectablePartComponent => _inspectablePartComponent;

    public void GetPartDataDuringAwake() => InitializeComponentRef();

    public void MoveTo(Vector3 position) => transform.position = position;
    public void SmoothMoveTo(Vector3 position) => SmoothMove(position);
    private IEnumerator SmoothMove(Vector3 offset)
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 targetPosition = offset;
        //Vector3 targetPosition = startPosition + transform.TransformPoint(offset);
        float elapsedTime = 0f;

        // Iniitalize the line renderer for the movement, growing while exploding or shrinking while reverting
        if (exploded)
            lineRenderer.SetPosition(0, targetPosition);
        else
            lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, startPosition);
        lineRenderer.enabled = true;

        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / movementDuration);
            var currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

            transform.localPosition = currentPosition;
            lineRenderer.SetPosition(1, currentPosition); // Update the line renderer's end position

            yield return null;
        }

        // Finalize state to eliminate any floating point inaccuracies
        transform.localPosition = targetPosition;
        lineRenderer.SetPosition(1, targetPosition);

        // Toggle exploded state and keep rendering line if currently exploded
        exploded = !exploded;
        lineRenderer.enabled = exploded;
    }

    private XRInspectablePart InitializeComponentRef()
    {
        if (!_InitializeComponentRefd)
        {
            _inspectablePartComponent = GetComponent<XRInspectablePart>();
            _InitializeComponentRefd = true;
        }
        return _inspectablePartComponent;
    }

    void Awake()
    {
        InitializeComponentRef();

        if (!inspectionComponent)
            inspectionComponent = GetComponent<XRInspectablePart>();

        if (!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer)
            lineRenderer = gameObject.AddComponent<LineRenderer>(); // Add a LineRenderer if not present

        // Set up the lineRenderer properties
        lineRenderer.enabled = false; // Disable the line renderer initially
        lineRenderer.materials = lineMaterials;
        lineRenderer.textureMode = lineTextureMode;
        lineRenderer.generateLightingData = false;
        lineRenderer.positionCount = 2; // We only need two points for a line
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.1f; // Set the width of the line
        lineRenderer.endWidth = 0.1f;
        lineRenderer.numCapVertices = 0;
        lineRenderer.numCornerVertices = 0;

        // TODO: Remove this
        explosionVector = new Vector3(0.5f, 0, 0);
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

/****************************
 * 
 * ************************/



/* TODO: Consider Implementation details for the following.
/// <summary>
///     If the model is exploded,
/// </summary>

public void HardReset()
{
    if (exploded)
    {
        lineRenderer.enabled = false;
        transform.localPosition -= explosionVector;
        exploded = false;
    }
}

/// <summary>
///     Set up the explosion end-position and duration, as well as whether or not the movement is in world space or local space 
/// </summary>
/// <param name="explosionVector">
///     End-position of the explosion, in local space
/// </param>
/// <param name="duration">
///     Duration of the explosion animation in seconds
/// </param>
public void SetExplosionDetails(Vector3 explosionVector, float duration)
{
    // Ensure not exploded, then set properties
    if (exploded)
        HardReset();
    this.explosionVector = explosionVector;
    movementDuration = duration;

    return;
}

/// <summary>
///     LERP to the local-space position <see cref="explosionVector"/> if not exploded, or back to the original position if exploded.
/// </summary>
public void ToggleExplosion()
{
    Vector3 moveVector = exploded ? -explosionVector : explosionVector;
    StartCoroutine(SmoothMove(moveVector, movementDuration));
}

*/
