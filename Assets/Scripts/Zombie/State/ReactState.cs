using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactState : IState
{
    private FSM stateManager;
    
    private EnemyParameter enemyParameter;

    private AnimatorStateInfo animatorInfo;

    public ReactState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
    }
    
    public void OnEnter()
    {
        enemyParameter.Animator.SetBool("React", true);
    }

    public void OnUpdate()
    {
        animatorInfo = enemyParameter.Animator.GetCurrentAnimatorStateInfo(0);
        // 转身面向玩家
        stateManager.TurnToPlayer();
        // 超出仇恨范围重新回到idle状态
        if (enemyParameter.DistanceFromPlayer > enemyParameter.HatredAreaRadius)
        {
            stateManager.TransitionState(StateType.Idle);
        }
        // 动画播放即将完成时切换到Chase状态
        if (animatorInfo.normalizedTime >= 0.9f)
        {
            stateManager.TransitionState(StateType.Chase);
        }
    }

    public void OnExit()
    {
        enemyParameter.Animator.SetBool("React", false);
    }
}