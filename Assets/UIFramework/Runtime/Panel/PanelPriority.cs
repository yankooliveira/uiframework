using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework
{
    /// <summary>
    /// Defines to which para-layer
    /// the panel is going to be parented to
    /// </summary>
    public enum PanelPriority {
        None = 0,
        Prioritary = 1,
        Tutorial = 2,
        Blocker = 3,
    }
    
    [System.Serializable] 
    public class PanelPriorityLayerListEntry {
        [SerializeField] 
        [Tooltip("The panel priority type for a given target para-layer")]
        private PanelPriority priority;
        [SerializeField] 
        [Tooltip("The GameObject that should house all Panels tagged with this priority")]
        private Transform targetParent;

        public Transform TargetParent {
            get { return targetParent; }
            set { targetParent = value; }
        }

        public PanelPriority Priority {
            get { return priority; }
            set { priority = value; }
        }

        public PanelPriorityLayerListEntry(PanelPriority prio, Transform parent) {
            priority = prio;
            targetParent = parent;
        }
    }

    [System.Serializable] 
    public class PanelPriorityLayerList {
        [SerializeField] 
        [Tooltip("A lookup of GameObjects to store panels depending on their Priority. Render priority is set by the hierarchy order of these GameObjects")]
        private List<PanelPriorityLayerListEntry> paraLayers = null;

        private Dictionary<PanelPriority, Transform> lookup;

        public Dictionary<PanelPriority, Transform> ParaLayerLookup {
            get {
                if (lookup == null || lookup.Count == 0) {
                    CacheLookup();
                }

                return lookup;
            }
        }

        private void CacheLookup() {
            lookup = new Dictionary<PanelPriority, Transform>();
            for (int i = 0; i < paraLayers.Count; i++) {
                lookup.Add(paraLayers[i].Priority, paraLayers[i].TargetParent);
            }
        }

        public PanelPriorityLayerList(List<PanelPriorityLayerListEntry> entries) {
            paraLayers = entries;
        }
    }
}
