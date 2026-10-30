
Module One Goals

Issue 1 — Establish the Unity scene and import representative timber components
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

Issue 2 — Select a component and display its identity and role
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

Issue 3 — Inspect a component and return to its structural context
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

Issue 4 — Explore one connection in assembled and exploded views
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

Issue 5 — Browse the kit of parts and locate components in the structure
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

Issue 6 — Guide the learner through a short exploration activity
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

Issue 7 — Check understanding through identification tasks
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

Issue 8 — Extend the module using configurable content
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

Shared Definition of Done

Changes are committed, reviewed, and merged into the team’s repository.
Acceptance criteria are demonstrated on the agreed target device.
The issue includes a short demonstration video or screenshots appropriate to the feature.
Relevant setup and content-authoring instructions are updated.
Known limitations and unresolved domain questions are recorded
