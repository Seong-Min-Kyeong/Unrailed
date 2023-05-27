using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameScene : MonoBehaviour
{
    [SerializeField] private GameObject _loadingSceneUI;
    [SerializeField] private MapCreator _mapCreator;

    private void Awake()
    {
        FileManager.LoadGame();

        // �ε� �� �����ֱ�
        _loadingSceneUI.SetActive(true);

        // �����ٵ��� 
        // �÷��̾� ����
        // �� ����
        // ����, ����, ai ����
        // �� ���� ����� �ڸ���??
        // astar �� �ֽ�ȭ
        StartCoroutine(LodingCo());
        Debug.Log(Time.realtimeSinceStartup);

        // �� �۾� �� ������ �ε� �� Ǯ��
        // �ڸ� �� �������鼭 �� ����
    }

/*    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(LodingCo());
    }*/

    private IEnumerator LodingCo()
    {
        // �� ����
        // �÷��̾� ����
        // ����, ����, ai ����
        yield return StartCoroutine(_mapCreator.CreateMapCo(0));
        Debug.Log(Time.realtimeSinceStartup);

        _loadingSceneUI.SetActive(false);

    }
}
