using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private FSM stateManager;
    
    private EnemyParameter enemyParameter;

    private NavMeshAgent navAgent;

    public ChaseState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
        this.navAgent = manager.GetNavAgent();
    }
    
    public void OnEnter()
    {
        enemyParameter.Animator.SetBool("Chase", true);
        // 开启导航组件
        stateManager.EnableNavComponent(true);
        // 开始导航
        navAgent.isStopped = false;
    }

    public void OnUpdate()
    {
        // 设置导航的目的地为玩家的位置
        navAgent.SetDestination(stateManager.GetPlayerTransform().position);
        // 进入攻击范围
        if (enemyParameter.DistanceFromPlayer <= enemyParameter.AttackDistance)
        {
            stateManager.TransitionState(StateType.Attack);
        }
        // 超出追击范围
        if (enemyParameter.DistanceFromPlayer > enemyParameter.HatredAreaRadius)
        {
            stateManager.TransitionState(StateType.Idle);
        }
        // 血量小于等于0时死亡
        if (enemyParameter.CurrentHealth <= 0)
        {
            stateManager.TransitionState(StateType.Death);
        }
    }
    
    public void OnExit()
    {
        enemyParameter.Animator.SetBool("Chase", false);
        // 结束导航
        navAgent.isStopped = true;
        // 非需要导航组件时关闭避免任务动画双脚悬空
        stateManager.EnableNavComponent(false);
    }
}