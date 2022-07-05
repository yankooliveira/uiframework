using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace deVoid.UIFramework.Examples
{
	/// <summary>
	/// Important: this only works if the template UI element
	/// is anchored to the bottom left corner. It also considers
	/// the RectTransform that contains it is stretched to fit
	/// the screen.
	/// </summary>
	public class UIFollowComponent : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI label = null;
		[SerializeField] private Image icon = null;
		[SerializeField] private bool clampAtBorders = true;
		[SerializeField] private bool rotateWhenClamped = true;
		[SerializeField] private RectTransform rotatingElement = null;

		public event Action<UIFollowComponent> LabelDestroyed;

		private Transform currentFollow;
		private RectTransform rectTransform;
		private CanvasScaler parentScaler;
		private RectTransform mainCanvasRectTransform;
	
		/// <summary>
		/// Calculates the anchored position for a given Worldspace transform for a Screenspace-Camera UI
		/// </summary>
		/// <param name="viewingCamera">The worldspace camera</param>
		/// <param name="followTransform">The transform to be followed</param>
		/// <param name="canvasScaler">The canvas scaler</param>
		/// <param name="followElementRect">The rect of the UI element that will follow the transform</param>
		/// <returns></returns>
		public static Vector2 GetAnchoredPosition(Camera viewingCamera, Transform followTransform, CanvasScaler canvasScaler, Rect followElementRect) {
			// We need to calculate the object's relative position to the camera make sure the
			// follow element's position doesn't end up getting "inverted" by WorldToViewportPoint when far away
			var relativePosition = viewingCamera.transform.InverseTransformPoint(followTransform.position);
			relativePosition.z = Mathf.Max(relativePosition.z, 1f);
			var viewportPos = viewingCamera.WorldToViewportPoint(viewingCamera.transform.TransformPoint(relativePosition));
			
			return new Vector2(viewportPos.x * canvasScaler.referenceResolution.x - followElementRect.size.x / 2f,
							   viewportPos.y * canvasScaler.referenceResolution.y - followElementRect.size.y / 2f);
		}
	
		/// <summary>
		/// Clamps the position on the screen for a Screenspace-Camera UI
		/// </summary>
		/// <param name="onScreenPosition">The current on-screen position for an UI element</param>
		/// <param name="followElementRect">The rect that follows the worldspace object</param>
		/// <param name="mainCanvasRectTransform">The rect transform of this UI's main canvas</param>
		/// <returns></returns>
		public static Vector2 GetClampedOnScreenPosition(Vector2 onScreenPosition, Rect followElementRect, RectTransform mainCanvasRectTransform) {
			return new Vector2(Mathf.Clamp(onScreenPosition.x, 0f, mainCanvasRectTransform.sizeDelta.x - followElementRect.size.x),
							   Mathf.Clamp(onScreenPosition.y, 0f, mainCanvasRectTransform.sizeDelta.y - followElementRect.size.y));
		}
		
		private void Start() {
			mainCanvasRectTransform = transform.root as RectTransform;
			rectTransform = transform as RectTransform;
			parentScaler = mainCanvasRectTransform.GetComponent<CanvasScaler>();

			if (rotatingElement == null) {
				rotatingElement = rectTransform;
			}
		}

		private void OnDestroy() {
			if (LabelDestroyed != null) {
				LabelDestroyed(this);
			}
		}

		public void SetFollow(Transform toFollow) {
			currentFollow = toFollow;
		}

		public void SetText(string label) {
			this.label.text = label;
		}
	
		public void SetIcon(Sprite icon) {
			this.icon.sprite = icon;
		}
	
		/// <summary>
		/// Positions element at the center of the screen
		/// </summary>
		protected void PositionAtOrigin() {
			var mainSize = mainCanvasRectTransform.sizeDelta;
			var labelSize = rectTransform.rect.size;
			rectTransform.anchoredPosition = new Vector2((mainSize.x - labelSize.x) / 2f, mainSize.y / 2f);
		}
	
		public void UpdatePosition(Camera cam) {
			if (currentFollow != null) {
				var onScreenPosition = GetAnchoredPosition(cam, currentFollow.transform, parentScaler, rectTransform.rect);
				if (!clampAtBorders) {
					rectTransform.anchoredPosition = onScreenPosition;
					return;
				}
	
				var clampedPosition = GetClampedOnScreenPosition(onScreenPosition, rectTransform.rect, mainCanvasRectTransform);
				rectTransform.anchoredPosition = clampedPosition;
	
				if (!rotateWhenClamped) {
					return;
				}
	
				if (onScreenPosition != clampedPosition) {
					var delta = clampedPosition - onScreenPosition;
					var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
					rotatingElement.localRotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);
				}
				else {
					rotatingElement.localRotation = Quaternion.identity;
				}
			}
		}
	}
}

