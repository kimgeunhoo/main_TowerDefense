using System;
using UnityEngine;

[Serializable]
public class InterfaceReference<TInterface> where TInterface : class
{
    [SerializeField] UnityEngine.Object target;

    public UnityEngine.Object Target => target;

    public TInterface Value
    {
        get
        {
            if (target == null) return null;

            if (target is TInterface direct) return direct;

            if (target is GameObject go) return go.GetComponent<TInterface>();

            if (target is Component component)
            {
                if (component is TInterface componentInterface)
                    return componentInterface;

                return component.GetComponent<TInterface>();
            }

            return null;
        }
    }

    public bool HasValue => Value != null;

    public bool TryGet(out TInterface value)
    {
        value = Value;
        return value != null;
    }

    public void SetTarget(UnityEngine.Object newTarget)
    {
        target = newTarget;
    }
}