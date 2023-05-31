using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Text DebugTarget;

    private Vector3 _itemPosition;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;

    protected WorldResource _target;
    protected WorldResource.EType _order;

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
        _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, WorldResource.EType.Wood);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");
        BTRoot.AddService<BTServiceBase>("����� ��ٸ��� Service", (float deltaTime) =>
        {
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            _order = _helper.TargetResource;
            
        });

        var OrderRoot = BTRoot.Add<BTNode_Sequence>("����� �ֳ���?");
        var deco = OrderRoot.AddDecorator<BTDecoratorBase>("����� �ٲ������ Ȯ��", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order;
         });

        var mainSequence = OrderRoot.Add<BTNode_Sequence>("����� �ִ� ���");
        var woodRoot = mainSequence.Add<BTNode_Sequence>("��� : ����/�� ĳ��");
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
            DebugTarget.text = _item.name;
            _agent.MoveTo(_item.InteractionPoint);
            _animator.SetBool("isMove", true);
            //Ÿ�� �ڿ� ����
            _target = _helper.Home.GetGatherTarget(_helper);

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {

            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var pickUpTool = mainSequence.Add<BTNode_Action>("���� ���", () =>
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


        var MoveToResource = mainSequence.Add<BTNode_Action>("�ڿ����� �̵�", () =>
         {
             //�ڿ����� �̵��ϱ�
             DebugTarget.text = _target.name;
             Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
             _agent.MoveTo(position);

             _animator.SetBool("isMove", true);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });


        var CollectResource = mainSequence.Add<BTNode_Action>("�ڿ� ä���ϱ�", () =>
         {
             _animator.SetBool("isDig", true);
             _target.isDig();
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return _target.isDig() ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Succeeded;
         }
        );




        var OrderChange = BTRoot.Add<BTNode_Sequence>("����� �ٲ� ���");
        OrderChange.Add<BTNode_Action>("���� ��������", () =>
         {
             //��������
             _item.transform.position = transform.position;
             _item.transform.rotation = Quaternion.identity;
             _item.transform.parent = null;
             _animator.SetBool("isDig", false);

             //������ ������Ʈ
             _item = null;
             _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

             return BehaviorTree.ENodeStatus.Succeeded;
         });




    }
}
