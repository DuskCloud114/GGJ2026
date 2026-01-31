using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 确保添加Player组件时，自动添加所有必要的子模块
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerAnimation))]
[RequireComponent(typeof(PlayerAttack))]
public class Player : MonoBehaviour
{
    // 这里可以作为一个中心访问点，或者保留为空，仅作为组件容器
    // 将来可以在这里初始化或协调各个子组件
}
