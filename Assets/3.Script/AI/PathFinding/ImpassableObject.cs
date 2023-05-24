using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpassableObject : MonoBehaviour
{
    // ToDo : 20230524
    // ����1, 2, ��, ö, ����, empty prefab���ٰ� �� ImpassableObject�־�� ��

    // PathFindingField�� awake���� �����ǹǷ� start���� ����
    private void Start()
    {
        PathFindingField.Instance.UpdateMapData(transform.position.x, transform.position.z, false);
    }

    // �� ��ü�� �ı��Ǹ� PathFindingField���� �ش� ��ǥ�� �� �� �ִ°����� ����
    private void OnDestroy()
    {
        PathFindingField.Instance.UpdateMapData(transform.position.x, transform.position.z, true);
    }
}
