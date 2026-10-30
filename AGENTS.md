# Project Details

Timber construction requires assembling structural components according to precise spatial relationships and sequences, which drawings alone often fail to convey to inexperienced users. Construction workers, students, and trainees typically need significant experience to translate two-dimensional documentation into correct physical assembly actions — errors in component selection, orientation, order, or placement increase construction time and cause rework. This project develops an XR training system that guides users through assembling a representative timber structure, using interactive visualization and spatial guidance to help them identify components, understand placement, and follow the correct assembly sequence. It builds on ongoing research in augmented/extended reality for timber construction, with a focus on a robust software platform for guided assembly training.

The team is focusing on making a specific structure for a course at our school. This XR program need to be able to teach students how to build the structure and grade their progress against a module like training course. This makes the code interactive and gradable by the professor thats teaching that course.

---

## Development Enviorment

**Software being used:**
- Unity 6.3 LTS
- C#
- Backend TBD

**Hardware being used:**
- VR headset(TBD)
- VR Controllers(TBD)

**NOTE FOR AGENTS, KEEP CODE DRY!**

---

## Repository Structure

- `Timber-Assembly/` — the Unity project root (open this folder in Unity Hub, not the repo root)
  - `Assets/TimberFrameAssets/` — project-specific content: `Scripts/`, `Models/`, `Materials/`, `Shaders/`, `InteractivePrefabs/`
  - `Assets/VRTemplateAssets/`, `Assets/XR/`, `Assets/XRI/` — XR rig, input, and XR Interaction Toolkit resources
  - `Assets/Scenes/` — Unity scenes (e.g., `BasicScene.unity`)
  - `Assets/Localization/Parts/` — localized strings for part names/info
  - `Assets/AddressableAssetsData/` — Addressables groups/settings
  - `Assets/CompositionLayers/` — Composition Layers settings
  - `Assets/Samples/` — imported package samples (XR Hands, XR Interaction Toolkit) — reference only, avoid editing in place
  - `Packages/manifest.json` — package dependencies and versions
  - `ProjectSettings/` — Unity project configuration (editor version, build settings, etc.)

## Setup Instructions

1. Install **Unity Hub** and add the exact editor version pinned in `Timber-Assembly/ProjectSettings/ProjectVersion.txt` (currently `6000.3.26f1`, Unity 6.3 LTS).
2. In Unity Hub, add the project by pointing it at the `Timber-Assembly/` folder (the folder containing `Assets/`, `Packages/`, `ProjectSettings/`) — not the repository root.
3. During installation, include the **Android Build Support** module (with OpenJDK/Android SDK/NDK) since the manifest references `com.unity.xr.androidxr-openxr` and `com.unity.xr.meta-openxr`.
4. Let Unity resolve packages from `Packages/manifest.json` on first open (XR Interaction Toolkit 3.5.1, XR Hands 1.8.1, OpenXR 1.17.1, URP 17.3.0, etc.).
5. Headset/controller pairing steps: **TBD** — hardware has not been finalized yet. Once a headset is selected, document the OpenXR runtime install and device-pairing steps here.

## Coding Conventions

- New C# scripts belong under `Assets/TimberFrameAssets/Scripts/`; as the project grows, organize into feature subfolders (e.g., `Scripts/Interaction/`, `Scripts/UI/`, `Scripts/Grading/`) rather than one flat folder.
- Follow standard Unity/C# naming: `PascalCase` for classes, methods, and public members; `camelCase` for private fields; prefer `[SerializeField] private` over public fields for Inspector-exposed values.
- Keep MonoBehaviours focused on Unity lifecycle and wiring; move reusable/testable logic into plain C# classes or `ScriptableObject`s (see `PartInfo.cs` as the existing data-driven pattern for part metadata).
- Prefer composing XR Interaction Toolkit interactables/interactors over writing custom input handling.
- Keep code DRY — extract shared behavior instead of duplicating it across scripts (e.g., `XRHoverOutline.cs` and `XRInspectableObject.cs`).

## Build / Test / Validation Commands

- No automated test suite is currently set up in this repo. If adding one, use Unity's **Test Runner** (EditMode/PlayMode) with tests under an `Assets/Tests/` folder backed by an `.asmdef`.
- Builds are currently done manually via **File > Build Settings** in the Unity Editor; there is no CLI/batch-mode build script yet. If one is added, document the exact `Unity -batchmode -quit -executeMethod ...` command here.
- Before committing, open the target scene(s) in the Editor and confirm there are no console errors.

## Scene & Asset Workflow

- Avoid concurrent edits to the same `.unity` scene file by multiple people — scenes are hard to merge; use prefabs for shared/reusable objects instead.
- Always commit a file's `.meta` alongside it; never delete or rename an asset without its `.meta` counterpart.
- Addressables: use `Assets/AddressableAssetsData/` groups for any content that should be loaded dynamically rather than referenced directly by scenes.
- Localization: add/update part names and descriptions in `Assets/Localization/Parts/` rather than hardcoding user-facing strings in scripts.

## Do's and Don'ts for AI Agents

- **Keep code DRY** — this is a standing project requirement.
- Don't modify `ProjectSettings/` or `.meta` files unless the change explicitly requires it.
- No Git LFS is configured yet — be cautious committing large binary assets (models, textures); flag it if large files start bloating the repo.
- Prefer minimal, scoped changes; avoid regenerating or reformatting Unity-managed files (scenes, prefabs, `.meta`) as a side effect of unrelated edits.

## PR / Commit Guidelines

- Use descriptive branch names, e.g. `feature/assembly-sequence-grading`, `fix/outline-shader`, `docs/agents-md`.
- Write clear, imperative commit messages (e.g., "Add part highlight on hover").
- In PR descriptions, include: what changed and why, screenshots/recordings for UI or in-XR visual changes, and which headset/device (if any) was used to test.


