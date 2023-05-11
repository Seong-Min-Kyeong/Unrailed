using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditEraseState : BaseMapEditState
{
    public MapEditEraseState(MapEditor mapEditor, MapEditStateFactory stateFactory) : base(mapEditor, stateFactory)
    {
    }

    public override void Click()
    {

    }

    public override void Enter()
    {
        // ���õ� �� ����
        _content.DestroyBlock();
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        // �� ����� ����
        // _content.EraseBlock();

        RaycastHit hit;
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("BaseBlock")))
        {
            Vector3 hitAroundPoint = new Vector3(Mathf.RoundToInt(hit.point.x), 1, Mathf.RoundToInt(hit.point.z));

            int x = (int)hitAroundPoint.x + _content.MinX;
            int y = (int)hitAroundPoint.z + _content.MinY;

            // ���� �̹� ���� �ִٸ� ������
            if (_content.mapData[y, x] != null)
            {
                _content.prevBlock = _content.selectedBlock;
                _content.selectedBlock = _content.mapData[y, x];

                if(_content.selectedBlock != _content.prevBlock)
                {
                    if (_content.prevBlock != null)
                        _content.prevBlock.NonSelectBlock();
                    _content.selectedBlock.SelectBlock();
                }

                // �� �����
                if (Input.GetMouseButtonDown(0))
                {
                    _content.mapData[_content.selectedBlock.Y, _content.selectedBlock.X] = null;
                    _content.DestroyBlock();
                }
            }
            else
            {
                if(_content.selectedBlock != null)
                    _content.selectedBlock.NonSelectBlock();
            }
        }
    }
}
