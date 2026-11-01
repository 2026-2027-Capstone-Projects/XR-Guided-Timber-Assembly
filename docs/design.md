# Design

## General Project Description and Objectives
Timber construction often involves assembling structural components according to precise spatial relationships and construction sequences. Understanding drawings alone may not be sufficient for inexperienced users to immediately understand where components belong, how they should be oriented, and in what order they should be assembled.

This project will develop an XR training system that guides users through the assembly of a representative timber structure. The system will provide interactive visualization and spatial guidance to help users identify components, understand their placement, and complete the assembly sequence correctly.

The project builds on ongoing research in augmented and extended reality for timber construction and will focus on creating a robust software platform for guided assembly training.

### Objectives

The final system should include:

    An XR application supporting a representative timber assembly task.
    Digital models of the required structural components.
    A clearly defined assembly sequence.
    Visualization of the current assembly step.
    Identification of the component required for each step.
    Spatial guidance showing where components should be placed.
    Orientation and alignment guidance.
    Detection of correct and incorrect assembly actions.
    User feedback when errors occur.
    Progress tracking.
    Basic training metrics such as time per step, errors, and completed steps.
    Support for restarting or replaying training procedures.
    An architecture that allows additional assembly scenarios to be configured.
    User and developer documentation.

## Assets and Other Provided Material
This project will be based around a pre-exsting timber design. We have been provided three different [documents](./structureDocs) that contain instructions for construction, deconstruction, and a guide to the parts, and design intentions of the structure. Additionally, a 3d model asset containing the complete structure, comprised of the necessary individual components has also been provided and can be found in the [assets]() folder

## Technologies and Languages Utilized
This project will be developed and designed utilizing the Unity Game Engine and C# language. A database has not yet been decided, however it is likely the existing Unity database system will be used. 
