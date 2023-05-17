using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : DonDestroySingleton<MapCreator>
{
    private enum EEnvironment
    {
        grass = 1,
        field,
        rock,
        breakableRock,
        tree1,
        tree2,
        water,
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Block _blockPrefab;

    [Header("Materials")]
    [SerializeField] private Material[] blockMaterials;

    [Header("�� ����")]
    [SerializeField] private float waterHeight = 0.8f;

    public void CreateMap(Block[,] mapData, Transform parent, bool isClear = true)
    {
        if (isClear)
            transform.DestroyAllChild();

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                if (mapData[i, j] == null)
                    continue;

                int index = mapData[i, j].Index;
                if (mapData[i, j].Index != 0)
                {
                    if (prefabs[index] != null)
                    {
                        GameObject go = Instantiate(prefabs[index], parent);
                        if (index == (int)EEnvironment.grass)
                            go.transform.position = mapData[i, j].transform.position + Vector3.up * 0.5f;
                        else
                            go.transform.position = mapData[i, j].transform.position + Vector3.up;
                        mapData[i, j].transform.localScale = Vector3.one;
                    }
                    // ��
                    if (index == (int)EEnvironment.water)
                    {
                        mapData[i, j].transform.localScale = Vector3.one - Vector3.up * (1 - waterHeight);
                        mapData[i, j].transform.localPosition -= Vector3.up * ((1 - waterHeight) / 2);
                    }
                }
            }
        }
    }
    public void CreateRandomMap()
    {
        MapData mapData = FileManager.MapsData.mapsData[Random.Range(0, FileManager.MapsData.mapsData.Count)];
        CreateMap(mapData);
    }


    // �̰� ���� ������ �� �� �����ϴ� �޼ҵ�
    private void CreateMap(MapData mapData)
    {
        // �� �ʱ�ȭ
        transform.DestroyAllChild();

        // ��ǥ, �迭 �ʱ�ȭ
        int x = mapData.mapData[0].arr.Length;
        int y = mapData.mapData.Length;
        Block[,] mapArr = new Block[y, x];
        int minX = (x / 2) - 1;
        int minY = (y / 2) - 1;

        // ������ ����ŭ �� ����, MapData ���� �־��ְ� ���� �ʱ�ȭ
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int index = mapData.mapData[j].arr[i];
                if (index != 0)
                {
                    Block block = Instantiate(_blockPrefab, transform);
                    block.transform.localPosition = new Vector3(i - minX, 0, j - minY);
                    block.SetPos(i, j);
                    mapArr[j, i] = block;
                    block.SetBlockInfo(blockMaterials[index], index);
                }
            }
        }

        // �� ������ ����
        CreateMap(mapArr, transform, false);
    }
}
