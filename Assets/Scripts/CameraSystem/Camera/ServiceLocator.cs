using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    static readonly Dictionary<Type, object> services = new();

    public static bool Register<T>(T service, bool replace = false) where T : class
        => Register(typeof(T), service, replace);

    public static bool Register(Type serviceType, object service, bool replace = false)
    {
        if (serviceType == null || service == null)
        {
            Debug.LogError("[ServiceLocator] 등록 실패: serviceType 또는 service가 null입니다.");
            return false;
        }

        if (!serviceType.IsInterface)
            Debug.LogWarning($"[ServiceLocator] {serviceType.Name}은 인터페이스 타입이 아닙니다.");

        if (!serviceType.IsInstanceOfType(service))
        {
            Debug.LogError($"[ServiceLocator] {service.GetType().Name}은 {serviceType.Name}을 구현하지 않았습니다.");
            return false;
        }

        if (services.TryGetValue(serviceType, out var oldService) && IsAlive(oldService))
        {
            if (ReferenceEquals(oldService, service)) return true;

            if (!replace)
            {
                Debug.LogWarning($"[ServiceLocator] {serviceType.Name} 등록 실패: 이미 등록된 서비스가 있습니다.");
                return false;
            }

            Debug.LogWarning($"[ServiceLocator] {serviceType.Name} 서비스가 교체되었습니다.");
        }

        services[serviceType] = service;
        return true;
    }

    public static bool Unregister<T>(T service = null) where T : class
        => Unregister(typeof(T), service);

    public static bool Unregister(Type serviceType, object service = null)
    {
        if (serviceType == null) return false;
        if (!services.TryGetValue(serviceType, out var oldService)) return false;

        if (service != null && !ReferenceEquals(oldService, service))
            return false;

        services.Remove(serviceType);
        return true;
    }

    public static bool TryGet<T>(out T service) where T : class
    {
        Type type = typeof(T);

        if (services.TryGetValue(type, out var rawService))
        {
            if (IsAlive(rawService) && rawService is T typedService)
            {
                service = typedService;
                return true;
            }

            services.Remove(type);
        }

        service = null;
        return false;
    }

    public static T Get<T>() where T : class
        => TryGet<T>(out var service)
            ? service
            : throw new Exception($"[ServiceLocator] {typeof(T).Name} 서비스가 등록되지 않았습니다.");

    public static void Clear() => services.Clear();

    static bool IsAlive(object obj)
        => obj != null && (obj is not UnityEngine.Object unityObj || unityObj != null);
}