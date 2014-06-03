Unity-Helpers
=============

A set of utils and extensions for making unity development easier.

Unity is a great tool for making games however there are a few annoyances with its API that I have tried to fix with this library. 

One such annoyance is the innability to use interfaces in GetComponent(), so I wrote some extension methods to help with that:

```
using UnityHelpers;

var obj = new GameObject();
obj.AddComponent<MyComponent>();

obj.Has<MyComponent>(); // Returns true or false
obj.Has<IMyComponent>(); // Can also handle interfaces

obj.Get<MyComponent>(); // Returns the first component
obj.Get<IMyComponent>(); // Returns the first component that implements the interface

obj.GetAll<MyComponent>(); // Gets all the components
obj.GetAll<IMyComponent>(); // Gets all the components that implement the interface
```

Another utility is for adding children to GameObjects:

```
using UnityHelpers;

var obj = new GameObject();

obj.AddChild("Mike"); // Creates a new GameObject named "Mike" and adds it as a child

var player = obj.AddChild<Player>("Dave"); // Creates a new GameObject named "Dave" and adds the component "Player" returning it

obj.AddChild(typeof(Player), typeof(Friendly), typeof(AI)); // Creates a new GameObject and adds a number of components
```

There are many other utils and extensions, checkout the source: https://github.com/mikecann/Unity-Helpers/tree/master/Scripts

Tests
=====

I have included a number of Unit Tests with the project. If you would like to run the tests yourself then just make sure you include the "Unity Test Tools" project from the asset store.

Installation
============

Simply include the source in your project, to use the extension methods dont forget to include the namespace:

```
using UnityHelpers;
```