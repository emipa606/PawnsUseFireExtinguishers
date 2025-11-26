# GitHub Copilot Instructions for Pawns Use Fire Extinguishers (Continued)

## Mod Overview and Purpose
The "Pawns Use Fire Extinguishers (Continued)" mod allows pawns in RimWorld to automatically use fire extinguishers when combating fires. The mod serves to enhance the firefighting mechanics by enabling pawns to equip and use fire extinguishing weapons before defaulting back to the vanilla method of beating out fires with their hands. It essentially integrates firefighting tools into the game's workflow, making firefighting more efficient and realistic.

## Key Features and Systems
- **Automatic Fire Extinguisher Use:** Pawns will equip fire extinguishers found in their inventory and use them to put out fires. If no extinguisher is available, they resort to the vanilla method.
- **Compatibility with Other Mods:** Works automatically with any mod using the vanilla "Extinguish" damage definition. Known compatible mods include "Firefoam Things", "Vanilla Weapons Expanded", and "Fire Extinguisher (Continued)".
- **Integration with Weapon/Tool Mods:** Encouraged but not required. Supports mods like "Simple Sidearms", "Grab Your Tool!", and "Combat Extended" for enhanced gameplay features.
- **Ammo System Support:** For mods like "Combat Extended", the system recognizes if a fire extinguisher lacks ammunition, only allowing usage if properly loaded with fire extinguishing mediums like Firefoam.

## Coding Patterns and Conventions
- **Naming:** 
  - Classes typically have a clear, descriptive name, suffixed with their role (e.g., `JobDriver`, `Utils`).
  - Methods are named using PascalCase, reflecting their specific action or role.
- **Access Modifiers:** 
  - Static classes like `Compatibility`, `HarmonyPatches`, and `ModCompatibility` are often marked as `internal` to logically partition functionality.
- **Modularity:** 
  - Utilities and compatibility checks are separated into distinct classes/files (e.g., `CastUtils.cs`, `InventoryUtils.cs`).

## XML Integration
The mod interacts with the game's XML modding ecosystem using custom job definitions and work givers. This includes:
- **Job Definitions:** Declared in `JobDefOf_ExtinguishFire.cs` to integrate seamlessly with the pawn's job system.
- **Work Givers:** Detailed in `WorkGiver_ExtinguishFire.cs`, instructing pawns on when and how to equip and use fire extinguishers.

## Harmony Patching
- The mod uses Harmony to intercept and modify methods at runtime, ensuring compatibility and integration with the vanilla game without direct source modification.
- **HarmonyPatches.cs:** This file encapsulates the method patches necessary for overriding base game functionality where pawns decide how to fight fires.

## Suggestions for Copilot
- **Automated Testing Hooks:** Include automated tests for different scenarios (e.g., with fire extinguishers, without fire extinguishers, with varying mod compatibilities).
- **Enhanced Error Handling:** Implement comprehensive logging within Harmony patches to assist with debugging and compatibility reporting.
- **Dynamic Compatibility Checking:** Allow dynamic registration of compatible mods based on their usage of the extinguish damage definition.
- **Performance Optimizations:** Consider performance profiling during heavy in-game firefighting events to minimize lag.

This document should serve as a guide for enhancing and developing the "Pawns Use Fire Extinguishers (Continued)" mod on GitHub using Copilot as an auxiliary tool. For bug reports or further assistance, please utilize the specified communication channels such as GitHub issues, logs via the Log Uploader, or Discord for error-reporting.
