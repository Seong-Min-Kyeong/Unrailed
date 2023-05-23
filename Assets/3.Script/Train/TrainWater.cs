using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWater : TrainMovement
{
    public float fireTime;
    public float overFireTime;

    [SerializeField] private float fireLimit;
    [SerializeField] TrainMovement[] trains;


    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        trains = FindObjectsOfType<TrainMovement>();


    }

    private void OnEnable()
    {
        fireTime = 0;
        GetMesh();
        fireEffect.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        fireTime += Time.deltaTime;
        TrainMovePos();
        FireOn();
    }

    public void FireOn()
    {
        //��Ÿ����
        if(fireLimit < fireTime)
        {
            isBurn = true;
            Fire();
            fireEffect.gameObject.SetActive(true);
            //���� ����
            if (overFireTime < fireTime)
            {
                for (int i = 0; i < trains.Length; i++)
                {
                    trains[i].gameObject.SetActive(false);
                    fireTime = 0;
                    //bool������ ���ӿ��� �� �ð��� �Ȱ��� ���ǹ� �����
                }
            }
        }
    }
    public void FireOff()
    {
        fireTime = 0;
        isBurn = false;
        for (int i = 0; i < trains.Length; i++)
        {
            trains[i].isBurn = false;
            trains[i].fireEffect.gameObject.SetActive(false);
        }
    }

    private void Fire()
    {
        if(70 < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if(trains[i].trainNum == 0)
                {
                    trains[i].isBurn = true;
                    //��ƼŬ �ý��� ���⿡�� �ҷ� ��ü�Ұ�
                }
            }
        }
        if (80 < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainNum == 1)
                {
                    trains[i].isBurn = true;
                    trains[i].fireEffect.gameObject.SetActive(true);
                }

            }
        }
        if (90 < fireTime)
        {
            for (int i = 0; i < trains.Length; i++)
            {
                if (trains[i].trainNum == 2)
                {
                    trains[i].isBurn = true;
                    trains[i].fireEffect.gameObject.SetActive(true);
                }
          

            }
        }
    }

}
