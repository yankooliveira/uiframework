using UnityEngine;
using System.Collections.Generic;

namespace deVoid.UIFramework
{
    /// <summary>
    /// This is a "helper" layer so Windows with higher priority can be displayed.
    /// By default, it contains any window tagged as a Popup. It is controlled by the WindowUILayer.
    /// </summary>
    public class WindowParaLayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject darkenBgObject = null;

        private List<GameObject> containedScreens = new List<GameObject>();

        private int screensCount;

        public void AddScreen(Transform screenRectTransform)
        {
            screenRectTransform.SetParent(transform, false);
            containedScreens.Add(screenRectTransform.gameObject);
        }

        public void RefreshDarken()
        {
            screensCount = containedScreens.Count;

            for (int i = 0; i < screensCount; i++)
            {
                if (containedScreens[i] != null && containedScreens[i].activeSelf)
                {
                    darkenBgObject.SetActive(true);
                    return;
                }
            }

            darkenBgObject.SetActive(false);
        }

        public void DarkenBG()
        {
            darkenBgObject.SetActive(true);
            darkenBgObject.transform.SetAsLastSibling();
        }
    }
}
