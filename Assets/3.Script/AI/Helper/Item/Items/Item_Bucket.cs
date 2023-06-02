using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Bucket : AI_Item
{
    public bool IsOn { get; protected set; } = false;

    public void PickUp()
    {
        IsOn = !IsOn;
        Debug.Log($"�絿�̴� ���� {(IsOn ? "����־��" : "�ٴ��̿���")}");
    }
}
