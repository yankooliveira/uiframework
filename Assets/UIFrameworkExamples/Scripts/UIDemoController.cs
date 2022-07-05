using System;
using System.Collections.Generic;
using deVoid.Utils;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    public class UIDemoController : MonoBehaviour
    {
        [SerializeField] private UISettings defaultUISettings = null;
        [SerializeField] private FakePlayerData fakePlayerData = null;
        [SerializeField] private Camera cam = null;
        [SerializeField] private Transform transformToFollow = null;

        private UIFrame uiFrame;

        private void Awake() {
            uiFrame = defaultUISettings.CreateUIInstance();
            Signals.Get<StartDemoSignal>().AddListener(OnStartDemo);
            Signals.Get<NavigateToWindowSignal>().AddListener(OnNavigateToWindow);
            Signals.Get<ShowConfirmationPopupSignal>().AddListener(OnShowConfirmationPopup);
        }

        private void OnDestroy() {
            Signals.Get<StartDemoSignal>().RemoveListener(OnStartDemo);
            Signals.Get<NavigateToWindowSignal>().RemoveListener(OnNavigateToWindow);
            Signals.Get<ShowConfirmationPopupSignal>().RemoveListener(OnShowConfirmationPopup);
        }

        private void Start() {
            uiFrame.OpenWindow(ScreenIds.StartGameWindow);
        }

        private void OnStartDemo() {
            // The navigation panel will automatically navigate
            // to the first screen upon opening
            uiFrame.ShowPanel(ScreenIds.NavigationPanel);
            uiFrame.ShowPanel(ScreenIds.ToastPanel);
        }

        private void OnNavigateToWindow(string windowId) {
            // You usually don't have to do this as the system takes care of everything
            // automatically, but since we're dealing with navigation and the Window layer
            // has a history stack, this way we can make sure we're not just adding
            // entries to the stack indefinitely
            uiFrame.CloseCurrentWindow();

            switch (windowId) {
                case ScreenIds.PlayerWindow:
                    uiFrame.OpenWindow(windowId, new PlayerWindowProperties(fakePlayerData.LevelProgress));
                    break;
                case ScreenIds.CameraProjectionWindow:
                    transformToFollow.parent.gameObject.SetActive(true);
                    uiFrame.OpenWindow(windowId, new CameraProjectionWindowProperties(cam, transformToFollow));
                    break;
                default:
                    uiFrame.OpenWindow(windowId);
                    break;
            }
        }

        private void OnShowConfirmationPopup(ConfirmationPopupProperties popupPayload) {
            uiFrame.OpenWindow(ScreenIds.ConfirmationPopup, popupPayload);
        }
    }
}