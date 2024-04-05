using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    private FSM stateManager;

    private EnemyParameter enemyParameter;

    private NavMeshAgent navAgent;

    private Vector3 randomPatrolPosition;

    private Vector3 enemyPosition;
    
    private float timer;

    public PatrolState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
        this.navAgent = manager.GetNavAgent();
        this.enemyPosition = stateManager.GetEnemyTransform().position;
    }

    public void OnEnter()
    {
        enemyParameter.Animator.SetBool("Patrol", true);
        // 开启导航组件
        stateManager.EnableNavComponent(true);
        // 开始导航
        navAgent.isStopped = false;
        // 获取一个随机巡逻位置
        randomPatrolPosition = GetRandomPosition(enemyParameter.PatrolDistance);
        // 前往巡逻目的地
        navAgent.SetDestination(randomPatrolPosition);
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;
        // 接近目的地或者计时器超时，转为空闲状态
        if (navAgent.remainingDistance <= 1f || timer >= enemyParameter.PatrolMaxTime)
        {
            stateManager.TransitionState(StateType.Idle);
        }
    }

    public void OnExit()
    {
        timer = 0;
        enemyParameter.Animator.SetBool("Patrol", false);
        // 结束导航
        navAgent.isStopped = true;
        // 非需要导航组件时关闭避免任务动画双脚悬空
        stateManager.EnableNavComponent(false);
    }
    
    // 获取随机方向外的一个位置
    public Vector3 GetRandomPosition(float distance)
    {
        // distance距离外随机方向的一个点
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        Vector3 randomPoint = enemyPosition + randomDirection;
        // 在这个随机点范围内进行采样
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, distance, -1);
        return navHit.position;
    }
}