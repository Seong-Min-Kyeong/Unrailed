using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Pick : AI_Item
{
    private int id = 2;
    public bool IsOn { get; protected set; } = false;

    public override void PickUp()
    {
        IsOn = !IsOn;
        Debug.Log($"��̴� ���� {(IsOn ? "����־��" : "�ٴ��̿���")}");
    }


    public override int Id()
    {
        return id;
    }
}
