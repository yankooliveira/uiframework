namespace deVoid.UIFramework
{
    /// <summary>
    /// Base implementation for Window ScreenControllers that need no special Properties
    /// </summary>
    public abstract class AWindowController : AWindowController<WindowProperties> { }

    /// <summary>
    /// Base implementation for Window ScreenControllers. Its parameter is a specific type of IWindowProperties.
    /// In case your window doesn't need special properties, inherit from AWindowScreenController, without Generic param.
    /// <seealso cref="IWindowProperties"/>
    /// <seealso cref="AWindowController"/>
    /// </summary>
    public abstract class AWindowController<TProps> : AUIScreenController<TProps>, IWindowController
        where TProps : IWindowProperties
    {
        public bool HideOnForegroundLost {
            get { return Properties.HideOnForegroundLost; }
        }

        public bool IsPopup {
            get { return Properties.IsPopup; }
        }

        public WindowPriority WindowPriority {
            get { return Properties.WindowQueuePriority; }
        }

        /// <summary>
        /// Requests this Window to be closed, handy for rigging it directly in the Editor.
        /// I use the UI_ prefix to group all the methods that should be rigged in the Editor so that it's
        /// easy to find the screen-specific methods. It breaks naming convention, but does more good than harm as
        /// the amount of methods grow.
        /// This is *not* called every time it is closed, just upon user input - for that behaviour, see
        /// WhileHiding();
        /// </summary>
        public virtual void UI_Close() {
            CloseRequest(this);
        }
        
        protected sealed override void SetProperties(TProps props) {
            if (props != null) {
                // If the Properties set on the prefab should not be overwritten,
                // copy the default values to the passed in properties
                if (!props.SuppressPrefabProperties) {
                    props.HideOnForegroundLost = Properties.HideOnForegroundLost;
                    props.WindowQueuePriority = Properties.WindowQueuePriority;
                    props.IsPopup = Properties.IsPopup;
                }

                Properties = props;
            }
        }

        protected override void HierarchyFixOnShow() {
            transform.SetAsLastSibling();
        }
    }
}
