using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private BoidSettings _settings;

    [HideInInspector]
    public Vector3 Position;
    [HideInInspector]
    public Vector3 Forward;

    private Vector3 _velocity;

    //�̵� ����
    [HideInInspector]
    public Vector3 AvgFlockHeading;

    //��� ȸ�� ����
    [HideInInspector]
    public Vector3 AvgAvoidanceHeading;

    //���� �߽� ��ġ�� ��Ÿ���� ����
    [HideInInspector]
    public Vector3 CentreOfFlockmates;

    //�νĵ� �ֺ��� Boids ��
    [HideInInspector]
    public int NumPerceivedFlockmates;

    private Transform _cachedTransform;
    private Transform _target;

    void Awake()
    {
        _cachedTransform = transform;
    }

    public void Initialize(BoidSettings _settings, Transform _target)
    {
        this._settings = _settings;
        this._target = _target;

        Position = _cachedTransform.position;
        Forward = _cachedTransform.forward;

        float startSpeed = (_settings.MinSpeed + _settings.MaxSpeed) / 2;
        _velocity = transform.forward * startSpeed;
    }

    public void UpdateBoid()
    {
        //���ӵ� �ʱ�ȭ
        Vector3 acceleration = Vector3.zero;

        //Ÿ���� �ִٸ�
        if (_target != null)
        {
            Vector3 offsetToTarget = (_target.position - Position);

            //���ӵ� = �ӵ� ���(Ÿ�ٰ� ���� �Ÿ�) * _target������ �̵��ϴ°� �󸶳� �߿����� ��
            acceleration = SteerTowards(offsetToTarget) * _settings.TargetWeight;
        }

        //������ ������ �ϳ��� ���ٸ�
        if (NumPerceivedFlockmates != 0)
        {
            //������ �߽� ��ġ(Vector3) / ������ ������ ��(int) ������ 
            CentreOfFlockmates /= NumPerceivedFlockmates;

            //�߽ɿ��� �� ��ġ������ ����
            Vector3 offsetToFlockmatesCentre = (CentreOfFlockmates - Position);

            //������ ���ϴ� ��
            var alignmentForce = SteerTowards(AvgFlockHeading) * _settings.AlignWeight;
            //�߽����� ���ϴ� �� 
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * _settings.CohesionWeight;
            //�ٸ� Boid�� ȸ���ϴ� ��
            var seperationForce = SteerTowards(AvgAvoidanceHeading) * _settings.SeperateWeight;


            //�� �����ֱ�
            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        //�浹 ���ɼ��� �ִٸ�
        if (IsHeadingForCollision())
        {
            //�浹�� �߻����� �ʴ� ���� 
            Vector3 collisionAvoidDir = ObstacleRays();

            //�浹�� ���ϱ� ���� ����ġ * �ӵ�(����)
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * _settings.AvoidCollisionWeight;
            //�����ֱ�
            acceleration += collisionAvoidForce;
        }

        //�����̰� �ϱ�
        _velocity += acceleration * Time.deltaTime;
        //�̵� �ӵ�
        float speed = _velocity.magnitude;
        //�̵� ����
        Vector3 dir = _velocity / speed;
        //�̵� �ӵ� ����
        speed = Mathf.Clamp(speed, _settings.MinSpeed, _settings.MaxSpeed);
        //���� �̵� �ӵ�
        _velocity = dir * speed;

        //��ġ ������Ʈ
        _cachedTransform.position += _velocity * Time.deltaTime;
        Position = _cachedTransform.position;
        Forward = Vector3.forward;
    }

    bool IsHeadingForCollision()
    {
        //���� ���� ������ �浹 ���ɼ��� �ִ��� �˻�
        //�浹 ���ɼ��� �ִٸ� ȸ�� ������ ������ �� �ֵ��� 

        RaycastHit hit;
        if (Physics.SphereCast(Position, _settings.BoundsRadius, Forward, out hit, _settings.CollisionAvoidDst, _settings.ObstacleMask))
        {
            //���� ��ġ���� �������� ��ü �߻�
            //�浹�� �߻��ϸ� true ��ȯ
            return true;
        }
        // �浹���� �ʾҴٸ� false ��ȯ
        else return false;
    }

    Vector3 ObstacleRays()
    {
        //��ֹ� ȸ�� 

        Vector3[] rayDirections = BoidHelper.Directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = _cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(Position, dir);
            if (!Physics.SphereCast(ray, _settings.BoundsRadius, _settings.CollisionAvoidDst, _settings.ObstacleMask))
            {
                // ��ü�� ������, �˻�Ÿ�, ��ֹ� ����ũ
                // �浹�� �߻����� �ʴ´ٸ� �ش� ���� Vector ��ȯ
                return dir;
            }
        }

        //��ȿ�� ������ ���ٸ� ���� ���� ��ȯ 
        return Forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        //�ӵ� ��� 

        //vector�� ����ȭ �� �� * �ִ� �ӵ� - ���� �ӵ�(�ӵ� ����)
        Vector3 v = vector.normalized * _settings.MaxSpeed - _velocity;

        //���� �ӵ� ��ȯ
        return Vector3.ClampMagnitude(v, _settings.MaxSteerForce);
        //v���� MaxSteerForce�� ���� ( ���ڱ� ���� �ٲ��� �ʰ� �������ִ� ���� ) 
    }
}
