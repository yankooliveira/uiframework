using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    public class AutoMove : MonoBehaviour
    {
        [SerializeField] private Vector2 minMaxPosition = Vector2.zero;
        [SerializeField] private float speed = 0f;
        [SerializeField] private Vector2 minMaxPositionY = Vector2.zero;
        [SerializeField] private float speedY = 0f;
        [SerializeField] private Vector3 rotationSpeed = Vector3.zero;

        private void Update() {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
            float x = Mathf.Lerp(minMaxPosition.x, minMaxPosition.y, (Mathf.Sin(Time.time*speed)+1)/2f);
            float y = Mathf.Lerp(minMaxPositionY.x, minMaxPositionY.y, (Mathf.Sin(Time.time*speedY)+1)/2f);
            transform.localPosition = new Vector3(x, y, transform.position.z);
        }
    }
}
