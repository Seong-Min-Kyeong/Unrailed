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
        if (Physics.Raycast(_content.MainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Block")))
        {
            // ���� �̹� ���� �ִٸ� ����
            _content.prevBlock = _content.selectedBlock;
            _content.selectedBlock = hit.transform.GetComponent<Block>();

            // ���� Block ������Ʈ�� ���ٸ� �׳� return
            if (_content.selectedBlock == null)
            {
                Debug.Log("Block ������Ʈ�� �����ϴ�.");
                return;
            }


            if (_content.mapData[_content.selectedBlock.Y, _content.selectedBlock.X] != 0)
            {
                if (_content.selectedBlock != _content.prevBlock)
                {
                    if (_content.prevBlock != null)
                        _content.prevBlock.NonSelectBlock();
                    _content.selectedBlock.SelectBlock();
                }

                // �� �����
                if (Input.GetMouseButtonDown(0))
                {
                    _content.mapData[_content.selectedBlock.Y, _content.selectedBlock.X] = 0;
                    _content.DestroyBlock();
                }
            }
        }
    }
}
