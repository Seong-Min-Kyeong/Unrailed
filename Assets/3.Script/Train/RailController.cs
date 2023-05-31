using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailController : MonoBehaviour
{
    public float range = 1f;

    [SerializeField] private GameObject[] railPrefabs;
    [SerializeField] private RailController neighborRail;
    /// ���߿� [SerializeField] private GoalManager trainManager;

    public TrainMovement[] trainComponents;
    public RailLine railLine;

    public int dirCount;
    int childCount;

    public bool isInstance;

    public bool isGoal;
    public bool isFront;
    public bool isBack;
    public bool isLeft;
    public bool isRight;
    public bool isStartRail;
    public bool isEndRail;

    public float poolingTime;
    public float lifeTime = 0;


    public void Init()
    {
        //// ���߿� trainManager = FindObjectOfType<GoalManager>();

        childCount = gameObject.transform.childCount;
        railPrefabs = new GameObject[childCount];

        for (int i = 0; i < railPrefabs.Length; i++)
        {
            railPrefabs[i] = gameObject.transform.GetChild(i).gameObject;
        }
    }

    public void PutRail()
    {
        /// ���߿� trainManager.railCon.Add(gameObject.GetComponent<RailController>());
        if (!isEndRail && !isStartRail)
        {
            //�����̵� ��ġ�� �ʱ�ȭ
            //��, ���� �ƴҶ����� Ȱ��ȭ�ȴ�.
            //������ ������ �� ����� ��� ��� ������ ���� �����Ѵ�.
            EnqueueRail();
        }

        if (!isGoal)
        {
            //��� ���ϼ�  �ʱ�ȭ
            railLine = null;

            //��ġ ���� �ʱ�ȭ
            isFront = false;
            isBack = false;
            isLeft = false;
            isRight = false;

            dirCount = 0;

            //�νĺҰ� bool �ʱ�ȭ
            isInstance = false;
        }

        //ö�� ����
        RaycastOn();

        if (!isGoal && !isEndRail && !isStartRail)
        {
            railLine.Line.SetActive(false);
        }
    }

    private void Awake()
    {
        Init();
    }
    /* private void OnEnable()
    {
        PutRail();
    }*/

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
        //�� ���� �������� isStartRail ö�� 2������ ��Ƶΰ� 2���� �⺻ ö��. �� �Ŀ� ���̸� ���󰡵�
        //isEndRail�� �ΰ��� �ٿ������� 

        RaycastHit raycastHit = new RaycastHit();

        RailDir();


        if ((Physics.Raycast(transform.position, transform.forward, out raycastHit, range, LayerMask.GetMask("Rail")) && (!raycastHit.collider.GetComponentInParent<RailController>().isInstance))
            || (Physics.Raycast(transform.position, transform.right, out raycastHit, range, LayerMask.GetMask("Rail")) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance) 
            || (Physics.Raycast(transform.position, -transform.forward, out raycastHit, range, LayerMask.GetMask("Rail")) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance)
            || (Physics.Raycast(transform.position, -transform.right, out raycastHit, range, LayerMask.GetMask("Rail")) && !raycastHit.collider.GetComponentInParent<RailController>().isInstance))
        {
            neighborRail = raycastHit.collider.GetComponentInParent<RailController>();

            //�� �� �� �� Ȯ���Ͽ� isIntance�� Ȯ��
            if (neighborRail != null && 
                !neighborRail.isInstance && !neighborRail.isStartRail)
            {
                if (isFront) neighborRail.isBack = true;
                if (isRight) neighborRail.isLeft = true;
                if (isBack) neighborRail.isFront = true;
                if (isLeft) neighborRail.isRight = true;

                if(!neighborRail.isEndRail) neighborRail.isInstance = true;

                neighborRail.railDirSelet();
                neighborRail.RailSwitch();
                neighborRail.railLine.Line.SetActive(true);
            }
            //�� �� �� �� Ȯ���Ͽ� isGoal�� Ȯ��
            if (neighborRail != null && neighborRail.isEndRail && !isEndRail)
            {
                /// ���߿� trainManager.TrainGoal();
                neighborRail.isEndRail = false;
                neighborRail.enabled = false;
                neighborRail.enabled = true;
            }
        }

        railDirSelet();
        RailSwitch();

        if (isInstance)
            railLine.Line.SetActive(true);
    }
    void RailDir()
    {
        isFront = Physics.Raycast(transform.position, transform.forward, range, LayerMask.GetMask("Rail"));
        if (isFront) return;
        isRight = Physics.Raycast(transform.position, transform.right, range, LayerMask.GetMask("Rail"));
        if (isRight) return;
        isBack = Physics.Raycast(transform.position, -transform.forward, range, LayerMask.GetMask("Rail"));
        if (isBack) return;
        isLeft = Physics.Raycast(transform.position, -transform.right, range, LayerMask.GetMask("Rail"));
        if (isLeft) return;

    }
    private void railDirSelet()
    {
        if (isFront) dirCount = 5;
        if (isRight) dirCount = 0;
        if (isBack) dirCount = 6;
        if (isLeft) dirCount = 7;

        if (isFront && isRight) dirCount = 3;
        else if (isFront && isLeft) dirCount = 1;
        else if (isBack && isRight) dirCount = 4;
        else if (isBack && isLeft) dirCount = 2;
    }
    public void ResetLine()
    {
        railLine.Line.SetActive(true);
    }
    void Update()
    {
        if (isGoal)
        {
            lifeTime += Time.deltaTime;

            if (lifeTime >= poolingTime)
            {
                lifeTime = 0;
                transform.position = Vector3.zero;
                gameObject.SetActive(false);
            }
        }
    }

/*    private void OnDisable()
    {
        trainManager.railCon.Remove(gameObject.GetComponent<RailController>());


        if (neighborRail != null)
        {
            if (!isStartRail && !neighborRail.isStartRail && !isGoal)
            {
                neighborRail.isInstance = false;
                neighborRail.railLine.Line.SetActive(false);
            }
        }

        //Ȥ�� ���� ���� ������ ���̾ �ǵ����� ������ �����ص� �ʿ��ϸ� �ۼ�
        //foreach (Transform child in neighborRail.railChild)
        //{
        //
        //    child.gameObject.layer = 23;
        //}
    }*/

    public void EnqueueRail()
    {
        trainComponents = FindObjectsOfType<TrainMovement>();

        for (int i = 0; i < trainComponents.Length; i++)
        {
            //������ ��ġ�� �߰�
            trainComponents[i].EnqueueRailPos(gameObject.GetComponent<RailController>());
        }
    }
}
