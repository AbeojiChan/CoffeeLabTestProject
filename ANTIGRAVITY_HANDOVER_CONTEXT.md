# 🚀 Coffee Lab - Antigravity Session Handover & Context

This document contains the complete context, architectural state, design decisions, and active task status for continuing the **Coffee Lab** project session seamlessly on another computer.

---

## 📌 Project & Repository Overview

- **Repository**: [https://github.com/AbeojiChan/CoffeeLabTestProject.git](https://github.com/AbeojiChan/CoffeeLabTestProject.git) (`main` branch)
- **Engine**: Unity 6 (6000.3.13f1) / Universal Render Pipeline (URP)
- **Primary Goal**: Transitioning from procedural UI prototyping to a **physical, GameObject-oriented / diegetic 3D workbench gameplay experience**.

---

## 🛠️ Strict Project Coding Standards

When generating or editing code in this repository:
1. **Field Naming**:
   - `public` fields must start with `m_` (e.g. `m_dosedIngredients`, `m_driftSpeed`).
   - `private` / `protected` fields must start with `_` (e.g. `_sfxSource`, `_beakerZone`).
2. **Exact 5 `#region` Blocks in Order**:
   Every class must strictly contain these 5 region tags in exact order:
   ```csharp
   #region Publics
   #endregion

   #region Unity API
   #endregion

   #region Main API
   #endregion

   #region Tools and Utilities
   #endregion

   #region Private and Protected
   #endregion
   ```

---

## 🧠 What Was Accomplished (Current State)

1. **Complete Chemistry & Formulation Engine (`Assets/_/CODE/GAME/MolecularBrewing/Data/` & `Core/`)**:
   - `MoleculeType.cs`: Chemical definitions (Caffeine, L-Theanine, Chlorogenic Acid, Trigonelline, BioFlavonoids, etc.) with bio-stat deltas and colors.
   - `RawIngredientData.cs`: Unprocessed raw pantry botanicals requiring specific apparatus tools.
   - `IngredientData.cs`: Refined ingredients yielding molecule clusters.
   - `DrinkRecipePattern.cs`: Blueprint targets with required covalent bonds, target stats, and hints.
   - `DrinkEvaluator.cs`: Mathematical chemistry engine computing stats, accuracy (0-100), letter rank (S/A/B/C/D), special effects, and barista critique notes.

2. **Interactive Reaction Crucible (`UIMolecularWorkspaceScreen.cs`, `UIMoleculeNode.cs`, `UILinkRenderer.cs`)**:
   - **Brownian Motion**: Continuous floating drift of molecule nodes.
   - **Cryogenic Freeze**: Toggle button slows drift to `0.035x` for precise manual link placement.
   - **Covalent Bond Snapping**: Right-click drag draws bonds; clicking severs bonds.
   - **Interference Line Collision**: If a link crosses another bond (>50% line intersection), the conflicting bond automatically snaps (`SFX_Break`).

3. **Audio Suite & BGM Routing (`AudioManager.cs`)**:
   - Looping SFX for tools: `SFX_Coffee_Grind.mp3`, `SFX_Leaves_drying.mp3`, `SFX_Milk_Froth.mp3`, `SFX_Solvent.mp3`.
   - Interaction SFX: `SFX_Bond.mp3`, `SFX_Break.mp3`, `SFX_Synthesis.mp3`, `SFX_Metro.mp3`, `SFX_Menu_Nav.mp3`.
   - Dynamic Crossfading BGM: `BGM_Game.wav` (Prep/Synthesis), `BGM_Dialog.mp3` (Narrative), `BGM_City.wav` (Map & Shop) with calibrated volume (0.35f).

4. **GameObject-Oriented HUD Architecture & Tools**:
   - All 8 UI screens refactored with serialized `[SerializeField]` fields for full Inspector/Hierarchy editing with non-destructive procedural fallback.
   - `UIGameObjectHUDBaker.cs`: Editor tool under `Tools > Molecular Brewing > Build GameObject HUD Scene & Prefabs` that can automatically generate complete hierarchies and bake Prefabs.

5. **Physical Prototype Scenes Added (`Assets/Scenes/`)**:
   - `_Proto_Ingredients_Preparation.unity`: Prototype for physical tool interactions (`GrinderMiniGame.cs`, `DraggableItem.cs`).
   - `_Proto_Ingredients_Synthesis.unity`: Prototype for reaction crucible.
   - `MolecularBrewing_GameObjectHUD.unity`: Canvas/GameObject HUD scene.

---

## 🎯 Current Vision & Next Objective

**User Direction for Final Version**:
> *"The final version will NOT be a flat UI — you have physical in-world tools (grinder, botanical mortar, milk frother, solvent kettle, measuring beaker, synthesizer crucible) on a 3D workbench. We drag unlocked raw ingredients directly into the physical tools, hold/process them to refine, collect them into the measuring beaker, and drop the beaker into the synthesizer crucible."*

### Key Implementation Mapping:
- **Physical Tools**: 3D GameObjects with colliders / drop zones mapping to `RawIngredientData.m_requiredApparatus` and looping audio via `AudioManager`.
- **Measuring Beaker**: 3D vessel tracking `m_dosedIngredients` (up to 7 doses) that can be picked up and docked onto the Synthesizer.
- **Synthesizer Chamber**: Visual reaction crucible rendering `UIMoleculeNode` and `UILinkRenderer` inside its ocular lens, evaluating concoctions via `DrinkEvaluator.EvaluateConcoction()`.

---

## 📋 Instructions for the AI on the Work Laptop

When you open Antigravity on your work laptop:
1. Clone / pull the repository: `git pull origin main`.
2. Start a new chat and paste:
   > *"I am continuing the Coffee Lab project. Please read `ANTIGRAVITY_HANDOVER_CONTEXT.md` in the project root to load full architectural context, coding conventions, and current objectives."*
3. The assistant will immediately have 100% of the context, conventions, and design direction ready to continue.
