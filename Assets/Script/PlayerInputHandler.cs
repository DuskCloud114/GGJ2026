using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action References")]
    [SerializeField] private InputActionReference moveAction;

    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (_playerMovement != null && moveAction != null)
        {
            Vector2 movementInput = moveAction.action.ReadValue<Vector2>();
            _playerMovement.SetMoveInput(movementInput);
        }
    }
}
