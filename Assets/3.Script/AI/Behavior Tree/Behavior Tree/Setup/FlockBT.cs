using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlockBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Separation = new BlackBoardKey() { Name = "Separation" };
        public static readonly BlackBoardKey Alignment = new BlackBoardKey() { Name = "Alignment" };

        public string Name;
    }

    [Header("���� �̵� �Ÿ�")]
    [SerializeField] private float _wanderRange = 3f;


    private BehaviorTree _tree;
    private Blackboard<BlackBoardKey> _localMemory;

    private PathFindingAgent _agent;
    private Transform _transform;

    private Flock _flock;

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
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Separation, Vector3.zero);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.Alignment, Vector3.zero);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT START");
        BTRoot.AddService<BTServiceBase>("�浹 ���ɼ� ����", (float deltaTime) =>
        {
            //�浹 ���ɼ��� �ִٸ�
            if(_flock.IsHeadingForCollision())
            {
                Debug.Log("�����ؿ�");
            }
        });

        var isHeadingCollison = BTRoot.Add<BTNode_Sequence>("�浹 ���ɼ� �˻�");
        isHeadingCollison.AddDecorator<BTDecoratorBase>("�浹 ���ɼ��� �ֳ���?", () =>
         {
             return !_flock.IsHeadingForCollision();
         });

        //�浹 ���ɼ��� ���ٸ�
        var wanderRoot = isHeadingCollison.Add <BTNode_Sequence>("�浹 ���ɼ� ����");
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


        //���� ����� ���̱�(Cohesion)
        var cohesionRoot = wanderRoot.Add<BTNode_Sequence>("Cohesion");
        var cohesion = cohesionRoot.Add<BTNode_Action>("���� ����� ���̱�",
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

        //������ ������ ���󰡱�(Alignment)
        var AlignmentRoot = wanderRoot.Add<BTNode_Sequence>("Alignment");
        AlignmentRoot.Add<BTNode_Action>("������ ������ ���󰡱�",
        () =>
        {
            //Alignment
            Debug.Log("Alignment");
            _agent.MoveTo(_flock.AlignmentPosition);
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });



        //�浹 ���ɼ��� �ִٸ�
        //�ڱ�鳢���� ���ϱ�(Separation)
        var SeparationRoot = BTRoot.Add<BTNode_Sequence>("Separation");
        SeparationRoot.Add<BTNode_Action>("������ �浹���� �ʱ�",
        () =>
        {
            //Separation
            Debug.Log("Separation");
            _agent.MoveTo(_flock.ObstacleRays());
            return BehaviorTree.ENodeStatus.InProgress;
        }, () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });

        var CollisionRoot = SeparationRoot.Add<BTNode_Sequence>("�ڱ�鳢�� �ھ��� �� ");
        CollisionRoot.Add<BTNode_Action>("�ƹ����� ������", 
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




    }


}


