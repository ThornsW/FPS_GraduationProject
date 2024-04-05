using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private FSM stateManager;
    
    private EnemyParameter enemyParameter;

    private float timer;

    public IdleState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
    }
    
    public void OnEnter()
    {  
        enemyParameter.Animator.SetBool("Idle", true);
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;
        // 检测到玩家进入仇恨范围后进入察觉状态
        if (enemyParameter.DistanceFromPlayer <= enemyParameter.HatredAreaRadius)
        {
            stateManager.TransitionState(StateType.React);
        }
        // 空闲一段时间后继续巡逻
        if (timer >= enemyParameter.IdleTime)
        {
            stateManager.TransitionState(StateType.Patrol);
        }
    }

    public void OnExit()
    {
        timer = 0;
        enemyParameter.Animator.SetBool("Idle", false);
    }
}
