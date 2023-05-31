using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // ������ ���� �����ϴ� ��ũ��Ʈ
    [SerializeField] private MapCreator _mapCreator;
    [SerializeField] private MapSlicer _mapSlicer;

    public List<List<BlockMK2>>[] entireMap { get; private set; } = new List<List<BlockMK2>>[2];
    public List<BlockBundle>[] entireMapBlockBundle { get; private set; } = new List<BlockBundle>[2];
    
    // �ΰ��� �����ϸ� ������ �޼ҵ�
    public async UniTask LoadMap()
    {
        MapData[] mapData = new MapData[2];

        mapData[0] = FileManager.MapsData.mapsData[5];
        mapData[1] = FileManager.MapsData.mapsData[7];

        // �� ����
        for(int i = 0; i < mapData.Length; i++)
        {
            // ��ġ, �θ� ����
            float width = mapData[i].mapData[0].arr.Length;
            Vector3 parentPosition = Vector3.right * width * i;
            Transform currentParent = new GameObject("World " + i).transform;
            currentParent.position = parentPosition;

            entireMap[i] = await _mapCreator.CreateMapAsync(mapData[i], currentParent);
        }
        
        // �� �ڸ���
        for(int i = 0; i < mapData.Length; i++)
        {
            entireMapBlockBundle[i] = await _mapSlicer.SliceMap(entireMap[i]);
        }

        // ������ ��ٸ���
        await UniTask.WaitForEndOfFrame(this);
    }

    // index��° �� ����ġ
    public async UniTask RePositionAsync(int index)
    {
        await _mapCreator.RePositionAsync(entireMapBlockBundle[index]);
    }
}
