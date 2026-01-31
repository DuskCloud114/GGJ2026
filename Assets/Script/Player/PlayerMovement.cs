using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 8f;

    private Rigidbody2D _rb;
    private PlayerAnimation _playerAnimation;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();

        // 确保俯视角下重力倍率为0
        _rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        // 应用移动
        _rb.velocity = _moveInput * moveSpeed;

        // 标准俯视角翻转 (可选，面向左/右)
        if (_moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (_moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        // 更新动画
        if (_playerAnimation != null)
        {
            _playerAnimation.SetSpeed(_rb.velocity.magnitude);
        }
    }

    private void Update()
    {
        // 输入通过 InputHandler 的 SetMoveInput 处理
    }

    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }
}
