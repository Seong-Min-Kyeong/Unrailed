using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum EBlock { empty, grass, water, tree1, tree2, iron, blackRock, size }
public enum EBlock { empty, grass, water, tree1, tree2, iron,
    blackRock, station, fence,
    rail, treeItem, ironItem, axe,
    pick, bucket, bolt, size}

public class MapEditorMK2 : MonoBehaviour
{
    [Header("Blocks")]
    [SerializeField] private BlockMK2 _blockPrefab;
    [SerializeField] private Material[] _blocksMaterial; 

    [Header("Transform")]
    [SerializeField] private Transform _blockParent;
    [SerializeField] private Transform _environmentParent;

    [Header("Info")]
    [SerializeField] private const int _defaultX = 40;
    [SerializeField] private const int _defaultY = 20;

    [Header("ETC")]
    [SerializeField] private Transform _target;

    [HideInInspector] public int materialIndex;
    [HideInInspector] public bool isDraw = true;

    // ��� �����ϴϱ� ����Ʈ��?
    private List<List<BlockMK2>> groundList = new List<List<BlockMK2>>();

    private int _minX;
    private int _minY;
    private Camera _mainCam;
    private Vector3 _mainCamOriginPos;

    public void Awake()
    {
        isDraw = true;

        _mainCam = Camera.main;
        _mainCamOriginPos = _mainCam.transform.position;
        _target.gameObject.SetActive(true);

        InitMap();
    }

    // �� �ʱ�ȭ
    public void InitMap()
    {
        // ���� ������Ʈ ����
        _blockParent.DestroyAllChild();
        _environmentParent.DestroyAllChild();

        // ī�޶� ����ġ
        _mainCam.transform.position = _mainCamOriginPos;

        groundList = new List<List<BlockMK2>>();

        _minX = (_defaultX / 2) - 1;
        _minY = (_defaultY / 2) - 1;

        for (int i = 0; i < _defaultY; i++)
        {
            groundList.Add(new List<BlockMK2>());
            for (int j = 0; j < _defaultX; j++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);
                go.Init((int)EBlock.grass, _blocksMaterial[(int)EBlock.grass]);
                go.transform.localPosition = new Vector3(j - _minX, 0, i - _minY);
                groundList[i].Add(go);
            }
        }
    }


    // x�� ��ȭ
    public void ResizeXMap(int value)
    {
        if(value > 0)
        {
            int x = groundList[0].Count;
            for(int i = 0; i < groundList.Count; i++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);
                go.Init((int)EBlock.grass, _blocksMaterial[(int)EBlock.grass]);
                go.transform.localPosition = new Vector3(x - _minX, 0, i - _minY);
                groundList[i].Add(go);
            }
        }
        else if(value < 0)
        {
            for (int i = 0; i < groundList.Count; i++)
            {
                BlockMK2 go = groundList[i][groundList[i].Count - 1];
                groundList[i].RemoveAt(groundList[i].Count - 1);
                Destroy(go.gameObject);
            }
        }
        _mainCam.transform.position = new Vector3((groundList[0].Count - _defaultX) / 2, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

    // y�� ��ȭ
    public void ResizeYMap(int value)
    {
        if(value > 0)
        {
            int y = groundList.Count;
            groundList.Add(new List<BlockMK2>());
            for(int i = 0; i < groundList[0].Count; i++)
            {
                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);
                go.Init((int)EBlock.grass, _blocksMaterial[(int)EBlock.grass]);
                go.transform.localPosition = new Vector3(i - _minX, 0, y - _minY);
                groundList[groundList.Count - 1].Add(go);
            }
        }
        else if(value < 0)
        {
            for (int i = 0; i < groundList[0].Count; i++)
            {
                BlockMK2 go = groundList[groundList.Count - 1][i];
                Destroy(go.gameObject);
            }
            groundList.RemoveAt(groundList.Count - 1);
        }
        _mainCam.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, (groundList.Count - _defaultY) / 2 + 2);
    }

    private void Update()
    {
        if(isDraw)
        {
            RaycastHit hit;
            if (Physics.Raycast(_mainCam.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << LayerMask.NameToLayer("Block")))
            {
                int x = Mathf.RoundToInt(hit.point.x);
                int z = Mathf.RoundToInt(hit.point.z);

                _target.position = new Vector3(x, 0.51f, z);


                if (Input.GetMouseButton(0))
                {
                    // �ٸ� ���� �ٸ� ������ ĥ���ش�.
                    if (groundList[z + _minY][x + _minX].Index != materialIndex)
                    {
                        if(materialIndex >= _blocksMaterial.Length)
                            groundList[z + _minY][x + _minX].Init(materialIndex, null);
                        else
                            groundList[z + _minY][x + _minX].Init(materialIndex, _blocksMaterial[materialIndex]);
                    }
                }
            }
        }
    }

    // ���� �����ϱ�
    public void SaveMap(string mapName)
    {
        MyArr<int>[] mapData = new MyArr<int>[groundList.Count];
        for(int i = 0; i < groundList.Count; i++)
        {
            mapData[i] = new MyArr<int>(groundList[0].Count);
            for(int j = 0; j < groundList[0].Count; j++)
            {
                mapData[i].arr[j] = groundList[i][j].Index;
            }
        }

        FileManager.MapsData.Add(new MapData(mapName, 0, mapData));

        FileManager.SaveGame();
    }

    // �����
    public void SaveMap(int index)
    {
        MyArr<int>[] mapData = new MyArr<int>[groundList.Count];
        for (int i = 0; i < groundList.Count; i++)
        {
            mapData[i] = new MyArr<int>(groundList[0].Count);
            for (int j = 0; j < groundList[0].Count; j++)
            {
                mapData[i].arr[j] = groundList[i][j].Index;
            }
        }

        string mapName = FileManager.MapsData.mapsData[index].mapDataName;
        FileManager.MapsData.mapsData[index] = new MapData(mapName, 0, mapData);

        FileManager.SaveGame();
    }

    public void LoadMap(MapData mapData)
    {
        int x = mapData.mapData[0].arr.Length;
        int y = mapData.mapData.Length;

        // ���� ������Ʈ ����
        _blockParent.DestroyAllChild();
        _environmentParent.DestroyAllChild();

        groundList = new List<List<BlockMK2>>();

        _minX = (_defaultX / 2) - 1;
        _minY = (_defaultY / 2) - 1;

        for (int i = 0; i < y; i++)
        {
            groundList.Add(new List<BlockMK2>());
            for (int j = 0; j < x; j++)
            {
                int index = mapData.mapData[i].arr[j];

                BlockMK2 go = Instantiate(_blockPrefab, _blockParent);

                if (index >= _blocksMaterial.Length)
                    go.Init(index, null);
                else
                    go.Init(index, _blocksMaterial[index]);

                go.transform.localPosition = new Vector3(j - _minX, 0, i - _minY);
                groundList[i].Add(go);
            }
        }

        // ī�޶� ��ġ ����
        _mainCam.transform.position = new Vector3((groundList[0].Count - _defaultX) / 2, Camera.main.transform.position.y, (groundList.Count - _defaultY) / 2 + 2);
    }
}
