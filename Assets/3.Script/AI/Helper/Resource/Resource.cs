using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Resource : MonoBehaviour
{
    public bool NonethisResourceType { get; private set; } = false;

    private WorldResource.EType _defaultResource = WorldResource.EType.Wood;
    private Dictionary<WorldResource.EType, List<WorldResource>> _trackedResources = null;
    private float _range = 30f;
    private AI_Stack _stack;

    public void SetHome(BaseAI robot)
    {
        robot.SetHome(this);
    }


    private void Start()
    {
        PopulateResources();
    }

    private void Update()
    {
        if(_trackedResources==null)
        {
            _stack = FindObjectOfType<AI_Stack>();
            PopulateResources();
        }
    }

    private void PopulateResources()
    {
        //�ڿ� ����
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        _trackedResources = new Dictionary<WorldResource.EType, List<WorldResource>>();
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            _trackedResources[type] = ResourceTracker.Instance.GetResourcesInRange(type, transform.position, _range);
            //�� �ڿ���
        }
    }

    public WorldResource GetGatherTarget(Helper brain)
    {
        //�ڿ� ������Ʈ
        PopulateResources();
        WorldResource.EType targetResource = _defaultResource;
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));

        foreach (var typeValue in resourceTypes)
        {
            var resourceType = (WorldResource.EType)typeValue;
            //����̶� ������ Ȯ��
            if(resourceType==brain.TargetResource)
            {
                targetResource = resourceType;
                break;
            }
        }
        //�ڿ��� �ִ��� Ȯ��
        if (_trackedResources[targetResource].Count <1)
        {
            NonethisResourceType = true;
            Debug.Log($"{targetResource} :  �ڿ��� ���� �����");
        }
        else
            NonethisResourceType = false;

        var sortedResources = _trackedResources[targetResource]
            //�� �� �ִ� ���� �ִ� �ڿ��� �߸���
            .Where(resource => Vector3.Distance(resource.transform.position, brain.Agent.
                                                FindCloestAroundEndPosition(resource.transform.position)) <= 1.5f)
            //����� ������ ����
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            //���� ����� �ڿ� ��ȯ
            .FirstOrDefault();

        return sortedResources;
    }


    public WorldResource TargettoSteal(PathFindingAgent brain)
    {
        //�ڿ� ������Ʈ
        PopulateResources();
        WorldResource.EType targetResource = WorldResource.EType.Resource;
        //�ڿ��� �ִ��� Ȯ��
        if (_trackedResources[targetResource].Count < 1)
        {
            NonethisResourceType = true;
            Debug.Log($"{targetResource} :  ��ĥ �ڿ��� ���� �����");
        }
        else
            NonethisResourceType = false;

        var sortedResources = _trackedResources[targetResource]
            //�� �� �ִ� ���� �ִ� �ڿ��� �߸���
            .Where(resource => Vector3.Distance(resource.transform.position, brain.
                                                FindCloestAroundEndPosition(resource.transform.position)) <= 1.5f)
            //����� ������ ����
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            //���� ����� �ڿ� ��ȯ
            .FirstOrDefault();
        return sortedResources;
    }





}
