using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(BehaviorTree))]
public class HelperBT : MonoBehaviour
{
    public class BlackBoardKey : BlackboardKeyBase
    {
        public static readonly BlackBoardKey Order = new BlackBoardKey() { Name = "Order" };
        public static readonly BlackBoardKey CurrentTarget = new BlackBoardKey() { Name = "CurrentTarget" };

        public string Name;

    }


    private Vector3 _itemPosition;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    WorldResource Target;

    private void Awake()
    {
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<DetectableTarget>(BlackBoardKey.CurrentTarget, null);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");
        BTRoot.AddService<BTServiceBase>("����� ��ٸ��� Service", (float deltaTime) =>
        {
            var order = _helper.TargetResource;
            _localMemory.SetGeneric(BlackBoardKey.Order, order);
            
        });

        var dd = BTRoot.Add<BTNode_Sequence>("����� �ֳ���?");
        var deco = dd.AddDecorator<BTDecoratorBase>("��� Ȯ��", () =>
         {
             return _helper.Home != null;
         });

        var mainSelector = dd.Add<BTNode_Sequence>("����� �ִ� ���");
        var woodRoot = mainSelector.Add<BTNode_Sequence>("��� : ����/�� ĳ��");
        //���� ���
        //������ �̵��ϱ�
        //���� ����
        var MoveToItem = woodRoot.Add<BTNode_Action>("������ �̵�",()=>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            Vector3 itemPosition;

            switch(order)
            {
                case WorldResource.EType.Stone:
                    //itemPosition = �����ġ;
                    Debug.Log("���");
                    break;
                case WorldResource.EType.Wood:
                    Debug.Log("����");
                    break;
                case WorldResource.EType.Water:
                    Debug.Log("�絿��");
                    break;

            }

            //Ÿ�� ����
            Target = _helper.Home.GetGatherTarget(_helper);
            //������ �̵��ϱ�
            Debug.Log(Target);

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {
            return BehaviorTree.ENodeStatus.Succeeded;
            //return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var MoveToResource = mainSelector.Add<BTNode_Action>("�ڿ����� �̵�", () =>
         {
             Vector3 position = Target.transform.position;
             //�ڿ����� �̵��ϱ�
             Vector3 dd = _agent.FindCloestAroundEndPosition(position);
             _agent.MoveTo(dd);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
             //return BehaviorTree.ENodeStatus.InProgress;

         });














        var ss = BTRoot.Add<BTNode_Action>("����� ���� ���", () =>
         {
             if(_helper.Home!=null)
             {
             Target = _helper.Home.GetGatherTarget(_helper);
             _agent.MoveTo(Target.transform.position);

             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {

            return BehaviorTree.ENodeStatus.InProgress;
        });





    }
}
