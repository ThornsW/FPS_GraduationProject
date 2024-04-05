using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private FSM stateManager;

    private EnemyParameter enemyParameter;

    private Transform enemyTransform;

    private AnimatorStateInfo animatorInfo;

    private int attackTimes; // 攻击次数

    private Vector3 attackDetectDirection; // 攻击检测方向

    private Transform playerTarget; // 被攻击目标（玩家）
    
    private float sectorDistance = 3; // 扇形范围大小
    
    private float sectorAngle = 150; // 扇形的角度
    
    public AttackState(FSM manager)
    {
        this.stateManager = manager;
        this.enemyParameter = manager.EnemyParameter;
        this.enemyTransform = stateManager.GetEnemyTransform();
        this.playerTarget = manager.GetPlayerTransform();
    }

    public void OnEnter()
    {
        enemyParameter.Animator.SetBool("Attack", true);
        stateManager.TurnToPlayer();
        attackDetectDirection = enemyTransform.forward;
        attackTimes = 0;
    }

    public void OnUpdate()
    {
        // 超出攻击范围则继续追击
        if (enemyParameter.DistanceFromPlayer > enemyParameter.AttackDistance)
        {
            stateManager.TransitionState(StateType.Chase);
            return;
        }

        // 获取动画播放进度数据
        animatorInfo = enemyParameter.Animator.GetCurrentAnimatorStateInfo(0);
        double playingProgressNumber = animatorInfo.normalizedTime;
        double playingIntegerPart = Math.Truncate(playingProgressNumber); // 播放进度整数部分
        double playingDecimalPart = playingProgressNumber - playingIntegerPart; // 播放进度小数部分

        // 每次攻击前都调整下攻击方向
        if (playingDecimalPart <= 0.05f)
        {
            stateManager.TurnToPlayer();
            attackDetectDirection = enemyTransform.forward;
        }

        // 检测攻击动画播放进度在40%~50%之间时，进行攻击检测和造成伤害
        if (animatorInfo.IsName("Base.Attack") && playingDecimalPart >= 0.4f && playingDecimalPart < 0.5f 
                                               && attackTimes < playingIntegerPart + 1)
        {
            // 此变量用于保证每个攻击动画播放循环，只能检测一次攻击
            attackTimes++;
            AttackDetection();
        }
    }

    public void OnExit()
    {
        enemyParameter.Animator.SetBool("Attack", false);
        attackTimes = 0;
    }
    
    // 攻击检测
    private void AttackDetection()
    {
        // 与攻击目标的距离
        float distance = Vector3.Distance(enemyTransform.position, playerTarget.position);
        // 敌人与目标的方向向量
        Vector3 enemyToTargetDirection = playerTarget.position - enemyTransform.position;
        // 求两个向量的夹角
        float angle = Mathf.Acos(Vector3.Dot(attackDetectDirection.normalized, enemyToTargetDirection.normalized)) * Mathf.Rad2Deg;
        if (distance < sectorDistance)
        {
            // 绘制扇形区域
            // ToDrawSectorSolid(enemyTransform.position, sectorAngle, sectorDistance);

            if (angle <= sectorAngle * 0.5f)
            {
                // 调用玩家血量模块进行扣血
                playerTarget.GetComponent<PlayerHealthController>().TakeDamage(20);
            }
        }
    }

    GameObject go;
    MeshFilter mf;
    MeshRenderer mr;
    Shader shader;

    // 绘制实心扇形
    private void ToDrawSectorSolid(Vector3 center, float angle, float radius)
    {
        int pointAmount = 100;
        float eachAngle = angle / pointAmount;

        Vector3 forward = attackDetectDirection;
        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(center);
        for (int i = 0; i < pointAmount; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * radius + center;
            vertices.Add(pos);
        }
        CreateMesh(vertices);
    }

    // 创建网格
    private GameObject CreateMesh(List<Vector3> vertices)
    {
        int[] triangles;
        Mesh mesh = new Mesh();

        int triangleAmount = vertices.Count - 2;
        triangles = new int[3 * triangleAmount];

        // 根据三角形的个数，来计算绘制三角形的顶点顺序
        for (int i = 0; i < triangleAmount; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        if (go == null)
        {
            go = new GameObject("mesh");
            go.transform.position = new Vector3(0f, 0.1f, 0.5f);

            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();

            shader = Shader.Find("Unlit/Color");
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;

        mf.mesh = mesh;
        mr.material.shader = shader;
        mr.material.color = Color.red;

        return go;
    }
}