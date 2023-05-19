using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackedTarget
{
    public DetectableTarget Detectable;
    public Vector3 RawPosition;

    public float Awareness;

    //Ÿ���� �ִ��� ��� Ȯ��
    public bool UpdateAwareness(DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        var oldAwareness = Awareness;

        if (target != null)
            Detectable = target;
        RawPosition = position;

        Awareness = Mathf.Clamp(Mathf.Max(Awareness, minAwareness) + awareness, 0f, 2f);

        //Ÿ���� �־��µ� ������ٸ�
        if (oldAwareness >= 1 && Awareness <= 0)
            return false;

        //�ƴ϶��
        return true;
    }

}
[RequireComponent(typeof(EnemyAI))]
public class AwarenessSystem : MonoBehaviour
{

    //�ν��� �ּҰ�
    [SerializeField] private float _minimumAwareness = 0f;
    //�ν��� ���� �ӵ�
    [SerializeField] private float _awarenessBuildRate = 1f;


    private Dictionary<GameObject, TrackedTarget> _targets = new Dictionary<GameObject, TrackedTarget>();
    public Dictionary<GameObject, TrackedTarget> ActiveTargets => _targets;

    private EnemyAI _enemyAI;


    private void Start()
    {
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Update()
    {
        List<GameObject> toCleanup = new List<GameObject>();
        foreach(var targetGO in _targets.Keys)
        {
            //�� �̻� �ν��� �� ���� ��
            if(_targets[targetGO].Awareness<=0)
            {
                toCleanup.Add(targetGO);
            }

        }
        // ����
        foreach (var target in toCleanup)
            _targets.Remove(target);

    }

    private void AddTarget(GameObject targetGO, DetectableTarget target, Vector3 position)
    {
        if (!_targets.ContainsKey(targetGO))
        {
            _targets[targetGO] = new TrackedTarget();
        }
    }

    public void FindTarget(DetectableTarget target)
    {
        AddTarget(target.gameObject, target, target.transform.position);
    }

    private void UpdateAwareness(GameObject targetGO, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        //���⼭ Ÿ���� Ű�� �߰�
        if (!_targets.ContainsKey(targetGO))
        {
            _targets[targetGO] = new TrackedTarget();

        }
        if (_targets[targetGO].UpdateAwareness(target, position, awareness, minAwareness))
        {
            if (_targets[targetGO].Awareness >= 2f)
            {
                _enemyAI.OnFullyDetected(targetGO); //���� ����
            }
            else if (_targets[targetGO].Awareness >= 1f) //
                _enemyAI.OnDetected(targetGO);
            else if (_targets[targetGO].Awareness >= 0f)
                _enemyAI.OnSuspicious();
        }
    }

    public void Report(DetectableTarget target)
    {
        //�ν� �������� �ִ�.
        var awareness = _awarenessBuildRate * Time.deltaTime;
        UpdateAwareness(target.gameObject, target, target.transform.position, awareness, _minimumAwareness);
    }


}
