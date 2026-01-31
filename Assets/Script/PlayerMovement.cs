using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;

    private Rigidbody2D _rb;
    private PlayerAnimation _playerAnimation;
    private Vector2 _moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimation = GetComponent<PlayerAnimation>();

        // Ensure gravity is off for top-down
        _rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        // Apply Movement
        _rb.velocity = _moveInput * moveSpeed;

        // Standard Top-Down Flip (optional, faces left/right)
        if (_moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (_moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Update Animation
        if (_playerAnimation != null)
        {
            _playerAnimation.SetSpeed(_rb.velocity.magnitude);
        }
    }

    private void Update()
    {
        // Input is handled via SetMoveInput from InputHandler
    }

    public void SetMoveInput(Vector2 input)
    {
        _moveInput = input;
    }
}
