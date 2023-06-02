using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType { wood, steel, rail, pick, axe, bucket, train, animal };

public abstract class MyItem : MonoBehaviour
{
    [SerializeField] protected float stackInterval = 0.15f;
    [SerializeField] protected EItemType itemType;
    [SerializeField] protected LayerMask blockLayer;

    protected PlayerController player;

    protected virtual void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    public EItemType ItemType => itemType;

    public abstract Pair<Stack<MyItem>, Stack<MyItem>> PickUp(Stack<MyItem> handItem, Stack<MyItem> detectedItem);  // �ݴ� �޼ҵ�
    public abstract Pair<Stack<MyItem>, Stack<MyItem>> PutDown(Stack<MyItem> handItem, Stack<MyItem> detectedItem);  // ������ �޼ҵ�
    public abstract Pair<Stack<MyItem>, Stack<MyItem>> Change(Stack<MyItem> handItem, Stack<MyItem> detectedItem);   // ��ü�ϴ� �޼ҵ�
    public abstract Pair<Stack<MyItem>, Stack<MyItem>> AutoGain(Stack<MyItem> handItem, Stack<MyItem> detectedItem);  // �ڵ����� �Դ� �޼ҵ�

    public virtual void RePosition(Transform parent, Vector3 pos)
    {
        transform.SetParent(parent);
        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public virtual bool CheckItemType(MyItem item)
    {
        return itemType.Equals(item.ItemType);
    }


    // ���� ��ġ �յ��¿쿡 ��ġ��(isInstance) ������ ������ true, ������ false
    public virtual bool CheckConnectedRail()
    {
        Vector3[] dir = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.left, Vector3.back };
        for(int i = 0; i < dir.Length; i++)
        {
            if(Physics.Raycast(player.CurrentBlockTransform.position, dir[i], out RaycastHit hit, 1f, blockLayer))
            {
                if(hit.transform.childCount > 0)
                {
                    RailController rail = hit.transform.GetChild(0).GetComponent<RailController>();
                    if(rail != null && rail.isInstance)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
