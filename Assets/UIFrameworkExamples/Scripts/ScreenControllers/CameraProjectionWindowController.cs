using System;
using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    [Serializable]
    public class CameraProjectionWindowProperties : WindowProperties
    {
        public readonly Camera WorldCamera;
        public readonly Transform TransformToFollow;

        public CameraProjectionWindowProperties(Camera worldCamera, Transform toFollow) {
            WorldCamera = worldCamera;
            TransformToFollow = toFollow;
        }
    }

    public class CameraProjectionWindowController : AWindowController<CameraProjectionWindowProperties>
    {
        [SerializeField] 
        private UIFollowComponent followTemplate = null;

        private List<UIFollowComponent> allElements = new List<UIFollowComponent>();

        protected override void OnPropertiesSet() {
            CreateNewLabel(Properties.TransformToFollow,"Look at me!", null);
        }

        protected override void WhileHiding() {
            foreach (var element in allElements) {
                Destroy(element.gameObject);
            }
            allElements.Clear();
            // This is the kind of thing you *COULD* do, but you usually wouldn't
            // want to - in theory this is UI code, so it shouldn't control external things.
            // This is an example of "with great power comes great responsibility":
            // the UI Framework enforces very few rules, but the rest is up to you.
            Properties.TransformToFollow.parent.gameObject.SetActive(false);
        }

        private void LateUpdate() {
            for (int i = 0; i < allElements.Count; i++) {
                allElements[i].UpdatePosition(Properties.WorldCamera);
            }
        }

        private void CreateNewLabel(Transform target, string label, Sprite icon) {
            var followComponent = Instantiate(followTemplate, followTemplate.transform.parent, false);
            followComponent.LabelDestroyed += OnLabelDestroyed;
            followComponent.gameObject.SetActive(true);
            followComponent.SetFollow(target);
            followComponent.SetText(label);
            
            if (icon != null) {
                followComponent.SetIcon(icon);
            }

            allElements.Add(followComponent);
        }

        private void OnLabelDestroyed(UIFollowComponent destroyedLabel) {
            allElements.Remove(destroyedLabel);
        }
    }
}
