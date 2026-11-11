using Unity.Tutorials.Editor;
using UnityEngine;

// Functional behavior for the MeasurementLine prefab.
[RequireComponent(typeof(LineRenderer))]
public class MLine : MonoBehaviour
{
    [SerializeField] private ObjectReference arrowHead1;
    [SerializeField] private ObjectReference arrowHead2;

    private LineRenderer lineRenderer;
    private LineRenderer endLine1;
    private LineRenderer endLine2;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        endLine1 = arrowHead1.SceneObjectReference.ReferencedObjectAsGameObject.GetComponent<LineRenderer>();
        endLine2 = arrowHead2.SceneObjectReference.ReferencedObjectAsGameObject.GetComponent<LineRenderer>();
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
