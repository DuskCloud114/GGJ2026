using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference mainAttackAction;

    private PlayerMovement _playerMovement;
    private PlayerAttack _playerAttack;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAttack = GetComponent<PlayerAttack>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }

        if (mainAttackAction != null)
        {
            mainAttackAction.action.Enable();
            mainAttackAction.action.performed += OnMainAttackPerformed;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (mainAttackAction != null)
        {
            mainAttackAction.action.performed -= OnMainAttackPerformed;
            mainAttackAction.action.Disable();
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

    private void OnMainAttackPerformed(InputAction.CallbackContext context)
    {
        if (_playerAttack != null)
        {
            _playerAttack.Attack();
        }
    }
}
