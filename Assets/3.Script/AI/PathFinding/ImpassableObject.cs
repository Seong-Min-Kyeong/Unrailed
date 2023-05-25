using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpassableObject : MonoBehaviour
{
    // ToDo : 20230524
    // ����1, 2, ��, ö, ����, empty prefab���ٰ� �� ImpassableObject�־�� ��

    // PathFindingField�� awake���� �����ǹǷ� start���� ����
    public Vector3 originPos;

    private void Start()
    {
        originPos = new Vector3(transform.position.x, 0, transform.position.z);

        if(PathFindingField.Instance != null)
            PathFindingField.Instance.UpdateMapData(originPos.x, originPos.z, false);
    }

    // �� ��ü�� �ı��Ǹ� PathFindingField���� �ش� ��ǥ�� �� �� �ִ°����� ����
    private void OnDestroy()
    {
        if(PathFindingField.Instance != null)
            PathFindingField.Instance.UpdateMapData(originPos.x, originPos.z, true);
    }
}
