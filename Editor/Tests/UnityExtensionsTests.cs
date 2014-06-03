using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityHelpers;

namespace UnityHelpers.Tests
{    

    [TestFixture]
    public class UnityExtensionsTests : UnityUnitTest
    {
        interface IMockComponent { }
        class MockComponent : MonoBehaviour, IMockComponent { }
        class AnotherMockComponent : MonoBehaviour, IMockComponent { }

        [Test]
        public void HasComponentFalseWhenNoComponentExists()
        {
            var obj = CreateGameObject();
            Assert.IsFalse(obj.Has<MockComponent>());
        }

        [Test]
        public void HasComponentFalseWhenNoComponentInterfaceExists()
        {
            var obj = CreateGameObject();
            Assert.IsFalse(obj.Has<IMockComponent>());
        }

        [Test]
        public void HasComponentTrueWhenComponentExists()
        {
            var obj = CreateGameObject();
            obj.AddComponent<MockComponent>();
            Assert.IsTrue(obj.Has<MockComponent>());
        }

        [Test]
        public void HasComponentTrueWhenComponentInterfaceExists()
        {
            var obj = CreateGameObject();
            obj.AddComponent<MockComponent>();
            Assert.IsTrue(obj.Has<IMockComponent>());
        }

        [Test]
        public void GetsNullComponentWhenNoComponentExists()
        {
            var obj = CreateGameObject();
            Assert.IsNull(obj.Get<MockComponent>());
        }

        [Test]
        public void GetsNullComponentWhenNoComponentInterfaceExists()
        {
            var obj = CreateGameObject();
            Assert.IsNull(obj.Get<IMockComponent>());
        }

        [Test]
        public void GetsComponent()
        {
            var obj = CreateGameObject();
            var comp = obj.AddComponent<MockComponent>();
            Assert.AreEqual(comp, obj.Get<MockComponent>());
        }

        [Test]
        public void GetsComponentInterface()
        {
            var obj = CreateGameObject();
            var comp = obj.AddComponent<MockComponent>();
            Assert.AreEqual(comp, obj.Get<IMockComponent>());
        }   

        [Test]
        public void GetsNoComponentsWhenNoneExists()
        {
            var obj = CreateGameObject();
            Assert.AreEqual(0, obj.GetAll<MockComponent>().Count());
        }

        [Test]
        public void GetsNoComponentInterfacesWhenNoneExists()
        {
            var obj = CreateGameObject();
            Assert.AreEqual(0, obj.GetAll<IMockComponent>().Count());
        }

        [Test]
        public void GetsAllComponents()
        {
            var obj = CreateGameObject();
            var comp1 = obj.AddComponent<MockComponent>();
            var comp2 = obj.AddComponent<MockComponent>();

            var all = obj.GetAll<MockComponent>();

            Assert.AreEqual(2, all.Count());
            Assert.IsTrue(all.Contains(comp1));
            Assert.IsTrue(all.Contains(comp2));
        }

        [Test]
        public void GetsAllComponentInterfaces()
        {
            var obj = CreateGameObject();
            var comp1 = obj.AddComponent<MockComponent>();
            var comp2 = obj.AddComponent<AnotherMockComponent>();

            var all = obj.GetAll<IMockComponent>();

            Assert.AreEqual(2, all.Count());
            Assert.IsTrue(all.Contains(comp1));
            Assert.IsTrue(all.Contains(comp2));
        }

        [Test]
        public void AddsChildObject()
        {
            var parent = CreateGameObject();
            var child = parent.AddChild();
            Assert.AreEqual(parent.transform, child.transform.parent);
        }

        [Test]
        public void AddsChildObjectWithComponent()
        {
            var parent = CreateGameObject();
            var childComponent = parent.AddChild<MockComponent>();
            Assert.AreEqual(parent.transform, childComponent.transform.parent);
        }

        [Test]
        public void AddsNamedChildObject()
        {
            var parent = CreateGameObject();
            var child = parent.AddChild("Test Child Game Object");
            Assert.AreEqual("Test Child Game Object", child.name);
        }

        [Test]
        public void AddsNamedChildObjectWithComponent()
        {
            var parent = CreateGameObject();
            var child = parent.AddChild<MockComponent>("Test Child Game Object");
            Assert.AreEqual("Test Child Game Object", child.name);
        }

        [Test]
        public void AddsChildObjectWithComponents()
        {
            var parent = CreateGameObject();
            var child = parent.AddChild(typeof(MockComponent), typeof(AnotherMockComponent));

            var all = child.GetAll<IMockComponent>();

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
    }
}
