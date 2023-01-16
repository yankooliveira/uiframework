namespace deVoid.UIFramework {
    /// <summary>
    /// An entry for controlling window history and queue
    /// </summary>
    public struct WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IWindowProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IWindowProperties properties) {
            Screen = screen;
            Properties = properties;
        }

        public void Show() {
            Screen.Show(Properties);
        }
    }
}
