using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour
{

    #region ������ �ֿ��� �� ��ġ
    [SerializeField] public Transform rightHand;//[����] : Ʈ�������� �ǰ� ���ӿ�����Ʈ�� �� �ȵ�?..
    [SerializeField] public Transform betweenTwoHands;
    [SerializeField] public Transform rayStart;
    [SerializeField] public float DropDistance = 1.5f;
    #endregion
    #region �̵� ���� ����
    public float speed = 5f;
    float xAxis;
    float zAxis;
    bool isDash;
    Vector3 moveVec;
    [SerializeField] float dashCool = 0.3f;
    #endregion
    #region ������ �Ⱦ� ���� ����
    [SerializeField] float GapOfItems = 0.25f;
    [SerializeField] float pickUpDistance;
    [SerializeField] LayerMask BlockLayer;
    [SerializeField] LayerMask StackItemLayer;
    [SerializeField] LayerMask NonStackItemLayer;
    public bool isHaveItem;
    #endregion
    #region ��ư�� �����°�?
    bool isDashKeyDown;
    public bool isGetItemKeyDown;
    #endregion
    #region ������Ʈ ����
    private Rigidbody _rigidbody;
    private Animator _animator;
    private Item item;
    #endregion
    public List<GameObject> currentStackItem = new List<GameObject>();
    public GameObject currentNonStackItem;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator.SetBool("isWalk", false);
        currentNonStackItem = null;
        isGetItemKeyDown = false;
    }
    void Update()
    {
        GetInput();
        Walk();
        Turn();
        Dash();
        FirstPickUpItem();
        DropItem();
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        zAxis = Input.GetAxisRaw("Vertical");
        isDashKeyDown = Input.GetButtonDown("Dash");
        isGetItemKeyDown = Input.GetButtonDown("getItem");
    }

    void Walk()
    {
        if (!isDash) moveVec = new Vector3(xAxis, 0, zAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
        _animator.SetBool("isWalk", moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(moveVec + transform.position);
    }

    void Dash()
    {
        if (isDashKeyDown && !isDash)
        {

            isDash = true;
            speed *= 2;
            Invoke("DashOff", dashCool);
        }
    }

    void DashOff()
    {
        speed *= 0.5f;
        isDash = false;
    }

    public void FirstPickUpItem()
    {

        if (isGetItemKeyDown && !isHaveItem)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f))
            {
                GameObject hitItem = hit.transform.gameObject;
                if (hitItem != null)
                {
                    if (hitItem.tag == "StackItem" && currentStackItem.Count == 0)
                    {
                        SetItemBetweenTwoHands(hitItem);
                        isHaveItem = true;
                    }
                    else if (hitItem.tag == "NonStackItem")
                    {
                        SetItemInRightHand(hitItem);
                        isHaveItem = true;
                    }
                }
            }
            isGetItemKeyDown = false;
        }
    }
    public void DropItem()
    {
        if (isGetItemKeyDown && isHaveItem)
        {
            #region DropItem ����� ��
            /*
             * ���� ������ / �ٴ� ������
             * Stack       / Stack    -> ��ü, �ٴ� ������ ������(rightHand.position)����, ���� ������ �ٴ�(transform.position)����
             * NonStack    / Stack    -> ��ü, �ٴ� ������ ����������, ���� ������ �ٴ����� [�Ϸ�]
             * Stack       / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����
             * NonStack    / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ����� [�Ϸ�]
             * Stack       / Block    -> ��������, �����տ��� �ٴ�����
             * NonStack    / Block    -> ��������, ��տ��� �ٴ�����
             */
            #endregion

            RaycastHit hit;
            RaycastHit[] hits = Physics.RaycastAll(rayStart.position, Vector3.down, 10f, StackItemLayer);

            // �ٴ� ������ ���̾� : NonStack
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, NonStackItemLayer))
            {
                GameObject rayHitNonStackItem = hit.transform.gameObject;
                //NonStack / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����[�Ϸ�]
                if (currentNonStackItem != null)
                {
                    SetItemOnBlock(currentNonStackItem);
                    SetItemInRightHand(rayHitNonStackItem);
                }
                //Stack / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����
                else if(currentStackItem.Count !=0)
                {
                    SetItemOnBlock(currentStackItem);
                    SetItemInRightHand(rayHitNonStackItem);
                }
            }

            // �ٴڿ� �ִ� �������� ���̾ Stack�� ��� -> ��ü
            else if (hits.Length > 0)
            {
                if (currentNonStackItem != null)
                {
                    SetItemOnBlock(currentNonStackItem);
                    SetItemBetweenTwoHands(hits);
                }
                else if (currentStackItem.Count != 0)
                {
                    SetItemOnBlock(currentStackItem);
                    SetItemBetweenTwoHands(hits);
                }
                
            }

            // �ٴڿ� �ִ� �������� ���̾ Block�� ��� -> ��������
            else if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
            {
                if (currentNonStackItem == null && currentStackItem.Count == 0)
                {
                    Debug.Log("������ �������� ��� return �ҰԿ�.");
                    return;
                }
                else if (currentNonStackItem != null)
                {
                    SetItemOnBlock(currentNonStackItem);
                    isHaveItem = false;
                }
                else if (currentStackItem.Count != 0)
                {
                    SetItemOnBlock(currentStackItem);
                    isHaveItem = false;
                }
            }

        }
    }

    #region ������ ��ġ -> ������
    void SetItemInRightHand(GameObject Item)
    {
        Item.transform.SetParent(rightHand);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        currentNonStackItem = Item;
    }
    #endregion
    #region ������ ��ġ -> ��� ����
    void SetItemBetweenTwoHands(GameObject Item)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        currentStackItem.Add(Item);
    }
    
    void SetItemBetweenTwoHands(GameObject Item, float GapOfItems)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = new Vector3(0, GapOfItems * (currentStackItem.Count), 0);
        Item.transform.localRotation = Quaternion.identity;
        currentStackItem.Add(Item);
    }

    void SetItemBetweenTwoHands(RaycastHit[] Item)
    {
        for (int i = 0; i < Item.Length; i++)
        {
            GameObject itemObject = Item[i].transform.gameObject;
            itemObject.transform.SetParent(betweenTwoHands);
            //itemObject.transform.localPosition = (Vector3.up * 0.5f) + (Vector3.up * GapOfItems * i);
            itemObject.transform.localPosition = new Vector3(0, GapOfItems * i, 0);
            itemObject.transform.localRotation = Quaternion.identity;
            currentStackItem.Add(itemObject.transform.gameObject);
        }
    }
    #endregion
    #region ������ ��ġ -> �ٴ�
    void SetItemOnBlock(GameObject Item)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            Item.transform.SetParent(hit.transform);
            Item.transform.localPosition = new Vector3(0, 0.5f, 0);
            Item.transform.localRotation = Quaternion.identity;
            currentNonStackItem = null;
        }
    }
    void SetItemOnBlock(List<GameObject> Item)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            for (int i = 0; i < Item.Count; i++)
            {
                Item[i].transform.SetParent(hit.transform);
                Item[i].transform.localPosition = new Vector3(0, (GapOfItems * i) + 0.5f, 0);
                Item[i].transform.localRotation = Quaternion.identity;
            }
            currentStackItem = new List<GameObject>(); //[����]
        }
    }
    #endregion
    #region ���� ������ �ڵ� ����
    private void OnTriggerEnter(Collider other)
    {
        //const string STACK_ITEM_TAG = "StackItem"; 
        /*if (other.tag == "StackItem")
        {*/
            if (currentStackItem.Count > 0 && currentStackItem.Count < 3)
            {
                if (currentStackItem[0].gameObject.name == other.gameObject.name)
                {
                    SetItemBetweenTwoHands(other.gameObject, GapOfItems);
                }
            }
        /*}*/
    }
    #endregion
}