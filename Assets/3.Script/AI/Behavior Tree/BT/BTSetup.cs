using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BehaviorTree))]
public class BTSetup : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey NewDestination = new BlackBoardKey() { Name = "NewDestination" };

        public string Name;


    }


    //������ �Ÿ�, Wander ������ �� �̸�ŭ �̵���
    [Header("���� �̵� �Ÿ�")]
    [SerializeField] private float _wanderRange = 10f;
    [SerializeField] private float _newDestination = 30f;


    [Header("�Ѿư��� ����")]
    //������ ������ �� �ִ� �ּ� ����
    [SerializeField] private float _minAwarenessToChase = 1f;
    //������ ���ߴ� ����
    [SerializeField] private float _awarenessToStopChase = 2f;

    protected BehaviorTree _tree;
    protected CharacterAgent _agent;
    protected AwarenessSystem _sensors;
    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<CharacterAgent>();
        _sensors = GetComponent<AwarenessSystem>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����"); //������

        var service = BTRoot.AddService<BTServiceBase>("��ǥ ã�� Service", (float deltaTime) => //�������� ��� ����
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

        var canChaseSel = BTRoot.Add<BTNode_Sequence>("SEQ. ��ǥ�� �ֳ���?");
        var canChaseDeco = canChaseSel.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {
            //Ÿ���� ������ true ������ false
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;
        });

        var mainSeq = canChaseDeco.Add<BTNode_Sequence>("Seq1 : ��ǥ�� �̵� �õ�");

        mainSeq.Add<BTNode_Action>("A ��ǥ ã�� : ã�Ƽ� ���",
        () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            _agent.MoveTo(currentTarget.transform.position);

            if (Vector3.Distance(_agent.transform.position, currentTarget.transform.position) < 1)
            {
                _agent.CancelCurrentCommand();
                return BehaviorTree.ENodeStatus.Succeeded;

            }

            else
                return BehaviorTree.ENodeStatus.InProgress;

        },
            () =>
            {
                /*var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                  _agent.MoveTo(currentTarget.transform.position);*/

                //return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                return BehaviorTree.ENodeStatus.Succeeded;
            });

        var stealRoot = mainSeq.Add<BTNode_Sequence>("Seq2 : ��ġ�� �õ�");
        var dd = stealRoot.AddDecorator<BTDecoratorBase>("��ĥ Ÿ���� �����ϴ��� Ȯ���ϴ� Decorator", () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;

        });

        var stealRootAction = dd.Add<BTNode_Action>("A Ÿ�� ���� : ��ġ�� ����",
        () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             currentTarget.transform.SetParent(transform);
             currentTarget.transform.localPosition = Vector3.zero;
             _agent.CancelCurrentCommand();
             return BehaviorTree.ENodeStatus.Succeeded;


            /* if (currentTarget != null)
             {
                 //�ٰ����� �ֿ� �� �ִ� ���°� �Ǿ����
                 if (Vector3.Distance(_agent.transform.position, currentTarget.transform.position) < 1)
                 {
                     currentTarget.transform.SetParent(transform);
                     currentTarget.transform.localPosition = Vector3.zero;
                     _agent.CancelCurrentCommand();
                     //_agent.StopNav();
                     return BehaviorTree.ENodeStatus.Succeeded;

                 }
             }

             else Debug.Log("���������");
             return BehaviorTree.ENodeStatus.Failed;*/
             //�� �ֿ�� ó������ ���ư���
         },
         () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

             return (currentTarget.transform.parent = this.transform) ? 
             BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });


        var runRoot = mainSeq.Add<BTNode_Sequence>("Seq3 : ���� �õ�");
        runRoot.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {
            Vector3 pos = new Vector3(-20, 0, -2);
            _localMemory.SetGeneric(BlackBoardKey.NewDestination, pos);
            var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
            return newDestination != null;
        });
        var runAction = runRoot.Add<BTNode_Action>("������ ���� : ���� ����",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
                _agent.MoveTo(newDestination);

                if (Vector3.Distance(_agent.transform.position, newDestination) < 2)
                {
                    Debug.Log("�ٿԾ��");
                    //currentTarget.transform.parent = null;
                    _agent.CancelCurrentCommand();
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                {
                    Debug.Log("���� ���̿���");
                    return BehaviorTree.ENodeStatus.InProgress;
                }


                //return BehaviorTree.ENodeStatus.Succeeded;
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;

            });

        //��������

        var discardRoot = mainSeq.Add<BTNode_Sequence>("Seq4 : �������� �õ�");
        discardRoot.AddDecorator<BTDecoratorBase>("�������� �� �ֳ���?", () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             return currentTarget != null;
         });
        var discardAction = discardRoot.Add<BTNode_Action>("������ ����",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

                currentTarget.transform.parent = null;
                _agent.CancelCurrentCommand();
                Debug.Log("���Ⱦ��");
                return BehaviorTree.ENodeStatus.Succeeded;
            },
            () =>
            {

                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );



        var wanderRoot = canChaseSel.Add<BTNode_Sequence>("��ǥ �� ã�� : ������ �̵�");
        wanderRoot.Add<BTNode_Action>("������ �̵���",
        () =>
        {
            //�̻��ϰ� �����̴°� ��ġ��
            Vector3 location = _agent.PickLocationInRange(_wanderRange);
            _agent.MoveTo(location);
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            //return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });




    }

}
