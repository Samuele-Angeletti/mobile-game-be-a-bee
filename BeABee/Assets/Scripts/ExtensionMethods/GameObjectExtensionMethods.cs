using UnityEngine;


public static class GameObjectExtensionMethods
{
    public static T SearchComponent<T>(this GameObject gameObject)
    {
        if (!gameObject.TryGetComponent(out T component))
        {
            component = gameObject.GetComponentInChildren<T>();
            component ??= gameObject.GetComponentInParent<T>();
        }

        return component;
    }
}

