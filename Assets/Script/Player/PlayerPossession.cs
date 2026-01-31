using UnityEngine;
using GGJ2026.CameraSystem;

public class PlayerPossession : MonoBehaviour
{
    [Header("附身设置")]
    [SerializeField] private float maxPossessionTime = 5.0f; // 如果需要，可以设定子弹时间/选择阶段的限制时间
    [SerializeField] private float maxPossessionDistance = 10.0f; // 附身最大距离

    private bool _isInPossessionMode = false;
    private CameraFollow _cameraFollow;

    private void Start()
    {
        // 尝试获取主摄像机上的 CameraFollow 组件
        if (Camera.main != null)
        {
            _cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
    }

    public void OnPossessionAction()
    {
        // 切换或触发附身模式
        // 根据设计：“当玩家脱离时...触发子弹时间”
        // 目前，按下按钮进入“附身模式”（子弹时间）

        if (!_isInPossessionMode)
        {
            StartPossessionMode();
        }
        else
        {
            // 理想情况下，再次点击或选择一个目标会结束该模式
            // 目前先确保能进入该模式
            // 为了调试，允许手动切换关闭，或者用户暂时只需要入口实现
            EndPossessionMode();
        }
    }

    /// <summary>
    /// 执行附身逻辑：将控制权和视角转移到目标身上
    /// 供外部调用（例如在UI选择或鼠标点击敌人后）
    /// </summary>
    /// <param name="targetEnemy">目标敌人对象</param>
    public void PossessTarget(GameObject targetEnemy)
    {
        if (targetEnemy == null) return;

        // 检查距离
        float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);
        if (distance > maxPossessionDistance)
        {
            Debug.LogWarning($"目标距离太远，无法附身！当前距离: {distance:F2}, 最大距离: {maxPossessionDistance}");
            return;
        }

        ISwitchable switchable = targetEnemy.GetComponent<ISwitchable>();
        if (switchable != null && switchable.bCanSwitch)
        {
            // 1. 如果还在子弹时间，先退出
            if (_isInPossessionMode)
            {
                EndPossessionMode();
            }

            // 2. 执行附身逻辑（通知目标已被附身）
            switchable.OnSwitchEnter();

            // 3. 切换摄像机跟随
            UpdateCameraTarget(targetEnemy.transform);

            Debug.Log($"Possession Successful: Player is now controlling {targetEnemy.name}");
        }
        else
        {
            Debug.LogWarning($"Cannot possess target: {targetEnemy.name}. Either no ISwitchable or bCanSwitch is false.");
        }
    }

    private void UpdateCameraTarget(Transform newTarget)
    {
        if (_cameraFollow == null)
        {
            if (Camera.main != null)
            {
                _cameraFollow = Camera.main.GetComponent<CameraFollow>();
            }
        }

        if (_cameraFollow != null)
        {
            _cameraFollow.SetTarget(newTarget);
        }
        else
        {
            Debug.LogWarning("CameraFollow script not found on Main Camera!");
        }
    }

    private void StartPossessionMode()
    {
        _isInPossessionMode = true;
        Debug.Log("进入附身模式：子弹时间开始");

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.EnterBulletTime();
        }
        else
        {
            Debug.LogWarning("未找到 TimeManager 实例！");
        }
    }

    public void EndPossessionMode()
    {
        _isInPossessionMode = false;
        Debug.Log("退出附身模式：子弹时间结束");

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ExitBulletTime();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxPossessionDistance);
    }
}
