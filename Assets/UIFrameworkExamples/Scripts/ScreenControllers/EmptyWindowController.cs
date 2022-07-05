namespace deVoid.UIFramework.Examples
{
    /// <summary>
    /// You usually won't have empty windows, as you'll almost always have
    /// data to be passed onto them for set up. However, you may have multiple
    /// windows that use the same controller, you just need them to have different
    /// ids. More than one window in this demo is totally static and defined by
    /// its prefab, and they are all using this empty window.
    /// Even though it has no implementation, it still provides the functionality
    /// for animation/priority etc.
    /// </summary>
    public class EmptyWindowController : AWindowController { }
}
