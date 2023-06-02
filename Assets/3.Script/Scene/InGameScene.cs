using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InGameScene : MonoBehaviour
{
    [Header("�׽�Ʈ")]
    [SerializeField] private bool isTest = false;

    [Header("UI")]
    [SerializeField] private GameObject _loadingSceneUI;

    [Header("Manager")]
    [SerializeField] private WorldManager _worldManager;

    [SerializeField] private ShopManager _shopManager;

    [SerializeField] private int worldCount = 0;

    // ��ȯ�� �� isStart�� true�� ���� player�� �۵��� �� �ְ� ����~~
    public bool isStart { get; private set; } = false;

    private void Awake()
    {
        // ���� �ε�
        FileManager.LoadGame();

        // �ε�
        _loadingSceneUI.SetActive(true);
        isStart = false;

        // �ε� ����
        LoadingFirstGame(isTest, 
            () =>
            {
                _loadingSceneUI.SetActive(false);
                RePositionAsync().Forget();

                _shopManager.StartTrainMove();
            }).Forget();
    }


    /// <summary>
    /// �� �����ϸ� ����� �޼ҵ�
    /// </summary>
    public void ArriveStation()
    {
        // ��Ʈ �ϳ� �߰� ���ְ�

        // �����ִٰ� ���������ֱ�
        _shopManager.ShopOn();
    }

    /// <summary>
    /// �� ������ ����� �޼ҵ�
    /// </summary>
    public void LeaveStation()
    {
        // ������ ������ 100�Ǹ� ����� �޼ҵ�

        // ���ο� �� ����
        _shopManager.currentStation = _worldManager.stations[2].transform;

        // ���� ����ǰ�
        _shopManager.ShopOff();

        // �� ����ġ �����ֱ�
        RePositionAsync().Forget();
    }


    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _worldManager.RePositionAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(bool isTest, System.Action onCreateNextMapAsyncEvent = null)
    {
        // �� ����
        await _worldManager.GenerateWorld(isTest);
        onCreateNextMapAsyncEvent?.Invoke();
    }
}
