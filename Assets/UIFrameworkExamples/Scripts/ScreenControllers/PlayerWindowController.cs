using System;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    /// <summary>
    /// This is the Properties class for this specific window.
    /// It carries the payload which will be used to fill up this
    /// window upon opening.
    /// </summary>
    [Serializable]
    public class PlayerWindowProperties : WindowProperties
    {
        public readonly List<PlayerDataEntry> PlayerData;

        public PlayerWindowProperties(List<PlayerDataEntry> data) {
            PlayerData = data;
        }
    }

    public class PlayerWindowController : AWindowController<PlayerWindowProperties>
    {
        [SerializeField] 
        private LevelProgressComponent templateLevelEntry = null;
        
        private List<LevelProgressComponent> currentLevels = new List<LevelProgressComponent>();

        /// <summary>
        /// Here I'm listening to a global signal that is fired by the ScriptableObject
        /// itself as a way of exemplifying how you could do this in your codebase.
        /// I could optionally carry the ScriptableObject itself, store a reference to it
        /// and do the same process via direct event hooks.
        /// </summary>
        protected override void AddListeners() {
            Signals.Get<PlayerDataUpdatedSignal>().AddListener(OnDataUpdated);
        }

        protected override void RemoveListeners() {
            Signals.Get<PlayerDataUpdatedSignal>().RemoveListener(OnDataUpdated);
        }

        protected override void OnPropertiesSet() {
            OnDataUpdated(Properties.PlayerData);
        }

        private void OnDataUpdated(List<PlayerDataEntry> data) {
            VerifyElementCount(data.Count);
            RefreshElementData(data);
        }

        private void VerifyElementCount(int levelCount) {
            if (currentLevels.Count == levelCount) {
                return;
            }

            if (currentLevels.Count < levelCount) {
                while (currentLevels.Count < levelCount) {
                    var newLevel = Instantiate(templateLevelEntry, 
                        templateLevelEntry.transform.parent, 
                        false); // Never forget to pass worldPositionStays as false for UI!
                    newLevel.gameObject.SetActive(true);
                    currentLevels.Add(newLevel);
                }
            }
            else {
                while (currentLevels.Count > levelCount) {
                    var levelToRemove = currentLevels[currentLevels.Count - 1];
                    currentLevels.Remove(levelToRemove);
                    Destroy(levelToRemove.gameObject);
                }
            }
        }
        
        private void RefreshElementData(List<PlayerDataEntry> playerLevelProgress) {
            for (int i = 0; i < currentLevels.Count; i++) {
                currentLevels[i].SetData(playerLevelProgress[i], i);
            }
        }
    }
}
