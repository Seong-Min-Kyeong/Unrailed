using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Resource : MonoBehaviour
{
    public bool NonethisResourceType { get; private set; } = false;
    
    Helper Robot;

    [SerializeField] WorldResource.EType DefaultResource = WorldResource.EType.Wood;
    Dictionary<WorldResource.EType, List<WorldResource>> TrackedResources = null;


    [SerializeField] float PerfectKnowledgeRange = 30f;

    void Awake()
    {
        //���߿� �κ� ���� ����� �ٲٱ�
        Robot = FindObjectOfType<Helper>();
        Robot.SetHome(this);
    }

    private void Start()
    {
        PopulateResources();
    }

    private void PopulateResources()
    {
        //�ڿ� ����
        var resourceTypes = System.Enum.GetValues(typeof(WorldResource.EType));
        TrackedResources = new Dictionary<WorldResource.EType, List<WorldResource>>();
        foreach (var value in resourceTypes)
        {
            var type = (WorldResource.EType)value;
            TrackedResources[type] = ResourceTracker.Instance.GetResourcesInRange(type, transform.position, PerfectKnowledgeRange);
            //�� �ڿ���
        }
    }

    public WorldResource GetGatherTarget(Helper brain)
    {
        //�ڿ� ������Ʈ
        PopulateResources();
        WorldResource.EType targetResource = DefaultResource;
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
        if (TrackedResources[targetResource].Count <1)
        {
            NonethisResourceType = true;
            Debug.Log($"{targetResource} :  �ڿ��� ���� �����");
        }
        else
            NonethisResourceType = false;

        var sortedResources = TrackedResources[targetResource]
            //�� �� �ִ� ���� �ִ� �ڿ��� �߸���
            .Where(resource => Vector3.Distance(resource.transform.position, brain.Agent.
                                                FindCloestAroundEndPosition(resource.transform.position)) <= 1.5f)
            //����� ������ ����
            .OrderBy(resource => Vector3.Distance(brain.transform.position, resource.transform.position))
            //���� ����� �ڿ� ��ȯ
            .FirstOrDefault();
        return sortedResources;



    }

}
