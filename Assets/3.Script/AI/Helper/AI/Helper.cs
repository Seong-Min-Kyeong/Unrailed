using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helper : MonoBehaviour
{
    public Resource Home { get; private set; } = null;
    public Resource dd;

    public Dictionary<KeyCode, System.Action> Order;
    [HideInInspector]
    public WorldResource.EType TargetResource;
    [HideInInspector]
    public WorldResource.EType DefaultResource = WorldResource.EType.Wood;
    private void Awake()
    {
        TargetResource = DefaultResource;
        Order = new Dictionary<KeyCode, Action>();
        Init();

        //���߿� �ϼ��Ǹ� ����
        Home = dd;
    }

    public void SetHome(Resource _Home)
    {
        Home = _Home;
    }

    void Init()
    {
        Order[KeyCode.Alpha1] = () =>
        {
            TargetResource = WorldResource.EType.Wood;
        };

        Order[KeyCode.Alpha2] = () =>
        {
            TargetResource = WorldResource.EType.Stone;
        };

        Order[KeyCode.Alpha3] = () =>
        {
            TargetResource = WorldResource.EType.Water;
        };
    }


    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var dic in Order)
            {
                if (Input.GetKeyDown(dic.Key))
                {
                    dic.Value();
                }

            }
        }
    }

}
