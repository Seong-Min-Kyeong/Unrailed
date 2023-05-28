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

    public List<GameObject> stackItem = new List<GameObject>();//���� ���� ������
    public GameObject nonStackItem;// ���� ���� ������
    //public GameObject currentItem = null;
    public GameObject detectedItem = null;
    //public GameObject detectedItem;

    //private Item item;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator.SetBool("isWalk", false);
        nonStackItem = null;
        //item = GetComponent<Item>();
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
            Debug.Log("");
            RaycastHit hit;
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f))
            {
                Debug.Log(hit.transform.gameObject.name);
                //item = hit.collider.GetComponent<Item>();
                GameObject hitItem = hit.transform.gameObject;
                //Item hitItem = hit.collider.GetComponent<Item>();
                if (hitItem != null)
                {
                    if (hitItem.tag == "StackItem" && stackItem.Count == 0)
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
            Debug.Log("��ü�ϰ� �־��~");
            //����� ��
            /*
             * ���� ������ / �ٴ� ������
             * Stack       / Stack    -> ��ü, �ٴ� ������ ������(rightHand.position)����, ���� ������ �ٴ�(transform.position)����
             * NonStack    / Stack    -> ��ü, �ٴ� ������ ����������, ���� ������ �ٴ����� [�Ϸ�]
             * Stack       / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ�����
             * NonStack    / NonStack -> ��ü, �ٴ� ������ ��� ���̷�, ���� ������ �ٴ����� [�Ϸ�]
             * Stack       / Block    -> ��������, �����տ��� �ٴ�����
             * NonStack    / Block    -> ��������, ��տ��� �ٴ�����
             * 
             */


            // �ٴ� ������ �������� if�� ������.
            RaycastHit hit;
            // �ٴڿ� �ִ� �������� ���̾ NonStack�� ��� -> ��ü
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, NonStackItemLayer))
            {
                Debug.Log("NonStackItem ����");
                // ���� ������ : Stack / NonStack
                Debug.Log(hit.transform.gameObject.name + "-> ����ĳ��Ʈ�� ���� ��");
                if (hit.collider)
                {
                    if (nonStackItem.tag == "StackItem")
                    {
                        /*Debug.Log($"�� : {nonStackItem.name} - �� : {detectedItem.name} ��ü ��!!");
                        SetItemOnBlock(nonStackItem);
                        //nonStackItem = hit.transform.gameObject;
                        SetItemInRightHand(detectedItem);
                        Debug.Log($"�� : {nonStackItem.name} - �� : {detectedItem.name} ��ü ��!!");*/

                    }
                    else if (nonStackItem.tag == "NonStackItem")
                    {
                        SetItemOnBlock(nonStackItem);
                        SetItemInRightHand(hit.transform.gameObject);
                    }
                }
            }


            // �ٴڿ� �ִ� �������� ���̾ Stack�� ��� -> ��ü
            RaycastHit[] hits = Physics.RaycastAll(rayStart.position, Vector3.down, 10f, StackItemLayer);
            if (hits.Length > 0)
            {
                Debug.Log("StackItem ����");
                // ���� ������ : Stack / NonStack

                if (nonStackItem.tag == "StackItem")
                {
                    // ���� ������ : ���� / �ٴ� ������ : ����
                    // ���� ������ �ٴ����� ����
                    //SetItemOnBlock(nonStackItem);
                    // �ٴ� ������ ��� ���̷� ����
                    //SetItemBetweenTwoHands();



                }
                if (nonStackItem.tag == "NonStackItem")
                {
                    Debug.Log("������ ö�� ��ȯ�ϰ� �;��.");
                    SetItemOnBlock(nonStackItem);
                    SetItemBetweenTwoHands(hits);
                    
                }
            }


            // �ٴڿ� �ִ� �������� ���̾ Block�� ��� -> ��������
            if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
            {
                Debug.Log("�ٴڿ� �������� ����");
                if (nonStackItem == null && stackItem.Count == 0)
                {
                    Debug.Log("������ �������� ��� return �ҰԿ�.");
                    return;
                }
                else if (nonStackItem != null)
                {
                    Debug.Log("NonStackItem �����Կ�.");
                    SetItemOnBlock(nonStackItem);
                    isHaveItem = false;
                }
                else if (stackItem.Count != 0)
                {
                    Debug.Log("StackItem �����Կ�.");
                    SetItemOnBlock(stackItem);
                    
                    // ���� - ö, ����ó��
                    isHaveItem = false;
                }
            }
            
       
        }
    }


    void SetItemInRightHand(GameObject Item)
    {
        Item.transform.SetParent(rightHand);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        nonStackItem = Item;
    }


    void SetItemBetweenTwoHands(GameObject Item)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = Vector3.zero;
        Item.transform.localRotation = Quaternion.identity;
        stackItem.Add(Item);
    }


    void SetItemBetweenTwoHands(GameObject Item, float GapOfItems)
    {
        Item.transform.SetParent(betweenTwoHands);
        Item.transform.localPosition = new Vector3(0, GapOfItems * (stackItem.Count), 0);
        Item.transform.localRotation = Quaternion.identity;
        stackItem.Add(Item);
    }


    // ���� ���� ������
    void SetItemBetweenTwoHands(RaycastHit[] Item)
    {
        for (int i = 0; i < Item.Length; i++)
        {
            GameObject itemObject = Item[i].transform.gameObject;
            itemObject.transform.SetParent(betweenTwoHands);
            //itemObject.transform.localPosition = (Vector3.up * 0.5f) + (Vector3.up * GapOfItems * i);
            itemObject.transform.localPosition = new Vector3(0, GapOfItems * i, 0);
            itemObject.transform.localRotation = Quaternion.identity;
            stackItem.Add(itemObject.transform.gameObject);
        }
        //Item = new List<GameObject>(); //[����]
    }


    void SetItemOnBlock(GameObject Item)
    {
        RaycastHit hit;
        if(Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            Item.transform.SetParent(hit.transform);
            Item.transform.localPosition = new Vector3(0, 0.5f, 0);
            Item.transform.localRotation = Quaternion.identity;
            nonStackItem = null;
        }
    }
    void SetItemOnBlock(List<GameObject> Item)
    {
        RaycastHit hit;
        if (Physics.Raycast(rayStart.position, Vector3.down, out hit, 10f, BlockLayer))
        {
            for(int i=0; i< Item.Count; i++)
            {
                Debug.Log("��� ���� ������");
                Item[i].transform.SetParent(hit.transform);
                Item[i].transform.localPosition = new Vector3(0, (GapOfItems*i) + 0.5f, 0);
                Item[i].transform.localRotation = Quaternion.identity;
            }
            stackItem = new List<GameObject>(); //[����]
        }
    }

 

    private void OnTriggerEnter(Collider other)
    {
        //const string STACK_ITEM_TAG = "StackItem"; 
        if (other.tag == "StackItem")
        {
            if (stackItem.Count > 0 && stackItem.Count < 3)
            {
                SetItemBetweenTwoHands(other.gameObject, GapOfItems);
            }
        }
    }








    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "StacKItem" || other.tag == "NonStackItem")
        {
            detectedItem = other.gameObject;
        }

        item = other.GetComponent<Item>();
        //Item item = other.GetComponent<Item>();
        if (item != null)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 10.0f, StackItemLayer))
            {
                if (stackItem.Count > 0 && stackItem.Count < 3)
                {
                    stackItem.Add(gameObject);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "StacKItem" || other.tag == "NonStackItem")
        {
            if (detectedItem == other.gameObject)
                detectedItem = null;
        }
    }*/




    /*void DigUp()
    {
        RaycastHit hit;
        Debug.DrawRay(rayStart.position, transform.forward * pickUpDistance, Color.red);
        if (Physics.Raycast(rayStart.position, transform.forward, out hit, pickUpDistance))
        {
            IDig target = hit.collider.GetComponent<IDig>();
            if (target != null)
            {
                // item�� null�� �� ��츦 �����ؾ� �� 
                if (hit.transform.name == "Tree01(Clone)" && item.name == "ItemAxe")
                {
                    target.OnDig(hit.point);
                }
            }
        }
    }*/


}