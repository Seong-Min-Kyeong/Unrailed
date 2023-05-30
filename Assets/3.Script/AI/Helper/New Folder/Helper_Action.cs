using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helper_Action : MonoBehaviour
{
    public enum EState
    {
        Wood,
        Stone,
        Tracks,
        Sleep

    }

    public Dictionary<KeyCode, System.Action> Order;
    public EState HelperState = EState.Wood;

    private void Start()
    {
        Order = new Dictionary<KeyCode, Action>();
        Init();

    }

    void Init()
    {
        Order[KeyCode.Alpha1] = () =>
        {
            //���⼭ ������ ������� Ȯ���ϴ°� ���߿� �߰��ϱ�(�ٴڿ� ������ �ֳ���?)
            HelperState = EState.Wood;
            Debug.Log("���� ĳ����");
        };

        Order[KeyCode.Alpha2] = () =>
        {
            HelperState = EState.Stone;
            Debug.Log("�� ĳ����");
        };

        Order[KeyCode.Alpha3] = () =>
        {
            HelperState = EState.Tracks;
            Debug.Log("�ڿ� ����");
        };
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach(var dic in Order)
            {
                if(Input.GetKeyDown(dic.Key))
                {
                    dic.Value();
                }

            }
        }
    }

}
