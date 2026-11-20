using UnityEngine;

public class sizeVerification : MonoBehaviour
{
    void Start()
    {
        if (TryGetDimensions(transform, out _, out _, out Vector3 size))
        {
            Debug.Log($"{name}: {size.x:F4} x {size.y:F4} x {size.z:F4} m");
        }
    }

    public static bool TryGetDimensions(Transform part, out Transform meshTransform, out Bounds meshBounds, out Vector3 dimensions)
    {
        MeshFilter meshFilter = part != null ? part.GetComponentInChildren<MeshFilter>() : null;
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            meshTransform = null;
            meshBounds = default;
            dimensions = default;
            return false;
        }

        meshTransform = meshFilter.transform;
        meshBounds = meshFilter.sharedMesh.bounds;
        Vector3 scaledSize = Vector3.Scale(meshBounds.size, meshTransform.lossyScale);
        dimensions = new Vector3(Mathf.Abs(scaledSize.x), Mathf.Abs(scaledSize.y), Mathf.Abs(scaledSize.z));
        return true;
    }
}
