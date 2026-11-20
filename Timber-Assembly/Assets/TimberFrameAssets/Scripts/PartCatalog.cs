using UnityEngine;
using System.Collections.Generic;

public class PartCatalog : MonoBehaviour
{
    /*****************
     * Lookup Tables *
     *****************/

    private static readonly Dictionary<PartInfo.ID, XRInspectablePart[]> _partsByPartID = new(); // Lists of all parts of the key's type

    // Bimap of parts and their uIDs
    private static readonly Dictionary<string, XRInspectablePart> _partsByUniqueID = new(); // <UID, XRInspectablePart> unique part lookup
    private static readonly Dictionary<XRInspectablePart, string> _uniqueIdsByPart = new(); // uID definitions are protected and uID strings are instantiated each time

    private static readonly Dictionary<XRInspectablePart, ConnectionNode[]> _connectionsByPart = new();
    private static readonly Dictionary<ConnectionNode, XRInspectablePart[]> _partsByConnection = new(); // Faster lookup by storing these here, in this format


    private static ConnectionNode[] _allConnections;
    private static XRInspectablePart[] _allParts;

    // Getters (Lists return a shallow copy of the stored list, defaulting to an empty list if key not found)

    /// <summary>
    ///     Fetches a reference to the part corresponding to the given unique ID.
    /// </summary>
    /// <param name="uID">The unique ID of the <see cref="XRInspectablePart"/> to retrieve.</param>
    /// <returns>The <see cref="XRInspectablePart"/> corresponding to the given unique ID, or <see langword="null"/> if the given <paramref name="uID"/> is not found.</returns>
    public static XRInspectablePart GetPart(string uID) => _partsByUniqueID.GetValueOrDefault(uID);
    /// <summary>Get all parts of type <paramref name="pID"/>.</summary>
    public static IReadOnlyList<XRInspectablePart> GetAllMatchingParts(PartInfo.ID pID) => _partsByPartID.TryGetValue(pID, out var parts) ? parts : new XRInspectablePart[0];
    /// <summary>Get all parts of the same type as <paramref name="part"/>.</summary>
    public static IReadOnlyList<XRInspectablePart> GetAllMatchingParts(XRInspectablePart part) => GetAllMatchingParts(part.pID);
    /// <summary>Get the UniqueID string of the given <paramref name="part"/>.</summary>
    public static string GetUID(XRInspectablePart part) => _uniqueIdsByPart.TryGetValue(part, out var uID) ? uID : part.pID.ToString();
    /// <summary>Get all connections within which the given <paramref name="part"/> exists.</summary>
    public static IReadOnlyList<ConnectionNode> GetConnectionsByPart(XRInspectablePart part) => _connectionsByPart.TryGetValue(part, out var cd) ? cd : new ConnectionNode[0];
    /// <summary>Get all parts which exist within the given <paramref name="connection"/></summary>
    public static IReadOnlyList<XRInspectablePart> GetConnectedParts(ConnectionNode connection) => _partsByConnection.TryGetValue(connection, out var parts) ? parts : new XRInspectablePart[0];
    /// <summary>Get all <see cref="ConnectionNode"/>s descended from the object containing the <see cref="PartCatalog"/> component</summary>
    public static IReadOnlyList<ConnectionNode> GetAllConnections() => _allConnections;
    /// <summary>Get all <see cref="XRInspectablePart"/>s descended from the object containing the <see cref="PartCatalog"/> component</summary>
    public static IReadOnlyList<XRInspectablePart> GetAllParts() => _allParts;

    private void BuildLookupTables()
    {

        // Clear Any Existing Lookup Tables
        _partsByPartID.Clear();
        _partsByUniqueID.Clear();
        _uniqueIdsByPart.Clear();
        _connectionsByPart.Clear();

        // Temporary caches to build the lookup tables before converting lists to arrays (reduces final memory usage)
        Dictionary<PartInfo.ID, List<XRInspectablePart>> partsByPartIDCache = new Dictionary<PartInfo.ID, List<XRInspectablePart>>();
        Dictionary<XRInspectablePart, List<ConnectionNode>> connectionsByPartCache = new Dictionary<XRInspectablePart, List<ConnectionNode>>();
        Dictionary<ConnectionNode, List<XRInspectablePart>> partsByConnectionCache = new Dictionary<ConnectionNode, List<XRInspectablePart>>();

        // Set up to populate Part Lookup Tables
        var pIDLength = PartInfo.KeyMap.Length;
        byte[] uIDAccumulator = new byte[pIDLength]; // Accumulates the next available unique ID suffix for each PartInfo.ID
        for (int i = 0; i < pIDLength; i++)
        {
            var pID = (PartInfo.ID)i;
            uIDAccumulator[i] = pID.MinUniqueID();
            partsByPartIDCache.Add(pID, new List<XRInspectablePart>()); // Initialize empty cache lists for each PartInfo.ID
        }
        // Populate Part Lookup Tables (Bimap of uID strings and objects, pID and object relationships)
        foreach (var inspectablePart in _allParts) // This will put the compontents in their Unity-Inspector order
        {
            var pID = inspectablePart.pID;
            _uniqueIdsByPart.Add(inspectablePart, pID.ToString() + "-" + (uIDAccumulator[(int)pID]++).ToUniqueIDSuffix());
            _partsByUniqueID.Add(_uniqueIdsByPart[inspectablePart], inspectablePart);
            partsByPartIDCache[pID].Add(inspectablePart);
        }

        // Populate Connection Lookup Tables
        foreach (var connectionNode in _allConnections)
        {
            var connectedParts = connectionNode.GetParts();

            // TODO: Add Connection IDs if needed (I think it is)

            partsByConnectionCache.TryAdd(connectionNode, connectedParts);

            foreach (var part in connectedParts)
            {
                connectionsByPartCache.TryAdd(part, new List<ConnectionNode>());
                connectionsByPartCache[part].Add(connectionNode);
            }
        }

        // Convert caches to arrays and store in the final lookup tables
        foreach (var kvp in partsByPartIDCache)
        {
            _partsByPartID.Add(kvp.Key, kvp.Value.ToArray());
        }

        foreach (var kvp in connectionsByPartCache)
        {
            _connectionsByPart.Add(kvp.Key, kvp.Value.ToArray());
        }

        foreach (var kvp in partsByConnectionCache)
        {
            _partsByConnection.Add(kvp.Key, kvp.Value.ToArray());
        }

    }

    void Awake()
    {
        _allConnections = GetComponentsInChildren<ConnectionNode>(true); // Load all ConnectionNode into the PartCatalog
        _allParts = GetComponentsInChildren<XRInspectablePart>(true); // Load all XRInspectablePart components

        // ScriptableObject? _allConnections = Resources.LoadAll<ConnectionNode>(""); // Load all ConnectionNode into the PartCatalog
        // Debug.Log($"Loaded {_allConnections.Length} parts into the catalog.");

        BuildLookupTables();
    }
}

public static class PartCatalogExtensions
{
    // Extend XRInspectablePart
    public static IReadOnlyList<XRInspectablePart> GetAllMatchingParts(this XRInspectablePart inspectablePartComponent) => PartCatalog.GetAllMatchingParts(inspectablePartComponent);
    public static IReadOnlyList<ConnectionNode> GetConnectionsByPart(this XRInspectablePart inspectablePartComponent) => PartCatalog.GetConnectionsByPart(inspectablePartComponent);

    // Extend PartInfo.ID
    public static IReadOnlyList<XRInspectablePart> GetAllMatchingParts(this PartInfo.ID pID) => PartCatalog.GetAllMatchingParts(pID);
}
