using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    // Parameter IDs for optimization
    private int _speedParamID;
    private int _attackParamID;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _speedParamID = Animator.StringToHash("Speed");
        _attackParamID = Animator.StringToHash("Attack");
    }

    public void SetSpeed(float speed)
    {
        _animator.SetFloat(_speedParamID, speed);
    }

    public void OnAttack()
    {
        _animator.SetTrigger(_attackParamID);
    }
}
