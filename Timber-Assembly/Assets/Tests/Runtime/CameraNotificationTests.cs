#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[TestFixture]
public class CameraNotificationTests
{
    [Test]
    public void CameraNotification_IsAComponent()
    {
        Assert.IsTrue(typeof(MonoBehaviour).IsAssignableFrom(typeof(CameraNotification)));
    }

    [TestCase("ObjectEnlarged", "Object enlarged")]
    [TestCase("ObjectNormalSize", "Object returned to normal size")]
    public void InspectionMessage_HasEnglishTranslation(string key, string expected)
    {
        LocalizationSettings.InitializationOperation.WaitForCompletion();
        var locale = LocalizationSettings.AvailableLocales.GetLocale("en");
        Assert.IsNotNull(locale, "English locale could not be found.");
        var table = LocalizationSettings.StringDatabase.GetTableAsync("InspectionMessages", locale).WaitForCompletion();
        Assert.IsNotNull(table, "InspectionMessages English table could not be loaded.");
        var entry = table.GetEntry(key);
        Assert.IsNotNull(entry, $"InspectionMessages is missing '{key}'.");
        Assert.AreEqual(expected, entry.GetLocalizedString());
    }

    [Test]
    public void Reset_ShowsNormalMessageOnlyWhenEnlargementWasCleared()
    {
        if (!Application.isPlaying || Camera.main != null)
            Assert.Ignore("Requires PlayMode and a scene without an existing MainCamera.");

        LocalizationSettings.InitializationOperation.WaitForCompletion();
        new LocalizedString("InspectionMessages", "ObjectEnlarged").GetLocalizedStringAsync().WaitForCompletion();
        new LocalizedString("InspectionMessages", "ObjectNormalSize").GetLocalizedStringAsync().WaitForCompletion();

        var cameraObject = new GameObject("Test Camera", typeof(Camera));
        cameraObject.tag = "MainCamera";
        var part = new GameObject("Test Part", typeof(InspectionViewController));
        try
        {
            var inspection = part.GetComponent<InspectionViewController>();
            int changeCount = 0;
            inspection.EnlargementChanged += () => changeCount++;
            inspection.ResetInspectionView();
            Assert.IsNull(cameraObject.GetComponent<CameraNotification>());

            inspection.ToggleEnlargement();
            var notification = cameraObject.GetComponent<CameraNotification>();
            Assert.IsNotNull(notification);
            var text = notification.GetComponentInChildren<TMP_Text>();
            Assert.IsNotNull(text);
            Assert.AreEqual("Object enlarged", text.text);

            inspection.ResetInspectionView();
            Assert.IsFalse(inspection.IsEnlarged());
            Assert.AreEqual("Object returned to normal size", text.text);

            notification.Show("Unchanged");
            inspection.ResetInspectionView();
            Assert.AreEqual("Unchanged", text.text);

            inspection.ToggleEnlargement();
            inspection.ResetInspectionViewOnExit();
            Assert.IsFalse(inspection.IsEnlarged());
            Assert.AreEqual("Object enlarged", text.text);
            Assert.AreEqual(6, changeCount);
        }
        finally
        {
            Object.Destroy(part);
            Object.Destroy(cameraObject);
        }
    }
}
#endif
