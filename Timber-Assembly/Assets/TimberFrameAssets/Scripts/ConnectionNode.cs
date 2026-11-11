using System;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;


public class ConnectionNode : MonoBehaviour
{
    /// <summary>
    /// struct to hold data for diagrammatic exploding, including the <see cref="ExplodableObject"/>, the <see cref="Vector3"/> total movement when the exploded state is enabled, and the object's position relative to the connection node.
    /// </summary>
    [Serializable]
    public struct ExplosionData
    {
        public ExplodableObject explodableObject;
        public Vector3 moveVector;
        public Vector3 positionRelativeToConnection;
        public Quaternion rotationRelativeToConnection;

        /// <summary>
        ///     Fetch the <see cref="XRInspectablePart"/> component attached to the <see cref="explodableObject"/> if it exists.
        /// </summary>
        /// <returns>The <see cref="XRInspectablePart"/> component if it exists, otherwise <see langword="null"/>.</returns>
        public XRInspectablePart InspectablePartData => explodableObject?.InspectablePartComponent;
    }

    [SerializeField] private List<ExplosionData> _explosionData;
    private Vector3 _initialPosition;
    private Vector3 _initialRotation;


    public ExplodableObject[] components => _explosionData.Select(datum => datum.explodableObject).ToArray(); // Do not use this in update
    public Vector3[] componentMoveVectors => _explosionData.Select(datum => datum.moveVector).ToArray();
    public Vector3[] componentRelativePositions => _explosionData.Select(datum => datum.positionRelativeToConnection).ToArray();
    public IReadOnlyList<ExplosionData> explosionData => _explosionData;

    public List<XRInspectablePart> GetParts()
    {
        var parts = new List<XRInspectablePart>();

        foreach (var ed in _explosionData)
        {
            var ip = ed.InspectablePartData;

            if (ip != null)
                parts.Add(ip);
        }

        return parts;
    }

    public bool Explode()
    {
        Vector3 position = new Vector3(); // TODO: Set up method

        foreach (var ed in _explosionData)
        {
            ed.explodableObject.SmoothMoveTo(position);
        }
        return false;
    }

    private void MovePartsToNode()
    {
        foreach (var ed in _explosionData)
        {
            ed.explodableObject.MoveTo(transform.position + ed.positionRelativeToConnection);
            ed.explodableObject.transform.rotation = ed.rotationRelativeToConnection;
        }

        return;
    }

    private void Awake()
    {
        _initialPosition = transform.position;
        for (int i = 0; i < _explosionData.Count; i++)
        {
            var ed = _explosionData[i];
            ed.positionRelativeToConnection = ed.explodableObject.transform.position - _initialPosition;
        }
    }
}
