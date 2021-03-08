using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework
{
    /// <summary>
    /// Template for an UI. You can rig the prefab for the UI Frame itself and all the screens that should
    /// be instanced and registered upon instantiating a new UI Frame.
    /// </summary>
    
    [CreateAssetMenu(fileName = "UISettings", menuName = "deVoid UI/UI Settings")]
    public class UISettings : ScriptableObject
    {
        [Tooltip("Prefab for the UI Frame structure itself")]
        [SerializeField] private UIFrame templateUIPrefab = null;
        [Tooltip("Prefabs for all the screens (both Panels and Windows) that are to be instanced and registered when the UI is instantiated")]
        [SerializeField] private List<GameObject> screensToRegister = null;
        [Tooltip("In case a screen prefab is not deactivated, should the system automatically deactivate its GameObject upon instantiation? If false, the screen will be at a visible state upon instantiation.")]
        [SerializeField] private bool deactivateScreenGOs = true;

        /// <summary>
        /// Creates an instance of the UI Frame Prefab. By default, also instantiates
        /// all the screens listed and registers them. If the deactivateScreenGOs flag is
        /// true, it will deactivate all Screen GameObjects in case they're active.
        /// </summary>
        /// <param name="instanceAndRegisterScreens">Should the screens listed in the Settings file be instanced and registered?</param>
        /// <returns>A new UI Frame</returns>
        public UIFrame CreateUIInstance(bool instanceAndRegisterScreens = true) {
            var newUI = Instantiate(templateUIPrefab);

            if (instanceAndRegisterScreens) {
                foreach (var screen in screensToRegister) {
                    var screenInstance = Instantiate(screen);
                    var screenController = screenInstance.GetComponent<IUIScreenController>();

                    if (screenController != null) {
                        newUI.RegisterScreen(screen.name, screenController, screenInstance.transform);
                        if (deactivateScreenGOs && screenInstance.activeSelf) {
                            screenInstance.SetActive(false);
                        }
                    }
                    else {
                        Debug.LogError("[UIConfig] Screen doesn't contain a ScreenController! Skipping " + screen.name);
                    }
                }
            }

            return newUI;
        }
        
        private void OnValidate() {
            List<GameObject> objectsToRemove = new List<GameObject>();
            for(int i = 0; i < screensToRegister.Count; i++) {
                var screenCtl = screensToRegister[i].GetComponent<IUIScreenController>();
                if (screenCtl == null) {
                    objectsToRemove.Add(screensToRegister[i]);
                }
            }

            if (objectsToRemove.Count > 0) {
                Debug.LogError("[UISettings] Some GameObjects that were added to the Screen Prefab List didn't have ScreenControllers attached to them! Removing.");
                foreach (var obj in objectsToRemove) {
                    Debug.LogError("[UISettings] Removed " + obj.name + " from " + name + " as it has no Screen Controller attached!");
                    screensToRegister.Remove(obj);
                }
            }
        }        
    }
}
