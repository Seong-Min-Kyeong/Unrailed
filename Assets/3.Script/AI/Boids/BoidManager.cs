using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    Boid[] boids;
    public Transform target;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Initialize(settings, target);
        }

    }

    void Update()
    {
        if (boids != null)
        {

            
            int numBoids = boids.Length;

            //boid���� ��ġ��, �ٶ󺸴� ������
            //boidData�� �ֱ�
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++)
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }


            //CPU�� �۾��� GPU�� ��ȯ�ϴ� ����
            //�뷮�� ���� �˰��� ó���� Ưȭ
            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);
            
            //�޸� ���� ���� (Ŀ�� 0���� boids ��� �̸����� boidBuffer�� �ִ´�)
            compute.SetBuffer(0, "boids", boidBuffer);

            //boid�� ��
            compute.SetInt("numBoids", boids.Length);
            
            //������ �ν��ϴ� ����
            compute.SetFloat("viewRadius", settings.perceptionRadius);

            // �浹�� ���ϱ� ���� �ν��ϴ� ����
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            //boid�� �� / ������ �׷� ������
            //numBoids * numBoids �� ���� ������ ���� ������ ����ó�� �� �� �ְ� ��
            //���� ���� �ø��� �� = threadGroups

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            // compute shader ���� �޼���
            // Ŀ���� �ϳ��� ���������Ƿ� 0
            // ������ �׷��� ������ 1024, 1, 1

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}
