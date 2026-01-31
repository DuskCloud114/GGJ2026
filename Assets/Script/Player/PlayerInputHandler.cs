using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("输入动作引用")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference mainAttackAction;
    [SerializeField] private InputActionReference possessionAction;

    private PlayerMovement _playerMovement;
    private PlayerAttack _playerAttack;
    private PlayerPossession _playerPossession;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerPossession = GetComponent<PlayerPossession>();
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

        if (possessionAction != null)
        {
            possessionAction.action.Enable();
            possessionAction.action.performed += OnPossessionPerformed;
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

        if (possessionAction != null)
        {
            possessionAction.action.performed -= OnPossessionPerformed;
            possessionAction.action.Disable();
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

    private void OnPossessionPerformed(InputAction.CallbackContext context)
    {
        if (_playerPossession != null)
        {
            _playerPossession.OnPossessionAction();
        }
    }
}
