using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;

[TestFixture]
public class PartInfoEditorTests
{
    // Get the Collections to be tested from PartInfo
    private static IEnumerable<string> TargetTables
    {
        get
        {
            yield return PartInfo.NamesTable;
            yield return PartInfo.DescriptionsTable;
        }
    }

    [Test]
    public void KeyMap_MatchesIDEnumLength()
    {
        int enumCount = System.Enum.GetValues(typeof(PartInfo.ID)).Length;
        Assert.IsNotNull(PartInfo.KeyMap, "PartInfo.KeyMap is null.");
        Assert.IsNotEmpty(PartInfo.KeyMap, "PartInfo.KeyMap array is empty."); // This will catch if PartInfo.ID is emptied somehow
        Assert.AreEqual(enumCount, PartInfo.KeyMap.Count(), message: "PartInfo.KeyMap count does not match IDEnum value count.");
    }
    [Test]
    public void LocalizationTables_ContainAllKeyMapEntries([ValueSource(nameof(TargetTables))] string tableName)
    {
        StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(tableName);
        Assert.IsNotNull(collection, $"StringTableCollection '{tableName}' could not be found in LocalizationEditorSettings.");

        SharedTableData sharedData = collection.SharedData;

        // If KeyMap is a string[] or IReadOnlyList<string>, iterate directly. 
        // If KeyMap is a Dictionary<PartInfo.ID, string>, use PartInfo.KeyMap.Values.
        IEnumerable<string> requiredKeys = PartInfo.KeyMap;

        List<string> missingFromSchema = requiredKeys
            .Where(key => !sharedData.Contains(key))
            .ToList();

        Assert.IsEmpty(missingFromSchema,
            $"Table '{tableName}' SharedData is missing keys: {string.Join(", ", missingFromSchema)}");

        foreach (StringTable localeTable in collection.StringTables)
        {
            List<string> missingInLocale = requiredKeys
                .Where(key =>
                {
                    StringTableEntry entry = localeTable.GetEntry(key);
                    return entry == null || string.IsNullOrWhiteSpace(entry.GetLocalizedString());
                })
                .ToList();

            Assert.IsEmpty(missingInLocale,
                $"Table '{tableName}' ({localeTable.LocaleIdentifier}) is missing or has empty strings for keys: {string.Join(", ", missingInLocale)}");
        }
    }
}
