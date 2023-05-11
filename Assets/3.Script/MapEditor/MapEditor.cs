using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapEditor : MonoBehaviour
{
    [Header("Blocks")]
    [SerializeField] private Material[] blockMaterials;
    [SerializeField] private GameObject _baseBlockPrefab;
    [SerializeField] private LineGrid _lineGrid;
    [SerializeField] private Block _blockPrefab;

    [Header("Level")]
    [SerializeField] private Transform _level;

    [Header("UI")]
    [SerializeField] private GameObject _mapEditorUI;


    [HideInInspector] public Block selectedBlock = null;
    [HideInInspector] public Block prevBlock = null;
    public int[,] mapData;

    private int _currentBlockIndex = 0;
    private int _minX, _minY;
    private Camera _mainCam;
    private MapEditStateFactory _factory;

    public int MinX => _minX;
    public int MinY => _minY;
    public int CurrentBlockIndex => _currentBlockIndex;
    public Camera MainCam => _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
        _factory = new MapEditStateFactory(this);
    }

    private void Update()
    {
        if(_factory.CurrentState != null)
            _factory.CurrentState.Update();
    }

    // ���� ��� ��ȯ
    public void ChangeEditMode(EMapEditState editState)
    {
        _factory.ChangeState(editState);
    }

    // �� ���� ���� ��ư�� �־���
    public void StartMapEdit()
    {
        _mapEditorUI.SetActive(false);
        InitLevel();
    }

    // ���� �ʱ�ȭ (�ȿ� �ִ� ������Ʈ �� ����)
    private void InitLevel()
    {
        _level.DestroyAllChild();
    }

    // �Ƕ��� ����
    public void MakeBaseMap(int x, int z)
    {
        _mapEditorUI.SetActive(true);

        GameObject baseBlock = Instantiate(_baseBlockPrefab, _level);
        baseBlock.transform.localScale = new Vector3(x, 1, z);
        LineGrid lineGrid = Instantiate(_lineGrid, baseBlock.transform);
        lineGrid.transform.localPosition = new Vector3(0, 0.51f, 0);
        lineGrid.MakeGrid(-(x / 2 - 0.5f), -(z / 2 - 0.5f), x, z);

        mapData = new int[z, x];

        _minX = (x / 2) - 1;
        _minY = (z / 2) - 1;

        ChangeEditMode(EMapEditState.Draw);
    }

    #region �� ���¿� �־��� �Լ���

    // �� ����
    public void SelectBlock(int index)
    {
        if (selectedBlock != null)
            Destroy(selectedBlock.gameObject);
        selectedBlock = Instantiate(_blockPrefab, _level);
        selectedBlock.InitBlock(blockMaterials[index]);
    }

    public void DestroyBlock()
    {
        if (selectedBlock != null)
            Destroy(selectedBlock.gameObject);

        selectedBlock = null;
    }

    public void InputBlock()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            _currentBlockIndex = 0;
            SelectBlock(_currentBlockIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _currentBlockIndex = 1;
            SelectBlock(_currentBlockIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            _currentBlockIndex = 2;
            SelectBlock(_currentBlockIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            _currentBlockIndex = 3;
            SelectBlock(_currentBlockIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            _currentBlockIndex = 4;
            SelectBlock(_currentBlockIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            _currentBlockIndex = 5;
            SelectBlock(_currentBlockIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            _currentBlockIndex = 6;
            SelectBlock(_currentBlockIndex);
        }
    }

    #endregion
}
