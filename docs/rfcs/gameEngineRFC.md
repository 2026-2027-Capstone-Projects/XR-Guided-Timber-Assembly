<!-- docs/rfcs/YYYY-MM-DD-short-title.md (or docs/adr/, whichever your repo uses) -->

# RFC: Game Engine Choice

<!-- Decision record header. Leave Status as "Proposed" until your team decides,
     before or after the reviews. Then fill in the outcome and the decision date.
     The document stays where it is; this header is what makes it a record. -->

| Field | Value |
| --- | --- |
| Ethan Robinson | |
| Date | 2026-10-22 |
| Status | Accepted |
| Decision Date | 2026-10-23 |
| Deciders | Ethan Robinson, Sean Capers, Kaitlyn McLaughlin, Abraham Smith |
| Superseded By | (link, if applicable) |

## Problem and Context

Of the many hard-to-reverse decisions our team will face when developing our XR-Guided Timber Assembly program, one vital one is our choice of game engine. The game engine will be an essential element upon which our program is designed and our choice of engine will determine many development processes and decisions moving forward. Much like a programming language, reversing this decision would effectively require completely recreating and rewriting most, if not the entirety of our project. As such, this decision cannot be easily reversed and will require heavy consideration to ensure we make the best choice for our project. As this decision is a major pillar in the design and procedure of coding our program, we will need to make this decision soon as development cannot begin until it is made. If our team delays this decision or chooses to do nothing, we will end up delaying or not even not completing our program and as such this decision must be taken into consideration first.

## Options

There are three options that we can take into consideration; Unreal Engine, Unity, or doing nothing. Unreal Engine provides greater performance; being a highly powerful, capable engine, making it well-suited for an XR application. Two of our team members are also already familiar with VR development in Unreal. Unity is often considered much more intuitive, and due to its longer history, has much more documentation and resources than Unreal. This would make it an ideal choice for developers who have no experience working with game engines. Only one member of our team has previously worked with Unity, but due to its beginner-friendly design, this lack of experience poses less concern than it does with Unreal. For this reason, our partner has recommended Unity as our engine of choice. Doing nothing would result in little to no development. Without an engine, there is no software. Our program would never be completed.

### Option A: Unity

Unity is often considered an excellent choice for new game developers, with an editor that is less overwhelming and has a greater amount of tutorials and learning tools available. Its performance is often more predictable, and though less powerful than Unreal, is capable of generating excellent stylized 3D games. Its lower performance specs does result in a lower hardware requirement, allowing the editor and created programs to be run on a greater variety of machines than Unreal (Strayspark, 2026).

### Option B: Unreal

Much of Unreal’s popularity in recent years has been due to its ability to generate highly-detailed, realistic 3D visuals. Its ability to do so can’t even be matched in Unity without a significant amount of time and work being dedicated solely to custom rendering (Strayspark, 2026). However, this does come at the cost of much higher hardware requirements, and a more complex editor. Unreal is highly suited for large games, particularly those with high-fidelity graphics. As such, many small and indie development teams often choose to stick with Unity when possible, as noted by Rocket Brush Studio notes in their blog page; “The tradeoff is that Unreal rewards ambition and punishes unrealistic scope. Smaller teams can absolutely ship with it, but they need to budget for heavier content, stronger machines and more disciplined optimization.” (Rocket Brush Studio, 2026).

### Option C: Do Nothing

This option, though listed, should not be considered as an option. To do nothing would result in a failure to deliver a product and fulfill the requirements set out for our team.

## Analysis

As not every member of our team has experience with 3D game development or game engines, this is a factor that will need to be heavily weighed when making a decision. Our ability to code and utilize the editor on our personal devices should also factor into our decision as a lack of adequate hardware may make development and testing in Unreal difficult. Furthermore, as a team of only four developers, all of which have other classes/commitments, an editor that may present a greater workload will not be ideal. Finally, the primary goal of our program is a functional, learning tool. Controls, physics, and user-program interaction are the highest priorities. Detailed graphics are welcome and can enhance the experience, making for a more realistic simulation, however are not as necessary as the functionality.

## Recommendation and Verification Plan

With all these factors considered; I reccomend Unity as a our engine of choice. As a small, relatively inexperienced development team, a beginner-friendly editor with vast learning resources will prove more well-suited to our needs. Our program will utilize 3D models and graphics, however will remain within the scope of Unity's capabilities, making Unreal's higher performance not a necessity. However, in order to verify that Unity will be easy to use and learn, each of us will experiment with the software, creating a small, simple, interactive 3D program to learn the basics of the engine. We will then discuss, weigh the options and each's benefits, as well as our experience with Unity. If we feel Unity wil be capable of delivering on all our project's requirements, it will be our engine of choice.

## Constraints

None apply; both engine options are free to use when revenue is under a specified threshold; there will be no revenue for our project. 

## Delegation and Validation

Sources:
https://www.strayspark.studio/blog/unreal-engine-vs-unity-2026-indie-developers

https://rocketbrush.com/blog/unity-vs-unreal-engine-which-one-is-right-for-your-game

No AI utilized for this RFC

## Decision

Accepted
