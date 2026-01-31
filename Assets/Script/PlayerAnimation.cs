using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    // Parameter IDs for optimization
    private int _speedParamID;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _speedParamID = Animator.StringToHash("Speed");
    }

    public void SetSpeed(float speed)
    {
        _animator.SetFloat(_speedParamID, speed);
    }
}
