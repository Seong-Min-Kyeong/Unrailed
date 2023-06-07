using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Stack : MonoBehaviour
{
    public Stack<AI_StackItem> _handItem = new Stack<AI_StackItem>();
    public Stack<AI_StackItem> _detectedItem = new Stack<AI_StackItem>();
    [SerializeField] private LayerMask BlockLayer;

    private Transform _currentblock;


    private void Awake()
    {

        _handItem = new Stack<AI_StackItem>();
        _detectedItem = new Stack<AI_StackItem>();
    }

    //Helper
    public void InteractiveItem()
    {
        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // 처음 줍기
        {

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    public void InteractiveItemAuto()
    {
        if (_detectedItem.Count != 0 && _handItem.Count != 0) //두 번째부터는 자동 줍기
        {
            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().AutoGain(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    public void PutDown()
    {
        if (_handItem.Count != 0 && _detectedItem.Count == 0) // 내려놓기
        {
            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;

            _detectedItem.Clear();
        }

    }

    //Enemy
    public void EnemyThrowResource()
    {
        if (_handItem.Count != 0 && _detectedItem.Count == 0) // 버리기
        {

            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().EnemyThrowResource(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;

            _detectedItem.Clear();
            _handItem.Clear();
        }

    }


    public void EnemyPutDown()
    {
        if (_handItem.Count != 0 && _detectedItem.Count == 0) // 내려놓기
        {
            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().EnemyPutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;

            _detectedItem.Clear();
        }

    }



    public void EnemyInteractiveItem()
    {
        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // 처음 줍기
        {
            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _detectedItem.Peek().EnemyPickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    public void EnemyInteractiveAuto()
    {
        if (_detectedItem.Count != 0 && _handItem.Count != 0) //두 번째부터는 자동 줍기
        {
            Pair<Stack<AI_StackItem>, Stack<AI_StackItem>> p = _handItem.Peek().EnemyAutoGain(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }



    public void EnemyDetectGroundBlock(WorldResource resource)
    {
        _detectedItem.Push(resource.Stack);
        Destroy(resource);
    }


    public void DetectGroundBlock(WorldResource resource)
    {
        _detectedItem.Push(resource.Stack);
        Destroy(resource);
    }


    public Transform BFS(BaseAI _ai)
    {
        if (Physics.Raycast(_ai.RayStartTransfrom.position, Vector3.down, out RaycastHit hit, 1, BlockLayer))
        {
            _currentblock = hit.transform;
        }

        Queue<Transform> queue = new Queue<Transform>();
        HashSet<Transform> hashSet = new HashSet<Transform>();

        if(_currentblock!=null)
        {
            queue.Enqueue(_currentblock);
            hashSet.Add(_currentblock);
        }

        Vector3[] dir = new Vector3[8] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left,
        new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1)};

        while (queue.Count != 0)
        {
            Transform currentBlock = queue.Dequeue();

            if (currentBlock.childCount == 0)
                return currentBlock;

            for (int i = 0; i < 8; i++)
            {
                if (Physics.Raycast(currentBlock.position, dir[i], out RaycastHit hot, 1f, BlockLayer))
                {
                    if (hashSet.Add(hot.transform))
                    {
                        queue.Enqueue(hot.transform);
                    }
                }
            }
        }

        return null;
    }
}

