using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityHelpers;

namespace UnityHelpers.Tests
{    
    [TestFixture]
    public class UnityExtensionsTests
    {
        interface IMockComponent { }
        class MockComponent : MonoBehaviour, IMockComponent { }
        class AnotherMockComponent : MonoBehaviour, IMockComponent { }

        [Test]
        public void HasComponentFalseWhenNoComponentExists()
        {
            var obj = new GameObject();
            Assert.IsFalse(obj.HasComponentOrInterface<MockComponent>());
        }

        [Test]
        public void HasComponentFalseWhenNoComponentInterfaceExists()
        {
            var obj = new GameObject();
            Assert.IsFalse(obj.HasComponentOrInterface<IMockComponent>());
        }

        [Test]
        public void HasComponentTrueWhenComponentExists()
        {
            var obj = new GameObject();
            obj.AddComponent<MockComponent>();
            Assert.IsTrue(obj.HasComponentOrInterface<MockComponent>());
        }

        [Test]
        public void HasComponentTrueWhenComponentInterfaceExists()
        {
            var obj = new GameObject();
            obj.AddComponent<MockComponent>();
            Assert.IsTrue(obj.HasComponentOrInterface<IMockComponent>());
        }

        [Test]
        public void GetsNullComponentWhenNoComponentExists()
        {
            var obj = new GameObject();
            Assert.IsNull(obj.GetComponentOrInterface<MockComponent>());
        }

        [Test]
        public void GetsNullComponentWhenNoComponentInterfaceExists()
        {
            var obj = new GameObject();
            Assert.IsNull(obj.GetComponentOrInterface<IMockComponent>());
        }

        [Test]
        public void GetsComponent()
        {
            var obj = new GameObject();
            var comp = obj.AddComponent<MockComponent>();
            Assert.AreEqual(comp, obj.GetComponentOrInterface<MockComponent>());
        }

        [Test]
        public void GetsComponentInterface()
        {
            var obj = new GameObject();
            var comp = obj.AddComponent<MockComponent>();
            Assert.AreEqual(comp, obj.GetComponentOrInterface<IMockComponent>());
        }   

        [Test]
        public void GetsNoComponentsWhenNoneExists()
        {
            var obj = new GameObject();
            Assert.AreEqual(0, obj.GetAllComponentsOrInterfaces<MockComponent>().Count());
        }

        [Test]
        public void GetsNoComponentInterfacesWhenNoneExists()
        {
            var obj = new GameObject();
            Assert.AreEqual(0, obj.GetAllComponentsOrInterfaces<IMockComponent>().Count());
        }

        [Test]
        public void GetsAllComponents()
        {
            var obj = new GameObject();
            var comp1 = obj.AddComponent<MockComponent>();
            var comp2 = obj.AddComponent<MockComponent>();

            var all = obj.GetAllComponentsOrInterfaces<MockComponent>();

            Assert.AreEqual(2, all.Count());
            Assert.IsTrue(all.Contains(comp1));
            Assert.IsTrue(all.Contains(comp2));
        }

        [Test]
        public void GetsAllComponentInterfaces()
        {
            var obj = new GameObject();
            var comp1 = obj.AddComponent<MockComponent>();
            var comp2 = obj.AddComponent<AnotherMockComponent>();

            var all = obj.GetAllComponentsOrInterfaces<IMockComponent>();

            Assert.AreEqual(2, all.Count());
            Assert.IsTrue(all.Contains(comp1));
            Assert.IsTrue(all.Contains(comp2));
        }

        [Test]
        public void AddsChildObject()
        {
            var parent = new GameObject();
            var child = parent.AddChild();
            Assert.AreEqual(parent.transform, child.transform.parent);
        }

        [Test]
        public void AddsChildObjectWithComponent()
        {
            var parent = new GameObject();
            var childComponent = parent.AddChild<MockComponent>();
            Assert.AreEqual(parent.transform, childComponent.transform.parent);
        }

        [Test]
        public void AddsNamedChildObject()
        {
            var parent = new GameObject();
            var child = parent.AddChild("Test Child Game Object");
            Assert.AreEqual("Test Child Game Object", child.name);
        }

        [Test]
        public void AddsNamedChildObjectWithComponent()
        {
            var parent = new GameObject();
            var child = parent.AddChild<MockComponent>("Test Child Game Object");
            Assert.AreEqual("Test Child Game Object", child.name);
        }

        [Test]
        public void AddsChildObjectWithComponents()
        {
            var parent = new GameObject();
            var child = parent.AddChild(typeof(MockComponent), typeof(AnotherMockComponent));

            var all = child.GetAllComponentsOrInterfaces<IMockComponent>();

            Assert.AreEqual(2, all.Count());
            Assert.AreEqual(parent.transform, child.transform.parent);
        }

        [Test]
        public void ConvertsColor()
        {
            var original = new Color(0.1f, 0.2f, 0.3f, 0.3f);
            var newColor = original.ToUInt().ToColor();
            Assert.IsTrue(newColor.r > original.r - 0.01 && newColor.r < original.r + 0.01);
            Assert.IsTrue(newColor.g > original.g - 0.01 && newColor.g < original.g + 0.01);
            Assert.IsTrue(newColor.b > original.b - 0.01 && newColor.b < original.b + 0.01);
            Assert.IsTrue(newColor.a > original.a - 0.01 && newColor.a < original.a + 0.01);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot randomly pick an item from the list, the list is null!")]
        public void ThrowsExceptionIfTryingToRandomlyPickFromANullEnumarable()
        {
            List<Color> colors = null;
            colors.RandomOne();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot randomly pick an item from the list, there are no items in the list!")]
        public void ThrowsExceptionIfTryingToRandomlyPickFromAnEmptyList()
        {
            var colors = new List<Color>();
            colors.RandomOne();
        }

        [Test]
        public void ReturnsTheFirstElementWhenRandomlyPickingFromAListWithOneElment()
        {
            var colors = new List<Color>() { Color.blue };
            var color = colors.RandomOne();
            Assert.AreEqual(Color.blue, color);
        }

        [Test]
        public void RoughlyRandomlyPicksElementsFromAList()
        {
            var colors = new List<Color>() { Color.blue, Color.red };
            var redCount = 0;
            var blueCount = 0;
            // Random it 1000 times
            for (int i = 0; i < 1000; i++)
            {
                if (colors.RandomOne() == Color.red) redCount++;
                else blueCount++;
            }

            // Should expect roughly 500 reds and 500 blues
            Assert.IsTrue(redCount + blueCount == 1000);
            Assert.IsTrue(redCount > 400 && redCount < 600);
            Assert.IsTrue(blueCount > 400 && blueCount < 600);
        }

        [Test]
        public void AddOnceRemovesTheCallbackAfterInvokation()
        {
            var evnt = new UnityEvent();
            var callCount = 0;
            evnt.AddOnce(() =>
            {
                callCount++;
            });
            evnt.Invoke();
            evnt.Invoke(); // second invoke shouldnt call the handler
            Assert.AreEqual(1, callCount);
        }

    }
}
