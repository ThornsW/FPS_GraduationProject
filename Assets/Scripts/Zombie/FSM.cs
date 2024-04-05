using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSM : MonoBehaviour
{

    public EnemyParameter EnemyParameter; // 敌人参数
    
    private IState currentState; // 当前状态

    public StateType CurrenStateType; // 当前状态类型
    
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>(); // 状态字典，用于注册状态
    
    private Transform enemyTransform; // 当前敌人变换组件
    
    private NavMeshAgent navAgent; // 导航代理

    private Transform playerTransform; // 玩家变换组件

    // Get方法
    public NavMeshAgent GetNavAgent() => navAgent;
    public Transform GetPlayerTransform() => playerTransform;
    public Transform GetEnemyTransform() => enemyTransform;
    
    void Start()
    {
        // 获取当前敌人的变换组件
        enemyTransform = GetComponent<Transform>();
        // 获取动画控制器
        EnemyParameter.Animator = GetComponent<Animator>();
        // 获取导航组件
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = EnemyParameter.MoveSpeed;
        // 获取玩家的变换组件
        playerTransform = GameObject.FindWithTag("Player").transform;
        
        // 注册各种状态
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Hurt, new HurtState(this));
        states.Add(StateType.Death, new DeathState(this));
        
        // 默认状态为空闲
        TransitionState(StateType.Idle);
    }
    
    void Update()
    {
        // 实时更新当前敌人与玩家的距离
        EnemyParameter.DistanceFromPlayer = Vector3.Distance(
            playerTransform.position, 
            enemyTransform.position);
        
        //Debug.Log("玩家距离: " + EnemyParameter.DistanceFromPlayer);
        
        // 调用当前状态的onUpdate()方法
        currentState.OnUpdate();
    }

    // 状态转换
    public void TransitionState(StateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[type];
        CurrenStateType = type;
        currentState.OnEnter();
    }
    
    // 各个状态通用的方法
    
    // 转身朝向玩家
    public void TurnToPlayer()
    {
        // 计算出朝向玩家的目标方向
        var targetDirection = playerTransform.position - enemyTransform.position;
        // 转换为四元数
        var targetRotation = Quaternion.LookRotation(targetDirection);
        // 只旋转y轴，其它轴固定
        targetRotation.Set(0f, targetRotation.y, 0f, targetRotation.w);
        // 平滑转动角度
        enemyTransform.rotation = Quaternion.Slerp(
            enemyTransform.rotation, targetRotation, 
            EnemyParameter.TurnToSmoothTime * Time.deltaTime);
    }

    // 启用/禁用Nav组件
    public void EnableNavComponent(bool state)
    {
        navAgent.enabled = state;
    }

}