using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.Localization;

public class QuestInspectionControlsTests
{
    private Type inputType;
    private InputActionAsset actions;

    [SetUp]
    public void SetUp()
    {
        inputType = typeof(InspectionViewController).Assembly.GetType("QuestInspectionInput");
        Assert.IsNotNull(inputType);
        inputType.GetMethod("ResetSession", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        inputType.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        actions = Resources.Load<InputActionAsset>("QuestInspectionControls");
        Assert.IsNotNull(actions);
    }

    [TearDown]
    public void TearDown()
    {
        if (actions != null)
            actions.Disable();
    }

    [TestCase(true, "LeftHand", "RightHand")]
    [TestCase(false, "RightHand", "LeftHand")]
    public void ChoosingHand_MapsInspectionToDominantAndCloseToOffHand(
        bool leftHanded, string dominant, string offHand)
    {
        inputType.GetMethod("SetHandedness", BindingFlags.NonPublic | BindingFlags.Static)
            .Invoke(null, new object[] { leftHanded });

        Assert.IsTrue((bool)inputType.GetProperty("HasHandedness", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null));
        Assert.AreEqual(leftHanded, inputType.GetProperty("IsLeftHanded", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null));

        AssertBinding("Enlarge", dominant, "PrimaryButton");
        AssertBinding("Reset", dominant, "SecondaryButton");
        AssertBinding("Grip", dominant, "GripButton");
        AssertBinding("Position", dominant, "devicePosition");
        AssertBinding("Tracked", dominant, "isTracked");
        AssertBinding("Close", offHand, "SecondaryButton");
    }

    [Test]
    public void NewSession_ClearsHandedness()
    {
        inputType.GetMethod("SetHandedness", BindingFlags.NonPublic | BindingFlags.Static)
            .Invoke(null, new object[] { true });
        inputType.GetMethod("ResetSession", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        Assert.IsFalse((bool)inputType.GetProperty("HasHandedness", BindingFlags.NonPublic | BindingFlags.Static)
            .GetValue(null));
    }

    [Test]
    public void SelectionActionsAndPrompt_AreAvailable()
    {
        Assert.AreEqual("<XRController>{LeftHand}/{TriggerButton}",
            actions.FindAction("Inspection/ChooseLeft", true).bindings.Single().path);
        Assert.AreEqual("<XRController>{RightHand}/{TriggerButton}",
            actions.FindAction("Inspection/ChooseRight", true).bindings.Single().path);
        var collection = LocalizationEditorSettings.GetStringTableCollection(PartInfo.NamesTable);
        Assert.IsNotNull(collection);
        foreach (var table in collection.StringTables)
        {
            var entry = table.GetEntry("HandednessPrompt");
            Assert.IsNotNull(entry);
            Assert.IsNotEmpty(entry.GetLocalizedString());
        }
    }

    [TestCase(true, false, false, true, true, true)]
    [TestCase(false, true, true, false, true, false)]
    [TestCase(false, false, true, false, true, true)]
    [TestCase(false, false, false, true, true, false)]
    [TestCase(false, false, true, true, false, false)]
    [TestCase(true, true, true, false, false, false)]
    [TestCase(false, false, false, false, false, false)]
    public void KeyboardChoice_TakesPriorityOverSimulatedControllers(bool keyboardLeft, bool keyboardRight,
        bool controllerLeft, bool controllerRight, bool expectedChoice, bool expectedLeft)
    {
        var method = typeof(PopupUIManager).GetMethod("TryChooseHand", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        object[] arguments = { keyboardLeft, keyboardRight, controllerLeft, controllerRight, false };
        Assert.AreEqual(expectedChoice, method.Invoke(null, arguments));
        Assert.AreEqual(expectedLeft, arguments[4]);
    }

    private void AssertBinding(string actionName, string hand, string control)
    {
        InputAction action = actions.FindAction("Inspection/" + actionName, true);
        Assert.IsTrue(action.bindingMask.HasValue, actionName);
        InputBinding mask = action.bindingMask.Value;
        var bindings = action.bindings.Where(binding => mask.Matches(binding)).ToArray();
        Assert.AreEqual(1, bindings.Length, actionName);
        StringAssert.Contains("{" + hand + "}", bindings[0].path, actionName);
        StringAssert.Contains(control, bindings[0].path, actionName);
    }
}
