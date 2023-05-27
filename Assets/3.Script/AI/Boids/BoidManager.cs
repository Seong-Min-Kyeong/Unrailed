using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public BoidSettings Settings;
    public ComputeShader Compute;
    public Transform Target;

    const int threadGroupSize = 1024;
    private Boid[] boids;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid b in boids)
        {
            b.Init(Settings, Target);
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
                boidData[i].Position = boids[i].Position;
                boidData[i].Direction = boids[i].Forward;
            }


            //CPU�� �۾��� GPU�� ��ȯ�ϴ� ����
            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);
            
            //�޸� ���� ���� (Ŀ�� 0���� boids ��� �̸����� boidBuffer�� �ִ´�)
            Compute.SetBuffer(0, "boids", boidBuffer);

            //boid�� ��
            Compute.SetInt("numBoids", boids.Length);
            
            //������ �ν��ϴ� ����
            Compute.SetFloat("viewRadius", Settings.PerceptionRadius);

            // �浹�� ���ϱ� ���� �ν��ϴ� ����
            Compute.SetFloat("avoidRadius", Settings.AvoidanceRadius);

            //boid�� �� / ������ �׷� ������
            //numBoids * numBoids �� ���� ������ ���� ������ ����ó�� �� �� �ְ� ��
            //���� ���� �ø��� �� = threadGroups

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            Compute.Dispatch(0, threadGroups, 1, 1);

            // Compute shader ���� �޼���
            // Ŀ���� �ϳ��� ���������Ƿ� 0
            // ������ �׷��� ������ 1024, 1, 1

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++)
            {
                boids[i].AvgFlockHeading = boidData[i].FlockHeading;
                boids[i].CentreOfFlockmates = boidData[i].FlockCentre;
                boids[i].AvgAvoidanceHeading = boidData[i].AvoidanceHeading;
                boids[i].NumPerceivedFlockmates = boidData[i].NumFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 Position;
        public Vector3 Direction;

        public Vector3 FlockHeading;
        public Vector3 FlockCentre;
        public Vector3 AvoidanceHeading;
        public int NumFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}
