# Requirements

## Problem Statement

Timber construction often involves assembling structural components according to precise spatial relationships and construction sequences. Understanding drawings alone may not be sufficient for inexperienced users to immediately understand where components belong, how they should be oriented, and in what order they should be assembled.

This project will develop an XR training system that guides users through the assembly of a representative timber structure. The system will provide interactive visualization and spatial guidance to help users identify components, understand their placement, and complete the assembly sequence correctly.

The project builds on ongoing research in augmented and extended reality for timber construction and will focus on creating a robust software platform for guided assembly training.

---

## Objectives

The final system should include:

- An XR application supporting a representative timber assembly task.
- Digital models of the required structural components.
- A clearly defined assembly sequence.
- Visualization of the current assembly step.
- Identification of the component required for each step.
- Spatial guidance showing where components should be placed.
- Orientation and alignment guidance.
- Detection of correct and incorrect assembly actions.
- User feedback when errors occur.
- Progress tracking.
- Basic training metrics such as:
  - Time per step
  - Errors per step
  - Completed steps
- Support for restarting or replaying training procedures.
- An architecture that allows additional assembly scenarios to be configured.
- User documentation.
- Developer documentation.

---

# Functional Requirements

## FR-1 Component Visualization

The system shall display timber components within an XR environment.

### Acceptance Criteria

- Components appear in their correct relative positions.
- Components remain individually selectable.
- Components retain unique identifiers.

---

## FR-2 Component Identification

The system shall allow learners to select a component and view information about it.

### Acceptance Criteria

- Selecting a component displays:
  - Identifier
  - Name
  - Description
- Deselecting clears displayed information.
- Switching selection updates displayed information.

---

## FR-3 Component Inspection

The system shall provide an inspection mode for individual components.

### Acceptance Criteria

- Components can be rotated.
- Components can be enlarged.
- Components can be reset.
- Returning preserves selection state.

---

## FR-4 Connection Exploration

The system shall allow learners to inspect timber connections.

### Acceptance Criteria

- Participating components are highlighted.
- Relevant interfaces are identified.
- Both assembled and exploded views are available.
- Original positions can be restored.

---

## FR-5 Component Catalog

The system shall provide a catalog of available timber component types.

### Acceptance Criteria

- Component types are listed.
- Components include previews.
- Catalog entries locate corresponding structure instances.

---

## FR-6 Guided Learning Activities

The system shall guide learners through structured educational activities.

### Acceptance Criteria

- Next, Previous, Restart, and Exit actions function correctly.
- Progress is displayed.
- Activity content can be completed without developer assistance.

---

## FR-7 Knowledge Assessment

The system shall assess learner understanding through identification activities.

### Acceptance Criteria

- Component identification tasks are evaluated.
- Connection identification tasks are evaluated.
- Correct and incorrect answers produce feedback.
- Learners may retry incorrect answers.

---

## FR-8 Assembly Guidance

The system shall provide step-by-step assembly guidance.

### Acceptance Criteria

- Current assembly step is displayed.
- Required component is identified.
- Placement guidance is provided.
- Orientation guidance is provided.

---

## FR-9 Assembly Validation

The system shall detect correct and incorrect assembly actions.

### Acceptance Criteria

- Correct actions are recognized.
- Incorrect actions produce feedback.
- Users can attempt corrections.

---

## FR-10 Progress Tracking

The system shall track learner progress.

### Acceptance Criteria

- Completed steps are tracked.
- Progress indicators are displayed.
- Training metrics are recorded.

---

## FR-11 Training Session Management

The system shall allow restarting and replaying training sessions.

### Acceptance Criteria

- Users can restart training.
- Users can replay training activities.
- Session data resets appropriately.

---

## FR-12 Extensible Content Architecture

The system shall support additional training content through configurable data structures.

### Acceptance Criteria

- New components can be added without modifying core interaction systems.
- New connections can be added without modifying core interaction systems.
- Invalid references generate diagnostic messages.

---

# Non-Functional Requirements

## NFR-1 Platform Compatibility

The system shall use Unity 6.3 LTS and OpenXR.

### Acceptance Criteria

- The application builds successfully in Unity 6.3 LTS.
- OpenXR functionality operates on the target XR device.

---

## NFR-2 Performance

The system shall provide smooth and responsive XR interactions.

### Acceptance Criteria

- XR interactions remain responsive during normal operation.
- The application performs acceptably on the target device.
- No significant lag occurs during training activities.

---

## NFR-3 Maintainability

The system shall support future extension through configurable content.

### Acceptance Criteria

- Content additions require minimal code changes.
- Content-authoring instructions are documented.

---

## NFR-4 Usability

The system shall be usable by learners with minimal assistance.

### Acceptance Criteria

- A learner can complete guided activities independently.
- Navigation controls are understandable and accessible.

---

## NFR-5 Reliability

The system shall maintain valid activity state throughout operation.

### Acceptance Criteria

- Progress tracking remains accurate.
- Scene state is not corrupted during navigation.
- Training activities can be completed without crashes during normal use.

---

## NFR-6 Documentation

The system shall provide user and developer documentation.

### Acceptance Criteria

- README instructions support setup from a clean clone.
- Content-authoring guidance is provided.
- Known limitations are documented.

---

## NFR-7 Traceability

The system shall maintain stable identifiers for all timber components.

### Acceptance Criteria

- Every component has a unique identifier.
- Identifiers remain consistent across system features.

---

# Constraints

The following constraints are imposed on the project:

- Development must use Unity 6.3 LTS.
- Development must use OpenXR.
- The project focuses on training rather than structural engineering analysis.
- Interaction will prioritize XR controllers rather than advanced custom hand-tracking systems.
- Timber geometry and reference information are provided externally.
- Development is limited by available XR hardware.
- The project timeline follows OSU Capstone schedules and milestones.

---

# Success Metrics

The project will be considered successful if:

1. Users can complete the guided assembly activity.
2. Users can correctly identify timber components and connections.
3. The application successfully runs on the target XR device.
4. Additional content can be added without redesigning core systems.
5. Learners receive meaningful feedback throughout training.
6. Training progress and metrics are recorded successfully.

---

# Non-Goals

The project will not:

- Perform structural engineering analysis.
- Generate new timber structure designs.
- Provide construction certification.
- Replace professional construction supervision.
- Simulate every possible timber connection type.
- Support multi-user collaboration during the initial release.
- Implement custom hand-tracking interaction systems.

---

# Open Questions

- Which XR headset(s) will be officially supported?
- What level of assembly validation will be implemented?
- How will training results be stored and exported?
- Which metrics are most valuable to instructors?
- How many representative timber connections will be included in the final release?

---

## Module 1 Issues

### Issue 1 — Establish the Unity scene and import representative timber components
Goal: Verify that the supplied models can support interactive exploration in Unity.
Tasks

Create the Unity project and document the editor version, required packages, and launch instructions.
Review the reference models and select a small set of components containing one representative connection.
Import the selected geometry, preserving its assembled positions and keeping components individually selectable.
Check units, scale, orientation, materials, and geometry quality.
Assign stable identifiers linking imported components to the source materials.
Create a runnable scene on the agreed target device.

Acceptance criteria

A teammate can open the project and run the scene using the README.
The representative components appear in the correct relative positions.
At least one known dimension is checked against the source model.
Each component can be addressed independently through a stable identifier.
The scene runs on the agreed target device, with any import limitations documented.

### Issue 2 — Select a component and display its identity and role
Goal: Let learners identify parts directly within the structure.
Tasks

Implement selection using the agreed interaction method.
Highlight the selected component.
Display its identifier, name, and a short explanation of its role.
Implement deselection and switching between components.
Store component information separately from interaction code.

Acceptance criteria

Selecting a component displays the information associated with that component.
Selecting another component updates both the highlight and information panel.
Deselecting clears the selection state.
Missing descriptive content produces a clear fallback without breaking interaction.

Dependency: Issue 1.

### Issue 3 — Inspect a component and return to its structural context
Goal: Let learners examine component geometry independently.
Tasks

Add an inspection view for the selected component.
Support rotation, enlargement, and resetting the inspection view.
Show relevant dimensions and at least one annotated geometric feature.
Provide a return action that restores the structural view.
Clearly indicate when the inspection display is enlarged.

Acceptance criteria

The learner can inspect the component from multiple directions.
Manipulating the inspection view does not change the component’s assembled position.
Displayed dimensions agree with the validated source information.
Returning to the structure preserves the component selection.

Dependency: Issue 2.

### Issue 4 — Explore one connection in assembled and exploded views
Goal: Explain how a representative connection joins its participating components.
Tasks

Confirm the connection’s participating parts and interfaces using the reference materials.
Define the connection relationship using component identifiers.
Highlight the participating parts and their interfaces.
Implement assembled and exploded detail views.
Add concise annotations explaining how the interfaces fit together.
Label the exploded view as an explanatory visualization.

Acceptance criteria

The connection view displays the correct participating parts.
The learner can distinguish the relevant interfaces.
Switching between assembled and exploded views is reversible and repeatable.
Returning to the structure restores the original component positions.
The connection explanation is checked against the reference materials; unresolved technical questions are documented.

Dependency: Issue 2. Reuse the inspection functionality from Issue 3 where appropriate.

### Issue 5 — Browse the kit of parts and locate components in the structure
Goal: Connect component types with their instances in the complete structure.
Tasks

Create a catalog of the component types included in the current increment.
Distinguish a component type from its individual instances.
Display a name and preview for each type.
Allow learners to select a catalog entry and locate its instances in the structure.
Provide access to component inspection from the catalog.

Acceptance criteria

Every catalog entry maps to the correct component instances.
Selecting an entry highlights its instances in the structure.
The learner can select an individual instance and inspect it.
The catalog clearly communicates which components are currently included.

Dependencies: Issues 2–3.

### Issue 6 — Guide the learner through a short exploration activity
Goal: Provide a purposeful introduction to components and connections.
Tasks

Define a short sequence covering component identification, inspection, and connection exploration.
Add prompts explaining what to observe.
Implement next, previous, restart, and exit actions.
Show progress through the activity.
Keep the activity content separate from navigation logic.

Acceptance criteria

A learner can complete the activity without developer assistance.
Each step displays the relevant component or connection.
Navigation works without losing or corrupting the scene state.
The learner can exit to free exploration and restart the activity.

Dependencies: Issues 3–4.

### Issue 7 — Check understanding through identification tasks
Goal: Check whether learners recognize the components and relationships they explored.
Tasks

Add one component-identification task and one connection-identification task.
Define the accepted answers explicitly.
Provide explanatory feedback for correct and incorrect responses.
Allow learners to retry and revisit the relevant explanation.
Show activity completion separately from answer correctness.

Acceptance criteria

Each task evaluates responses against its configured answer.
Feedback explains the component’s role or connection relationship.
Incorrect responses allow another attempt.
Restarting clears the previous attempt state.
Completing the activity does not automatically imply that all answers were correct.

Dependencies: Issues 2 and 4.

### Issue 8 — Extend the module using configurable content
Goal: Demonstrate that the module can support additional components and connections through a consistent content structure.
Tasks

Consolidate component, connection, annotation, and activity data.
Document how to add a component type, an instance, and a connection.
Add another representative component and connection using that process.
Validate missing identifiers, broken references, and incomplete required content.

Acceptance criteria

A teammate can add the additional content by following the documentation.
Adding content does not require changes to the core selection or inspection logic.
The new content works with the existing exploration and learning interactions.
Invalid references produce actionable diagnostic messages.

Dependencies: Issues 5–7.

Constrained Decisions:
The team has decided not to implement hand controller design. We are more focused on using controllers instead.

## Shared Definition of Done

Changes are committed, reviewed, and merged into the team’s repository.
Acceptance criteria are demonstrated on the agreed target device.
The issue includes a short demonstration video or screenshots appropriate to the feature.
Relevant setup and content-authoring instructions are updated.
Known limitations and unresolved domain questions are recorded
