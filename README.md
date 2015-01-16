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
obj.HasComponentOrInterface<IMyComponent>(); // Can also handle interfaces

obj.Get<MyComponent>(); // Returns the first component
obj.GetComponentOrInterface<IMyComponent>(); // Returns the first component that implements the interface

obj.GetAll<MyComponent>(); // Gets all the components
obj.GetAllComponentsOrInterfaces<IMyComponent>(); // Gets all the components that implement the interface
```

Another utility is for adding children to GameObjects:

```
using UnityHelpers;

var obj = new GameObject();

obj.AddChild("Mike"); // Creates a new GameObject named "Mike" and adds it as a child

var player = obj.AddChild<Player>("Dave"); // Creates a new GameObject named "Dave" and adds the component "Player" returning it

obj.AddChild(typeof(Player), typeof(Friendly), typeof(AI)); // Creates a new GameObject and adds a number of components
```

There are a number of useful utils for loading assets into GameObjects too:

```
using UnityHelpers;

var obj = new GameObject();

obj.Load("Prefabs/Spaceship"); // Loads the Spaceship prefab from the Resources folder and adds it as a child
```

Most of the Utils also can be accessed via the UnityUtils.* class too rather than just extension methods:

```
using UnityHelerps;

var player = UnityUtils.Load<Player>("Prefabs/Spaceship");
```

Enumerate Resources
===================

![enumerate resources](http://i.imgur.com/OlkmYR6.png)

Enumerate Resources is a handy util for creating type-safe resource references. Traditionally you have to manually create constant strings to load resources at runtime:

```
Resources.Load("Prefabs/Cars/Porsche");
```

This is fragile. If the asset is moved you wont know about the crash until you run the game, this line of code may not be executed often and hence introduces a bug that may only present itself at a later date.

Enumerate Resources scans a resources directory and generates a type-safe alternative:

```
Resources.Load(GameResources.Prefabs.Cars.Porsche);
```

Now if you move the resource and run the enumerator you will get a compile error.

For added sugar there is a method to add the loaded resource as a child of a game object (handy for prefabs):

```
obj.LoadChild(GameResources.Prefabs.Icons.IndicatorArror);
```

ViewStateController
===================

![view state controller screenshot](http://i.imgur.com/kjBzATD.png)

The ViewStateController is a utility for UnityUI that allows you to manage view states. Simply add the states to a list then use the dropdown in the inspector to toggle between which one is active.

At runtime use the methods to change the states:

```
var states = GetComponent<ViewStateController>();

states.SetState("Main Menu"); // Sets the main menu state

states.SetState(someOtherStateGameObject); // Sets another state using the gameobject reference
```

FPSCounter
==========

Attach this component to a gameobject with a Text component and output the current FPS to the screen.

Misc Hacks
==========

The UnityHelpers.MiscHacks class contains a few helpful hacks, such as opening the SpriteEditorWindow directly from code (Unity provides no way of doing this).

Tests
=====

I have included a number of Unit Tests with the project. If you would like to run the tests yourself then just make sure you include the "Unity Test Tools" project from the asset store.

Installation
============

Simply include the source in your project, to use the extension methods dont forget to include the namespace:

```
using UnityHelpers;
```