using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Putting these in the global namespace so that they can be accessed from anywhere in the game without importing
/// </summary>
public static class UnityExtensions
{
    private static List<Component> componentCache = new List<Component>();

    /// <summary>
    /// A shorter way of testing if a game object has a component
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <param name="obj">The object to check on</param>
    /// <returns></returns>
    public static bool Has<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>() != null;
    }

    /// <summary>
    /// A slightly shorter way to get a component from an object
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <param name="obj">The object to get from</param>
    /// <returns></returns>
    public static T Get<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>();
    }

    /// <summary>
    /// A slightly shorter way to get a component from an object
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <param name="obj">The object to get from</param>
    /// <returns></returns>
    public static T[] GetAll<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponents<T>();
    }

    /// <summary>
    /// Checks to see if the game object has a component, accepts interfaces
    /// </summary>
    /// <typeparam name="T">Type of component to check for (can be an interface)</typeparam>
    /// <returns>Whether the object has the component</returns>
    public static bool HasComponentOrInterface<T>(this GameObject obj) where T : class
    {
        obj.GetComponents<Component>(componentCache);
        return componentCache.OfType<T>().Count() > 0;
    }

    /// <summary>
    /// Gets a component on a game object, accepts interfaces
    /// </summary>
    /// <typeparam name="T">Type of component to check for (can be an interface)</typeparam>
    /// <param name="inObj"></param>
    /// <returns>The component, null if it doesnt exist</returns>
    public static T GetComponentOrInterface<T>(this GameObject obj) where T : class
    {
        obj.GetComponents<Component>(componentCache);
        return componentCache.OfType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Gets all components of a given type, accepts interfaces
    /// </summary>
    /// <typeparam name="T">Type of components to get (accepts interfaces)</typeparam>
    /// <returns>Enumerable of components</returns>
    public static IEnumerable<T> GetAllComponentsOrInterfaces<T>(this GameObject obj) where T : class
    {
        return obj.GetComponents<Component>().OfType<T>();
    }

    /// <summary>
    /// Checks to see if the game object has a component, accepts interfaces
    /// </summary>
    /// <typeparam name="T">Type of component to check for (can be an interface)</typeparam>
    /// <returns>Whether the object has the component</returns>
    public static bool HasComponentOrInterface<T>(this Component component) where T : class
    {
        return HasComponentOrInterface<T>(component.gameObject);
    }

    /// <summary>
    /// Checks to see if the game object this component belongs to has a component, accepts interfaces
    /// </summary>
    /// <typeparam name="T">Type of component to check for (can be an interface)</typeparam>
    /// <returns>Whether the object has the component</returns>
    public static T GetComponentOrInterface<T>(this Component component) where T : class
    {
        return GetComponentOrInterface<T>(component.gameObject);
    }

    /// <summary>
    /// Gets all components of a given type, accepts interfaces
    /// </summary>
    /// <typeparam name="T">Type of components to get (accepts interfaces)</typeparam>
    /// <returns>Enumerable of components</returns>
    public static IEnumerable<T> GetAllComponentsOrInterfaces<T>(this Component component) where T : class
    {
        return GetAllComponentsOrInterfaces<T>(component.gameObject);
    }

    /// <summary>
    /// A shortcut for creating a new game object then adding a component then adding it to a parent object
    /// </summary>
    /// <typeparam name="T">Type of component</typeparam>
    /// <returns>The new component</returns>
    public static T AddChild<T>(this GameObject parent) where T : Component
    {
        return AddChild<T>(parent, typeof(T).Name);
    }

    /// <summary>
    /// A shortcut for creating a new game object then adding a component then adding it to a parent object
    /// </summary>
    /// <typeparam name="T">Type of component</typeparam>
    /// <param name="name">Name of the child game object</param>
    /// <returns>The new component</returns>
    public static T AddChild<T>(this GameObject parent, string name) where T : Component
    {
        var obj = AddChild(parent, name, typeof(T));
        return obj.GetComponent<T>();
    }

    /// <summary>
    /// A shortcut for creating a new game object with a number of components and adding it as a child
    /// </summary>
    /// <param name="components">A list of component types</param>
    /// <returns>The new game object</returns>
    public static GameObject AddChild(this GameObject parent, params Type[] components)
    {
        return AddChild(parent, "Game Object", components);
    }

    /// <summary>
    /// A shortcut for creating a new game object with a number of components and adding it as a child
    /// </summary>
    /// <param name="name">The name of the new game object</param>
    /// <param name="components">A list of component types</param>
    /// <returns>The new game object</returns>
    public static GameObject AddChild(this GameObject parent, string name, params Type[] components)
    {
        var obj = new GameObject(name, components);
        if (parent != null)
        {
            if (obj.transform is RectTransform) obj.transform.SetParent(parent.transform, true);
            else obj.transform.parent = parent.transform;
        }
        return obj;
    }

    /// <summary>
    /// Loads a resource and adds it as a child
    /// </summary>
    /// <param name="resourcePath">The path to the resource to load</param>
    /// <returns></returns>
    public static GameObject LoadChild(this GameObject parent, string resourcePath)
    {
        var obj = (GameObject)GameObject.Instantiate(Resources.Load(resourcePath));
        if (obj != null && parent != null)
        {
            if (obj.transform is RectTransform) obj.transform.SetParent(parent.transform, true);
            else obj.transform.parent = parent.transform;
        }
        return obj;
    }

    /// <summary>
    /// Loads a resource and adds it as a child
    /// </summary>
    /// <param name="resourcePath">The path to the resource to load</param>
    /// <returns></returns>
    public static GameObject LoadChild(this Transform parent, string resourcePath)
    {
        var obj = (GameObject)GameObject.Instantiate(Resources.Load(resourcePath));
        if (obj != null && parent != null)
        {
            if (obj.transform is RectTransform) obj.transform.SetParent(parent, true);
            else obj.transform.parent = parent;
        }
        return obj;
    }

    /// <summary>
    /// Destroys all the children of a given gameobject
    /// </summary>
    /// <param name="obj">The parent game object</param>
    public static void DestroyAllChildrenImmediately(this GameObject obj)
    {
        DestroyAllChildrenImmediately(obj.transform);
    }

    /// <summary>
    /// Destroys all the children of a given transform
    /// </summary>
    /// <param name="obj">The parent transform</param>
    public static void DestroyAllChildrenImmediately(this Transform trans)
    {
        while (trans.childCount != 0)
            GameObject.DestroyImmediate(trans.GetChild(0).gameObject);
    }

    /// <summary>
    /// Focuses the camera on a point in 2D space (just transforms the x and y to match the target)
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="gameObject"></param>
    public static void FocusOn2D(this Camera camera, GameObject target)
    {
        camera.transform.position = new Vector3(target.transform.localPosition.x, target.transform.localPosition.y, Camera.main.transform.position.z);
    }

    /// <summary>
    /// Converts a uint to a color
    /// </summary>
    /// <param name="color">The uint to convert</param>
    /// <returns></returns>
    public static uint ToUInt(this Color color)
    {
        Color32 c32 = color;
        return (uint)((c32.a << 24) | (c32.r << 16) |
                      (c32.g << 8) | (c32.b << 0));
    }

    /// <summary>
    /// Converts a color to a uint
    /// </summary>
    /// <param name="color">The uint to convert</param>
    /// <returns></returns>
    public static Color ToColor(this uint color)
    {
        return new Color32()
        {
            a = (byte)(color >> 24),
            r = (byte)(color >> 16),
            g = (byte)(color >> 8),
            b = (byte)(color >> 0)
        };
    }

    /// <summary>
    /// Converts a timespan to a readable format
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public static string ToReadableString(this TimeSpan span)
    {
        string formatted = string.Format("{0}{1}{2}{3}",
            span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
            span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
            span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
            span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

        if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

        if (string.IsNullOrEmpty(formatted)) formatted = "0 seconds";

        return formatted;
    }

    /// <summary>
    /// Randomly picks one elements from the enumerable
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="items">The items to random from</param>
    /// <returns></returns>
    public static T RandomOne<T>(this IEnumerable<T> items)
    {
        if (items == null) throw new ArgumentException("Cannot randomly pick an item from the list, the list is null!");
        if (items.Count() == 0) throw new ArgumentException("Cannot randomly pick an item from the list, there are no items in the list!");
        var r = UnityEngine.Random.Range(0, items.Count());
        return items.ElementAt(r);
    }

    /// <summary>
    /// Loops over each item in the list and logs out a particular value, handy for debugging!
    /// </summary>
    /// <typeparam name="T">The type of the item</typeparam>
    /// <param name="items">The list of items</param>
    /// <param name="objectToLog">A callback in which you will return the item to log</param>
    /// <returns>The original input list</returns>
    public static IEnumerable<T> DebugLog<T>(this IEnumerable<T> items, Func<T,object> objectToLog)
    {
        var count = 0;
        foreach (var item in items)
        {
            var obj = objectToLog(item);
            Debug.Log(String.Format("Item[{0}]: {1}", count, obj));
        }
        return items;
    }
}  