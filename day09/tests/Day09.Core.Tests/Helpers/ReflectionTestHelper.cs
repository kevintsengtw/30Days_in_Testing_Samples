using System;

namespace Day09.Core.Tests.Helpers;

/// <summary>
/// 反射測試輔助類別
/// </summary>
public static class ReflectionTestHelper
{
    /// <summary>
    /// 呼叫私有實例方法
    /// </summary>
    /// <typeparam name="T">回傳型別</typeparam>
    /// <param name="instance">物件實例</param>
    /// <param name="methodName">方法名稱</param>
    /// <param name="parameters">參數</param>
    /// <returns>方法執行結果</returns>
    public static T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (method is null)
        {
            throw new ArgumentException($"Method '{methodName}' not found in type '{type.Name}'");
        }

        var result = method.Invoke(instance, parameters);
        return result is null ? default! : (T)result;
    }

    /// <summary>
    /// 呼叫私有靜態方法
    /// </summary>
    /// <typeparam name="T">回傳型別</typeparam>
    /// <param name="type">型別</param>
    /// <param name="methodName">方法名稱</param>
    /// <param name="parameters">參數</param>
    /// <returns>方法執行結果</returns>
    public static T InvokePrivateStaticMethod<T>(Type type, string methodName, params object[] parameters)
    {
        var method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);

        if (method is null)
        {
            throw new ArgumentException($"Static method '{methodName}' not found in type '{type.Name}'");
        }

        var result = method.Invoke(null, parameters);
        return result is null ? default! : (T)result;
    }
}