using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Position;

    [HideInInspector]
    public Vector3 CentreOfFlockmates;

    [HideInInspector]
    public int NumPerceivedFlockmates;

    [HideInInspector]
    public Vector3 Center;
    [HideInInspector]
    public Vector3 TargetPosition;

    private Transform _transform;
    private Transform _target;

    private void Awake()
    {
        _transform = transform;

    }
    public void Init(Transform target)
    {
        this._target = target;
        Position = _transform.position;
    }

    public void UpdateFlock()
    {
        Center = Vector3.zero;
        TargetPosition = Vector3.zero;

        Position = _transform.position;

        if (NumPerceivedFlockmates != 0)
        {
            //������ �߽� ��ġ(Vector3) / ������ ������ ��(int) ������ 
            CentreOfFlockmates /= NumPerceivedFlockmates;

            //�߽ɿ��� �� ��ġ������ ����
            Vector3 offsetToFlockmatesCentre = (CentreOfFlockmates - Position);
            Center = offsetToFlockmatesCentre;

        }

        if(_target!=null)
        {
            TargetPosition = _target.position;
        }
    }
}
