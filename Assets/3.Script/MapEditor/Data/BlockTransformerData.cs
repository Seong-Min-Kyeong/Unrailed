using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BlockTransformerData : ScriptableObject
{
    // �߾����� �� ���� ��������??
    public bool isHeight;
    public GameObject originBlock;
    public GameObject[] transformedBlock;
}
