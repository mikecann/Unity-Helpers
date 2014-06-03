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

        public static GameObject InstantiateResource(GameObject parent, string resource)
        {
            var obj = (GameObject)GameObject.Instantiate(Resources.Load(resource));
            if (parent != null) obj.transform.parent = parent.transform;
            return obj;
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }               

        public static GameObject GetObjectUnderScreenPoint(Vector2 screenPos)
        {
            var r = GetSpriteRenderersUnderScreenPoint(screenPos);
            if (r.Count == 0) return null;
            return r[0].gameObject;
        }
    }
}
