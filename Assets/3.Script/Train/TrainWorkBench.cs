using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainWorkBench : TrainMovement
{
    // Start is called before the first frame update

    
    Animator anim;
    [SerializeField] GameObject prefabsRail;
    [SerializeField] Transform[] railSpawnPos;

    public int spawnIndex;
    public float spawnSpeed;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        GetMesh();
        fireEffect.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        TrainMovePos();
        anim.SetBool("isBurn", isBurn);
    }

    public void MakingRail()
    {
        if (!isBurn)
        {
            anim.SetInteger("GetRails", spawnIndex);
        }
        //�����Ǵ� ���� ���� ����
     
    }
    public override void TrainUpgrade()
    {
        base.TrainUpgrade();
        //���׷��̵� �޼���
        switch (trainUpgradeLevel)
        {
            case 1:
                spawnSpeed = 3;
                break;
            case 2:
                spawnSpeed = 1.8f;

                break;
            default:
                spawnSpeed = 0;
                break;
        }
    }
}