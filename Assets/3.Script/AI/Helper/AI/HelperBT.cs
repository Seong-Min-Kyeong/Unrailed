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
    public Transform RightHand;

    private Vector3 _itemPosition;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;
    protected WorldResource _target;

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
            _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, order);
            
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


            foreach(var item in ItemManager.Instance.RegisteredObjects)
            {
                //�������� ����̶� ȣȯ�� �ȴٸ�
                if(item.Type==order)
                {
                    foreach (var interaction in item.Interactions)
                    {
                        //�÷��̾ ��� �ִ��� Ȯ���ϱ�
                        if (!interaction.CanPerform())
                        {
                            Debug.Log("���� ���� �ֳ�����");
                        }
                        _item = item;
                        Debug.Log($"{item.name} �ֿ� �� �־��");
                        break;
                    }

                }
            }

            //������ �̵��ϱ�
            _agent.MoveTo(_item.InteractionPoint);
            _animator.SetBool("isMove", true);
            //Ÿ�� �ڿ� ����
            _target = _helper.Home.GetGatherTarget(_helper);

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {

            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var pickUpTool = mainSelector.Add<BTNode_Action>("���� ���", () =>
         {

             _item.transform.SetParent(RightHand);
             _item.transform.localPosition = Vector3.zero;
             _item.transform.localRotation = Quaternion.identity;

             return BehaviorTree.ENodeStatus.InProgress;
         },
        () =>
        { 
       
             return _item.transform.parent == RightHand ? 
            BehaviorTree.ENodeStatus.Succeeded:BehaviorTree.ENodeStatus.InProgress;
        });


        var MoveToResource = mainSelector.Add<BTNode_Action>("�ڿ����� �̵�", () =>
         {
             //�ڿ����� �̵��ϱ�
             Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
             _agent.MoveTo(position);

             _animator.SetBool("isMove", true);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });


        var CollectResource = mainSelector.Add<BTNode_Action>("�ڿ� ä���ϱ�", () =>
         {
             _animator.SetBool("isDig", true);
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );














        var ss = BTRoot.Add<BTNode_Action>("����� ���� ���", () =>
         {
             if(_helper.Home!=null)
             {
             _target = _helper.Home.GetGatherTarget(_helper);
             _agent.MoveTo(_target.transform.position);

             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {

            return BehaviorTree.ENodeStatus.InProgress;
        });





    }
}
