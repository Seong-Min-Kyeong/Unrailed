using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BehaviorTree))]
public class FlockBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };

        public string Name;
    }

    [Header("���� �̵� �Ÿ�")]
    [SerializeField] private float _wanderRange = 3f;


    protected BehaviorTree _tree;
    //protected CharacterAgent _agent;

    protected PathFindingAgent _agent;
    protected Transform _transform;
    protected Flock _flock;
    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _transform = GetComponent<Transform>();
        _flock = GetComponent<Flock>();
    }

    private void Start()
    {
        //Boids

        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, Vector3.zero);

        var BTRoot = _tree.RootNode.Add<BTNode_Sequence>("BT START");
        BTRoot.AddService<BTServiceBase>("�浹 ���ɼ� ����", (float deltaTime) =>
        {
            //�浹 ���ɼ��� �ִٸ�
            if(_flock.IsHeadingForCollision())
            {
                var separation = _flock.ObstacleRays();
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, separation);
                Debug.Log("�����ؿ�");
            }
        });

        var isHeadingCollison = BTRoot.Add<BTNode_Sequence>("�浹 ���ɼ� �˻�");
        isHeadingCollison.AddDecorator<BTDecoratorBase>("�浹 ���ɼ��� �ֳ���?", () =>
         {
             return !_flock.IsHeadingForCollision();
         });

        var wanderRoot = BTRoot.Add <BTNode_Sequence>("�浹 ���ɼ� ����");


        var wander = wanderRoot.Add<BTNode_Action>("���ƴٴϱ�",
            () =>
            {
                _agent.MoveToRandomPosition();
                return BehaviorTree.ENodeStatus.InProgress;
            }, () =>
            {
                return _agent.AtDestination ?
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            }
            );
        //1. ���� ����� ���̱�(Cohesion)
        var cohesion = wanderRoot.Add<BTNode_Sequence>("Cohesion");
        var cohesionRoot = cohesion.Add<BTNode_Action>("���� ����� ���̱�",
        () =>
        {
            //Cohesion
            Debug.Log("Cohesion");
            _agent.MoveTo(_flock.Center);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });

        //�浹 ���ɼ��� �ִٸ�

        //2.�ڱ�鳢���� ���ϱ�(Separation)
        var SeparationRoot = BTRoot.Add<BTNode_Sequence>("Separation");
        SeparationRoot.Add<BTNode_Action>("������ �浹���� �ʱ�",
        () =>
        {
            //Separation
            Debug.Log("Separation");
            var ff = _localMemory.GetGeneric<Vector3>(BlackBoardKey.Destination);
            _agent.MoveTo(ff);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });

        //3. ������ ������ ���󰡱�(Alignment)





    }


}


