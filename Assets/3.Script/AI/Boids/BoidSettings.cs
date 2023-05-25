using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Boid Settings", menuName ="ScriptableObject/Boid Settings")]
public class BoidSettings : ScriptableObject
{
    public float MinSpeed = 2;
    public float MaxSpeed = 5;
    [Header("������ �ν��ϴ� ����")]
    public float PerceptionRadius = 2.5f;
    [Header("�浹�� ���ϱ� ���� �ν��ϴ� ����")]
    public float AvoidanceRadius = 1;
    [Header("���� ������ ��")]
    public float MaxSteerForce = 3;


    //����ġ�� �������� �ش� Ư�� ����

    [Header("������ ��� �̵� ������ ���󰡿�")]
    public float AlignWeight = 1;
    [Header("������ �߽� ��ġ�� ���� �𿩿�")]
    public float CohesionWeight = 1;
    [Header("�浹�� ���ϴ°� �߿��ؿ�")]
    public float SeperateWeight = 1;
    [Header("��ǥ ������ ������ �����ϴ°� �߿��ؿ�")]
    public float TargetWeight = 1;


    [Header("�浹 ��� ���̾�")]
    public LayerMask ObstacleMask;
    
    //��� �ݰ�, �浹 ȸ�ǿ� ����
    public float BoundsRadius = .27f;
    //�浹�� ���ϱ� ���� ����ġ
    public float AvoidCollisionWeight = 10;
    //�浹�� �����ϱ� ���� �Ÿ�
    public float CollisionAvoidDst = 5;
}
