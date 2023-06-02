using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private Transform _rayStartTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _twoHandTransform;

    // ����  => �̰� ��������??
    private bool _isDash = false;

    // ������Ʈ
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private PlayerStat _playerStat;

    // ����
    // ������ �ٸ��� �ȵ�
    private Stack<MyItem> _handItem = new Stack<MyItem>();
    private Stack<MyItem> _detectedItem = new Stack<MyItem>();

    // �÷��̾� ��ġ
    private float _currentSpeed;

    // ���� �� �ִ� ��
    private Transform _currentblock;
    // ���濡 �ִ� ������Ʈ
    private Transform _currentFrontObject;


    public List<MyItem> handItemList = new List<MyItem>();
    public List<MyItem> detectedItemList = new List<MyItem>();

    public List<string> handItemGameObjectList = new List<string>();
    public List<string> detectedItemGameObjectList = new List<string>();


    // ������Ƽ
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;
    public Transform CurrentBlockTransform => _currentblock;
    public Transform AroundEmptyBlockTranform => BFS();


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();


        _playerStat = GetComponent<PlayerStat>();

        InitPlayer();
    }

    private void InitPlayer()
    {
        _isDash = false;
        _currentSpeed = _playerStat.moveSpeed;

        _handItem = new Stack<MyItem>();
        _detectedItem = new Stack<MyItem>();
    }

    private void FixedUpdate()
    {
        // �÷��̾� ������
        Move();
    }

    private void Update()
    {
        // ���� ��, ���� ��ü ����
        DetectGroundBlock();
        DetectFrontObject();

        // ������ ��ȣ�ۿ�
        if (_playerInput.IsSpace)
            InteractiveItemSpace();
        InteractivItem();

        // ȯ�� ��ȣ�ۿ�
        InteractiveEnvironment();

        // ���� ��ȣ�ۿ�
        InteractiveTrain();

        // ��������
        handItemList = _handItem.ToList();
        detectedItemList = _detectedItem.ToList();

        handItemGameObjectList = new List<string>();
        detectedItemGameObjectList = new List<string>();

        foreach (var go in handItemList)
            handItemGameObjectList.Add(go.ToString());

        foreach (var go in detectedItemList)
            detectedItemGameObjectList.Add(go.ToString());
        // ���� Ŭ�� ����

/*        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(AroundEmptyBlockTranform.position);
        }*/
    }

    private void Move()
    {
        // ������, ȸ��, ��ñ���
        if(_playerInput.IsShift && !_isDash)
        {
            _isDash = true;
            _currentSpeed = _playerStat.dashSpeed;
            Invoke("DashOff", _playerStat.dashDuration);
        }

        transform.position += _playerInput.Dir * _currentSpeed * Time.deltaTime;
        transform.LookAt(_playerInput.Dir + transform.position);
    }

    private void DashOff()
    {
        _currentSpeed = _playerStat.moveSpeed;
        _isDash = false;
    }

    // spacebar ���� ��
    private void InteractiveItemSpace()
    {
        if (_handItem.Count == 0 && _detectedItem.Count != 0)  // �ݱ�
        {
            Debug.Log("�ݱ�");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _detectedItem.Peek().PickUp(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if(_handItem.Count != 0 && _detectedItem.Count == 0) // ������
        {
            Debug.Log("������");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().PutDown(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
        else if(_handItem.Count != 0 && _detectedItem.Count != 0) // ��ü
        {
            Debug.Log("��ü");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().Change(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    // �� ���� ��
    private void InteractivItem()
    {
        if(_handItem.Count != 0 && _detectedItem.Count != 0)
        {
            Debug.Log("�ʹٴ� �ݱ�");

            Pair<Stack<MyItem>, Stack<MyItem>> p = _handItem.Peek().AutoGain(_handItem, _detectedItem);
            _handItem = p.first;
            _detectedItem = p.second;
        }
    }

    private void InteractiveEnvironment()
    {

    }

    private void InteractiveTrain()
    {

    }

    private void DetectGroundBlock()
    {
        if (Physics.Raycast(_rayStartTransform.position, Vector3.down, out RaycastHit hit, _playerStat.detectRange, _playerStat.blockLayer))
        {
            // ĳ��
            if (_currentblock == hit.transform)
                return;

            _currentblock = hit.transform;
            _detectedItem = new Stack<MyItem>();
            for(int i = 0; i < _currentblock.childCount; i++)
            {
                MyItem item = _currentblock.GetChild(i).GetComponent<MyItem>();
                if (item != null)
                    _detectedItem.Push(item);
            }
        }
    }

    private void DetectFrontObject()
    {
        if (Physics.Raycast(_rayStartTransform.position, transform.forward, out RaycastHit hit, _playerStat.detectRange))
        {
            // ĳ��
            if (_currentFrontObject == hit.transform)
                return;
            _currentFrontObject = hit.transform;
        }
    }

    private Transform BFS()
    {
        // CurrentBlockTransform => _currentblock
        // ���� ������ ���� ����� �ڽ��� ���� ���� ��ȯ����

        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(_currentblock);

        Vector3[] dir = new Vector3[8] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, 
        new Vector3(1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), new Vector3(-1, 0, 1)};

        HashSet<Transform> hashSet = new HashSet<Transform>();
        hashSet.Add(_currentblock);

        while(queue.Count != 0)
        {
            Transform currentBlock = queue.Dequeue();

            if (currentBlock.childCount == 0)
                return currentBlock;

            for (int i = 0; i < 8; i++)
            {
                if(Physics.Raycast(currentBlock.position, dir[i], out RaycastHit hit, 1f, _playerStat.blockLayer))
                {
                    if(hashSet.Add(hit.transform))
                    {
                        queue.Enqueue(hit.transform);
                    }
                }
            }
        }

        return null;
    }
}
