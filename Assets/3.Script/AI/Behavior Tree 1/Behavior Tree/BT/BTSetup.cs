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
        public static readonly BlackBoardKey RandomDestination = new BlackBoardKey() { Name = "RandomDestination" };

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
    DetectableTargetManager detectableTarget;


    private void Awake()
    {
        detectableTarget = FindObjectOfType<DetectableTargetManager>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<CharacterAgent>();
        _sensors = GetComponent<AwarenessSystem>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);
        _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, _agent.PickLocationInRange(_wanderRange));


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

        var canChaseSel = BTRoot.Add<BTNode_Sequence>("SEQ. ��ǥ�� �ֳ���?");
        var canChaseDeco = canChaseSel.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {
            //Ÿ���� ������ true ������ false
            //������ ������ Ÿ���� ����� ���� �ؿ��Ű� ��ŵ�ǰ� ���ڱ� ������ �ȴٴ� ��
            var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            return currentTarget != null;
        });

        var oo = canChaseSel.Add<BTNode_Sequence>("d�ƾ�");

        var mainSeq = oo.Add<BTNode_Sequence>("Seq1 : ��ǥ�� �̵� �õ�");
        var ff = mainSeq.Add<BTNode_Action>("A ��ǥ ã�� : ã�Ƽ� ���",
        () =>
        {
            //InProgress ���¿��� currentTarget�� �ٲ�� ���׳��µ�
            Debug.Log("����ϳ���?");
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
            //�������� �ʾҴٸ�

                _agent.MoveTo(currentTarget.transform.position);
                return BehaviorTree.ENodeStatus.InProgress;


/*            if(_agent.AtDestination)
            {

            }
            return BehaviorTree.ENodeStatus.Succeeded;*/
/*            if (!_agent.AtDestination)
            {
                _agent.MoveTo(currentTarget.transform.position);
                Debug.Log("�̵���");
                //Ÿ������ �̵�
                //�̰� Ÿ�� ��ǥ�� �ι�° Ÿ�ٶ� ������Ʈ�� �ȵ�
                //�̸��� �´µ� ��ǥ�� ù��° Ÿ�� ��ǥ�� ����........................................................����?
                //return BehaviorTree.ENodeStatus.Succeeded;
            }
                return BehaviorTree.ENodeStatus.Succeeded;*/
/*            else
            {
                //�����ߴٸ� ���� ��ȯ
                //!!�ι�° Ÿ�ٺ��� AI�� �̵��ϱ⵵ ���� ��ġ�ϴ� ���� ���ľ���
                if(!_agent.isMoving)
                {
                Debug.Log("�ٿԾ��");
                return BehaviorTree.ENodeStatus.Succeeded;

                }
                return BehaviorTree.ENodeStatus.InProgress;
            }
*/

        },
            () =>
            {
/*                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                if (!_agent.AtDestination)
                {
                    _agent.MoveTo(currentTarget.transform.position);
                    Debug.Log("�̵���");
                }*/
                //return BehaviorTree.ENodeStatus.Succeeded;


                //return BehaviorTree.ENodeStatus.Succeeded;  
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
             if (_agent.AtDestination)
             {
                 var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                 currentTarget.transform.SetParent(transform);
                 currentTarget.transform.localPosition = Vector3.zero;
                 var newDestination = _agent.PickLocationInRange(10);
                 _localMemory.SetGeneric<Vector3>(BlackBoardKey.NewDestination, newDestination);

             }

             
                 return BehaviorTree.ENodeStatus.InProgress;

         },
         () =>
         {
             //_agent.CancelCurrentCommand();
                 var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             //return BehaviorTree.ENodeStatus.Succeeded;
             return (currentTarget.transform.parent = this.transform) ? 
             BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });


        var runRoot = mainSeq.Add<BTNode_Sequence>("Seq3 : ���� �õ�");
        var runDeco = runRoot.AddDecorator<BTDecoratorBase>("��ǥ�� �̵��� �� �ֳ���?", () =>
        {


            return true;
            //Vector3 pos = new Vector3(-20, 0, -2);
            //_localMemory.SetGeneric(BlackBoardKey.NewDestination, pos);

/*            var newwDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
            return newwDestination != null;*/
        });
        var runAction = runRoot.Add<BTNode_Action>("������ ���� : ���� ����",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
                    var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);

                //�� �������� ������ ���¶��
                if(_agent.AtDestination)
                {
                    //����ġ��

                }
                    _agent.MoveTo(newDestination);
                    return BehaviorTree.ENodeStatus.InProgress;

            },
            () =>
            {

                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
                //return BehaviorTree.ENodeStatus.Succeeded;

            });

        //��������

        var discardRoot = runRoot.Add<BTNode_Sequence>("Seq4 : �������� �õ�");
        discardRoot.AddDecorator<BTDecoratorBase>("�������� �� �ֳ���?", () =>
         {
            return true;

             //var newDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.NewDestination);
             //var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
             //return currentTarget != null;
         });
        var discardAction = discardRoot.Add<BTNode_Action>("������ ����",
            () =>
            {
                var currentTarget = _localMemory.GetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget);
/*                if(currentTarget!=null&_agent.AtDestination)
                {
                    //������
                    currentTarget.transform.parent = null;
                    _agent.CancelCurrentCommand();
                    Debug.Log("���Ⱦ��");
                    Destroy(currentTarget);
                    _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

                    //���� ������ ����
                    var randomDestination = _agent.PickLocationInRange(_wanderRange);
                    _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, randomDestination);
                    return BehaviorTree.ENodeStatus.Succeeded;

                }
                else
                return BehaviorTree.ENodeStatus.InProgress;*/


                currentTarget.transform.parent = null;
                //_agent.CancelCurrentCommand();
                Debug.Log("���Ⱦ��");
                Destroy(currentTarget);
                _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

                //���� ������ ����
                var randomDestination = _agent.PickLocationInRange(_wanderRange);
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, randomDestination);
                return BehaviorTree.ENodeStatus.Succeeded;



                //������ ������ �̵�
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );






        var wanderRoot = BTRoot.Add<BTNode_Sequence>("��ǥ �� ã�� : ������ �̵�");
        /*        var wanderDeco = wanderRoot.AddDecorator<BTDecoratorBase>("������ ��ġ �����ϱ�", () =>
                {
                    return true;
                });*/
        wanderRoot.Add<BTNode_Action>("������ �̵���",
        () =>
        {
            var randomDestination = _localMemory.GetGeneric<Vector3>(BlackBoardKey.RandomDestination);
            //var randomDestination = _agent.PickLocationInRange(_wanderRange);
            //_agent.MoveTo(randomDestination);
            //return BehaviorTree.ENodeStatus.InProgress;
                _agent.MoveTo(randomDestination);
            Debug.Log(randomDestination);
            Debug.Log(_agent.AtDestination);
                return BehaviorTree.ENodeStatus.InProgress;
/*            if(!_agent.AtDestination)
            {

            }

            else
            {
                _agent.CancelCurrentCommand();
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, _agent.PickLocationInRange(_wanderRange));
                return BehaviorTree.ENodeStatus.Succeeded;

            }*/

        },
        () =>
        {

            if(_agent.AtDestination)
            {
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.RandomDestination, _agent.PickLocationInRange(_wanderRange));
            }
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

        });




    }

}
