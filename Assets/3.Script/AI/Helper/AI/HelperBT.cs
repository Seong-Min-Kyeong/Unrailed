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
        public static readonly BlackBoardKey Destination = new BlackBoardKey() { Name = "Destination" };

        public string Name;

    }
    public Transform RightHand;
    public Transform BetweenTwoHands;

    public Text DebugTarget;

    private Vector3 _itemPosition;
    private Vector3 _home;
    private Animator _animator;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;

    private float _rotateSpeed = 10;

    protected WorldResource _target;
    protected WorldResource.EType _order;

    private void Awake()
    {
        _home = transform.position;
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
        _itemPosition = new Vector3(0, 0.5f, 0.5f);
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
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
        var woodRoot = mainSequence.Add<BTNode_Sequence>("1. ���� ã��");

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


            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {

            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });

        var pickUpTool = woodRoot.Add<BTNode_Action>("���� ���", () =>
         {
             switch(_item.Type)
             {
                 case WorldResource.EType.Water:
                _item.transform.SetParent(BetweenTwoHands);
                     break;
                 default:
                     _item.transform.SetParent(RightHand);
                     break;
             }

             _item.transform.localPosition = Vector3.zero;
             _item.transform.localRotation = Quaternion.identity;

             return BehaviorTree.ENodeStatus.InProgress;
         },
        () =>
        { 
       
             return _item.transform.parent == RightHand || BetweenTwoHands ? 
            BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        });






        var workRoot = mainSequence.Add<BTNode_Sequence>("2. ���ϱ�",()=>
        {
            //Ÿ�� �ڿ� ����

            return BehaviorTree.ENodeStatus.InProgress;

        },()=>
        {
       
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
        });
        var target = workRoot.Add<BTNode_Action>("Ÿ�� ���ϱ�", () =>
         {
             if(_target==null)
             {
                _target = _helper.Home.GetGatherTarget(_helper);
                Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, position);

             }
             return BehaviorTree.ENodeStatus.Succeeded;

         });

        var MoveToResource = workRoot.Add<BTNode_Action>("�ڿ����� �̵�", () =>
         {
             //�ڿ����� �̵��ϱ�
             DebugTarget.text = _target.name;
             Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
             _agent.MoveTo(pos);

             _animator.SetBool("isMove", true);

             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });


        var CollectResource = workRoot.Add<BTNode_Action>("�ڿ� ä���ϱ�", () =>
         {
             switch(_item.Type)
             {
                 case WorldResource.EType.Water:
                     _item.GetComponent<Item_Bucket_Water>().StartFilling();

                     break;

                 default:
                    _animator.SetBool("isDig", true);
                    StartCoroutine(_target.isDigCo());
                     break;

             }
             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>

         {
             Vector3 dir = _target.transform.position - transform.position;
             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);

             switch(_item.Type)
             {
                 case WorldResource.EType.Water:
                     if (!_item.GetComponent<Item_Bucket_Water>().isFilling)
                     { 
                         return BehaviorTree.ENodeStatus.Succeeded;
                     }
                     else
                     _item.GetComponent<Item_Bucket_Water>().FillGauge();

                     break;

                 default:
                     //����, ��
                     //����� �ٲ�� ������ ���ѹݺ�
                     if (!_target.isDig())
                     {
                         Destroy(_target.gameObject);
                         return BehaviorTree.ENodeStatus.Failed;
                     }
                     break;

             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );


        var MoveToHome = workRoot.Add <BTNode_Action>("�� ���� �̵��ϱ�", ()=>
        {
            _agent.MoveTo(_home);
            return BehaviorTree.ENodeStatus.InProgress;

        }
        ,()=>
        {
            return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
        
        }
        );

        var Sleep = workRoot.Add<BTNode_Action>("�������� �ڱ�", () =>
         {
             _animator.SetBool("isMove", false);
             _item.transform.position = transform.position + Vector3.forward;
             _item.transform.rotation = Quaternion.identity;
             _item.transform.parent = null;
             return BehaviorTree.ENodeStatus.InProgress;

         },()=>
         {
        
             return BehaviorTree.ENodeStatus.InProgress;
        
         }
         );










        var OrderChange = BTRoot.Add<BTNode_Sequence>("����� �ٲ� ���");
        OrderChange.Add<BTNode_Action>("���� ��������", () =>
         {

             //��������
             //PutDown();
             _item.transform.position = transform.position;
             _item.transform.rotation = Quaternion.identity;
             _item.transform.parent = null;
             _animator.SetBool("isDig", false);

             //������ ������Ʈ
             _item = null;
             _target = null;
             _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

             return BehaviorTree.ENodeStatus.Succeeded;
         });

        



    }


    private void PutDown()
    {
        _item.transform.position = transform.position;
        _item.transform.rotation = Quaternion.identity;
        _item.transform.parent = null;
        _animator.SetBool("isDig", false);
    }
}
