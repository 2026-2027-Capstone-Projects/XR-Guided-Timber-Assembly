using System;
using UnityEngine.Localization;

// Handles Part ID's, fetching Part Names, and fetching Part Descriptions.
public static class PartInfo
{
    // Do not change these without updating the keys in "PartNames" and "PartDescriptions" Localization Tables!
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
        TAM
        // MPP variants, 4x6, 4x4, fiberglass roof, Dowel, and U-Channel brackets not added
        // TODO: Verify if they should be added to the Part ID system
    }

    private static readonly string[] KeyMap = Enum.GetNames(typeof(ID));

    // This all relies on using the keys from KeyMap as the keys within the following Localization Tables:

    private const string NamesTable = "PartNames"; // Relies on "PartNames" Localization Table existing
    private const string DescriptionsTable = "PartDescriptions"; // Relies on "PartDescriptions" Localization Table existing


    public static string ToString(ID id)
    {
        return KeyMap[(int)id];
    }

    public static LocalizedString GetName(ID id)
    {
        return new LocalizedString(NamesTable, KeyMap[(int)id]);
    }
    public static LocalizedString GetName(int id)
    {
        if (id < 0 || id >= KeyMap.Length)
            return null;
        return new LocalizedString(NamesTable, KeyMap[(int)id]);
    }
    public static LocalizedString GetDescription(ID id)
    {
        return new LocalizedString(DescriptionsTable, KeyMap[(int)id]);
    }
    public static LocalizedString GetDescription(int id)
    {
        if (id < 0 || id >= KeyMap.Length)
            return null;
        return new LocalizedString(DescriptionsTable, KeyMap[(int)id]);
    }
}
