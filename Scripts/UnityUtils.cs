using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityHelpers
{
    public class UnityUtils
    {
        public static List<SpriteRenderer> GetSpriteRenderersUnderScreenPoint(Vector2 point)
        {
            var ray = Camera.main.ScreenPointToRay(point);
            var hits = new List<RaycastHit2D>(Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity));
            var allRenderers = hits.ConvertAll(hit => hit.transform.gameObject.GetComponent<SpriteRenderer>());
            allRenderers.RemoveAll(r => r == null);

            var layers = new Dictionary<int, List<SpriteRenderer>>();
            allRenderers.ForEach(r =>
            {
                if (!layers.ContainsKey(r.sortingLayerID)) layers[r.sortingLayerID] = new List<SpriteRenderer>();
                layers[r.sortingLayerID].Add(r);
            });

            var sortedRenderers = new List<SpriteRenderer>();
            var sortedLayers = new List<int>(layers.Keys);
            sortedLayers.Sort((a, b) => b - a);
            sortedLayers.ForEach(key => layers[key].Sort((a, b) => b.sortingOrder - a.sortingOrder));
            sortedLayers.ForEach(key => sortedRenderers.AddRange(layers[key]));

            return sortedRenderers;
        }

        public static GameObject GetObjectUnderScreenPoint(Vector2 screenPos)
        {
            var r = GetSpriteRenderersUnderScreenPoint(screenPos);
            if (r.Count == 0) return null;
            return r[0].gameObject;
        }

        public static Texture2D MakeTexture2D(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }


        /// <summary>
        /// Copies a component using reflection into a destination. 
        /// Borrowed from: http://answers.unity3d.com/questions/458207/copy-a-component-at-runtime.html
        /// </summary>
        /// <param name="original">The original component to copy from</param>
        /// <param name="destination">The destination object to copy the componet to</param>
        /// <param name="illegalDeclaringTypes">A List of declaring types from which the property or field should not be copied from</param>
        /// <returns></returns>
        public static Component CopyComponent(Component original, GameObject destination, List<Type> illegalDeclaringTypes)
        {
            var type = original.GetType();
            var copy = destination.AddComponent(type);

            foreach (var field in type.GetFields())
            {
                if (illegalDeclaringTypes.Contains(field.DeclaringType)) continue;     
                field.SetValue(copy, field.GetValue(original));
            }

            foreach (var property in type.GetProperties())
            {
                if (property == null) continue;
                if (!property.CanWrite) continue;
                if(illegalDeclaringTypes.Contains(property.DeclaringType)) continue;               
                property.SetValue(copy, property.GetValue(original, null), null);
            }

            return copy;
        }
    }
}
