using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AwarenessSystem))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Text _feedbackDisplay;

    private AwarenessSystem _awareness;

    public Vector3 EyeLocation => transform.position;
    public Vector3 EyeDirection => transform.forward;
    //���� ����
    [Header("���� ����")]
    [SerializeField] private float _detectionRange = 10f;
    public float DetectionRange => _detectionRange;
   

    private void Awake()
    {
        _awareness = GetComponent<AwarenessSystem>();
    }
    public void OnSuspicious()
    {
        _feedbackDisplay.text = "�ָ� �ִ°� ������";
    }

    public void OnDetected(GameObject target)
    {
        _feedbackDisplay.text = "������ ������ // ��ǥ :" + target.gameObject.name;
    }

    public void OnFullyDetected(GameObject target)
    {
        _feedbackDisplay.text = "�Ϻ��� ������ // ��ǥ : " + target.gameObject.name;
    }

    public void OnLostSuspicion()
    {
        _feedbackDisplay.text = "��ǥ ����";
    }

    public void OnLostDetect(GameObject target)
    {
        _feedbackDisplay.text = "��ǥ ����� //  ��ǥ : " + target.gameObject.name;
    }

    public void OnFullyLost()
    {
        _feedbackDisplay.text = "�ƹ��͵� �� ã��";
    }

    public void Report(DetectableTarget target)
    {
        _awareness.Report(target);
    }


}
