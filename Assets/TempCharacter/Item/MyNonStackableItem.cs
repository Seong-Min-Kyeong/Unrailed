using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNonStackableItem : MyItem
{
    public override Pair<Stack<MyItem>, Stack<MyItem>> AutoGain(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // �̰� �ƹ��͵� ���ص� ��
        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> Change(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // handItem�� ������ nonStackableItem

        EItemType detectedItemType = detectedItem.Peek().ItemType;
        if(detectedItemType == EItemType.axe || detectedItemType == EItemType.pick || detectedItemType == EItemType.bucket)
        {
            MyItem temp = handItem.Pop();
            handItem.Push(detectedItem.Pop());
            detectedItem.Push(temp);
            handItem.Peek().RePosition(player.RightHandTransform, Vector3.zero);
            detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f);
        }
        else if(detectedItemType == EItemType.wood || detectedItemType == EItemType.steel)
        {
            MyItem temp = handItem.Pop();
            for(int i = 0; i < 3; i++)
            {
                if (detectedItem.Count == 0)
                    break;

                handItem.Push(detectedItem.Pop());
                handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * i * stackInterval);
            }
            detectedItem.Push(temp);
            detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
        }
        else if(detectedItemType == EItemType.rail)
        {
            if(!CheckConnectedRail())
            {
                MyItem temp = handItem.Pop();
                for (int i = 0; i < 3; i++)
                {
                    if (detectedItem.Count == 0)
                        break;

                    handItem.Push(detectedItem.Pop());
                    handItem.Peek().RePosition(player.TwoHandTransform, Vector3.up * i * stackInterval);
                }
                detectedItem.Push(temp);
                detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f + Vector3.up * (detectedItem.Count - 1) * stackInterval);
            }
        }

        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PickUp(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // �׳� �ֿ�
        handItem.Push(detectedItem.Pop());
        handItem.Peek().RePosition(player.RightHandTransform, Vector3.zero);
        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }

    public override Pair<Stack<MyItem>, Stack<MyItem>> PutDown(Stack<MyItem> handItem, Stack<MyItem> detectedItem)
    {
        // �׳� ����
        detectedItem.Push(handItem.Pop());
        detectedItem.Peek().RePosition(player.CurrentBlockTransform, Vector3.up * 0.5f);
        return new Pair<Stack<MyItem>, Stack<MyItem>>(handItem, detectedItem);
    }
}
