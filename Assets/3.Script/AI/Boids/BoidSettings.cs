using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Boid Settings", menuName ="ScriptableObject/Boid Settings")]
public class BoidSettings : ScriptableObject
{
    public float minSpeed = 2;
    public float maxSpeed = 5;
    [Header("������ �ν��ϴ� ����")]
    public float perceptionRadius = 2.5f;
    [Header("�浹�� ���ϱ� ���� �ν��ϴ� ����")]
    public float avoidanceRadius = 1;
    [Header("�������� ������ �ִ� ���� ũ��")]
    public float maxSteerForce = 3;


    //����ġ�� �������� �ش� Ư�� ����

    [Header("������ ��� �̵� ������ ���󰡿�")]
    public float alignWeight = 1;
    [Header("������ �߽� ��ġ�� ���� �𿩿�")]
    public float cohesionWeight = 1;
    [Header("�浹�� ���ϴ°� �߿��ؿ�")]
    public float seperateWeight = 1;
    [Header("��ǥ ������ ������ �����ϴ°� �߿��ؿ�")]
    public float targetWeight = 1;


    [Header("�浹 ��� ���̾�")]
    public LayerMask obstacleMask;
    
    //��� �ݰ�, �浹 ȸ�ǿ� ����
    public float boundsRadius = .27f;
    //�浹�� ���ϱ� ���� ����ġ
    public float avoidCollisionWeight = 10;
    //�浹�� �����ϱ� ���� �Ÿ�
    public float collisionAvoidDst = 5;
}
