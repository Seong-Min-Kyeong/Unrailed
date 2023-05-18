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

        BTRoot.AddService<BTServiceBase>("��ǥ ã�� Service", (float deltaTime) => //�� �� ����, Ÿ���� ������� �� �̻� ������������
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

        var chaseRoot = BTRoot.Add<BTNode_Sequence>("1. ��ǥ�� �ֳ���?");
        chaseRoot.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {

            //Ÿ���� ������ true ������ false
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;


        });

        chaseRoot.Add<BTNode_Action>("��ǥ ã�� : ã�Ƽ� ���",
        () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             //_agent.SetDestination(currentTarget.transform.position);
            _agent.MoveTo(currentTarget.transform.position);

            return BehaviorTree.ENodeStatus.InProgress;
        },
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                //_agent.MoveTo(currentTarget.transform.position);

                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                //return BehaviorTree.ENodeStatus.Succeeded;
            });

        //���⵵ ���� �ȵ�
        var StealRoot = BTRoot.Add<BTNode_Sequence>("2. ��ĥ �� �ֳ���?");
        //��ǥ�� ��Ҵ��� Ȯ���ϴ� Service
        StealRoot.AddDecorator<BTDecoratorBase>("��ǥ�� ��Ҵ��� Ȯ���ϴ� Decorator", () =>
        {
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            if (_agent.AtDestination)
            {
                //�̰� ���ص� �̻���
                Debug.Log("�ٿԾ��");
                return true;
                
            }
            return false;
            //return currentTarget != null;

        });
        StealRoot.Add<BTNode_Action>("��ǥ ���� : ��ġ�� ����",
        () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);

             if(currentTarget!=null)
             {
                 currentTarget.transform.SetParent(transform);
             }

             else Debug.Log("Ÿ���� �����");
             

             //���Ⱑ ���� Ÿ���� �� �����������
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
         {
             var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             currentTarget.transform.SetParent(gameObject.transform);
             return BehaviorTree.ENodeStatus.Succeeded;

         });

        var runRoot = BTRoot.Add<BTNode_Sequence>("3. �������� �ֳ���?");
/*        runRoot.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {
            //Ÿ���� ������ true ������ false
            //var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
            //return newDestination != null;
        });
        runRoot.Add<BTNode_Action>("������ ���� : ���� ����",
            () =>
            {
                var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
                _agent.SetDestination(newDestination);
                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
                _agent.SetDestination(newDestination);
                return BehaviorTree.ENodeStatus.InProgress;
            });
*/













        var wanderRoot = BTRoot.Add<BTNode_Sequence>("��ǥ �� ã�� : ������ �̵�");
        wanderRoot.Add<BTNode_Action>("������ �̵���",
        () =>
        {
            Vector3 location = _agent.PickLocationInRange(_wanderRange);
            _agent.MoveTo(location);
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });




    }

}
