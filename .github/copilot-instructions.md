# GitHub Copilot Instructions for Pawns Use Fire Extinguishers (Continued) Mod

## Mod Overview and Purpose
The **Pawns Use Fire Extinguishers (Continued)** mod is an update to the original mod created by pointfeev. This mod enhances the firefighting mechanics in RimWorld by allowing pawns to automatically use fire extinguishers if they have one in their inventory. If a pawn doesn't have a fire extinguisher or if the fire can't be accessed with one, they will resort to the vanilla method of beating out fires with their hands. This mod aims to provide an automated way for pawns to handle fires, improving the overall firefighting efficiency in your RimWorld colony.

## Key Features and Systems
### Features
- **Automatic Fire Extinguisher Use**: Pawns will automatically use fire extinguishers if they are equipped and available.
- **Compatibility**: Works with fire extinguishing weapons from various mods that use the vanilla Extinguish damage definition.
- **Weapon Reloading**: Supports Combat Extended's ammo system, allowing pawns to reload fire extinguishers when necessary.

### Systems
- **Inventory Check**: Checks if a pawn has a fire extinguisher in their inventory.
- **Fire Accessibility**: Determines if the pawn can reach and use a fire with a fire extinguisher.
- **Job System**: Modifies the job system to allow pawns to use fire extinguishers as a means of firefighting.
- **Compatibility Layer**: Ensures compatibility with other mods that may alter or add fire extinguishing weapons.

## Coding Patterns and Conventions
### File Structure
- **C# Files**: Located in the `Source\FireExtinguisher` and `Source\CompatUtils` directories.
  - **FireExtinguisher**: Contains classes related to the fire extinguisher functionality.
  - **CompatUtils**: Contains compatibility utility classes for other mods.
- **XML Files**: Located in the `Source\.cr\personal\FavoritesList`, `Source`, and `Source\TranslationTemplate` directories.
  - **FavoritesList**: Contains personal favorites configurations.
  - **About.xml**: Contains mod metadata and dependencies.
  - **TranslationTemplate**: Contains job and work giver definitions for translation purposes.

### Naming Conventions
- **Classes**: PascalCase, e.g., `FireExtinguisher`, `Compatibility`.
- **Methods**: PascalCase, e.g., `CanGotoCastPosition`, `Prefix`.
- **Variables**: camelCase, e.g., `castPosition`, `jobTracker`.

### Code Quality
- **Modular Design**: Each class and method should have a clear and specific purpose.
- **Comments**: Use comments to explain complex logic or important decisions.
- **Error Handling**: Implement error handling where necessary to ensure the mod doesn't crash or cause unexpected behavior.

## XML Integration
The mod uses XML files to define and configure various aspects of the mod, such as job definitions and work giver definitions. Here are some examples of how XML is integrated into the mod:

### Example: `About.xml`
xml
<About>
    <PackageId>Mlie.PawnsUseFireExtinguishers</PackageId>
    <Name>Pawns Use Fire Extinguishers (Continued)</Name>
    <Author>pointfeev</Author>
    <Description>Update of pointfeevs mod. Allows fire extinguishers to be used for firefighting automatically.</Description>
    <Dependencies>
        <Dependency>brrainz.harmony</Dependency>
    </Dependencies>
</About>


### Example: `ExtinguishFire.xml`
xml
<JobDef>
    <defName>ExtinguishFire</defName>
    <label>Extinguish Fire</label>
    <description>Use a fire extinguisher to put out a fire.</description>
</JobDef>


### Example: `ExtinguishFire.xml` (WorkGiverDef)
xml
<WorkGiverDef>
    <defName>ExtinguishFire</defName>
    <label>Extinguish Fire</label>
    <description>Work giver for extinguishing fires using fire extinguishers.</description>
    <prioritization>ByPriority</prioritization>
    <workType>Utility</workType>
</WorkGiverDef>


## Harmony Patching
The mod uses Harmony patching to modify vanilla behavior. Here are some examples of Harmony patches used in the mod:

### Example: `Pawn_JobTracker_EndCurrentJob.cs`
csharp
using HarmonyLib;

namespace PawnsUseFireExtinguishers
{
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob))]
    public static class Pawn_JobTracker_EndCurrentJob
    {
        public static void Prefix(Pawn_JobTracker __instance, bool forced = false, bool canReturnToPool = true)
        {
            // Custom logic to handle job ending
        }
    }
}


### Example: `CastUtils.cs`
csharp
using HarmonyLib;

namespace PawnsUseFireExtinguishers
{
    [HarmonyPatch(typeof(CastVerb), nameof(CastVerb.Cast))]
    public static class CastUtils
    {
        public static bool Prefix(CastVerb __instance, ref bool __result)
        {
            // Custom logic to handle casting
            return true;
        }
    }
}


## Suggestions for Copilot
- **Mod Compatibility**: Focus on compatibility with other mods that may add or alter fire extinguishing weapons.
- **Error Handling**: Implement robust error handling to prevent the mod from causing crashes or unexpected behavior.
- **Translation**: Ensure that all text and descriptions are properly translated and localized for different languages.
- **Performance**: Optimize the mod to ensure it doesn't significantly impact the performance of the game.

By following these guidelines and utilizing GitHub Copilot effectively, you can contribute to the development and improvement of the **Pawns Use Fire Extinguishers (Continued)** mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

