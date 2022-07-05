# deVoid UI Framework: Manual
## Part 1: Structure
The main entry point in the system is the ***UI Frame***. It is the way you interact with the framework from your code. Internally, it serves as a facade to the ***UI Layers***. Layers are responsible for containing and handling ***Screens***. Screens can be of two types:
* ***Panels*** are ad-hoc pieces of UI that are not bound to a specific history and can be visible and interactable at the same time. Eg: a HUD could be a *Panel*.
* ***Windows*** are elements that usually fill up most of your display's real estate, follow a history and are the main interaction spots at a given time: you won't ever have two windows being interactable at the same time, but you might have one or more *Panels* and a single *Window* open.
* ***Pop-Ups*** are a special case of Window that works the same way, but is displayed on top of the rest of the UI - it's a modal window.

Screens can contain one or more ***Widgets*** which are self-contained components that can be reused across multiple scenarios. ***Layers*** can also make use of ***Para-Layers***, which are used to organize render order and display priority. In the default implementation, there is one Window para-layer (which is used to house pop-ups) and multiple Panel para-layers.

For a Screen to be usable by the Framework, it needs to be ***Registered*** to a Layer. This operation is performed via the ***UI Frame***.

You can use the built-in ***UI Settings*** file to rig your UI Frame prefab and the screens to be registered, then get a fully initialized instance.

Since every UI Frame is its own self contained structure, this means that you can have multiple complete UIs at the same time, including in worldspace. You can also register it as a centralized service if it better fits your purpose, or you can mix and match, eg: you have a centralized UI that is for the general Game UI, that is permanent, but you have separate UIs for players that support split screen and have their own specific logic, and get created and destroyed with the players.

The best way to understand these concepts is by checking them out in practice, so make sure to take a look at the [examples repo](https://github.com/yankooliveira/uiframework_examples). If you're feeling adventurous, you can always read the [blog post](http://yankooliveira.com/index.php/2017/12/27/uisystem/) where I originally described the rationale for the architecture in detail.

## Part 2: Implementing a Screen
Every screen is identified by an unique ***Screen Id***. This id can be any `string`, but I usually use the name of the screen's Prefab.

The first thing is deciding if you're implementing a ***Panel*** (implemented by extending an `APanelController<T>` or a ***Window*** (implemented by extending an `AWindowController<T>`). The `<T>` parameter is optional: it's a type-safe ***Properties*** class (extended from `WindowProperties` or `PanelProperties`), which are data payloads that are passed onto screens when opening. 

You can reuse the same `IScreenController` across prefabs, and you can even use the same prefab multiple times, but *ScreenIds* must be unique. The implementation itself can be empty, but the most important method to know about is `OnPropertiesSet()`. This method is called as soon as the screen has had its `Properties` member set, which means it's your main entry point for filling up the screen with data.

Eg:
```c#
public class PlayerWindowController : AWindowController<PlayerWindowProperties>  
{
	protected override void OnPropertiesSet() { // At this point, Properties is guaranteed  
	    UpdateData(Properties.PlayerData); 		// to have the data passed by OpenWindow()
	}
}
```
But you could also have something like
```c#
public class EmptyWindowController : AWindowController { }
```
**IMPORTANT**: when implementing your Properties class, make sure it's tagged as `[System.Serializable]`, otherwise you won't be able to set it up on the prefab.

If no properties are passed to a Screen displaying method, it will use the ones set up in the prefab. This means that you can optionally store values for the properties directly on a prefab as well. 

Other important methods are `AddListeners()` and `RemoveListeners()`. These are, by default, called on Awake and OnDestroy. These are the places where you can hook for events, and are useful entry points for eg: an UI that is responsive to Signals.

Finally, `Close()` is a method that you can call from your UI to close itself - it is however not called when the screen is closed by something else - you can use `WhileHiding()` for cleanup operations that you might need to perform as a window closes.

Check the `AUIScreenController` code for a list of all the virtual methods.

**IMPORTANT:** While you can use the native Unity callbacks (`Awake`, `OnDestroy` etc), be aware that some of those are used for initialization on the abstract class code. So don't forget to always call the base methods when overriding those. 

## Part 3: Operating Screens
To Register a Screen, you can use `UIFrame.RegisterWindow`, `UIFrame.RegisterPanel` or  `UIFrame.RegisterScreen`, which tries to infer what kind of Screen that is.

After screens are registered, you can open them by using
```c#
public void ShowPanel<T>(string id, T properties) where T : IPanelProperties
public void OpenWindow<T>(string id, T properties) where T : IWindowProperties
```
or the parameterless versions, if your screen doesn't need a property payload
```c#
public void ShowPanel(string id)
public void OpenWindow(string id)
```
The analogous methods are 
```c#
public void HidePanel(string id)
public void CloseWindow(string id)
```
Panels can be shown and hidden at any time, but Windows will obey the history stack and, if you try to close a Window that is not the currently open one, the system will issue a warning (this usually means you're trying to execute a logically invalid operation in your screen flow).

If you have the need to un-register screens (eg: if you're doing manual memory management), you can use `UnregisterPanel()` and `UnregisterWindow()`.

### Addendum: default Properties setup
For `Panels`, the only built-in Property is their *Priority*. This defines to which ***Para-Layer*** the Panel is parented to (which, in turn, defines it's render priority, or "what is drawn in front of what").

For `Windows`, there are additional default parameters that are important to keep in mind when setting up your Prefab:
* **Hide On Foreground Lost**: this defines if this window should be hidden when another Window is opened. In practice, this defines if you have a visible "stack" of windows, or if opening a new one hides the current one.
* **Window Priority**: this defines if the Window should always open on top when the `Open()` command is issued.  If it's set to `Force Foreground`, it will open on top; if it's set to `None`, it will be enqueued and will be displayed as soon as the current Window is closed. 
* **Is Popup**: this defines if the window should work as a Pop-up. If marked, this will be parented to the `WindowPriorityLayer`, and it will automatically have a darkened background displayed under it.

The default Layer order is
1. *PanelLayer* (Panels with no priority)
2. *WindowLayer* (regular Windows)
3. *PriorityPanelLayer* (Panels tagged as Prioritary)
4. *PriorityWindowLayer* (Pop-ups)
5. *[Other Panel para-layers]*
## Part 4: Transition animations
Every Screen has two default members called ``AnimIn`` and ``AnimOut``. These can be rigged with any `ATransitionComponent`, which are classes that have to implement the following interface:
```c#
public abstract void Animate(Transform target, Action callWhenFinished);
```
These are called by the ***Layer*** code when opening and closing a Screen, and the `callWhenFinished` callback basically tells the Layer that that transition is finished. Since a very common bug in mobile games is the user interacting with the UI while there are screen transitions happening, the `WindowLayer` automatically blocks user input by disabling the UIFrame's main `GraphicRaycaster` if there are any animations playing. This behaviour is not enforced by the `PanelLayer`.

Creating multiple of these components can give your UI artists the flexibility in rigging and tweaking your UI animations, by mixing and matching multiple options. If no `ATransitionComponent` is rigged to a given transition, the screen's GameObject will simply be `SetActive(true/false)`.

## Epilogue
I've tried to comment the source code thoroughly, so I hope that this manual, the [examples repo](https://github.com/yankooliveira/uiframework_examples), the [blog post](http://yankooliveira.com/index.php/2017/12/27/uisystem/) but, most importantly, the code itself, will be enough for you to get up and running. Good luck, hope it's useful and even though I don't know how quickly I'd be able to fix them, let me know if you find any major bugs [@yankooliveira](http://twitter.com/yankooliveira)!