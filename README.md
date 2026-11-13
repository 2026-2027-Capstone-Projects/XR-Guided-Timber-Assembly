# XR-Guided-Timber-Assembly

## General Description
Timber construction often involves assembling structural components according to precise spatial relationships and construction sequences. Understanding drawings alone may not be sufficient for inexperienced users to immediately understand where components belong, how they should be oriented, and in what order they should be assembled.

This project will develop an XR training system that guides users through the assembly of a representative timber structure. The system will provide interactive visualization and spatial guidance to help users identify components, understand their placement, and complete the assembly sequence correctly.

The project builds on ongoing research in augmented and extended reality for timber construction and will focus on creating a robust software platform for guided assembly training.

## Requirements
Unity Editor Version: 6000.3.26f1

## Instructions to Run
In its current state, the project requires the Unity editor to view and run the program. To open and run the program in Unity, navigate to Projects, add the project by selecting "Add", and then "Add from repo". After the project is added, open the project in the Unity editor. In the project window, navigate to Assets/Scenes and select "MC1Scene" (make sure to select the scene file, not the folder of the same name)

After opening the file, click the Play button at the top of the editor to test the program. 

### Inspection controls

At the start of each session, choose your dominant hand by pressing the **lower
face button** on that controller. In the editor, click the Game view and press
**L** or **R** instead. The choice lasts for the session; it is not saved to a
profile.

Select a part to inspect it. On the dominant controller, press the **lower face
button** to toggle enlargement and the **upper face button** to reset rotation
and scale. Hold the dominant grip and move the controller left, right, up, or
down to rotate the part; release to stop. Press the **upper face button** on
the off hand to exit inspection. The information popup follows the off hand.
Point at and select another part to switch directly to it; the previous part
returns to its original position. On Quest Touch controllers the lower/upper
buttons are A/B on the right and X/Y on the left.

Desktop controls (mouse drag, 3 to enlarge, R to reset, and Escape to close)
remain available after choosing a hand. Quest bindings can be edited in
`Assets/TimberFrameAssets/Resources/QuestInspectionControls.inputactions`.
