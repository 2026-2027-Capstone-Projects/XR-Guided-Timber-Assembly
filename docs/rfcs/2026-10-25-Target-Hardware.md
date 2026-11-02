<!-- docs/rfcs/2026-10-25-Target-Hardware.md (or docs/adr/, whichever your repo uses) -->
# RFC: Target Hardware
<!-- Decision record header. Leave Status as "Proposed" until your team decides,
before or after the reviews. Then fill in the outcome and the decision date.
The document stays where it is; this header is what makes it a record. -->
| Field | Value |
| --- | --- |
| Author | Sean Capers |
| Date | 2026-10-25 |
| Status | Proposed |
| Decision Date | (fill when decided) |
| Deciders | (who agreed to the outcome) |
<!-- | Superseded By | (link, if applicable) | -->
## Problem and Context
Target hardware needs to be decided. Whether we are designing for a PC, web-app, or for
direct on-VR-Headset deployment will greatly change how we handle system resources and
user-interaction.
The partner has thus far given us ample freedom in this regard, but we need to make this
decision before too many systems/assets are in place.
## Options
### Option A: PC Deployment
- Build for PC's, optionally utilizing HMDs.
#### Benefits:
- Generally higher RAM and processing resources to work with
- Ease of deployment: default deployment route in Unity
#### Costs:
- Limits deployment to PC labs and similar settings
### Option B: Web Deployment
Build for browser-based web deployment. This allows the app to be run from theoretically any
hardware.
#### Benefits:
* Ease of deployment: Any general environment can be used by the partner to deploy the
system; no installation required
#### Costs:
* Would need to design UX around possible mobile, HMD, and keyboard/mouse PC interaction;
this increases development time for all user interactions
* Must minimize resource utilization, since the user hardware could be Mobile
### Option C: Do Nothing
Currently, our design is directed toward HMD-only deployment.
#### Costs:
* Actual UX testing and thus development requires using an HMD
- That cost arrives immediately, and persists throughout development. Requires a rationing use
of a finite technological resource.
* Deployment requires use of expensive hardware
- This cost is paid by the partner.
* HMD's have limited resource and render each eye separately, and users are especially
sensitive to framerate drops
- This is a constant limitation throughout development, and if handled poorly can render the
product unusable.
## Analysis
Option A has moderate complexity due to concurrent HMD and keyboard/mouse setups, the
best maintainability, and the lowest time-investment to produce. However, deployment requires
PCs which can run it.
- This is based on the Unity pipeline: direct-to-PC is very easy to implement.
Option B has the most complexity, highest difficulty in maintainability, and likely the most limited
resources. That said, deployment would be the easiest. By accessing a URL, anyone with
access should be able to run it. This could be a security issue if access is to be limited, as our
partner would need to deploy it via secure web-access methods.
- This is based on deploying Unity experiences for browser
Option C has added development costs around HMD use, the lowest complexity, and absolutely
optimal performance would be required of our program: anything below 90 FPS can cause
nausea.
- This has to do with the resources available to our team
- The framerate requirements were a topic of study in the XR development class.
## Recommendation and Verification Plan
I propose Option A as the best option. Development is fairly straightforward for a PC app on
Unity, it's easier to maintain, and we don't have to ultra-optimize and reduce asset quality at
every step.
Acceptance Criteria: Partner accepts PC deployment, app can launch on minimum
specifications sheet to-be-requested from partner
## Constraints
The partner's hardware for deployment and security wishes for access will impact this decision.
## Delegation and Validation
- **What you handed to AI tools** on this decision, whether that was the analysis, the prototype,
the tests, or the writing.
- Nothing
- **What checks made that reasonable**: tests, CI, review gates, a staging environment, a
rollback path. Name the ones that actually exist in your repo, not the ones you intend to build.
- Currently, the partner requests we upload videos of the experience running for CI: If done on
target hardware, this would work well.
- **Where you stopped and validated the result yourself, and why those points.** If this decision
is hard to reverse, say what you personally checked before committing to it.
- I looked into developing and maintaining multiple methods of user-interaction for web-based
Unity projects
- I looked up hardware specifications of the target HMD we've discussed (Oculus Quest II)
## Decision
Proposed
