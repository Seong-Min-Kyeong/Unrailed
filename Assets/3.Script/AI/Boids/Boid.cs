using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    BoidSettings settings;

    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    Vector3 velocity;

    //�̵� ����
    [HideInInspector]
    public Vector3 avgFlockHeading;

    //��� ȸ�� ����
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;

    //���� �߽� ��ġ�� ��Ÿ���� ����
    [HideInInspector]
    public Vector3 centreOfFlockmates;

    //�νĵ� �ֺ��� Boids ��
    [HideInInspector]
    public int numPerceivedFlockmates;

    Transform cachedTransform;
    Transform target;

    void Awake()
    {
        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.settings = settings;
        this.target = target;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void UpdateBoid()
    {
        //���ӵ� �ʱ�ȭ
        Vector3 acceleration = Vector3.zero;

        //Ÿ���� �ִٸ�
        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - position);

            //���ӵ� = �ӵ� ���(Ÿ�ٰ� ���� �Ÿ�) * target������ �̵��ϴ°� �󸶳� �߿����� ��
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }

        //������ ������ �ϳ��� ���ٸ�
        if (numPerceivedFlockmates != 0)
        {
            //������ �߽� ��ġ(Vector3) / ������ ������ ��(int) ������ 
            centreOfFlockmates /= numPerceivedFlockmates;

            //�߽ɿ��� �� ��ġ������ ����
            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            //������ ���ϴ� ��
            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            //�߽����� ���ϴ� �� 
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            //�ٸ� Boid�� ȸ���ϴ� ��
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;


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
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            //�����ֱ�
            acceleration += collisionAvoidForce;
        }

        //�����̰� �ϱ�
        velocity += acceleration * Time.deltaTime;
        //�̵� �ӵ�
        float speed = velocity.magnitude;
        //�̵� ����
        Vector3 dir = velocity / speed;
        //�̵� �ӵ� ����
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        //���� �̵� �ӵ�
        velocity = dir * speed;

        //��ġ ������Ʈ
        cachedTransform.position += velocity * Time.deltaTime;
        //cachedTransform.forward = 
        position = cachedTransform.position;
        forward = Vector3.forward;
        //forward = dir;
    }

    bool IsHeadingForCollision()
    {
        //���� ���� ������ �浹 ���ɼ��� �ִ��� �˻�
        //�浹 ���ɼ��� �ִٸ� ȸ�� ������ ������ �� �ֵ��� 

        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
        {
            //���� ��ġ���� �������� ��ü �߻�
            //�浹�� �߻��ϸ� true ��ȯ
            return true;
        }
        // �浹���� �ʾҴٸ� false ��ȯ
        else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        //��ֹ� ȸ�� 

        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask))
            {
                // ��ü�� ������, �˻�Ÿ�, ��ֹ� ����ũ
                // �浹�� �߻����� �ʴ´ٸ� �ش� ���� Vector ��ȯ
                return dir;
            }
        }

        //��ȿ�� ������ ���ٸ� ���� ���� ��ȯ 
        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        //�ӵ� ��� 

        //vector�� ����ȭ �� �� * �ִ� �ӵ� - ���� �ӵ�(�ӵ� ����)
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        //v���� maxSteerForce�� ���� ( ���ڱ� ���� �ٲ��� �ʰ� �������ִ� ���� ) 
        //���� �ӵ� ��ȯ
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
}
