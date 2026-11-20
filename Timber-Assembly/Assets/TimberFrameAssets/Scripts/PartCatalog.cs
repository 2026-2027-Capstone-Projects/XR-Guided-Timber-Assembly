using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;

public class PartCatalog : MonoBehaviour
{
    /*****************
     * Lookup Tables *
     *****************/

    private static readonly Dictionary<PartInfo.ID, XRInspectablePart[]> _partsByPartID = new(); // Lists of all parts of the key's type

    // Bimap of parts and their uIDs
    private static readonly Dictionary<string, XRInspectablePart> _partsByUniqueID = new(); // <UID, XRInspectablePart> unique part lookup
    private static readonly Dictionary<XRInspectablePart, string> _uniqueIdsByPart = new(); // uID definitions are protected and uID strings are instantiated each time

    private static readonly Dictionary<XRInspectablePart, Connection[]> _connectionsByPart = new();
    private static readonly Dictionary<Connection, List<XRInspectablePart>> _partsByConnection = new(); // Faster lookup by storing these here, in this format


    private static Connection[] _allConnections;
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
    public static IReadOnlyList<Connection> GetConnectionsByPart(XRInspectablePart part) => _connectionsByPart.TryGetValue(part, out var cd) ? cd : new Connection[0];
    /// <summary>Get all parts which exist within the given <paramref name="connection"/></summary>
    public static IReadOnlyList<XRInspectablePart> GetConnectedParts(Connection connection) => _partsByConnection.TryGetValue(connection, out var parts) ? parts : new XRInspectablePart[0];
    /// <summary>Get all <see cref="Connection"/>s descended from the object containing the <see cref="PartCatalog"/> component</summary>
    public static IReadOnlyList<Connection> GetAllConnections() => _allConnections;
    /// <summary>Get all <see cref="XRInspectablePart"/>s descended from the object containing the <see cref="PartCatalog"/> component</summary>
    public static IReadOnlyList<XRInspectablePart> GetAllParts() => _allParts;

    private void _buildPartLookupTables()
    {

        // Clear Any Existing Lookup Tables
        _partsByPartID.Clear();
        _partsByUniqueID.Clear();
        _uniqueIdsByPart.Clear();

        // Temporary caches to build the lookup tables before converting lists to arrays (reduces final memory usage)
        Dictionary<PartInfo.ID, List<XRInspectablePart>> partsByPartIDCache = new Dictionary<PartInfo.ID, List<XRInspectablePart>>();
        Dictionary<XRInspectablePart, List<Connection>> connectionsByPartCache = new Dictionary<XRInspectablePart, List<Connection>>();
        Dictionary<Connection, List<XRInspectablePart>> partsByConnectionCache = new Dictionary<Connection, List<XRInspectablePart>>();

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

        // Convert caches to arrays and store in the final lookup tables
        foreach (var kvp in partsByPartIDCache)
        {
            _partsByPartID.Add(kvp.Key, kvp.Value.ToArray());
        }
    }

    private void _connectionSetup()
    {
        _connectionsByPart.Clear();

        // Register Connection archetype local roots, and get their group hover outlines to enthrall them as I go.
        XRHoverOutline connDowelOutline = Connection.ArchetypeCode.CONN_DOWEL.InitializeSceneRoot(gameObject.GetNamedChild("PinnedBridalJoints"));
        XRHoverOutline connSlotOutline = Connection.ArchetypeCode.CONN_SLOT.InitializeSceneRoot(gameObject.GetNamedChild("NotchedPurlinJoints"));
        XRHoverOutline connTpinOutline = Connection.ArchetypeCode.CONN_TPIN.InitializeSceneRoot(gameObject.GetNamedChild("KeyedTenonMortiseJoints"));

        var vCCount = Connection.VariantCodeCount;

        Connection[] connections = GetComponentsInChildren<Connection>();
        Queue<Connection>[] queue = new Queue<Connection>[vCCount];

        byte[] uIDSuffixAccumulator = new byte[vCCount];

        for (int i = 0; i < vCCount; i++)
        {
            // Set to 1 or 'A'
            if (Connection.ID.UsesUniqueIDAlphabeticSuffix.Contains(i.IdxToVariantCode()))
                uIDSuffixAccumulator[i] = (byte)'A';
            else
                uIDSuffixAccumulator[i] = 1;
        }

        foreach (var connection in connections)
        {
            connection.SetUniqueIDSuffix((uIDSuffixAccumulator[connection.UniqueID.VariantPrefix.ToIndex()]++));
            List<XRInspectablePart> connectionParts = new List<XRInspectablePart>();

            var frameOrigin = transform.position;
        }

    }

    void Awake()
    {
        _allConnections = GetComponentsInChildren<Connection>(true); // Load all Connection into the PartCatalog
        _allParts = GetComponentsInChildren<XRInspectablePart>(true); // Load all XRInspectablePart components

        // ScriptableObject? _allConnections = Resources.LoadAll<Connection>(""); // Load all Connection into the PartCatalog
        // Debug.Log($"Loaded {_allConnections.Length} parts into the catalog.");

        _buildPartLookupTables();
        _connectionSetup();
    }
}

public static class PartCatalogExtensions
{
    // Extend XRInspectablePart
    public static IReadOnlyList<XRInspectablePart> GetAllMatchingParts(this XRInspectablePart inspectablePartComponent) => PartCatalog.GetAllMatchingParts(inspectablePartComponent);
    public static IReadOnlyList<Connection> GetConnectionsByPart(this XRInspectablePart inspectablePartComponent) => PartCatalog.GetConnectionsByPart(inspectablePartComponent);

    // Extend PartInfo.ID
    public static IReadOnlyList<XRInspectablePart> GetAllMatchingParts(this PartInfo.ID pID) => PartCatalog.GetAllMatchingParts(pID);
}
