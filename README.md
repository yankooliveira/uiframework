# deVoid UI Framework
![Example Project](https://img.itch.zone/aW1nLzE5NTQzODYuZ2lm/original/%2BrlumK.gif)

*You can see a live demo of the [the examples repo](https://github.com/yankooliveira/uiframework_examples) at my [itch.io page](https://yanko.itch.io/devui)*

**TL;DR:**
*Step 1:*

Right click on project view
`Create -> deVoid UI -> UIFrame Prefab`
then drag the Prefab onto your scene

*Step 2:*
Get a reference to your UI Frame and register some screens
```c#
uiFrame.RegisterScreen("YourScreenId", yourScreenPrefab);
```

*Step 3:*
Show your screens
```c#
uiFrame.OpenWindow("YourWindowId");
uiFrame.ShowPanel("YourPanelId");
```

Or, if you have some data payload

```c#
uiFrame.OpenWindow("YourWindowId", yourWindowProperties);
uiFrame.ShowPanel("YourPanelId", yourPanelProperties);
```

*Step 4:*
Now that you're familiar with the API, right click on project view
`Create -> deVoid UI -> UI Settings`

Rig your UI Frame prefab as the UI Template, drag all your screens in the Screens To Register list and simply do

```c#
uiFrame = yourUiSettings.CreateUIInstance();
```

Which will give you a new UI Frame instance and automatically do *Step 2* for you.

Make sure to check out [the examples repo](https://github.com/yankooliveira/uiframework_examples) and read [the manual](https://github.com/yankooliveira/uiframework/blob/master/MANUAL.md)!

## What?
The *deVoid UI Framework* (or **devUI** for short) is a simple architecture for UI handling and navigation in Unity. It enforces one simple rule: *you can **never** directly access the internals of your UI code from external code*, but anything else is fair game. You want to read data from Singletons from inside your UI? Sure. You want to sandwich mediators, views and controllers and do MVC by the book? Go for it. You want to skip the data passing functionalities and use your own MVVM framework? Power to you. Do whatever floats your boat and works best for your needs.

## Why?
Having worked with mobile F2P games for years, I've done my fair share of UI. And I *hate* doing UI. 
So I figured the more people learn how to do it, the smaller the chances that I ever have to do it again 🌈

*(Also, sharing is caring and I'm secretly a hippie)*

### Features
* Simple to extend and customize
* Support for navigation history and queuing
* Transition animations
* Blocking user input while screens transition (especially handy for touch input)
* Priority and layering
* Focus on type safety and flexibility

### Known Limitations
* No built in support for controller navigation

### Disclaimer:
This is **A** solution to work with UI - I don't believe in perfect, one-size-fits-all solutions (and nor should you in anyone who tells you otherwise). But this architecture was battle tested in more than one game over the last few years (from game jams to medium and bigger sized games), and it ticked all the boxes for me so far.

While I tried to keep this it as flexible and as easy to deal with as possible, there is still some structure to be followed. I recommend checking out [the examples repo](https://github.com/yankooliveira/uiframework_examples) and taking a look at [the manual](https://github.com/yankooliveira/uiframework/blob/master/MANUAL.md) before using.

Although the architecture itself is sound and was tested in live environments, this implementation was made from scratch on my free time. I have been using it for a few months now and did some cleanup and bugfixes for the public release, so it *Should Work(TM)*.

### Acknowledgements
A lot of the structure is inspired by a design used by [Renan Rennó](https://www.linkedin.com/in/renanrenno/) when we worked together. Special thanks to everyone who I made use this architecture through the years and to [Sylvain Cornillon](https://www.bossastudios.com/the-team/), who allowed me to open source it and helped me find more proper names for the classes, or I'd be still calling it *"UIManagerOrWhatevs"* cause I'm a rebel.

You can read my original (even more) verbose post about this architecture [in my blog](http://yankooliveira.com/index.php/2017/12/27/uisystem/) and poke me on twitter [@yankooliveira](https://twitter.com/yankooliveira).
