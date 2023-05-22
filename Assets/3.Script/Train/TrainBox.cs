using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainBox : TrainMovement
{
    public List<Item> woods = new List<Item>();
    public List<Item> steels = new List<Item>();

    [SerializeField] private TrainWorkBench workBench;
    public Player player;

    public int maxItem;
    // Start is called before the first frame update
    void Awake()
    {
        GetMesh();
        workBench = FindObjectOfType<TrainWorkBench>();
        fireEffect.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        TrainMovePos();

        if (!isBurn)
        {
            GiveMeItem();
            
        }
    }

    private void RelayItems()
    {
        woods.Remove(woods[0]);
        steels.Remove(steels[0]);
        workBench.MakingRail();
    }
    public void GiveMeItem()
    {
        //������ ������ �÷��̾� �� ��������� ����
        //�ִ� ������ 3��
        //����3�� ö 3��
        //�ִ� ������ �����ѹ��� ���� �ִ밪 ������ ����
        //����� ö 1���� ������ ��� if������ RelayItems ����
        //RelayItems();
    }
}
