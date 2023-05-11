using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditDrawState : BaseMapEditState
{
    private Block prevBlock;
    private Block currentBlock;

    public MapEditDrawState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Click()
    {

    }

    public override void Enter()
    {
        // ���� �� ������ ����ߴ� ���� ������ (������ 0)
        _content.SelectBlock(_content.CurrentBlockIndex);
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        _content.InputBlock();

        // ������ ���� ��� �� ���� ��� �׸��忡 ���õ� block�� �� �����Ѵ�.
        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            // ���� �̹� ���� �ִٸ� �Ұ�
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            int x = (int)hitAroundPoint.x + _content.MinX;
            int y = (int)hitAroundPoint.z + _content.MinY;

            // ���� �̹� �ִ� ��
            if (_content.mapData[y, x] != null)
            {
                prevBlock = currentBlock;
                currentBlock = _content.mapData[y, x];

                if(prevBlock != null && prevBlock != currentBlock)
                {
                    prevBlock.gameObject.SetActive(true);
                    currentBlock.gameObject.SetActive(false);
                }
            }
            else
            {
                if(currentBlock != null)
                    currentBlock.gameObject.SetActive(true);
            }

            _content.selectedBlock.transform.position = hitAroundPoint;

            // �� ����
            if (Input.GetMouseButtonDown(0))
            {
                // ���� �̹� �ִ� ���̸� �� ���ְ� ���� �׸�
                if (_content.mapData[y, x] != null)
                {
                    GameObject.Destroy(_content.mapData[y, x]);
                    _content.mapData[y, x] = null;
                }

                _content.mapData[y, x] = _content.selectedBlock;
                _content.selectedBlock.SetPos(x, y);
                _content.selectedBlock.transform.position = hitAroundPoint;
                _content.selectedBlock = null;
                _content.SelectBlock(_content.CurrentBlockIndex);
            }
        }
    }
}
