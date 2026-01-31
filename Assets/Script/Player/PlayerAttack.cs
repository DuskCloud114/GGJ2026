using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public System.Action OnAttack;
    private PlayerAnimation _playerAnimation;

    private void Awake()
    {
        _playerAnimation = GetComponent<PlayerAnimation>();

        OnAttack += AttackTest;
    }

    public void Attack()
    {
        OnAttack?.Invoke();
        // 如果 _playerAnimation 不为空
        // {
        //     _playerAnimation.OnAttack();
        // }
    }

    private void AttackTest()
    {
        Debug.Log("Player Attacked!");
    }
}
