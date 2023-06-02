using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Stack : MonoBehaviour
{
    public Stack<AI_StackItem> _handItem = new Stack<AI_StackItem>();
    private Stack<AI_StackItem> _detectedItem = new Stack<AI_StackItem>();

    public LayerMask BlockLayer;

    HelperBT _helper;


    private void Awake()
    {
        _helper = GetComponent<HelperBT>();

        _handItem = new Stack<AI_StackItem>();
        _detectedItem = new Stack<AI_StackItem>();
    }

    public void InteractiveItemSpace()
    {
        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // �ݱ�
        {
            Debug.Log("�ݱ�");

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if (_handItem.Count != 0 && _detectedItem.Count == 0) // ������
        {
            Debug.Log("������");

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if (_handItem.Count != 0 && _detectedItem.Count != 0) // ��ü
        {
            Debug.Log("��ü");

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().Change(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }
    public void InteractiveItem()
    {
        if (_detectedItem.Count != 0&&_handItem.Count!=0)
        {
            Debug.Log("�ʹٴ� �ݱ�");

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().AutoGain(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }
    public void DetectGroundBlock(WorldResource resource)
    {
/*        if (Physics.Raycast(helper.RayStartTransfrom.position, Vector3.down, out RaycastHit hit, 2, BlockLayer))
        {
            // ĳ��
            if (_currentblock == hit.transform)
                return;

            _currentblock = hit.transform;
            _detectedItem = new Stack<MyItem>();
            for (int i = 0; i < _currentblock.childCount; i++)
            {
                MyItem item = _currentblock.GetChild(i).GetComponent<MyItem>();
                if (item != null)
                    _detectedItem.Push(item);
            }
        }*/


        //_detectedItem = new Stack<AI_StackItem>();
        _detectedItem.Push(resource.GetComponent<AI_StackItem>());
/*        for (int i = 0; i < _helper.CurrentBlockTransform.childCount; i++)
            {
                AI_StackItem item = _helper.CurrentBlockTransform.GetChild(i).GetComponent<AI_StackItem>();
                if (item != null)
                    _detectedItem.Push(item);
            }*/
    }
}

