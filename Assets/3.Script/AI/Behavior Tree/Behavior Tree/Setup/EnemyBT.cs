using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BehaviorTree))]
public class EnemyBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey NewDestination = new BlackBoardKey() { Name = "NewDestination" };
        public static readonly BlackBoardKey RandomDestination = new BlackBoardKey() { Name = "RandomDestination" };

        public string Name;


    }


    [Header("�Ѿư��� ����")]
    //������ ������ �� �ִ� �ּ� ����
    [SerializeField] private float _minAwarenessToChase = 1f;
    //������ ���ߴ� ����
    [SerializeField] private float _awarenessToStopChase = 2f;



    private Vector3 _itemPosition;
    private Animator _animator;

    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected AwarenessSystem _sensors;
    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _sensors = GetComponent<AwarenessSystem>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
;    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
   


        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����"); //������

        BTRoot.AddService<BTServiceBase>("��ǥ ã�� Service", (float deltaTime) => //�������� ��� ����
        {
            if (_sensors.ActiveTargets == null || _sensors.ActiveTargets.Count == 0)
            {
                _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
                return;
            }

            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

            if (currentTarget != null)
            {
                foreach (var candidate in _sensors.ActiveTargets.Values)
                {
                    if (candidate.Detectable == currentTarget //ó���� ������ �ֶ� ���󰡰� �ִ� ���� ����
                        &&candidate.Awareness >= _awarenessToStopChase) //�ν� ������ 2���� ũ�ų� ������ ���ư���
                    {
                        return;
                    }
                }

                //�ν� ������ 1���� ���ٸ� ����
                currentTarget = null;
            }

            // Ÿ���� ���ٸ� ���ο� Ÿ�� ã��
            float highestAwareness = _minAwarenessToChase;
            foreach (var candidate in _sensors.ActiveTargets.Values)
            {
                // ���ο� Ÿ�ٿ� �Ҵ��ϱ�
                if (candidate.Awareness >= highestAwareness)
                {
                    currentTarget = candidate.Detectable;
                    highestAwareness = candidate.Awareness;
                }
            }


            //���⼭ Set���ֱ�
            _localMemory.SetGeneric(BlackBoardKey.CurrentTarget, currentTarget);
        });

        var canChaseSeq = BTRoot.Add<BTNode_Sequence>("SEQ. ��ǥ�� �ֳ���?");
        var canChaseDeco = canChaseSeq.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {
            //Ÿ���� ������ true ������ false
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;
        });

        var startRoot = canChaseSeq.Add<BTNode_Sequence>("����");
        var mainSeq = startRoot.Add<BTNode_Sequence>("Seq1 : ��ǥ�� �̵� �õ�");
        mainSeq.Add<BTNode_Action>("A ��ǥ ã�� : ã�Ƽ� ���",
        () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            _agent.MoveTo(currentTarget.transform.position);
            
            return BehaviorTree.ENodeStatus.InProgress;

        },
            () =>
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        var stealRoot = mainSeq.Add<BTNode_Sequence>("Seq2 : ��ġ�� �õ�");
        var stealDeco = stealRoot.AddDecorator<BTDecoratorBase>("��ĥ Ÿ���� �����ϴ��� Ȯ���ϴ� Decorator", () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;

        });

        var stealRootAction = stealDeco.Add<BTNode_Action>("A Ÿ�� ���� : ��ġ�� ����",
        () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             _animator.SetBool("Lifting", true);

             currentTarget.transform.SetParent(transform);
             currentTarget.transform.localPosition = _itemPosition;
             currentTarget.transform.localRotation = Quaternion.identity;
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             return (currentTarget.transform.parent = this.transform) ? 
             BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });


        var runRoot = mainSeq.Add<BTNode_Sequence>("Seq3 : ���� �õ�");
        runRoot.Add<BTNode_Action>("������ ���� : ���� ����",
            () =>
            {
                //�������� ���� ������
                _agent.MoveToClosestEndPosition();
                return BehaviorTree.ENodeStatus.InProgress;

            },
            () =>
            {
                return _agent.AtDestination ? 
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        //��������

        var discardRoot = runRoot.Add<BTNode_Sequence>("Seq4 : �������� �õ�");
        discardRoot.AddDecorator<BTDecoratorBase>("�������� �� �ֳ���?", () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

             return currentTarget.transform.parent = this.transform;
         });
        var discardAction = discardRoot.Add<BTNode_Action>("������ ����",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                _animator.SetBool("Lifting", false);

                currentTarget.gameObject.AddComponent<Rigidbody>();
                currentTarget.transform.parent = null;
                Destroy(currentTarget.gameObject, 1);
                Destroy(currentTarget);
                _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );



        var wanderRoot = BTRoot.Add<BTNode_Sequence>("��ǥ �� ã�� : ������ �̵�");
        wanderRoot.Add<BTNode_Action>("������ �̵���",
        () =>
        {
            _agent.MoveToRandomPosition();
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

    }



}
