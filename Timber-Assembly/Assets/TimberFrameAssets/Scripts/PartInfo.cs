using System;
using System.Linq;
using UnityEngine.Localization;

/// <summary>
///     Handles Part IDs via <see cref="ID"/>, fetching Part Names, and fetching Part Descriptions.
/// </summary>
/// <remarks>
///     Primary Usages:<para/>
///     <c><see langword="string"/> <see cref="PartInfo.IdToString"/></c><para/>
///     <c><see cref="LocalizedString"/> <see cref="PartInfo.GetName"/></c><para/>
///     <c><see cref="LocalizedString"/> <see cref="PartInfo.GetDescription"/></c>
/// </remarks>
public static class PartInfo
{
    /// <summary>
    ///     Represents the identifier for each part type.
    /// </summary>
    /// <remarks>
    ///     Do not change these without updating the keys in "PartNames" and "PartDescriptions" Localization Tables!<para/>
    ///     Everything other than None = 0 should be allowed to auto-assign values.
    /// </remarks>
    public enum ID
    {
        None = 0,
        EC1,
        EC2,
        MC,
        RB,
        RP,
        WP,
        TP,
        SAM,
        TAM,
        Dowel
        // MPP variants, 4x6, 4x4, fiberglass roof, and U-Channel brackets not added
        // TODO: Verify if they should be added to the Part ID system
    }

    // Define the ranges for numeric suffixes [1, 64] and Alphabetic Suffixes ['A', 'Z'] = [65, 90]
    // Numeric suffixes are to be considered the default, using Alphabetic suffixes for parts whose IDs end in a digit
    public const byte UniqueIDAlphabeticSuffixMin = (byte)'A';
    public const byte UniqueIDAlphabeticSuffixMax = (byte)'Z';
    public const byte UniqueIDNumericSuffixMin = 1;
    public const byte UniqueIDNumericSuffixMax = (byte)'A' - 1;

    /// <summary>
    ///     Array of all <see cref="PartInfo"/>.<see cref="ID"/> values that use an alphabetic suffix for their unique ID.
    ///     <para/><para/>
    ///     All other <see cref="PartInfo"/>.<see cref="ID"/> values use a numeric suffix for their unique ID.
    /// </summary>
    private static readonly ID[] UsesUniqueIDAlphabeticSuffix = { ID.EC1, ID.EC2 };
    /// <summary>
    ///     Cached array of string representations of the corresponding <see cref="PartInfo"/>.<see cref="ID"/> values.
    ///     <para/>
    ///     These values are used as keys when accessing the "PartNames" and "PartDescriptions" Localization Tables.
    /// </summary>
    /// <remarks>
    ///     <c><see cref="PartInfo.IdToString"/></c> is equivalent to <c><see cref="PartInfo"/>.KeyMap[(<see langword="int"/>)<see cref="ID"/>]</c>.
    ///     <para/> <see cref="Array.Length"/> properly reflects length of <see cref="ID"/>.
    /// </remarks>
    public static readonly string[] KeyMap = Enum.GetNames(typeof(ID)); // The use of these keys in the Localization Tables is validated in Tests/Editor/PartInfoEditorTests.cs

    // This all relies on using the keys from KeyMap as the keys within the following Localization Tables:

    /// <summary>
    ///     Used as the first argument of <c><see cref="LocalizedString.LocalizedString"/></c> to access the relevant Localization Table.
    ///     <para/>
    ///     Counts as an instance of <see cref="UnityEngine.Localization.Tables.TableReference"/> for this purpose.
    /// </summary>
    public const string NamesTable = "PartNames"; // The existence and contents of this table is validated in Tests/Editor/PartInfoEditorTests.cs

    /// <summary>
    ///     Used as the first argument of <c><see cref="LocalizedString.LocalizedString"/></c> to access the relevant Localization Table.
    ///     <para/>
    ///     Counts as an instance of <see cref="UnityEngine.Localization.Tables.TableReference"/> for this purpose.
    /// </summary>
    public const string DescriptionsTable = "PartDescriptions"; // The existence and contents of this table is validated in Tests/Editor/PartInfoEditorTests.cs

    /// <summary>
    ///     Gets the minimum valid unique ID suffix for the specified <paramref name="pID"/>.
    /// </summary>
    /// <param name="pID"></param>
    /// <returns></returns>
    public static byte MinUniqueID(ID pID) => UsesUniqueIDAlphabeticSuffix.Contains(pID)
                                            ? UniqueIDAlphabeticSuffixMin
                                            : UniqueIDNumericSuffixMin;
    /// <summary>
    ///     Gets the maximum valid unique ID suffix for the specified <paramref name="pID"/>.
    /// </summary>
    /// <param name="pID"></param>
    /// <returns></returns>
    public static byte MaxUniqueID(ID pID) => UsesUniqueIDAlphabeticSuffix.Contains(pID)
                                            ? UniqueIDAlphabeticSuffixMax
                                            : UniqueIDNumericSuffixMax;
    /// <summary>
    ///     Verifies if the given <paramref name="value"/> is a valid unique ID suffix for the specified <paramref name="pID"/>.
    /// </summary>
    /// <param name="pID"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsValidUniqueID(ID pID, byte value) => UsesUniqueIDAlphabeticSuffix.Contains(pID)
                                                            ? (UniqueIDAlphabeticSuffixMin <= value && value <= UniqueIDAlphabeticSuffixMax)
                                                            : (UniqueIDNumericSuffixMin <= value && value <= UniqueIDNumericSuffixMax);


    /// <summary>
    ///     Returns the string representation of the given <see cref="PartInfo"/>.<see cref="ID"/>
    /// </summary>
    /// <param name="partID">
    ///     the <see cref="PartInfo"/>.<see cref="ID"/> enum value of the relevant part.
    /// </param>
    /// <returns>
    ///     <see langword="string"/> partID
    /// </returns>
    public static string IdToString(ID partID) => KeyMap[(int)partID];

    /// <summary>
    ///     Fetch the <see cref="LocalizedString"/> for the name of the part corresponding to the given <see cref="PartInfo"/>.<see cref="ID"/>
    /// </summary>
    /// <param name="partID">
    ///     the <see cref="PartInfo"/>.<see cref="ID"/> enum value corresponding to the relevant part.
    /// </param>
    /// <returns>
    ///     <see cref="LocalizedString"/> part name
    /// </returns>
    public static LocalizedString GetName(ID partID) => new LocalizedString(NamesTable, KeyMap[(int)partID]);


    /// <summary>
    ///     Fetch the <see cref="LocalizedString"/> for the name of the part whose <see cref="PartInfo"/>.<see cref="ID"/>
    ///     corresponds to the given <paramref langword="int"/> value.
    /// </summary>
    /// <param name="partID">
    ///     An integer value equivalent to a valid <see cref="PartInfo"/>.<see cref="ID"/> enum value.<para/>
    ///     Return value will be <see langword="null"/> if the value does not correspond to a valid enum member.
    /// </param>
    /// <returns>
    ///     <see cref="LocalizedString"/> part name | <see langword="null"/><para/>
    ///     <see langword="null"/> if <paramref name="partID"/> does not correspond to a valid enum member.
    /// </returns>
    public static LocalizedString GetName(int partID) => partID < 0 || KeyMap.Length <= partID
                                                       ? null
                                                       : new LocalizedString(NamesTable, KeyMap[partID]);

    /// <summary>
    ///     Fetch the <see cref="LocalizedString"/> for the description of the part corresponding to the given <see cref="PartInfo"/>.<see cref="ID"/>
    /// </summary>
    /// <param name="partID">
    ///     the <see cref="PartInfo"/>.<see cref="ID"/> enum value corresponding to the relevant part.
    /// </param>
    /// <returns>
    ///     <see cref="LocalizedString"/> part description
    /// </returns>
    public static LocalizedString GetDescription(ID partID) => new LocalizedString(DescriptionsTable, KeyMap[(int)partID]);

    /// <summary>
    ///     Fetch the <see cref="LocalizedString"/> for the description of the part whose <see cref="PartInfo"/>.<see cref="ID"/>
    ///     corresponds to the given <paramref langword="int"/> value.
    /// </summary>
    /// <param name="partID">
    ///     An integer value equivalent to a valid <see cref="PartInfo"/>.<see cref="ID"/> enum value.<para/>
    ///     Return value will be <see langword="null"/> if the value does not correspond to a valid enum member.
    /// </param>
    /// <returns>
    ///     <see cref="LocalizedString"/> part description | <see langword="null"/><para/>
    ///     <see langword="null"/> if <paramref name="partID"/> does not correspond to a valid enum member.
    /// </returns>
    public static LocalizedString GetDescription(int partID) => partID < 0 || KeyMap.Length <= partID
                                                              ? null
                                                              : new LocalizedString(DescriptionsTable, KeyMap[partID]);
}

/********************
 * Class Extensions *
 ********************/

/// <summary>
///     Contains extension methods for the <see cref="PartInfo.ID"/> enum, providing convenient access to related functionality.
///     <para/>
///     Usage example:<para/>
///     <code>
///         var id = PartInfo.ID.SomePart;
///         var stringifiedId = id.ToString();
///     </code>
/// </summary>
/// <example>
/// </example>
public static class PartInfoIDExtensions
{
    /// <summary>
    ///     Returns the minimum valid unique ID suffix byte-value for the calling <see cref="PartInfo.ID"/>.
    /// </summary>
    public static byte MinUniqueID(this PartInfo.ID pID) => PartInfo.MinUniqueID(pID);
    /// <summary>
    ///     Returns the maximum valid unique ID suffix byte-value for the calling <see cref="PartInfo.ID"/>.
    /// </summary>
    public static byte MaxUniqueID(this PartInfo.ID pID) => PartInfo.MaxUniqueID(pID);
    /// <summary>
    ///     Returns whether the given <paramref name="value"/> is a valid unique ID for the calling <see cref="PartInfo.ID"/>.
    /// </summary>
    public static bool IsValidUID(this PartInfo.ID pID, byte value) => PartInfo.IsValidUniqueID(pID, value);
    /// <summary>
    ///     Returns the stringified version of the calling <see cref="PartInfo.ID"/>.
    /// </summary>
    public static string ToString(this PartInfo.ID pID) => PartInfo.IdToString(pID);
    /// <summary>
    ///     Functionally cast the pID enum to an integer
    /// </summary>
    /// <param name="pID"></param>
    /// <returns></returns>
    public static int ToInt(this PartInfo.ID pID) => (int)pID;
    /// <summary>
    ///     Converts the given unique ID suffix byte-value to its string representation, if valid.<para/>
    ///     Returns <see langword="null"/> if the value is invalid.
    /// </summary>
    /// <param name="uID"></param>
    /// <returns><see langword="string"/> ['1','64']U['A','Z'] or <see langword="null"/></returns>
    public static string ToUniqueIDSuffix(this byte uID) => uID <= PartInfo.UniqueIDAlphabeticSuffixMax
                                                                 ? uID <= PartInfo.UniqueIDNumericSuffixMax
                                                                        ? uID < PartInfo.UniqueIDNumericSuffixMin
                                                                              ? null // < min
                                                                              : uID.ToString() // Numeric
                                                                        : char.ToString((char)uID) // Alphabetic
                                                                 : null; // > max
}
