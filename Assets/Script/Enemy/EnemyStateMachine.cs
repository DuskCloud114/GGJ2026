using UnityEngine;
using System.Collections;


public enum EnemyState
{
    Idle,       // 待机
    Chase,      // 追踪
    PreAttack,  // 攻击前摇
    Attack,     // 攻击执行
    Stun,       // 硬直(可被附身)
    Possessed,  // 被附身 (逻辑被PlayerBrain接管)
    Dead        // 死亡
}

[RequireComponent(typeof(Enemy))]
public class EnemyStateMachine : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("侦测到玩家并开始追踪的距离")]
    [SerializeField] private float detectionRange = 10f;

    [Tooltip("移动速度")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Tooltip("硬直持续时间 (秒)")]
    [SerializeField] private float stunDuration = 3.0f;

    [Header("状态监控")]
    [SerializeField] private EnemyState currentState;

    // 内部变量
    private float stateTimer;
    private Transform playerTransform;
    private Enemy enemyComponent;
    private IEnemyAttackHandler attackHandler; // 外部攻击逻辑引用

    private void Start()
    {
        enemyComponent = GetComponent<Enemy>();

        // 获取外部挂载的攻击处理脚本
        attackHandler = GetComponent<IEnemyAttackHandler>();
        if (attackHandler == null)
        {
            Debug.LogError($"[{gameObject.name}] 缺少实现了 IEnemyAttackHandler 的组件！状态机无法攻击。");
        }

        // 初始设置：不可附身
        if (enemyComponent != null) enemyComponent.bCanSwitch = false;

        // 获取玩家引用
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        SwitchState(EnemyState.Idle);
    }

    private void Update()
    {
        // 如果是死亡状态，不做任何事
        if (currentState == EnemyState.Dead) return;

        // 如果是被附身状态，逻辑完全交给 PlayerBrain，状态机暂停 AI 决策
        if (currentState == EnemyState.Possessed) return;

        // 状态逻辑更新
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.PreAttack:
                UpdatePreAttack();
                break;
            case EnemyState.Stun:
                UpdateStun();
                break;
            case EnemyState.Attack:
                // 攻击状态由 SwitchState 处理进入，执行完逻辑后会自动切出
                break;
        }
    }

    // ==========================================
    //  外部接口 (供其他脚本调用)
    // ==========================================

    /// <summary>
    /// [接口] 受到攻击时调用
    /// </summary>
    public void OnTakeDamage()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Possessed) return;

        Debug.Log("敌人受到攻击，进入硬直!");
        SwitchState(EnemyState.Stun);
    }

    /// <summary>
    /// [接口] 当玩家成功附身时调用此方法。
    /// 通常在你的 PlayerBrain 或 InteractionManager 中调用。
    /// </summary>
    public void NotifyPossessionStart()
    {
        if (currentState == EnemyState.Stun || currentState == EnemyState.Idle)
        {
            SwitchState(EnemyState.Possessed);
        }
    }

    /// <summary>
    /// [接口] 当玩家主动脱离、或附身时间结束时调用。
    /// 此时敌人将直接死亡。
    /// </summary>
    public void NotifyPossessionEnd()
    {
        if (currentState == EnemyState.Possessed)
        {
            SwitchState(EnemyState.Dead);
        }
    }

    // ==========================================
    //  状态机逻辑
    // ==========================================

    private void SwitchState(EnemyState newState)
    {
        OnExitState(currentState);
        currentState = newState;
        stateTimer = 0f;
        OnEnterState(currentState);
    }

    private void OnEnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.PreAttack:
                // 调用外部攻击接口：开始前摇
                if (attackHandler != null) attackHandler.OnStartPreAttack(playerTransform);
                break;

            case EnemyState.Attack:
                // 调用外部攻击接口：发动攻击
                if (attackHandler != null) attackHandler.OnPerformAttack(playerTransform);

                // 攻击完成后，稍作判断是继续打还是追
                // 为防止攻击瞬间卡死，这里可以加一个极短的各个击破延迟，或者直接切回
                StartCoroutine(AttackRecovery());
                break;

            case EnemyState.Stun:
                // 进入硬直，允许附身
                if (enemyComponent != null) enemyComponent.bCanSwitch = true;
                break;

            case EnemyState.Possessed:
                // 被附身：虽然逻辑停了，但可能需要清理一下物理速度等，防止滑行
                // Rigidbody rb = GetComponent<Rigidbody>(); if(rb) rb.velocity = Vector3.zero;
                // 注意：CanSwitch 此时应该由 Enemy.cs 的 OnSwitchEnter 控制，或者保持开启允许被人再次附身(看设计)
                if (enemyComponent != null) enemyComponent.bCanSwitch = false; // 已经被占了，可能不让别人再占
                Debug.Log($"{gameObject.name} 被玩家控制中...");
                break;

            case EnemyState.Dead:
                HandleDeath();
                break;
        }
    }

    private void OnExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Stun:
                // 离开硬直（无论是超时死亡还是被附身），关闭可附身标记
                if (enemyComponent != null) enemyComponent.bCanSwitch = false;
                break;
        }
    }

    // 等待一帧后切回状态，防止死循环
    private IEnumerator AttackRecovery()
    {
        yield return null;

        // 如果在攻击那一瞬间没有被打断/死亡
        if (currentState == EnemyState.Attack)
        {
            if (playerTransform == null)
            {
                SwitchState(EnemyState.Idle);
            }
            else
            {
                // 根据当前距离决定是继续前摇还是追击
                float dist = Vector3.Distance(transform.position, playerTransform.position);
                float range = attackHandler != null ? attackHandler.GetAttackRange() : 1.5f;

                if (dist <= range)
                    SwitchState(EnemyState.PreAttack);
                else
                    SwitchState(EnemyState.Chase);
            }
        }
    }

    // ==========================================
    //  具体状态行为
    // ==========================================

    private void UpdateIdle()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= detectionRange)
        {
            SwitchState(EnemyState.Chase);
        }
    }

    private void UpdateChase()
    {
        if (playerTransform == null) return;
        if (attackHandler == null) return; // 没攻击手段就只追不打，或者报错

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        float atkRange = attackHandler.GetAttackRange();

        // 玩家是否在攻击范围内
        if (dist <= atkRange)
        {
            SwitchState(EnemyState.PreAttack);
            return;
        }

        // 移动逻辑：这里做最基础的Transform移动。
        // 如果你的游戏需要寻路，请在这里换成 agent.SetDestination()
        MoveTowards(playerTransform.position);
    }

    private void UpdatePreAttack()
    {
        stateTimer += Time.deltaTime;

        // 前摇期间，通常会一直盯着玩家
        LookAt(playerTransform.position);

        // 从接口获取前摇时间
        float preTime = attackHandler != null ? attackHandler.GetPreAttackDuration() : 0.5f;

        if (stateTimer >= preTime)
        {
            SwitchState(EnemyState.Attack);
        }
    }

    private void UpdateStun()
    {
        // 只有硬直可以被附身
        stateTimer += Time.deltaTime;

        // 超时未被附身，则死亡
        if (stateTimer >= stunDuration)
        {
            SwitchState(EnemyState.Dead);
        }
    }

    // ==========================================
    //  辅助方法
    // ==========================================

    private void MoveTowards(Vector3 target)
    {
        LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    private void LookAt(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        if (dir.x != 0)
        {
            // 简单的Sprite翻转
            float scaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(dir.x > 0 ? scaleX : -scaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} 彻底死亡。");
        Destroy(gameObject); // 或者对象池回收
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}