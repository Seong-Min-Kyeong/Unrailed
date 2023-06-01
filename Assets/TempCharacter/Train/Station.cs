using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    private Transform _parentBlock;
    private Vector3[] _dir = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.left, Vector3.back };

    public void InitStation(bool isStart = false)
    {
        _parentBlock = transform.parent;

        if (isStart)
            InitFirstStation();
        else
            InitNonFirstStation();
    }

    private void InitFirstStation()
    {
        // �� ó�� ������ �տ� ���� ���� �ִ� ���ϸ� �α�
        Transform railTransform = null;
        RailController rail = null;

        if (Physics.Raycast(_parentBlock.position, Vector3.back, out RaycastHit hit, 1f, 1 << LayerMask.NameToLayer("Block")))
        {
            if(hit.transform.childCount != 0)
            {
                railTransform = hit.transform.GetChild(0);
            }
        }

        while(true)
        {
            rail = railTransform.GetComponent<RailController>();
            rail.isInstance = true;

            if(Physics.Raycast(railTransform.position, Vector3.right, out RaycastHit railHit, 1f, 1 << LayerMask.NameToLayer("Rail")))
            {
                railTransform = railHit.transform;
            }
            else
            {
                break;
            }
        }

        if (rail != null)
        {
            rail.PutRail();
        }
    }

    private void InitNonFirstStation()
    {
        // �տ� ���ϵ��� endRail ���ֱ�! ������
        Transform railTransform = null;
        RailController rail = null;

        if (Physics.Raycast(_parentBlock.position, Vector3.back, out RaycastHit hit, 1f, 1 << LayerMask.NameToLayer("Block")))
        {
            if (hit.transform.childCount != 0)
            {
                railTransform = hit.transform.GetChild(0);
            }
        }

        while (true)
        {
            rail = railTransform.GetComponent<RailController>();
            rail.isEndRail = true;
            rail.isInstance = true;

            if (Physics.Raycast(railTransform.position, Vector3.right, out RaycastHit railHit, 1f, 1 << LayerMask.NameToLayer("Rail")))
            {
                railTransform = railHit.transform;
            }
            else
            {
                break;
            }
        }
    }
}
