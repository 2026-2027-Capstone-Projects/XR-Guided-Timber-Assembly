using UnityEngine;

public class sizeVerification : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var mf = GetComponentInChildren<MeshFilter>();
        Vector3 size = Vector3.Scale(mf.sharedMesh.bounds.size, mf.transform.lossyScale);
        Debug.Log($"{name}: {size.x:F4} x {size.y:F4} x {size.z:F4} m");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
