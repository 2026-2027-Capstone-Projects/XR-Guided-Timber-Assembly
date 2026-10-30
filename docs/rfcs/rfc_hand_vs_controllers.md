# RFC: Hand Tracking vs. Controller-Based Interaction for XR Timber Assembly Training

**Status:** Draft  
**Project:** XR Guided Timber Assembly Training System  
**Platform Constraint:** Unity 6.3 LTS

## Decision

Should the team implement the timber assembly training system using hand tracking, controller-based interaction, or a hybrid approach as the primary method of user interaction?

## 1. Problem and Context

The XR Guided Timber Assembly Training System is intended to provide an immersive educational experience in which users learn timber assembly procedures through virtual practice. The application is being developed in Unity 6.3 LTS and is intended for instructional use. Users will need to interact with virtual timber components, training prompts, assembly instructions, and environmental objects throughout the training experience.

A critical architectural decision facing the team is determining how users will interact with the system. At the current stage of development, the team has not yet finalized the target hardware platform. Because of this uncertainty, the interaction method selected today may significantly affect future development and deployment options.

Interaction systems are deeply integrated into XR applications. Every assembly task, menu interaction, instructional prompt, and object manipulation mechanism depends upon the chosen input model. If the project is built around controllers and later moves to hand tracking, many interaction systems may need to be redesigned. Likewise, if the system is designed around hand tracking but stakeholders later require hardware without reliable hand tracking support, the team may face substantial redevelopment costs.

This decision is considered hard to reverse because interaction systems form the foundation of the user experience. A change in interaction methodology would affect tutorials, object handling, user interface design, testing plans, training materials, and software architecture. The cost of altering these systems increases significantly as development progresses.

If the team does not make a deliberate decision, developers may implement inconsistent interaction methods across different parts of the project. This would create technical debt, increase maintenance costs, and make future testing more difficult. Therefore, an interaction strategy should be selected early in development.

## 2. Options

### Option A: Hand Tracking

Users interact directly with the virtual environment using their real hands without requiring physical controllers.

**Benefits**

- Natural interaction model
- Increased immersion
- Reduced learning curve
- More realistic simulation of timber assembly activities

**Costs**

- Hardware compatibility concerns
- Potential tracking inaccuracies
- Increased implementation complexity
- Reduced reliability under poor tracking conditions

### Option B: Controllers

Users interact with the virtual environment using XR controllers supplied with the headset.

**Benefits**

- Reliable tracking
- Mature development tools
- Broad hardware support
- Simpler implementation and testing

**Costs**

- Lower realism
- Less direct interaction with virtual objects
- Additional user onboarding

### Option C: Hybrid Support

Support both hand tracking and controllers, allowing users to choose their preferred interaction method.

**Benefits**

- Maximum flexibility
- Better accessibility
- Supports future hardware changes

**Costs**

- Increased development complexity
- Additional testing workload
- Higher maintenance costs

### Option D: Do Nothing

Delay selecting an interaction method until more information becomes available.

**Benefits**

- Additional time for research
- Potentially more stakeholder input

**Costs**

- Delayed development
- Inconsistent implementation decisions
- Greater risk of future rework

## 3. Analysis

### Option A: Hand Tracking

Hand tracking creates the most natural interaction experience because users directly manipulate virtual objects using their hands. For a timber assembly training application, this closely resembles real-world behavior. Trainees would be able to grab components, position pieces, and interact with instructional elements in a manner that feels similar to physical construction work.

From an educational perspective, this realism has significant value. Since the goal of the system is training rather than entertainment, a more natural interaction model may help users focus on the assembly process instead of learning how to operate controllers.

However, hand tracking introduces technical challenges. Tracking quality depends heavily on available hardware and environmental conditions. Hand occlusion, lighting conditions, and rapid movement can affect accuracy. If interactions fail due to tracking limitations, user frustration may outweigh immersion benefits.

Hand tracking also introduces uncertainty because the final hardware platform has not been selected. Not all XR devices provide equivalent hand-tracking capabilities.

Overall, hand tracking offers the highest realism but may introduce implementation and compatibility risks.

### Option B: Controllers

Controller-based interaction is the most established input method in XR development. Most XR hardware platforms support controllers, and Unity provides extensive tooling for controller-based interaction.

Controllers typically provide more reliable tracking than hand tracking. Buttons, triggers, and joysticks offer explicit interaction mechanisms that are less prone to accidental input. This reliability can simplify testing and reduce development risk.

Controller interactions are generally easier to implement because many tutorials, frameworks, and examples already support this workflow. As a result, controller-based systems may reduce development time.

The primary disadvantage is reduced realism. In real timber assembly, workers do not use handheld controllers to manipulate materials. This disconnect between training actions and real-world actions may reduce immersion and instructional effectiveness.

Controllers also create an additional learning step for users unfamiliar with XR hardware.

Overall, controllers provide stability and lower implementation risk but offer a less authentic training experience.

### Option C: Hybrid Support

A hybrid approach combines hand tracking and controller support into a single system. Users can select whichever interaction method best fits their hardware and preferences.

This option provides the greatest long-term flexibility. If project requirements change later, the application would already support multiple interaction methods. Accessibility also improves because users can choose the interaction style that is most comfortable for them.

However, supporting two systems significantly increases development complexity. Every interaction must be implemented and tested twice. Documentation, tutorials, and user support requirements also increase. For a small academic team with limited resources, these additional responsibilities may reduce productivity.

Hybrid support provides maximum flexibility but carries the highest implementation cost.

### Option D: Do Nothing

Delaying the decision may seem beneficial because hardware requirements are still uncertain. Additional stakeholder information could become available in future project meetings.

However, postponing the decision creates a risk that developers make informal decisions individually. Different approaches could be implemented across the project, resulting in inconsistent user experiences and expensive refactoring later.

Because interaction systems affect nearly every part of the application, delaying the decision creates more long-term risk than short-term benefit.

## 4. Recommendation and Verification Plan

### Recommendation

The team should adopt controller-based interaction as the primary interaction method while designing systems that can accommodate future hand-tracking support if stakeholder requirements change.

Although hand tracking provides greater realism, the current uncertainty surrounding hardware selection makes controllers the lower-risk choice. Controller support is widely available, well documented, and compatible with most XR devices.

This recommendation balances development efficiency, reliability, and future adaptability.

### Verification Plan

The decision will be considered successful if the following criteria are met:

#### Functional Validation

- Users can complete an entire timber assembly lesson.
- Objects can be selected, moved, and positioned without interaction failures.
- Menu navigation remains intuitive and consistent.

#### Performance Validation

- Interaction delays remain unnoticeable during training exercises.
- No major tracking failures occur during standard usage.
- The system maintains acceptable frame rates during interaction-heavy scenarios.

#### Educational Validation

- Users successfully complete assembly procedures.
- Users understand instructional guidance without requiring extensive controller training.
- Training objectives can be completed within expected time limits.

#### Development Validation

- Team members can implement new interactions without significant framework modifications.
- Interaction systems remain maintainable throughout the project lifecycle.
- Future hand-tracking support can be evaluated without requiring a complete redesign.

If these requirements cannot be met, the team should revisit the decision and re-evaluate hand tracking or hybrid solutions.

## 5. Constraints

Several constraints affect this decision. The project must be developed using Unity 6.3 LTS, and final hardware requirements have not yet been determined by project stakeholders. The system is being developed under academic deadlines that limit the team's ability to perform large-scale architectural migrations later in development.

The application's primary purpose is educational training rather than technological experimentation. As a result, reliable instruction and maintainability are more important than demonstrating advanced XR features. The team must also account for future stakeholder requests and evolving project requirements throughout development.

These constraints favor solutions that reduce implementation risk while preserving future flexibility.

## 6. Delegation and Validation

AI tools were used to brainstorm options, organize RFC sections, and identify tradeoffs between interaction methods. The interaction models, risks, benefits, and verification criteria presented in this document were reviewed against the team's workshop results and project goals.

No user-testing results, performance metrics, or hardware benchmarks were fabricated. Any future measurements referenced in project planning will be collected through actual implementation and testing activities.

The final recommendation reflects engineering judgment based on the team's current understanding of project requirements. As new information becomes available regarding hardware selection and stakeholder needs, this RFC should be reviewed and revised accordingly.
