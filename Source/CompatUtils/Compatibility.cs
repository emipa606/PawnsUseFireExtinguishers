using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
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
        if(!IsModActive(modPackageId))
        {
            return null;
        }
        if((object)methodInfo == null)
        {
            string modName = GetModName(modPackageId);
            if(logError && modName != null)
            {
                Log.Error(
                    $"Failed to support {modName}: Couldn't get {(((methodNameForLog == null) ? "an unknown method" : ($"method {methodNameForLog}")))}!");
                return null;
            }
            return null;
        }
        if(!IsMethodConsistent(methodInfo, correctMethodTypes, logError, modPackageId))
        {
            return null;
        }
        return methodInfo;
    }

    public static MethodInfo GetConsistentMethod(
        string modPackageId,
        string typeColonMethodName,
        Type[] correctMethodTypes,
        bool logError = false)
    {
        return GetConsistentMethod(
            modPackageId,
            GetMethod(typeColonMethodName),
            correctMethodTypes,
            logError,
            typeColonMethodName);
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

    public static MethodInfo GetMethod(string typeColonMethodName, Type[] parameters = null, Type[] generics = null)
    { return AccessTools.Method(typeColonMethodName, parameters, generics); }

    public static MethodInfo GetMethod(
        string className,
        string methodName,
        Type[] parameters = null,
        Type[] generics = null)
    { return AccessTools.Method(AccessTools.TypeByName(className), methodName, parameters, generics); }

    public static string GetModName(string modPackageId)
    {
        if(modPackageId != null)
        {
            return ModLister.AllInstalledMods
                .Where((ModMetaData x) => x.Active && x.PackageId.ToLower() == modPackageId.ToLower())?.First()?.Name;
        }
        return null;
    }

    public static bool IsMethodConsistent(
        MethodInfo methodInfo,
        Type[] correctMethodTypes,
        bool logError = false,
        string modPackageIdForLog = null)
    {
        if((object)methodInfo == null)
        {
            return false;
        }
        Type[] array = (from pi in methodInfo.GetParameters()
            select (!pi.ParameterType.IsByRef) ? pi.ParameterType : pi.ParameterType.GetElementType()).ToArray();
        if(array.Length != correctMethodTypes.Length)
        {
            if(logError)
            {
                Log.Error(
                    $"{(((modPackageIdForLog == null) ? "Failed to support a mod: " : ($"Failed to support {GetModName(modPackageIdForLog)}: ")))}Inconsistent number of parameters for method '{((((object)methodInfo.ReflectedType == null) ? string.Empty : ($"{methodInfo.ReflectedType.FullName}.")))}{methodInfo.Name}'");
            }
            return false;
        }
        for(int num = 0; num < array.Length; num++)
        {
            if(array[num] != correctMethodTypes[num])
            {
                if(logError)
                {
                    Log.Error(
                        $"{(((modPackageIdForLog == null) ? "Failed to support a mod: " : ($"Failed to support {GetModName(modPackageIdForLog)}: ")))}{($"Inconsistent parameter {num + 1} for method '{($"{((((object)methodInfo.ReflectedType == null) ? string.Empty : ($"{methodInfo.ReflectedType.FullName}.")))}{methodInfo.Name}")}'")}\n    {array[num]?.ToString()} != {correctMethodTypes[num]}");
                }
                return false;
            }
        }
        return true;
    }

    public static bool IsModActive(string modPackageId)
    {
        if(modPackageId != null)
        {
            return ModLister.AllInstalledMods
                .Any((ModMetaData x) => x.Active && x.PackageId.ToLower() == modPackageId.ToLower());
        }
        return false;
    }
}
