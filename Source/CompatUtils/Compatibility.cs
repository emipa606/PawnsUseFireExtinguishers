using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace CompatUtils;

public static class Compatibility
{
    private static MethodInfo GetConsistentMethod(
        string modPackageId,
        MethodInfo methodInfo,
        Type[] correctMethodTypes,
        bool logError = false,
        string methodNameForLog = null)
    {
        if (!IsModActive(modPackageId))
        {
            return null;
        }

        if ((object)methodInfo != null)
        {
            return !IsMethodConsistent(methodInfo, correctMethodTypes, logError, modPackageId) ? null : methodInfo;
        }

        var modName = GetModName(modPackageId);
        if (!logError || modName == null)
        {
            return null;
        }

        Log.Error(
            $"Failed to support {modName}: Couldn't get {(methodNameForLog == null ? "an unknown method" : $"method {methodNameForLog}")}!");
        return null;
    }

    public static MethodInfo GetConsistentMethod(
        string modPackageId,
        string className,
        string methodName,
        Type[] correctMethodTypes,
        bool logError = false)
    {
        return GetConsistentMethod(
            modPackageId,
            GetMethod(className, methodName),
            correctMethodTypes,
            logError,
            $"{className}.{methodName}");
    }

    private static MethodInfo GetMethod(
        string className,
        string methodName,
        Type[] parameters = null,
        Type[] generics = null)
    {
        return AccessTools.Method(AccessTools.TypeByName(className), methodName, parameters, generics);
    }

    private static string GetModName(string modPackageId)
    {
        if (modPackageId != null)
        {
            return ModLister.AllInstalledMods
                .First(x => x.Active && x.PackageId.ToLower() == modPackageId.ToLower())?.Name;
        }

        return null;
    }

    private static bool IsMethodConsistent(
        MethodInfo methodInfo,
        Type[] correctMethodTypes,
        bool logError = false,
        string modPackageIdForLog = null)
    {
        if ((object)methodInfo == null)
        {
            return false;
        }

        var array = (from pi in methodInfo.GetParameters()
            select !pi.ParameterType.IsByRef ? pi.ParameterType : pi.ParameterType.GetElementType()).ToArray();
        if (array.Length != correctMethodTypes.Length)
        {
            if (logError)
            {
                Log.Error(
                    $"{(modPackageIdForLog == null ? "Failed to support a mod: " : $"Failed to support {GetModName(modPackageIdForLog)}: ")}Inconsistent number of parameters for method '{((object)methodInfo.ReflectedType == null ? string.Empty : $"{methodInfo.ReflectedType.FullName}.")}{methodInfo.Name}'");
            }

            return false;
        }

        for (var num = 0; num < array.Length; num++)
        {
            if (array[num] == correctMethodTypes[num])
            {
                continue;
            }

            if (logError)
            {
                Log.Error(
                    $"{(modPackageIdForLog == null ? "Failed to support a mod: " : $"Failed to support {GetModName(modPackageIdForLog)}: ")}Inconsistent parameter {num + 1} for method '{((object)methodInfo.ReflectedType == null ? string.Empty : $"{methodInfo.ReflectedType.FullName}.")}{methodInfo.Name}'\n    {array[num]} != {correctMethodTypes[num]}");
            }

            return false;
        }

        return true;
    }

    private static bool IsModActive(string modPackageId)
    {
        if (modPackageId != null)
        {
            return ModLister.AllInstalledMods
                .Any(x => x.Active && x.PackageId.ToLower() == modPackageId.ToLower());
        }

        return false;
    }
}