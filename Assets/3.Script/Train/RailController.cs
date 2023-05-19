using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    [SerializeField] private GameObject[] railPrefabs;
    private Transform[] railChild;
    [SerializeField] private RailController neighborRail;
    [SerializeField] private RailLine railLine;
    [SerializeField] private List<bool> dirList = new List<bool>();

    private TrainSpawnRail _trainPos;
    
    public int dirCount;

    public bool isInstance;

    public bool isFront;
    public bool isBack;
    public bool isLeft;
    public bool isRight;

    private Vector3 _frontPos;
    private Vector3 _backPos;
    private Vector3 _rightPos;
    private Vector3 _leftPos;

    private void Awake()
    {
        _trainPos = GetComponentInChildren<TrainSpawnRail>();
        railChild = this.GetComponentsInChildren<Transform>();
    }
    private void OnEnable()
    {
        //�����̵� ��ġ�� �ʱ�ȭ
        _trainPos.EnqueueRail();


        //������ �̵������� �� �ν��� ������ ��ġ
        _frontPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);
        _backPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f);
        _rightPos = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
        _leftPos = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);

        //��� ���ϼ�  �ʱ�ȭ
        railLine = null;
        
        //��ġ ���� �ʱ�ȭ
        isFront = false;
        isBack = false;
        isLeft = false;
        isRight = false;
        dirCount = 0;

        //������Ʈ ���̾� �ʱ�ȭ
        foreach (Transform child in railChild)
        {
            child.gameObject.layer = 23;
        }
        //�νĺҰ� bool �ʱ�ȭ
        isInstance = false;
        
        //ö�� ����
        RaycastOn();
        railLine.Line.SetActive(false);
    }
    //todo 05 18 �� ö�ΰ� ������ ö�θ� ���� �� �� �ֵ��� ����� ������ �׸��� �����ϸ� - �ڻ�
    public void RailSwitch()
    {
        for (int i = 0; i < railPrefabs.Length; i++)
        {
            railPrefabs[i].SetActive(true);
            if (dirCount != i)
            {
                railPrefabs[i].SetActive(false);

            }
            else
            {
                railLine = railPrefabs[i].GetComponentInChildren<RailLine>();
            }
        }
    }
    public void RaycastOn()
    {
        RaycastHit raycastHit;

        RailDir();

        if ((Physics.Raycast(_frontPos, transform.forward, out raycastHit, 0.3f) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance)
            || (Physics.Raycast(_rightPos, transform.right, out raycastHit, 0.3f )&& !raycastHit.collider.GetComponentInParent<RailController>().isInstance) 
            || (Physics.Raycast(_backPos, -transform.forward, out raycastHit, 0.3f) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance)
            || (Physics.Raycast(_leftPos, -transform.right, out raycastHit, 0.3f) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance))
        {
            neighborRail = raycastHit.collider.GetComponentInParent<RailController>();

            if (neighborRail != null && !neighborRail.isInstance)
            {
                if (isFront) neighborRail.isBack = true;
                if (isRight) neighborRail.isLeft = true;
                if (isBack) neighborRail.isFront = true;
                if (isLeft) neighborRail.isRight = true;


                neighborRail.isInstance = true;
                neighborRail.railDirSelet();
                neighborRail.RailSwitch();
                neighborRail.railLine.Line.SetActive(true);
                foreach (Transform child in neighborRail.railChild)
                {
                    child.gameObject.layer = 0;
                }
            }
        }


        railDirSelet();
        RailSwitch();
    }

    void RailDir()
    {
        isFront = Physics.Raycast(_frontPos, transform.forward, 0.3f, LayerMask.GetMask("Rail"));
        if (isFront) return;
        isRight = Physics.Raycast(_rightPos, transform.right, 0.3f, LayerMask.GetMask("Rail"));
        if (isRight) return;
        isBack = Physics.Raycast(_backPos, -transform.forward, 0.3f, LayerMask.GetMask("Rail"));
        if (isBack) return;
        isLeft = Physics.Raycast(_leftPos, -transform.right, 0.3f, LayerMask.GetMask("Rail"));
        if (isLeft) return;

    }
    private void railDirSelet()
    {
        if (isFront || isBack) dirCount = 5;
        if (isRight || isLeft) dirCount = 0;
        if (isFront && isRight) dirCount = 3;
        else if (isFront && isLeft) dirCount = 1;
        else if (isBack && isRight) dirCount = 4;
        else if (isBack && isLeft) dirCount = 2;

       
    }
    void Update()
    {
        Debug.DrawRay(_frontPos, transform.forward * 0.3f, Color.red);
        Debug.DrawRay(_backPos, -transform.forward * 0.3f, Color.green);
        Debug.DrawRay(_rightPos, transform.right * 0.3f, Color.yellow);
        Debug.DrawRay(_leftPos, -transform.right * 0.3f, Color.blue);
    }
}
