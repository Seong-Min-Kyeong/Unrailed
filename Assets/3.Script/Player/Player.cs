using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // ����


    //0521 todo ��ȯ�� �̵��� ��ó���� �������
    #region ������ �ֿ��� �� ��ġ
    public Transform rightHand;
    public Transform betweenTwoHands;
    public Transform rayStart;
    #endregion
    #region �̵� ���� ����
    public float speed = 30f;
    float xAxis;
    float zAxis;
    Vector3 moveVec;
    [SerializeField] float dashCool = 0.3f;

    #endregion
    #region ������ �Ⱦ� ���� ����
    GameObject item;
    public List<GameObject> items = new List<GameObject>();
    GameObject replaceItemTemp;
    List<GameObject> replaceItemsTemp = new List<GameObject>();
    EItem getFirstItems;
    [SerializeField] float GapOfItems = 0.25f;
    [SerializeField] float pickUpDistance;
    [SerializeField] private LayerMask blockLayer;
    #endregion
    #region ��ư�� �����°�?
    bool dashKeyDown;//�뽬
    bool getItemKeyDown;
    #endregion
    #region �޼��尡 ���� �������ΰ�?
    bool isDash;
    public bool isHaveItem;
    bool isWalk;
    bool isWall;
    #endregion
    #region ������Ʈ ����
    private Rigidbody _rigidbody;
    private Animator _animator;
    #endregion
    Rigidbody rb;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rayStart = transform.GetChild(2);

        _animator.SetBool("isWalk", false);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Walk();
        Turn();
        Dash();
        DropItems();
        DigUp();
    }

    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        zAxis = Input.GetAxisRaw("Vertical");
        dashKeyDown = Input.GetButtonDown("Dash");
        getItemKeyDown = Input.GetButtonDown("getItem");
       /* if(Input.GetKey(KeyCode.E))
        {
            Debug.Log("ĳ��");
            _animator.SetBool("isDig",true);
        }
        else
        {
            _animator.SetBool("isMove", true);
            _animator.SetBool("isDig", false);
        }
        */
    }

    void Walk()
    {
        if (!isDash) moveVec = new Vector3(xAxis, 0, zAxis).normalized;

       // Vector3 getvel = new Vector3(xAxis, 0, zAxis) * speed* 3f;
       // rb.velocity = getvel;
        transform.position += moveVec * speed * Time.deltaTime;
        _animator.SetBool("isWalk", moveVec != Vector3.zero);
        
        
    }

    void Turn()
    {
        transform.LookAt(moveVec + transform.position);
    }

    void Dash()
    {
        if (dashKeyDown && !isDash)
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

    void PickUp()
    {
        item.transform.SetParent(rightHand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }


    void DummyPickUp()
    {
        _animator.SetBool("isTwoHandsPickUp", true);
        for (int i = 0; i < items.Count; i++)
        {
            items[i].tag = "Untagged";
            items[i].transform.SetParent(betweenTwoHands);
            items[i].transform.localPosition = Vector3.zero + new Vector3(0, (GapOfItems * i), 0);
            items[i].transform.localRotation = Quaternion.identity;
        }
    }

    void DropItems()
    {
        if (isHaveItem && getItemKeyDown)
        {
            Debug.Log("��ü�Ҷ��� �����϶��");
            isHaveItem = false;
            if (item != null)
            {
                PutDown();
            }
            if (items.Count != 0)
            {
                DummyPutDown();
            }
        }
    }

    void PutDown()
    {
        RaycastHit hitData;
        if (Physics.Raycast(betweenTwoHands.transform.position, Vector3.down, out hitData, 1000f))
        {
            if(hitData.transform.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                Debug.Log("�� ���������");
                item.transform.SetParent(hitData.transform);
                item.transform.localPosition = new Vector3(0f, 0.5f, 0f);//[����]
                item.transform.localRotation = Quaternion.identity;
                //item = null;
            }
            else if(hitData.transform.tag == "Item")
            {
                ReplaceItem();
            }
            
            
        }
    }


    void DummyPutDown()
    {
        _animator.SetBool("isTwoHandsPickUp", false);
        RaycastHit hitData;
        if (Physics.Raycast(betweenTwoHands.transform.position, Vector3.down, out hitData, 1000f))
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].tag = "Items";
                items[i].transform.SetParent(hitData.transform);
                items[i].transform.localPosition = (Vector3.up * 0.5f) + (Vector3.up * GapOfItems * i);
                items[i].transform.localRotation = Quaternion.identity;
            }
            items = new List<GameObject>();//[����]
        }
    }

    void ReplaceItem()
    {
        
        RaycastHit hitData;
        Vector3 temp;
        //���ǹ� �켱���� &&�� ||���� �켱�Դϴ�.
        if ((item != null || items.Count !=0) && (Physics.Raycast(transform.position, Vector3.down, out hitData, 1000f)) && getItemKeyDown)
        {
            if (hitData.transform.tag == "Item" && item.transform.tag == "Item")
            {
                Debug.Log("��ü�� �غ��ڽ��ϴ�");


                /*temp = hitData.transform.position;
                hitData.transform.position = item.transform.position;//���� -> hitdata�� ���ٴڿ� ���
                item.transform.position = temp;//��� -> grab�� ���*/


                // item ������ �ִ°�
                // hitData�� ���� �ִ°�

                item.transform.SetParent(hitData.transform.parent);
                item.transform.localPosition = new Vector3(0f, 0.5f, 0f);//[����]
                item.transform.localRotation = Quaternion.identity;

                hitData.transform.SetParent(rightHand);
                hitData.transform.localPosition = Vector3.zero;
                hitData.transform.localRotation = Quaternion.identity;

                item = hitData.transform.gameObject;

            }
            if (hitData.transform.tag == "Items")
            {
                //items ����Ʈ�� �ִ��� 
                //���� ��� �ִ� �� replaceItemsTemp ����Ʈ�� �ֱ�
            }
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (getItemKeyDown && !isHaveItem && other.gameObject.tag == "Item")
        {
            isHaveItem = true;
            item = other.gameObject;
            PickUp();
        }

        if (other.gameObject.tag == "Items" && getItemKeyDown && (items.Count == 0))
        {

            isHaveItem = true;
            items.Add(other.gameObject);
            getFirstItems = other.gameObject.GetComponent<Item>().itemType;
            DummyPickUp();
        }

        // �ϳ����ڸ��� �� ���� ���ǹ����� ����Ǵ� �� ����
        
        // �������� �� �ٽ� �Դ� ����
        // -> isHaveItem ������ ���� �����ذ�. DropItems�� ����Ǹ� isHaveItem�� false�� �Ǳ� ������, isHaveItem�� true�� ��쿡�� ����
        if (other.gameObject.gameObject.tag == "Items" && items.Count != 0 && (getFirstItems == other.gameObject.GetComponent<Item>().itemType) && isHaveItem)
        {
            if (items.Count < 3)
            {
                items.Add(other.gameObject.gameObject);
                DummyPickUp();
            }
        }
    }

    void DigUp()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.TransformPoint(0, 0.4f, 0), transform.forward * pickUpDistance*0.1f, Color.red) ;
        if (Physics.Raycast(transform.TransformPoint(0, 0.4f, 0), transform.forward, out hit, pickUpDistance))
        {
            Debug.Log("�߻�" + hit.transform.name);
            IDig target = hit.collider.GetComponent<IDig>();
            
           

            if (target != null )
            {
                if(hit.transform.name == "Tree01(Clone)" && item.name == "ItemAxe(Clone)")
                {
                    target.OnDig(hit.point);
                    _animator.SetBool("isDig",true);
                }
                if (hit.transform.name == "Tree02(Clone)" && item.name == "ItemAxe(Clone)")
                {
                    target.OnDig(hit.point);
                    _animator.SetBool("isDig", true);
                }
                if (hit.transform.name == "Iron(Clone)" && item.name == "ItemPick(Clone)")
                {
                    target.OnDig(hit.point);
                    _animator.SetBool("isDig", true);
                }
            }
            if (target == null)
            {

                _animator.SetBool("isMove", true);
                _animator.SetBool("isDig",false);
            }
        
        }
    }
    void DrawWater()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.TransformPoint(0, 0.1f, 0), transform.forward * pickUpDistance * 0.1f, Color.red);
        if(Physics.Raycast(transform.TransformPoint(0,0.1f,0),transform.forward,out hit, pickUpDistance))
        {
            Debug.Log("��" + hit.transform.name);
            
        }
    }
}
