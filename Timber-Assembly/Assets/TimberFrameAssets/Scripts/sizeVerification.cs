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

    /* public static bool TryGetDimensions(Transform part, out Transform meshTransform, out Bounds meshBounds, out Vector3 dimensions)
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
     } */

    // Changing to fix Dimension readings on display
    public static bool TryGetDimensions(Transform part, out Transform boundsTransform, out Bounds combinedBounds, out Vector3 dimensions)
    {
        boundsTransform = part;
        combinedBounds = default;
        dimensions = default;

        if (part == null)
        {
            return false;
        }

        // Find all meshes belonging to component
        MeshFilter[] meshFilters = part.GetComponentsInChildren<MeshFilter>();

        bool foundMesh = false;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh == null)
            {
                continue;
            }

            Bounds meshBounds = meshFilter.sharedMesh.bounds;

            // Examine all eight corners of each mesh's bounds
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = new Vector3((i & 1) == 0 ? meshBounds.min.x : meshBounds.max.x,
                                             (i & 2) == 0 ? meshBounds.min.y : meshBounds.max.y,
                                             (i & 4) == 0 ? meshBounds.min.z : meshBounds.max.z);
                // Convert mesh-local coordinates into world space then into the component's local coordinate system
                Vector3 worldCorner = meshFilter.transform.TransformPoint(corner);
                Vector3 partLocalCorner = part.InverseTransformPoint(worldCorner);

                if (!foundMesh)
                {
                    combinedBounds = new Bounds(partLocalCorner, Vector3.zero);
                    foundMesh = true;
                } else
                {
                    combinedBounds.Encapsulate(partLocalCorner);
                }
            }
        }

        if (!foundMesh)
        {
            return false;
        }

        // Dimensions now measured along the components local axes
        dimensions = combinedBounds.size;

        return true;
    }
}
