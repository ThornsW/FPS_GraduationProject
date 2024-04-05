using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IState
{
    private FSM stateManager;
    
    private EnemyParameter enemyParameter;
    
    public DeathState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
    }
    
    public void OnEnter()
    {
        enemyParameter.Animator.SetBool("Death", true);
    }

    public void OnUpdate()
    {
        // todo: 一定时间后销毁尸体（不一定做）
    }

    public void OnExit()
    {
        enemyParameter.Animator.SetBool("Death", false);
    }
}