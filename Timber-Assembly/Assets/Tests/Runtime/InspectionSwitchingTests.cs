#if UNITY_INCLUDE_TESTS
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

[InputControlLayout(displayName = "Inspection test controller")]
public class InspectionTestController : XRController
{
    [InputControl(usage = "GripButton")]
    public ButtonControl gripPressed { get; private set; }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        gripPressed = GetChildControl<ButtonControl>("gripPressed");
    }
}

public class InspectionSwitchingTests
{
    [Test]
    public void SelectingAnotherPartWhileGripIsHeldRestoresThePreviousPart()
    {
        Assert.IsTrue(QuestInspectionInput.IsAvailable);
        QuestInspectionInput.SetHandedness(false);
        InputSystem.RegisterLayout<InspectionTestController>();
        InspectionTestController controller = InputSystem.AddDevice<InspectionTestController>();
        InputSystem.SetDeviceUsage(controller, "RightHand");

        GameObject cameraObject = new GameObject("Inspection Test Camera", typeof(Camera));
        cameraObject.tag = "MainCamera";
        GameObject leftController = new GameObject("Left Controller");
        GameObject popupObject = new GameObject("Inspection Test Popup");
        popupObject.SetActive(false);
        PopupUIManager popup = popupObject.AddComponent<PopupUIManager>();
        GameObject panel = new GameObject("Panel");
        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        GameObject buttonObject = new GameObject("Button", typeof(RectTransform), typeof(Button));
        SetField(popup, "popupPanel", panel);
        SetField(popup, "popupTextHeader", textObject.GetComponent<TextMeshProUGUI>());
        SetField(popup, "popupTextBody", textObject.GetComponent<TextMeshProUGUI>());
        SetField(popup, "popupTextIdentifier", textObject.GetComponent<TextMeshProUGUI>());
        SetField(popup, "closeButton", buttonObject.GetComponent<Button>());

        GameObject first = GameObject.CreatePrimitive(PrimitiveType.Cube);
        first.transform.position = new Vector3(3f, 0f, 0f);
        Vector3 firstPosition = first.transform.position;
        InspectionViewController firstView = first.AddComponent<InspectionViewController>();
        XRInspectablePart firstPart = first.AddComponent<XRInspectablePart>();
        GameObject second = GameObject.CreatePrimitive(PrimitiveType.Cube);
        second.transform.position = new Vector3(-3f, 0f, 0f);
        InspectionViewController secondView = second.AddComponent<InspectionViewController>();
        XRInspectablePart secondPart = second.AddComponent<XRInspectablePart>();

        try
        {
            popupObject.SetActive(true);
            InputSystem.QueueDeltaStateEvent(controller.gripPressed, 1f);
            InputSystem.Update();
            Assert.IsTrue(QuestInspectionInput.Grip.IsPressed(), "The dominant grip must be held during this test.");

            firstPart.OnSelect(null);
            Assert.IsTrue(popup.IsInspecting(firstPart));
            firstView.ToggleEnlargement();
            first.transform.rotation = Quaternion.Euler(0f, 45f, 0f);

            secondPart.OnSelect(null);
            Assert.IsTrue(popup.IsInspecting(secondPart));
            Assert.IsFalse(popup.IsInspecting(firstPart));
            Assert.AreEqual(firstPosition, first.transform.position);
            Assert.Less(Quaternion.Angle(Quaternion.identity, first.transform.rotation), 0.01f);
            Assert.IsFalse(firstView.IsEnlarged());

            secondView.ToggleEnlargement();
            secondPart.OnActivate(null);
            Assert.IsTrue(popup.IsInspecting(secondPart));
            Assert.IsTrue(secondView.IsEnlarged(), "Re-selecting the active part must not reset it.");

            popup.ClosePopup();
            Assert.IsFalse(popup.HasActiveInspection);
        }
        finally
        {
            Object.Destroy(popupObject);
            Object.Destroy(panel);
            Object.Destroy(textObject);
            Object.Destroy(buttonObject);
            Object.Destroy(first);
            Object.Destroy(second);
            Object.Destroy(cameraObject);
            Object.Destroy(leftController);
            InputSystem.RemoveDevice(controller);
            InputSystem.RemoveLayout(nameof(InspectionTestController));
        }
    }

    private static void SetField(object target, string name, object value)
    {
        FieldInfo field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, name);
        field.SetValue(target, value);
    }
}
#endif
