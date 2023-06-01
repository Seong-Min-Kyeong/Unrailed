using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InGameScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _loadingSceneUI;

    [Header("Manager")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private ShopManager _shopManager;

    [SerializeField] private int worldCount = 0;

    List<Station> stations = new List<Station>();

    private bool _isInit = false;

    // ��ȯ�� �� isStart�� true�� ���� player�� �۵��� �� �ְ� ����~~
    public bool isStart { get; private set; } = false;

    private void Awake()
    {
        FileManager.LoadGame();

        // �ε�
        _loadingSceneUI.SetActive(true);
        isStart = false;

                Debug.Log(Time.realtimeSinceStartup);
        // �ε� ����
        LoadingFirstGame(
            () =>
            {
                _loadingSceneUI.SetActive(false);

                Debug.Log(Time.realtimeSinceStartup);
                RePositionAsync(() => { SettingStation(); }).Forget();
            }).Forget();

    }

/*    private void Update()
    {
        if(isStart && !_isInit)
        {
            _isInit = true;
            SettingStation();
        }
    }*/

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
        _shopManager.currentStation = stations[2].transform;

        // ���� ����ǰ�
        _shopManager.ShopOff();

        // �� ����ġ �����ֱ�
        RePositionAsync().Forget();
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

    private void SettingStation()
    {
        // ���� ������(x���� ���� ������ ������)
        stations = FindObjectsOfType<Station>().OrderBy(elem => elem.transform.position.x).ToList();
        for (int i = 0; i < stations.Count; i++)
        {
            if (i == 0)
                stations[i].InitStation(true);
            else
                stations[i].InitStation(false);
        }

        _shopManager.currentStation = stations[1].transform;
    }
}
