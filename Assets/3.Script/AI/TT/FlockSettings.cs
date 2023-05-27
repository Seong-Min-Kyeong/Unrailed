using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flock Settings", menuName = "ScriptableObject/Flock Settings")]
public class FlockSettings : ScriptableObject
{
    [Header("������ �ν��ϴ� ����")]
    public float PerceptionRadius = 2.5f;
    [Header("�浹�� ���ϱ� ���� �ν��ϴ� ����")]
    public float AvoidanceRadius = 1;


    [Header("�浹 ��� ���̾�")]
    public LayerMask ObstacleMask;

    [Header("ȸ�� �ݰ�")]
    public float BoundsRadius = .27f;
    [Header("�浹 ���� �Ÿ�")]
    public float CollisionAvoidDst = 5;


}
