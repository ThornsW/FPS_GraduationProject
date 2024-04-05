using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtState : IState
{
    private FSM stateManager;
    
    private EnemyParameter enemyParameter;
    
    private AnimatorStateInfo animatorInfo;

    public HurtState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
    }
    
    public void OnEnter()
    {
        enemyParameter.Animator.SetBool("Hurt", true);
    }

    public void OnUpdate()
    {
        stateManager.TurnToPlayer();
        animatorInfo = enemyParameter.Animator.GetCurrentAnimatorStateInfo(0);
        // 受伤或击退动画即将播放完成后回到空闲状态
        if (animatorInfo.IsName("Base.Hurt") &&
            animatorInfo.normalizedTime >= 0.8f) {
            stateManager.TransitionState(StateType.Idle);
        }
    }

    public void OnExit()
    {
        enemyParameter.Animator.SetBool("Hurt", false);
    }
}