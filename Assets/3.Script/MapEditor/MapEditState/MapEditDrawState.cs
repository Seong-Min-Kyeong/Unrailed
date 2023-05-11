using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawState : BaseMapEditState
{
    public MapEditDrawState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Click()
    {

    }

    public override void Enter()
    {
        Debug.Log("���Խ��ϴ�.");

        // ���� �� ������ ����ߴ� ���� ������ (������ 0)
        _content.SelectBlock(_content.CurrentBlockIndex);
    }

    public override void Exit()
    {
        Debug.Log("�������ϴ�.");
    }

    public override void Update()
    {
        Debug.Log("�������Դϴ�.");

        _content.InputBlock();

        // ������ ���� ��� �� ���� ��� �׸��忡 ���õ� block�� �� �����Ѵ�.
        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            // ���� �̹� ���� �ִٸ� �Ұ�
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            int x = (int)hitAroundPoint.x + _content.MinX;
            int y = (int)hitAroundPoint.z + _content.MinY;

            if (_content.mapData[y, x] != 0)
                return;

            _content.selectedBlock.transform.position = hitAroundPoint;

            // �� ����
            if (Input.GetMouseButtonDown(0))
            {
                _content.mapData[y, x] = _content.CurrentBlockIndex + 1;
                _content.selectedBlock.SetPos(x, y);
                _content.selectedBlock.transform.position = hitAroundPoint;
                _content.selectedBlock = null;
                _content.SelectBlock(_content.CurrentBlockIndex);
            }
        }
    }
}
