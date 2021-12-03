﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace deVoid.UIFramework {
    /// <summary>
    /// This Layer controls Panels.
    /// Panels are Screens that have no history or queuing,
    /// they are simply shown and hidden in the Frame
    /// eg: a HUD, an energy bar, a mini map etc.
    /// </summary>
    public class PanelUILayer : AUILayer<IPanelController> {
        [SerializeField]
        [Tooltip("Settings for the priority para-layers. A Panel registered to this layer will be reparented to a different para-layer object depending on its Priority.")]
        private PanelPriorityLayerList priorityLayers = null;

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform) {
            var ctl = controller as IPanelController;
            if (ctl != null) {
                ReparentToParaLayer(ctl.Priority, screenTransform);
            }
            else {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        public override void ShowScreen(IPanelController screen) {
            screen.Show();
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps properties) {
            if (properties is IPanelProperties) // This fixes oppening window with different priority during runtime
            {
                var panelProperties = (IPanelProperties) properties;
                if (screen.Priority != panelProperties.Priority)
                {
                    if (screen is MonoBehaviour)
                    {
                        var screenBehaviour = (MonoBehaviour) screen;
                        ReparentToParaLayer(panelProperties.Priority, screenBehaviour.transform);
                    }
                }
            }
            
            screen.Show(properties);
        }

        public override void HideScreen(IPanelController screen, bool animate = true) {
            screen.Hide(animate);
        }

        public bool IsPanelVisible(string panelId) {
            IPanelController panel;
            if (registeredScreens.TryGetValue(panelId, out panel)) {
                return panel.IsVisible;
            }

            return false;
        }
        
        private void ReparentToParaLayer(PanelPriority priority, Transform screenTransform) {
            Transform trans;
            if (!priorityLayers.ParaLayerLookup.TryGetValue(priority, out trans)) {
                trans = transform;
            }
            
            screenTransform.SetParent(trans, false);
        }
    }
}
