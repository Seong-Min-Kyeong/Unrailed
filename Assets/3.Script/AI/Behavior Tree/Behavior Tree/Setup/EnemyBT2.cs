using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBT2 : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey NewDestination = new BlackBoardKey() { Name = "NewDestination" };

        public string Name;

    }

    protected Blackboard<BlackBoardKey> _localMemory;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _stack = GetComponent<AI_Stack>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, null);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");

        BTRoot.AddService<BTServiceBase>("��ĥ �� ã�� Service", (float deltaTime) =>
        {
            if (!Home.NonethisResourceType)
            {
                _target = Home.TargettoSteal(_agent);
                var target = Home.TargettoSteal(_agent);
                _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, target);

            }
            else
                _target = null;
                return;
        });

        var HaveTarget = BTRoot.Add<BTNode_Selector>("��ĥ �� �ֳ���?");
        var CheckTarget = HaveTarget.AddDecorator<BTDecoratorBase>("��ĥ �� �ִ��� �Ÿ��� DECO", () =>
        {
            //Ÿ���� ������ true ������ false
            return _target != null;
        });

        var StartRoot = HaveTarget.Add<BTNode_Sequence>("1. �ִ� ���");

        var MainSeq = StartRoot.Add<BTNode_Sequence>("1");
        MainSeq.Add<BTNode_Action>("�̵��ϱ�",
        () =>
        {
            Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
            _agent.MoveTo(_target.transform.position);

            return BehaviorTree.ENodeStatus.InProgress;

        },
            () =>
            {
                return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        var stealRoot = MainSeq.Add<BTNode_Sequence>("2");
        var stealDeco = stealRoot.AddDecorator<BTDecoratorBase>("��ĥ Ÿ���� �����ϴ��� Ȯ���ϴ� Decorator", () =>
        {
            return _target != null;

        });

        stealRoot.Add<BTNode_Action>("Ÿ�� ���� : ��ġ�� ����",
        () =>
        {
            _animator.SetBool(isMove, false);
            if(_target!=null)
            {
                _stack.DetectGroundBlock(_target);
                //ó�� ��� �� 
                if (_stack._handItem.Count == 0)
                {
                    _stack.InteractiveItemSpace();
                }
                //�� �� �ױ�
                else
                {
                    _stack.InteractiveItem();
                }
                return BehaviorTree.ENodeStatus.InProgress;

            }
            return BehaviorTree.ENodeStatus.Failed;



        },
         () =>
         {
             return _target != null ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
         });


        var dd = MainSeq.Add<BTNode_Sequence>("3");
        dd.Add<BTNode_Action>("�ױ�",
            ()=>
            {
                return BehaviorTree.ENodeStatus.InProgress;

            },()=>
            {
                Home.TargettoSteal(_agent);
                //�ڿ��� �� �̻� ���ٸ� 
                if (Home.NonethisResourceType)
                {
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                    //�� �� ������� �ű��
                    return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;

            });



        var runRoot = MainSeq.Add<BTNode_Sequence>("4");
        runRoot.Add<BTNode_Action>("����",
            () =>
            {
                Vector3 position = _agent.MoveToClosestEndPosition();
                return BehaviorTree.ENodeStatus.InProgress;

            },
            () =>
            {
                return _agent.AtDestination ?
                BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
            });

        //��������

        var discardRoot = runRoot.Add<BTNode_Sequence>("4");
        discardRoot.Add<BTNode_Action>("������",
            () =>
            {
                _stack.ThrowResource();
                _target = null;
                _animator.SetBool(isMove, true);


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
            _animator.SetBool(isMove, true);
            _agent.MoveToRandomPosition();
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

    }
}
