using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{
    [SerializeField] private float _nearestPointSearchRange = 5f;

    private NavMeshAgent _agent;

    //������ ������ �Ǿ�����
    private bool _destinationSet = false;

    //�������� ��Ҵ���
    private bool _reachedDestination = false;
    EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;

    //AI�� �����̰� ������ true �ƴϸ� false
    public bool isMoving => _agent.velocity.magnitude > float.Epsilon;
    public bool AtDestination => _reachedDestination;
    public bool DestinationSet => _destinationSet;

    protected void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        
    }

    protected void Update()
    {
        //Ž������ �ƴϰ�, ������ ������ �Ǿ���, ���� �Ÿ��� ���� �Ÿ����� ������
        if ((_agent.remainingDistance <= _agent.stoppingDistance)&&_destinationSet&&!_agent.pathPending)
        //if (!_agent.pathPending && _destinationSet && !_agent.isOnOffMeshLink&&(_agent.remainingDistance <= _agent.stoppingDistance))
        {
            //�������� ����   
            _destinationSet = false;
            _reachedDestination = true;
        }

        if(_agent.remainingDistance > _agent.stoppingDistance&&_destinationSet)
        {
            _reachedDestination = false;
        }


        if (_agent.isOnOffMeshLink) //���� �޽� ��ũ ���� �ְ�
        {
            if (OffMeshLinkStatus == EOffmeshLinkStatus.NotStarted) //�������� �ʾҴٸ�
                StartCoroutine(FollowOffmeshLink()); // �Ѿư���
        }

    }

    IEnumerator FollowOffmeshLink()
    {
        //�׺�޽� ��Ʈ�Ѻ�Ȱ��ȭ
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        Vector3 newPosition = transform.position;
        //������ ������ ������ �̵�
        while (!Mathf.Approximately(Vector3.Distance(newPosition, _agent.currentOffMeshLinkData.endPos), 0f))
        {
            newPosition = Vector3.MoveTowards(transform.position, _agent.currentOffMeshLinkData.endPos, _agent.speed * Time.deltaTime);
            transform.position = newPosition;

            yield return new WaitForEndOfFrame();
        }

        // ����
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
        _agent.CompleteOffMeshLink();

        //�׺�޽� ��Ʈ�� Ȱ��ȭ
        _agent.updatePosition = true;
        _agent.updateRotation = true;
        _agent.updateUpAxis = true;
    }

    public void StopNav()
    {
        _agent.ResetPath();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }


    public Vector3 PickLocationInRange(float range) //������ �̵�
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;

        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchLocation, out hitResult, _nearestPointSearchRange, NavMesh.AllAreas))
            return hitResult.position;

        return transform.position;
    }

    public virtual void CancelCurrentCommand() //������ �ʱ�ȭ
    {
        _agent.ResetPath();

        _destinationSet = false;
        _reachedDestination = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    public virtual void MoveTo(Vector3 destination) // �������� �̵�
    {
        CancelCurrentCommand(); // �����ϰ�
        SetDestination(destination); //������ ������ �ϰ�

    }


    public virtual void SetDestination(Vector3 destination) // ������ �����ϱ�
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, _nearestPointSearchRange, NavMesh.AllAreas)
            )
        {
            _agent.SetDestination(hitResult.position);
            _destinationSet = true;
            _reachedDestination = false;
        }
    }





}
