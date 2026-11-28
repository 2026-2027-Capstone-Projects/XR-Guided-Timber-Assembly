RFC: Hand Tracking vs. Controller-Based 
Interaction for XR Timber Assembly 
Training 
 
Status: Accepted
Project: XR Guided Timber Assembly Training System 
Platform Constraint: Unity 6.3 LTS 
Decision 
Should the team implement the timber assembly training system using hand tracking, controller based interaction, or a hybrid approach as the primary method of user interaction? 
1. Problem and Context 
The XR Guided Timber Assembly Training System is intended to provide an immersive educational experience in which users learn timber assembly procedures through virtual practice. The application is being developed in Unity 6.3 LTS and is intended for instructional use. Users will need to interact with virtual timber components, training prompts, assembly instructions, and environmental objects throughout the training experience. 
A critical architectural decision facing the team is determining how users will interact with the system. The XR Guided Timber Assembly Training System is being developed for deployment on Meta Quest 2 and Meta Quest 3S headsets. Both devices support controller-based interaction, while newer Meta Quest devices also provide hand-tracking capabilities. Because the interaction system will be integrated into every assembly task, menu, and instructional workflow, the team must determine whether to build the application around controllers, hand tracking, or a hybrid approach before significant development occurs.
Interaction systems are deeply integrated into XR applications. Every assembly task, menu interaction, instructional prompt, and object manipulation mechanism depends upon the chosen input model. If the project is built around controllers and later moves to hand tracking, many interaction systems may need to be redesigned. Likewise, if the system is designed primarily around hand tracking and later testing reveals insufficient tracking reliability for precise timber assembly tasks, the team may face substantial redevelopment costs to migrate interactions to a controller-first design.
This decision is considered hard to reverse because interaction systems form the foundation of the user experience. A change in interaction methodology would affect tutorials, object handling, user interface design, testing plans, training materials, and software architecture. The cost of altering these systems increases significantly as development progresses. 
If the team does not make a deliberate decision, developers may implement inconsistent interaction methods across different parts of the project. This would create technical debt, increase maintenance costs, and make future testing more difficult. Therefore, an interaction strategy should be selected early in development. 
2. Options 
Option A: Hand Tracking 
Users interact directly with the virtual environment using their real hands without requiring physical controllers. 
Benefits 
•	Natural interaction model 
•	Increased immersion 
•	Reduced learning curve 
•	More realistic simulation of timber assembly activities 
Costs 
•	Reduced interaction accuracy when hands are occluded or outside the headset tracking volume
•	Potential tracking inaccuracies 
•	Increased implementation complexity 
•	Reduced reliability under poor tracking conditions 
Option B: Controllers 
Users interact with the virtual environment using XR controllers supplied with the headset. 
Benefits 
•	Reliable tracking 
•	Mature development tools 
•	Broad hardware support 
•	Simpler implementation and testing 
Costs 
•	Lower realism 
•	Less direct interaction with virtual objects 
•	Additional user onboarding 

Option C: Hybrid Support 
Support both hand tracking and controllers, allowing users to choose their preferred interaction method. 
Benefits 
•	Maximum flexibility 
•	Better accessibility 
•	Supports future interaction requirements
Costs 
•	Increased development complexity 
•	Additional testing workload 
•	Higher maintenance costs 
Option D: Do Nothing 
Delay selecting an interaction method until more information becomes available. 
Benefits 
•	Additional time for research 
•	Potentially more stakeholder input 
Costs 
•	Delayed development 
•	Inconsistent implementation decisions 
•	Greater risk of future rework 
3. Analysis 
Option A: Hand Tracking 
Hand tracking creates a natural interaction experience because users directly manipulate virtual objects using their hands, a capability specifically supported by Meta's hand-tracking system for Quest devices [2]. For a timber assembly training application, this closely resembles real-world behavior. Trainees would be able to grab components, position pieces, and interact with instructional elements in a manner that feels like physical construction work. 
From an educational perspective, this realism has significant value. Since the goal of the system is training rather than entertainment, a more natural interaction model may help users focus on the assembly process instead of learning how to operate controllers. 
However, hand tracking introduces technical challenges. Tracking quality depends heavily on available hardware and environmental conditions. Hand occlusion, lighting conditions, and rapid movement can affect accuracy. If interactions fail due to tracking limitations, user frustration may outweigh immersion benefits. 
Hand tracking is supported on both Meta Quest 2 and Meta Quest 3S; however, tracking quality can vary depending on hand position, occlusion, lighting conditions, and the headset's field of view [2][3]. Assembly activities may require users to position virtual timber pieces close together or partially block one hand with another, which can reduce tracking reliability during precise manipulation tasks. 
Overall, hand tracking offers the highest realism but may introduce implementation and compatibility risks. 
Option B: Controllers 
Controller-based interaction is the most established input method in XR development. Most XR hardware platforms support controllers, and Unity provides extensive tooling through the XR Interaction Toolkit, including object interaction, UI interaction, and locomotion systems [1][4].
Controller-based interaction is the most mature interaction method available on Meta Quest 2 and Meta Quest 3S. The Quest controller tracking system provides consistent positional tracking and dedicated input buttons that can simplify object selection, grabbing, rotation, and menu navigation. Because timber assembly exercises require accurate placement of components, controller reliability may reduce interaction errors during training sessions.
Controllers typically provide more reliable tracking than hand tracking. Buttons, triggers, and joysticks offer explicit interaction mechanisms that are less prone to accidental input. This reliability can simplify testing and reduce development risk. 
Controller interactions are generally easier to implement because Unity's XR Interaction Toolkit provides mature examples, framework support, and documentation for controller-based workflows [1]. As a result, controller-based systems may reduce development time. 
The primary disadvantage is reduced realism. In real timber assembly, workers do not use handheld controllers to manipulate materials. This disconnect between training actions and real world actions may reduce immersion and instructional effectiveness. 
Controllers also create an additional learning step for users unfamiliar with XR hardware. 

During initial Unity XR prototype development, controller interaction was implemented successfully using Unity XR Interaction Toolkit sample scenes [6]. Hand tracking support required additional platform-specific configuration and introduced greater implementation complexity than the controller-based prototype.

Controller Input Mapping Considerations
A controller-based implementation does not automatically determine how users access interaction features. The team considered two approaches: hard-coded button mappings and radial menu selection.
Hard-coded button mappings provide faster access to frequently used actions because users can activate functions directly through controller buttons. However, this approach introduces several challenges. Users must memorize button assignments, and future features may require remapping controls as the application grows. Since Meta Quest controllers provide a finite set of buttons and inputs, scalability becomes a concern when additional training tools or assembly actions are introduced [3][5].
Radial menus provide a visual method for selecting actions. Instead of memorizing individual button assignments, users can view available actions and select them using controller input or hand-tracking gestures. This improves discoverability and simplifies onboarding for users who may have little prior XR experience.
The primary cost of radial menus is slightly slower interaction speed because users must open and navigate the menu before executing an action. However, for a training-focused application, reducing cognitive load may be more valuable than maximizing interaction speed. Since assembly tasks prioritize accuracy over rapid action execution, radial menus may provide a better long-term solution.
Overall, controllers provide stability and lower implementation risk but offer a less authentic training experience. 
Option C: Hybrid Support 
A hybrid approach combines hand tracking and controller support into a single system. Users can select whichever interaction method best fits their hardware and preferences. 
This option provides the greatest long-term flexibility. If project requirements change later, the application would already support multiple interaction methods. Accessibility also improves because users can choose the interaction style that is most comfortable for them. 
Since both Meta Quest 2 and Meta Quest 3S support controller-based interaction and hand tracking, a hybrid solution is technically feasible from a platform perspective [2][3]. Users could perform assembly tasks with hand tracking while using controllers as a fallback when tracking precision becomes insufficient. While this flexibility improves accessibility and future-proofs the application, every interaction would still require dual implementation and testing.
However, supporting two systems significantly increases development complexity. Every interaction must be implemented and tested twice. Documentation, tutorials, and user support requirements also increase. For a small academic team with limited resources, these additional responsibilities may reduce productivity. 
Hybrid support provides maximum flexibility but carries the highest implementation cost. 
Option D: Do Nothing 
Delaying the decision may seem beneficial because future stakeholder requirements and interaction preferences may continue to evolve throughout development.
However, postponing the decision creates a risk that developers make informal decisions individually. Different approaches could be implemented across the project, resulting in inconsistent user experiences and expensive refactoring later. 
Because interaction systems affect nearly every part of the application, delaying the decision creates more long-term risk than short-term benefit. 
4. Recommendation and Verification Plan 
Recommendation 
While controller-based interaction is recommended as the primary interaction method, the team should avoid relying exclusively on hard-coded button mappings for advanced actions. Instead, commonly used actions should remain directly accessible while secondary functions are exposed through a radial menu system.
This approach supports future expansion of the training platform. Additional training tools, assembly modes, instructional functions, or accessibility features can be added without requiring large-scale controller remapping. Using a radial menu also aligns with the educational goals of the project by making available actions visible to new users rather than requiring memorization of multiple controller inputs.
Because the expected deployment platforms are Meta Quest 2 and Meta Quest 3S, the radial menu could also provide a future pathway toward hand-tracked interaction if the project later adopts hybrid input support.
This recommendation balances development efficiency, reliability, and future adaptability. 
Verification Plan 
The decision will be considered successful if the following criteria are met: 
Functional Validation 
•	Users can complete an entire timber assembly lesson. 
•	Objects can be selected, moved, and positioned without interaction failures. 
•	Menu navigation remains intuitive and consistent. 
Performance Validation 
•	Application maintains 72 FPS or higher during assembly exercises on Meta Quest 2 and Meta Quest 3S. 
•	Users successfully grab and place virtual timber components in 95% or more of interaction attempts. 
•	Input latency remains below 100 ms during normal operation.
Educational Validation 

•	Training objectives can be completed within expected time limits. 
•	At least 80% of test users complete the assembly lesson without instructor intervention. 
•	At least 90% of users correctly complete required assembly steps after following instructional guidance.
Development Validation 
•	Team members can implement new interactions without significant framework modifications. 

•	Interaction systems remain maintainable throughout the project lifecycle. 


•	Future hand-tracking support can be evaluated without requiring a complete redesign. 
If these requirements cannot be met, the team should revisit the decision and re-evaluate hand tracking or hybrid solutions. 
5. Constraints 
Several project constraints influence this decision. The application must be developed using Unity 6.3 LTS and deployed on Meta Quest 2 and Meta Quest 3S hardware. Any interaction system selected must be compatible with Unity's XR Interaction Toolkit and supported by Meta's XR platform tools [1][3].
The project is developed under academic time constraints, limiting the team's ability to redesign major interaction systems later in development. Supporting both hand tracking and controllers would require additional implementation, testing, and maintenance effort that may exceed available project resources.
The primary goal of the system is educational training rather than demonstrating advanced XR technology. Therefore, reliable completion of assembly exercises is more important than maximizing immersion. This requirement increases the value of interaction methods that provide predictable performance and consistent user experiences during training activities.
These constraints favor a controller-first design while preserving the possibility of adding hand-tracking support in future iterations.

User Experience Constraint
Meta Quest controllers contain a limited number of available buttons. As the XR timber assembly system evolves, additional interaction features may be required, including instructional hints, assembly step navigation, object manipulation tools, and accessibility functions. Designing the interaction system around fixed button mappings could make future expansion more difficult and increase the likelihood of redesigning controls later in development.
To mitigate this risk, the team should consider interaction models that can scale as new functionality is added, including contextual menus or radial menu systems.
6. Delegation and Validation 
AI tools were used to brainstorm interaction approaches, organize RFC structure, identify potential tradeoffs between controllers and hand tracking, and suggest verification criteria. AI was not used to make the final engineering recommendation.
The author independently validated several critical points because errors in these areas would result in costly redesigns later in development:
•	Compatibility of controller and hand-tracking support with Unity 6.3 LTS and Meta Quest 2/3S development requirements.
•	Feasibility of implementing controller interactions using Unity XR Interaction Toolkit sample scenes and existing project prototypes.
•	The impact of interaction-system changes on object manipulation, menu navigation, instructional workflows, and future maintainability.
•	The scalability benefits of radial menus compared to fixed controller mappings as additional training features are added.
These validation steps were prioritized because the interaction system affects nearly every user-facing component of the project. An incorrect decision would require significant rework across tutorials, UI systems, training content, and testing procedures.
The final recommendation reflects engineering judgment based on the team's current understanding of project requirements As new information becomes available regarding user testing results, interaction performance, and stakeholder requirements, this RFC should be reviewed and revised accordingly.

References
[1] Unity Technologies. XR Interaction Toolkit Documentation. Unity Documentation. Available: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest
[2] Meta. Hand Tracking in Unity. Meta Horizon Developer Documentation. Available: https://developers.meta.com/horizon/documentation/unity/unity-handtracking
[3] Meta. Meta Quest Developer Documentation. Meta Horizon Developer Documentation. Available: https://developers.meta.com/horizon/documentation/
[4] Unity Technologies. Unity XR Overview. Unity Documentation. Available: https://docs.unity3d.com/Manual/XR.html
[5] Unity Technologies. Input System Package Documentation. Unity Documentation. Available: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
[6] Project Team. Unity XR Interaction Toolkit Prototype Implementation. Internal prototype, XR Guided Timber Assembly Training System.
[7] Project Team. Interaction Design Workshop Results and Team Discussions. Internal project documentation.



Review Log

Source	Feedback	Decision	Where or Why
Reviewer 1	Excessive horizontal lines reduce readability because subsection breaks are visually similar to section breaks.
	Changed	Formatting throughout the RFC: horizontal rules were removed from subsection boundaries

Reviewer 2 	Verification criteria should include measurable performance targets rather than subjective terms such as "unnoticeable delays" or "acceptable frame rates."
	Changed	Recommendation and Verification Plan: added measurable validation criteria including target frame rates, interaction success rates, and user completion metrics for Meta Quest 2/3S testing.


