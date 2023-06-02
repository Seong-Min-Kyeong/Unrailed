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

    [Header("Transform")]
    [SerializeField] private Transform _rayStartTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _twoHandTransform;


    private Transform _currentblock;
    public Transform RayStartTransfrom => _rayStartTransform;
    public Transform CurrentBlockTransform => _currentblock;
    public Transform RightHandTransform => _rightHandTransform;
    public Transform TwoHandTransform => _twoHandTransform;

    public Text DebugTarget;

    private Vector3 _home;
    private Animator _animator;
    private float _rotateSpeed = 10;

    protected Helper _helper;
    protected BehaviorTree _tree;
    protected PathFindingAgent _agent;
    protected Blackboard<BlackBoardKey> _localMemory;
    protected AI_Item _item;
    protected AI_Stack _stack;


    protected WorldResource _target;
    protected WorldResource.EType _order;

    private void Awake()
    {
        _stack = GetComponent<AI_Stack>();
        _home = transform.position;
        _helper = GetComponent<Helper>();
        _animator = GetComponent<Animator>();
        _tree = GetComponent<BehaviorTree>();
        _agent = GetComponent<PathFindingAgent>();
    }

    private void Start()
    {
        _localMemory = BlackboardManager.Instance.GetIndividualBlackboard<BlackBoardKey>(this);
        _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, WorldResource.EType.Wood);

        var BTRoot = _tree.RootNode.Add<BTNode_Selector>("BT ����");
        BTRoot.AddService<BTServiceBase>("����� ��ٸ��� Service", (float deltaTime) =>
        {
            // �� ��� 
            var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
            // ���� ���
            _order = _helper.TargetResource;

        });

        var OrderRoot = BTRoot.Add<BTNode_Sequence>("����� �ֳ���?");
        var CheckOrder = OrderRoot.AddDecorator<BTDecoratorBase>("����� �ٲ������ Ȯ��", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order;
         });

        var MainSequence = OrderRoot.Add<BTNode_Sequence>("����� �ִ� ���");


        #region ���� ĳ��, �� ĳ��, �� ������ ���
        var FindTools = MainSequence.Add<BTNode_Sequence>("1. ���� ã��");

        var MoveToItem = FindTools.Add<BTNode_Action>("���� ���ϱ�, �̵��ϱ�", () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             foreach (var item in ItemManager.Instance.RegisteredObjects)
             {
                //�������� ����̶� ȣȯ�� �ȴٸ�
                if (item.Type == order)
                 {
                     foreach (var interaction in item.Interactions)
                     {
                        //�÷��̾ ��� �ִ��� Ȯ���ϱ�
                        if (interaction.CanPerform())
                         {
                             interaction.Perform();
                             _item = item;
                             _agent.MoveTo(_item.InteractionPoint);
                             DebugTarget.text = _item.name;
                         }
                         else
                         {
                            //������ ������϶�
                        }
                         break;
                     }

                 }

             }

            //������ �̵��ϱ�

            _animator.SetBool("isMove", true);


             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {

             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
         });

        var PickUpTool = FindTools.Add<BTNode_Action>("���� ���", () =>
         {
             if (_item != null)
             {

                 switch (_item.Type)
                 {
                     //�絿�̸� ���
                     case WorldResource.EType.Water:
                         _item.transform.SetParent(TwoHandTransform);
                         break;
                     //���, ������ �� ��
                     default:
                         _item.transform.SetParent(RightHandTransform);
                         break;
                 }

                 _item.transform.localPosition = Vector3.zero;
                 _item.transform.localRotation = Quaternion.identity;

                 return BehaviorTree.ENodeStatus.InProgress;
             }

             else
                 return BehaviorTree.ENodeStatus.Succeeded;
         },
        () =>
        {
            return BehaviorTree.ENodeStatus.Succeeded;
        });


        var workRoot = MainSequence.Add<BTNode_Sequence>("2. ���ϱ�", () =>
         {
            //Ÿ�� �ڿ� ����
            return BehaviorTree.ENodeStatus.InProgress;

         }, () =>
         {
             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
             return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
         });

        var target = workRoot.Add<BTNode_Action>("Ÿ�� ���ϱ�", () =>
         {
             if (_target == null)
             {
                 //��ǥ �ڿ�
                 _target = _helper.Home.GetGatherTarget(_helper);
                 Vector3 position = _agent.FindCloestAroundEndPosition(_target.transform.position);
                 _localMemory.SetGeneric<Vector3>(BlackBoardKey.Destination, position);

             }
             return BehaviorTree.ENodeStatus.Succeeded;

         });
        var PossibleToWork = workRoot.Add<BTNode_Selector>("���ϱ� ������");

        var PossibleSequence = PossibleToWork.Add<BTNode_Sequence>("����", () =>
         {
             Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
             return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;
         }
        );
        var MoveToResource = PossibleSequence.Add<BTNode_Action>("�ڿ����� �̵�", () =>
         {
             //�ڿ����� �̵��ϱ�
             DebugTarget.text = _target.name;
             _animator.SetBool("isMove", true);
             return BehaviorTree.ENodeStatus.InProgress;

         }
        , () =>
         {
             return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;

         });

        var ImpossibleToWork = PossibleToWork.Add<BTNode_Sequence>("�Ұ���", () =>
        {
            //�ڿ��� �� �ǳʿ� ���� ��
            Vector3 pos = _agent.FindCloestAroundEndPosition(_target.transform.position);
            return _agent.MoveTo(pos) ? BehaviorTree.ENodeStatus.Failed : BehaviorTree.ENodeStatus.InProgress;

        });

        ImpossibleToWork.Add<BTNode_Action>("�װ� ���ؿ�", () =>
         {
             DebugTarget.text = "�ڴ� ��";
             return BehaviorTree.ENodeStatus.InProgress;
         },
         () =>
          {
              return BehaviorTree.ENodeStatus.InProgress;
          });



        var sel = workRoot.Add<BTNode_Selector>("�ڿ� ������ ���� �ٸ� �ൿ�ϱ�");
        var wood = sel.Add<BTNode_Sequence>("[����, ��]", () =>
         {
             return _target.Type == WorldResource.EType.Wood || _target.Type == WorldResource.EType.Stone
                ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;

         });

        var CollectResource = wood.Add<BTNode_Action>("��� ä���ϱ�", () =>
         {

             _animator.SetBool("isDig", true);
             StartCoroutine(_target.isDigCo());

             return BehaviorTree.ENodeStatus.InProgress;

         }, () =>

         {
             Vector3 dir = _target.transform.position - transform.position;
             transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotateSpeed);

             if (!_target.isDig())
             {
                 Destroy(_target.gameObject);
                 _animator.SetBool("isDig", false);
                 return BehaviorTree.ENodeStatus.Succeeded;
             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        );

        // �� ==========================================================================
        var water = sel.Add<BTNode_Sequence>("[��, �ڿ�]", () =>
         {
             return _target.Type == WorldResource.EType.Water || _target.Type == WorldResource.EType.Resource
             ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;

         });

        var Filling = water.Add<BTNode_Action>("�� ä���", () =>
       {
           return BehaviorTree.ENodeStatus.InProgress;

       }
        , () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                    //������ ��ȣ�ۿ� �� ������ ��ȣ�ۿ� ã��
                    //���̶�� �� �߱�
                    foreach (var interaction in _item.Interactions)
                     {
                         if (interaction.CanPerform())
                         {
                             if (interaction.Perform())
                             {
                                 return BehaviorTree.ENodeStatus.InProgress;
                             }
                             else
                                 return BehaviorTree.ENodeStatus.Succeeded;
                         }
                     }
                     break;

                 case WorldResource.EType.Resource:
                    //�ڿ��̶�� ���� �ױ�
                    _currentblock = _target.transform;
                     _stack.DetectGroundBlock(_target);
                     if (_stack._handItem.Count == 0)
                     {
                         _stack.InteractiveItemSpace();
                     }

                     else
                     {
                         _stack.InteractiveItem();
                     }
                     ResourceTracker.Instance.DeRegisterResource(_target);
                     Destroy(_target);
                     return BehaviorTree.ENodeStatus.Succeeded;

             }


             return BehaviorTree.ENodeStatus.InProgress;

         }
        );

        var MoveToHome = water.Add<BTNode_Action>("�� ���ٳ��� / ���� �ڿ����� �̵��ϱ�", () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                     _agent.MoveTo(_home);
                     break;

                 case WorldResource.EType.Resource:
                     break;



             }
             return BehaviorTree.ENodeStatus.InProgress;
             //���߿� ���� ��ǥ�� �ٲٱ�

         }, () =>
          {
              return _agent.AtDestination ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.InProgress;
          }
         );

        var Sleep = water.Add<BTNode_Action>("�ڱ�", () =>
         {
             switch (_target.Type)
             {
                 case WorldResource.EType.Water:
                         //PutDown();
                         DebugTarget.text = "�ڴ� ��";
                     break;

                 case WorldResource.EType.Resource:
                     break;


             }

             return BehaviorTree.ENodeStatus.InProgress;
         }
        , () =>
         {

             switch(_target.Type)
             {
                 case WorldResource.EType.Resource:
                     return _stack._handItem.Count == 3 ? BehaviorTree.ENodeStatus.Succeeded : BehaviorTree.ENodeStatus.Failed;
                 case WorldResource.EType.Water:
                     return BehaviorTree.ENodeStatus.InProgress;



             }

             return BehaviorTree.ENodeStatus.InProgress;
             //�ڿ�


             //��
             /*             var order = _localMemory.GetGeneric<WorldResource.EType>(BlackBoardKey.Order);
                          return order == _order ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.Failed;*/

         });

        var Carrying = water.Add<BTNode_Action>("�ڿ� ����ϱ�", () =>
        {
            if(_target!=null)
            {
                _agent.MoveTo(_home);

            }
            return BehaviorTree.ENodeStatus.InProgress;
        },
        () =>
        {
            if (_target != null)
            {

                return _agent.AtDestination ? BehaviorTree.ENodeStatus.InProgress : BehaviorTree.ENodeStatus.InProgress;
            }
            else
                return BehaviorTree.ENodeStatus.InProgress;
            //���߿� �ٲٱ�
        }
        );




        #endregion




        var OrderChange = BTRoot.Add<BTNode_Sequence>("����� �ٲ� ���");
        OrderChange.Add<BTNode_Action>("���� �������� �ڱ�", () =>
         {
             //_target = null;
             PutDown();
             /*             if (_item != null)
                          {
                              foreach (var interaction in _item.Interactions)
                              {
                                  //��������
                                  if (!interaction.CanPerform())
                                  {
                                      interaction.Perform();
                                  }
                                  break;
                              }
                              PutDown();

                          }*/

             //������ ������Ʈ
             //_item = null;


             return BehaviorTree.ENodeStatus.Succeeded;
         });





    }


    private void PutDown()
    {
        if (_item != null)
        {
            foreach (var interaction in _item.Interactions)
            {
                //��������
                if (!interaction.CanPerform())
                {
                    interaction.Perform();
                }
                break;
            }
            _item.transform.position = transform.position + Vector3.left;
            _item.transform.rotation = Quaternion.identity;
            _item.transform.parent = null;

            _item = null;
            _target = null;
            _localMemory.SetGeneric<WorldResource.EType>(BlackBoardKey.Order, _order);

            _animator.SetBool("isDig", false);
            _animator.SetBool("isMove", false);

        }





}
}
