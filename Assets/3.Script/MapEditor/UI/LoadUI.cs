using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadUI : MonoBehaviour
{
    [SerializeField] private Button _loadButtonPrefab;
    [SerializeField] private RectTransform _content;

    [SerializeField] private MapEditor _mapEditor;

    // Ȱ��ȭ �� �� �ҷ����� ��ư ��������
    private void OnEnable()
    {
        // �����͸� �ε��ؿͼ� �����ֱ�
        for(int i = 0; i < FileManager.MapsData.mapsData.Count; i++)
        {
            int index = i;
            Button loadButton = Instantiate(_loadButtonPrefab, _content);
            loadButton.onClick.AddListener(() => _mapEditor.LoadMap(index));
        }


        _content.sizeDelta = new Vector3(_content.sizeDelta.x, (_content.transform.childCount / 2) * 310);
    }
}
