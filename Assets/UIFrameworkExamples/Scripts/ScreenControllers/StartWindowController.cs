using deVoid.Utils;

namespace deVoid.UIFramework.Examples
{
    public class StartDemoSignal : ASignal { }

    public class StartWindowController : AWindowController
    {
        public void UI_Start() {
            Signals.Get<StartDemoSignal>().Dispatch();
        }
    }
}
