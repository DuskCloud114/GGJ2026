using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISwitchable
{
    /// 检查当前是否可以进行附身（例如：敌人是否处于晕眩状态、距离是否足够近等）
    bool bCanSwitch { get; set; }   

    /// 当附身开始时调用。
    /// 在此处处理获得控制权的逻辑，如：禁用AI、启用玩家输入监听、切换摄像机跟随目标等。
    void OnSwitchEnter();

    /// 当附身结束时调用。
    /// 在此处处理失去控制权的逻辑，如：恢复AI行为、销毁对象或播放死亡动画等。
    void OnSwitchExit();
}
