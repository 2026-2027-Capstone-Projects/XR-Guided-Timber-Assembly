#if UNITY_INCLUDE_TESTS // Guard against the runtime tests being added to the player build; Add to each file or make custom .asmdef
using System;
using NUnit.Framework;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[TestFixture]
public class PartInfoRuntimeTests
{
    [OneTimeSetUp]
    public void SetUp()
    {
        LocalizationSettings.InitializationOperation.WaitForCompletion();
        LocalizationSettings.StringDatabase.GetTableAsync(PartInfo.NamesTable).WaitForCompletion();
        LocalizationSettings.StringDatabase.GetTableAsync(PartInfo.DescriptionsTable).WaitForCompletion();
    }

    private static Array AllPartIDs => Enum.GetValues(typeof(PartInfo.ID));

    [Test]
    public void GetName_ReturnsValidNonEmptyString_ForAllIDs([ValueSource(nameof(AllPartIDs))] PartInfo.ID id)
    {
        LocalizedString result = PartInfo.GetName(id);
        Assert.IsNotNull(result, $"GetName returned null for {id}.");

        string stringified = result.ToString();
        Assert.IsNotEmpty(stringified.Trim(), $"GetName returned an empty or whitespace string for {id}.");
        Assert.IsFalse(stringified.StartsWith("No translation found"), $"GetName returned fallback missing-translation text for {id}: {result}");
    }

    [Test]
    public void GetDescription_ReturnsValidNonEmptyString_ForAllIDs([ValueSource(nameof(AllPartIDs))] PartInfo.ID id)
    {
        LocalizedString result = PartInfo.GetDescription(id);
        Assert.IsNotNull(result, $"GetDescription returned null for {id}.");

        string stringified = result.ToString();
        Assert.IsNotEmpty(stringified.Trim(), $"GetDescription returned an empty or whitespace string for {id}.");
        Assert.IsFalse(stringified.StartsWith("No translation found"), $"GetDescription returned fallback missing-translation text for {id}: {result}");
    }
}
#endif