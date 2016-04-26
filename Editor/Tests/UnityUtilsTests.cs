using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditorInternal;
using UnityEngine;

namespace UnityHelpers.Tests
{
    public class UnityUtilsTests
    {
        [SetUp]
        public void Init()
        {

        }

        [Test]
        public void NothingUnderPointReturnsEmptyList()
        {
            Assert.AreEqual(0, UnityUtils.GetSpriteRenderersUnderScreenPoint(new Vector2(0, 0)).Count);
        }

        [Test]
        public void DoesNotReturnAnyObjectsWithoutASpriteRenderer()
        {
            var obj = new GameObject();
            obj.transform.localPosition = new Vector3(-10, -10, 0);
            obj.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);            

            Camera.main.transform.position = new Vector3(0, 0, -10);
            var renderers = UnityUtils.GetSpriteRenderersUnderScreenPoint(new Vector2(0, 0));
            renderers.RemoveAll(r => r==null);

            Assert.AreEqual(0, renderers.Count);
        }

        [Test]
        public void DoesNotReturnARendererNotUnderThePoint()
        {
            var obj = new GameObject();
            obj.transform.localPosition = new Vector3(-1000, -1000, 0);
            obj.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
            obj.AddComponent<SpriteRenderer>().sortingOrder = 10;

            Camera.main.transform.position = new Vector3(0, 0, -10);
            var renderers = UnityUtils.GetSpriteRenderersUnderScreenPoint(new Vector2(0, 0));
            Assert.AreEqual(0, renderers.Count);
        }

        [Test]
        public void ReturnsRenderersCorrectlySortedBySortingOrder()
        {
            var obj1 = new GameObject();
            obj1.transform.localPosition = new Vector3(-10, -10, 0);
            obj1.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
            obj1.AddComponent<SpriteRenderer>().sortingOrder = 10;

            var obj2 = new GameObject();
            obj2.transform.localPosition = new Vector3(-10, -10, 0);
            obj2.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
            obj2.AddComponent<SpriteRenderer>().sortingOrder = 3;

            Camera.main.transform.position = new Vector3(0, 0, -10);
            var renderers = UnityUtils.GetSpriteRenderersUnderScreenPoint(new Vector2(0, 0));

            Assert.AreEqual(2, renderers.Count);
            Assert.AreEqual(obj1, renderers[0].gameObject);
            Assert.AreEqual(obj2, renderers[1].gameObject);
        }

        //[Test]
        //public void ReturnsRenderersCorrectlySortedBySortingLayer()
        //{
        //    var sortingLayerNames = GetSortingLayerNames();
        //    if (sortingLayerNames.Length < 2) throw new Exception("Cannot run test, not enough sorting layer names!");

        //    var obj1 = CreateGameObject("obj1");
        //    obj1.transform.localPosition = new Vector3(-10, -10, 0);
        //    obj1.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
        //    obj1.AddComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[0];

        //    var obj2 = CreateGameObject("obj2");
        //    obj2.transform.localPosition = new Vector3(-10, -10, 0);
        //    obj2.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
        //    obj2.AddComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[1];

        //    Camera.main.transform.position = new Vector3(0, 0, -10);
        //    var renderers = UnityUtils.GetSpriteRenderersUnderScreenPoint(new Vector2(0, 0));

        //    Assert.AreEqual(2, renderers.Count);
        //    Assert.AreEqual(obj2, renderers[0].gameObject);
        //    Assert.AreEqual(obj1, renderers[1].gameObject);
        //}

        //[Test]
        //public void ReturnsRenderersCorrectlySortedBySortingLayerAndSortingOrder()
        //{
        //    var sortingLayerNames = GetSortingLayerNames();
        //    if (sortingLayerNames.Length < 2) throw new Exception("Cannot run test, not enough sorting layer names!");

        //    var obj1a = CreateGameObject("obj1a");
        //    obj1a.transform.localPosition = new Vector3(-10, -10, 0);
        //    obj1a.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
        //    obj1a.AddComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[0];
        //    obj1a.GetComponent<SpriteRenderer>().sortingOrder = 0;

        //    var obj1b = CreateGameObject("obj1b");
        //    obj1b.transform.localPosition = new Vector3(-10, -10, 0);
        //    obj1b.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
        //    obj1b.AddComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[0];
        //    obj1b.GetComponent<SpriteRenderer>().sortingOrder = 2;

        //    var obj2a = CreateGameObject("obj2a");
        //    obj2a.transform.localPosition = new Vector3(-10, -10, 0);
        //    obj2a.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
        //    obj2a.AddComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[1];
        //    obj2a.GetComponent<SpriteRenderer>().sortingOrder = 2;

        //    var obj2b = CreateGameObject("obj2a");
        //    obj2b.transform.localPosition = new Vector3(-10, -10, 0);
        //    obj2b.AddComponent<BoxCollider2D>().size = new Vector2(100, 100);
        //    obj2b.AddComponent<SpriteRenderer>().sortingLayerName = sortingLayerNames[1];
        //    obj2b.GetComponent<SpriteRenderer>().sortingOrder = 0;

        //    Camera.main.transform.position = new Vector3(0, 0, -10);
        //    var renderers = UnityUtils.GetSpriteRenderersUnderScreenPoint(new Vector2(0, 0));

        //    Assert.AreEqual(4, renderers.Count);
        //    Assert.AreEqual(obj2a, renderers[0].gameObject);
        //    Assert.AreEqual(obj2b, renderers[1].gameObject);
        //    Assert.AreEqual(obj1b, renderers[2].gameObject);
        //    Assert.AreEqual(obj1a, renderers[3].gameObject);
        //}

        // Get the sorting layer names
        public string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        // Get the unique sorting layer IDs -- tossed this in for good measure
        public int[] GetSortingLayerUniqueIDs()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);

        }
    }
}
