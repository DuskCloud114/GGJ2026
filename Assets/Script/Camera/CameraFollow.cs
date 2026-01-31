using UnityEngine;

namespace GGJ2026.CameraSystem
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("跟随设置")]
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        /// 切换摄像机跟随的目标
        /// <param name="newTarget">新的跟随目标</param>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
