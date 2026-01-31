/// <summary>
/// 攻击逻辑接口。
/// 将此接口实现并挂载在敌人身上，状态机就会自动调用它。
/// </summary>
public interface IEnemyAttackHandler
{
    /// <summary>
    /// 获取攻击范围
    /// </summary>
    float GetAttackRange();

    /// <summary>
    /// 获取攻击前摇时间
    /// </summary>
    float GetPreAttackDuration();

    /// <summary>
    /// 当进入前摇状态时调用（播放蓄力动画、提示音等）
    /// </summary>
    /// <param name="target">攻击目标</param>
    void OnStartPreAttack(Transform target);

    /// <summary>
    /// 当攻击真正发生时调用（发射子弹、判定伤害区域等）
    /// </summary>
    /// <param name="target">攻击目标</param>
    void OnPerformAttack(Transform target);
}
