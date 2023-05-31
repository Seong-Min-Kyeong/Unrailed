using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingSceneUI;
    [SerializeField] private MapCreator _mapCreator;

    [SerializeField] private MapManager _mapManager;

    [SerializeField] private int worldCount = 0;

    [SerializeField] private Transform test;

    // ��ȯ�� �� isStart�� true�� ���� player�� �۵��� �� �ְ� ����~~
    public bool isStart { get; private set; } = false;

    private void Awake()
    {
        FileManager.LoadGame();
        _loadingSceneUI.SetActive(true);
        isStart = false;

        LoadingFirstGame(
            () =>
            {
                _loadingSceneUI.SetActive(false);

                RePositionAsync().Forget();
            }).Forget();
    }

/*    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RePositionAsync().Forget();
    }*/

    /// <summary>
    /// �� �����ϸ� ����� �޼ҵ�
    /// </summary>
    public void ArriveStation()
    {
        // ��Ʈ �ϳ� �߰� ���ְ�
        // �����ִٰ� ���������ֱ�
    }

    /// <summary>
    /// �� ������ ����� �޼ҵ�
    /// </summary>
    public void LeaveStation()
    {
        // ������ ������ 100�Ǹ� ����� �޼ҵ�

        // ���� ����ǰ�
        // �� ����ġ �����ֱ�
    }


    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _mapManager.RePositionAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(System.Action onCreateNextMapAsyncEvent = null)
    {
        // �� �ε�
        await _mapManager.LoadMap();
        onCreateNextMapAsyncEvent?.Invoke();
    }
}
