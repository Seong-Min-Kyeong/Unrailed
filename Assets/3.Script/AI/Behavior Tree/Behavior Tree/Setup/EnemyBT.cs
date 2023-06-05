using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBT : BaseAI
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };
        public static readonly BlackBoardKey HP = new BlackBoardKey() { Name = "HP" };

        public string Name;
    }

    private Blackboard<BlackBoardKey> _localMemory;
    private AnimalHealth _health;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _stack = GetComponent<AI_Stack>();
        _health = GetComponent<AnimalHealth>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource>(BlackBoardKey.CurrentTarget, null);
        _localMemory.SetGeneric(BlackBoardKey.HP, _health.CurrentHp);
        _animator.SetBool(isMove, true);

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
            var hp = _localMemory.GetGeneric<int>(BlackBoardKey.HP);
            return _target != null && hp == _health.CurrentHp;
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
        var stealDeco = stealRoot.AddDecorator<BTDecoratorBase>("��ĥ Ÿ���� ���� �����ϴ��� Ȯ���ϴ� Decorator", () =>
        {
            return _target != null;

        });

        stealRoot.Add<BTNode_Action>("Ÿ�� ���� : ��ġ�� ����",
        () =>
        {
            if(_target!=null)
            {
                _animator.SetBool(isMove, false);
                _stack.EnemyDetectGroundBlock(_target);
                //ó�� ��� �� 
                if (_stack._handItem.Count == 0)
                {
                    _stack.EnemyInteractiveItem();
                }
                //�� �� �ױ�
                else
                {
                    _stack.EnemyInteractiveAuto();
                }
                return BehaviorTree.ENodeStatus.InProgress;

            }
            return BehaviorTree.ENodeStatus.Failed;



        },
         () =>
         {
             return _target != null ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
         });


        var StackResource = MainSeq.Add<BTNode_Sequence>("3");
        StackResource.Add<BTNode_Action>("�ױ�",
            ()=>
            {
                return BehaviorTree.ENodeStatus.InProgress;

            },()=>
            {
                Home.TargettoSteal(_agent);
                //�ڿ��� �� �̻� ���ٸ� 
                if (Home.NonethisResourceType || _stack._handItem.Peek().EnemyCheckItemType)
                {
                    return BehaviorTree.ENodeStatus.Succeeded;
                }
                else
                    //�� �� ������� �ű��
                    return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;

            });


        var RunRoot = MainSeq.Add<BTNode_Sequence>("4");
        RunRoot.Add<BTNode_Action>("����",
        () =>
        {
            Vector3 position = _agent.MoveToClosestEndPosition();
            return BehaviorTree.ENodeStatus.InProgress;

        },
        () =>
        {
            return _agent.AtDestination ?
            BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;
        });


        //��������

        var DiscardRoot = RunRoot.Add<BTNode_Sequence>("5");
        DiscardRoot.Add<BTNode_Action>("������",
            () =>
            {
                SoundManager.Instance.PlaySoundEffect("Enemy_Laugh");
                _stack.EnemyThrowResource();
                _target = null;
                _animator.SetBool(isMove, true);
                return BehaviorTree.ENodeStatus.InProgress;
            },
            () =>
            {
                return BehaviorTree.ENodeStatus.Succeeded;
            }
            );


        var Attacked = BTRoot.Add<BTNode_Sequence>("����");
        Attacked.Add<BTNode_Action>("���� ����", () =>
         {
             if(_stack._handItem.Count!=0)
             {
                _currentblock = _stack.BFS(this);
                _stack.EnemyPutDown();
                _localMemory.SetGeneric(BlackBoardKey.HP, _health.CurrentHp);
                 _animator.SetBool(isMove, true);
                 _target = null;
             }
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return _target==null ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;

         });



        var WanderRoot = BTRoot.Add<BTNode_Sequence>("��ǥ �� ã��");
        WanderRoot.Add<BTNode_Action>("������ �̵���",
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
