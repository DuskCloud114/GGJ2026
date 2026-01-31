using UnityEngine;

public class Enemy : MonoBehaviour, ISwitchable
{
    [Header("附身设置")]
    [SerializeField] private bool canSwitch = true;

    // 实现接口属性
    public bool bCanSwitch
    {
        get { return canSwitch; }
        set { canSwitch = value; }
    }

    public virtual void OnSwitchEnter()
    {
        Debug.Log($"Player possessed enemy: {gameObject.name}");

        // 在这里处理获得控制权的逻辑
        // 例如：
        // 1. 禁用敌人的AI脚本
        // 2. 启用玩家的输入控制脚本 (或者把PlayerInputHandler转移过来)
        // 3. 改变图层或Tag为Player

        // 示例：变色表示被附身
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public virtual void OnSwitchExit()
    {
        Debug.Log($"Player left enemy: {gameObject.name}");

        // 在这里处理失去控制权的逻辑
        // 例如：
        // 1. 重新启用AI
        // 2. 禁用输入控制

        // 示例：恢复颜色
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
