using UnityEngine;
using System;

namespace deVoid.UIFramework
{
    /// <summary>
    /// Base implementation for UI Screens. You'll probably want to inherit
    /// from one of its child classes: AWindowController or APanelController, not this.
    /// <seealso cref="AWindowController"/>
    /// <seealso cref="APanelController"/>
    /// </summary>
    public abstract class AUIScreenController<TProps> : MonoBehaviour, IUIScreenController
        where TProps : IScreenProperties
    {
        [Header("Screen Animations")] 
        [Tooltip("Animation that shows the screen")] 
        [SerializeField]
        private ATransitionComponent animIn;

        [Tooltip("Animation that hides the screen")] 
        [SerializeField]
        private ATransitionComponent animOut;

        [Header("Screen properties")]
        [Tooltip(
            "This is the data payload and settings for this screen. You can rig this directly in a prefab and/or pass it when you show this screen")]
        [SerializeField]
        private TProps properties;

        /// <summary>
        /// Unique identifier for this ID. If using the default system, it should be the same name as the screen's Prefab.
        /// </summary>
        public string ScreenId { get; set; }

        /// <summary>
        /// Transition component for the showing up animation
        /// </summary>
        public ATransitionComponent AnimIn
        {
            get { return animIn; }
            set { animIn = value; }
        }

        /// <summary>
        /// Transition component for the hiding animation
        /// </summary>
        public ATransitionComponent AnimOut
        {
            get { return animOut; }
            set { animOut = value; }
        }

        /// <summary>
        /// Occurs when "in" transition is finished.
        /// </summary>
        public Action<IUIScreenController> InTransitionFinished { get; set; }

        /// <summary>
        /// Occurs when "out" transition is finished.
        /// </summary>
        public Action<IUIScreenController> OutTransitionFinished { get; set; }

        /// <summary>
        /// Screen can fire this event to request its responsible layer to close it
        /// </summary>
        /// <value>The close request.</value>
        public Action<IUIScreenController> CloseRequest { get; set; }

        /// <summary>
        /// If this screen is destroyed for some reason, it must warn its layer
        /// </summary>
        /// <value>The destruction action.</value>
        public Action<IUIScreenController> ScreenDestroyed { get; set; }

        /// <summary>
        /// Is this screen currently visible?
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// The properties of this screen. Can contain
        /// serialized values, or passed in private values.
        /// </summary>
        /// <value>The properties.</value>
        protected TProps Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        protected virtual void Awake()
        {
            AddListeners();
        }

        protected virtual void OnDestroy()
        {
            if (ScreenDestroyed != null)
            {
                ScreenDestroyed(this);
            }

            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequest = null;
            ScreenDestroyed = null;
            RemoveListeners();
        }

        /// <summary>
        /// For setting up all the listeners for events/messages. By default, called on Awake()
        /// </summary>
        protected virtual void AddListeners()
        {
        }

        /// <summary>
        /// For removing all the listeners for events/messages. By default, called on OnDestroy()
        /// </summary>
        protected virtual void RemoveListeners()
        {
        }

        /// <summary>
        /// When Properties are set for this screen, this method is called.
        /// At this point, you can safely access Properties.
        /// </summary>
        protected virtual void OnPropertiesSet()
        {
        }

        /// <summary>
        /// When the screen animates out, this is called
        /// immediately 
        /// </summary>
        protected virtual void WhileHiding()
        {
        }

        /// <summary>
        /// When setting the properties, this method is called.
        /// This way, you can extend the usage of your properties by
        /// certain conditions.
        /// </summary>
        /// <param name="props">Properties.</param>
        protected virtual void SetProperties(TProps props)
        {
            properties = props;
        }

        /// <summary>
        /// In case your screen has any special behaviour to be called
        /// when the hierarchy is adjusted
        /// </summary>
        protected virtual void HierarchyFixOnShow()
        {
        }

        /// <summary>
        /// Hides the screen
        /// </summary>
        /// <param name="animate">Should animation be played? (defaults to true)</param>
        public void Hide(bool animate = true)
        {
            DoAnimation(animate ? animOut : null, OnTransitionOutFinished, false);
            WhileHiding();
        }

        /// <summary>
        /// Show this screen with the specified properties.
        /// </summary>
        /// <param name="props">The data for the screen.</param>
        public void Show(IScreenProperties props = null)
        {
            if (props != null)
            {
                if (props is TProps)
                {
                    SetProperties((TProps) props);
                }
                else
                {
                    Debug.LogError("Properties passed have wrong type! (" + props.GetType() + " instead of " +
                                   typeof(TProps) + ")");
                    return;
                }
            }

            HierarchyFixOnShow();
            OnPropertiesSet();

            if (!gameObject.activeSelf)
            {
                DoAnimation(animIn, OnTransitionInFinished, true);
            }
            else
            {
                if (InTransitionFinished != null)
                {
                    InTransitionFinished(this);
                }
            }
        }

        private void DoAnimation(ATransitionComponent caller, Action callWhenFinished, bool isVisible)
        {
            if (caller == null)
            {
                gameObject.SetActive(isVisible);
                if (callWhenFinished != null)
                {
                    callWhenFinished();
                }
            }
            else
            {
                if (isVisible && !gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }

                caller.Animate(transform, callWhenFinished);
            }
        }

        private void OnTransitionInFinished()
        {
            IsVisible = true;

            if (InTransitionFinished != null)
            {
                InTransitionFinished(this);
            }
        }

        private void OnTransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);

            if (OutTransitionFinished != null)
            {
                OutTransitionFinished(this);
            }
        }
    }
}
