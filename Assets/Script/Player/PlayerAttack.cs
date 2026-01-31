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
        // if (_playerAnimation != null)
        // {
        //     _playerAnimation.OnAttack();
        // }
    }

    private void AttackTest()
    {
        Debug.Log("Player Attacked!");
    }
}
