<!-- docs/rfcs/YYYY-MM-DD-short-title.md (or docs/adr/, whichever your repo uses) -->

# RFC: XR Type Decision

<!-- Decision record header. Leave Status as "Proposed" until your team decides.
     Then fill in the outcome and the decision date.
     The document stays where it is; this header is what makes it a record. -->

| Field | Value |
| --- | --- |
| Author | Kaitlyn McLaughlin |
| Date | 2026-10-25 |
| Status | Proposed |
| Decision Date | (fill when decided) |
| Deciders | Full Team |
| Superseded By | (link, if applicable) |

## Problem and Context

One of the largest decisions our group had to make for this project was what type of extended reality (XR) we want to build our project around. This choice determines how the user interacts with the program, which tools and engines we build with, and what hardware we need access to during production.

The decision has to be made very early on in production. Assets, scripts, interaction systems, and tracking code all are all going to be created specifically for a certain medium. Switching between different mediums after any of those things already exist would mean rewriting most of them from the beginning, because separate mediums differ in how they handle the environment, tracking, input, and rendering. 

The decision shapes what the tutorial is able to teach and how. In a virtual world, we as the programmers have full control over everything the user sees, hears, and experiences. With augmented reality, the user will be able to have a more accurate sense of scale to themselves and the world around them with the items they use. If the team does not decide on anything, development can’t truly start, since every other early task depends on this decision.


## Options

### Option A: Virtual Reality (VR)

The user wears a headset that blocks their view of the surrounding world completely, and sees a virtual environment rendered on screens in front of their eyes. The headset uses sensors and external cameras to track the position of the user’s head and the controllers, so the user can pick up, move, and connect virtual timber pieces and follow instructions displayed to them in the virtual world to complete the framing tutorial.

Benefits:
- The environment is fully controlled. Lighting, workspace, and the position of every part is known, so the application never needs to detect or map a real room.
- Mistakes are safe and free. Users can attempt a step wrong, undo it, and retry with no consequences.
- Rendering is simple. The program draws the whole scene, so virtual timber never has to blend with real lighting conditions, surfaces, or objects in front of it.
- Instructions and models can be placed and scaled freely.
- It works in any area with enough clear, well lit space, regardless of what else is in the room.

Costs:
- A full virtual environment needs to be built in addition to timber and tool models, which creates more asset work.
- Joining behavior has to be fully simulated, and a poor simulation could teach the wrong lessons.
- Some users experience motion sickness or vertigo with VR due to sensory conflict. Users also can’t see any of their surroundings while the program is running, so the use area needs to be clear and safe, and the headset's built-in safety mechanisms need to be set up correctly.
- VR headsets are somewhat expensive hardware items that often also require some level of training or experience to use properly, which could reduce the overall accessibility of the training.

### Option B: Augmented Reality (AR) / Mixed Reality

The user sees the real world around them, either through a headset with see-through or camera passthrough activated (mixed reality) or through a handheld device such as a phone or tablet. The timber pieces and instructions are all still virtual, but they are displayed in the user's real room and anchored to real surfaces such as the floor or a table. This way, the user builds the frame in their own space with true to life ratios that are easier to mentally process.

Benefits:
- The user maintains awareness of their surroundings, which lowers the chance of motion sickness and reduces the need for a cleared use area.
- Little environment art is needed, since the real room is the workspace.
- Virtual timber can be placed at full scale on a real floor or table, and the real surroundings give a natural reference for size.
- A handheld version can run on devices users already own, which would lower hardware costs.
Costs:
- The system has to be able to understand the room the user is in. It has to be able to detect floors and surfaces and keep virtual pieces anchored without drift. Things like poor lighting, clutter, reflective surfaces, and tracking loss can make pieces jitter or float, and results will vary depending on the user's room.
- Occlusion and depth are hard. Virtual timber can appear in front of real objects it should be behind, which breaks the illusion and makes placing pieces precisely harder.
- The environment is uncontrolled. Furniture, other people, and lighting can interfere, so we can't guarantee a consistent experience.
- Capable passthorugh or see-through hardware may cost more or be harder to access, and optical see-through displays often have a narrow field of view and dim content in bright light.


### Option C: Do Nothing

The team does not use any XR elements and instead delivers a tutorial in a more conventional form like printed or on screen plans, video instructions, or step by step 2D tutorials.
Costs:
The project would no longer be an XR program, so it would not meet its goal of creating a proper XR tutorial. We would also lose the spatial advantage of seeing a joint in three dimensions at full scale. This cost arrives immediately, because the project is dependent on XR application.


## Analysis

| Factor | VR | AR/MR |
| --- | --- | --- |
| Control of environment| Full: the scene is known and predictable | Limited: depends on the user’s room and lighting |
| Technical risk | Lower: no need to process the real room | Higher: surface detection, anchoring, drift, and occlusion must work in varying spaces |
| Asset workload | Higher: timber, tools, and a full environment | Lower: timber, tools, and instructions only, real world provides the setting |
| Awareness of surroundings | Low: the user cannot see the room and needs a clear use area | High: the user can see their whole room |
| Motion Discomfort | Possible | Less likely |
| Hardware | Widely available headsets, different pricing options | Needs good passthrough technology, or a handheld device |

Uncertainties:
- How reliable the chosen hardware can anchor and hold virtual timber steady in typical rooms.
- Whether seeing the real room helps users learn or distracts them from their task.
- How well skills learned with extended reality translate to the real world.

## Recommendation and Verification Plan

Option A: Virtual Reality
VR gives a fully controlled scene and removes the main technical risk of detecting and anchoring items to a real room. Our project partner has asked that the team not commit to a model prematurely, the decision is scoped to a fall term feasibility study and walking skeleton, and component, connection, and activity data stay separate from interaction code so other devices can be added later.

How we will know it worked:
- Performance: the application holds the headset’s target frame rate during the full tutorial. Checked at each build milestone.
- Task Successful: in a test with a group of real users unfamiliar with timber framing, the majority of them were able to successfully assemble the structure without external assistance.
- Accuracy of learning: users can accurately describe the assembly sequence afterward.
- Comfort: users report acceptable comfort levels in a questionnaire provided after the test, and minimal testers stop early due to discomfort.


## Constraints

Data use: headsets can collect tracking data about the user’s body, hands, or eyes, and AR or passthrough medals also process camera images of the user's surroundings.


## Delegation and Validation

No parts of this decision have been made with the use of AI. This decision is being made with the input of all team members and our project partner.
## Decision

Proposed. At our project partners' suggestion, we have not come to a full decision quite yet. We have, however, decided to start building our project in Unity’s VR core, allowing us to create a skeleton environment and build from there.
