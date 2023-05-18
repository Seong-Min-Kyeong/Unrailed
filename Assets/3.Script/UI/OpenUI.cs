using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUI : MonoBehaviour
{
    [SerializeField] private FileElem fileElemPrefab;
    [SerializeField] private RectTransform content;

    private MapEditorMK2 mapEditor;

    private void Awake()
    {
        mapEditor = FindObjectOfType<MapEditorMK2>();
    }

    private void OnEnable()
    {
        mapEditor.isDraw = false;

        OpenFileData();
    }

    private void OnDisable()
    {
        mapEditor.isDraw = true;
    }

    public void OpenFileData()
    {
        // ���̺����ϸ�ŭ fileElem ������ֱ�
        // ���� �� ��ŭ content ���� �÷��ֱ�
        // fileElem ����鼭 index �Ѱ��ֱ�

        content.DestroyChildren(1);

        int count = FileManager.MapsData.mapsData.Count;
        content.sizeDelta = new Vector2(content.sizeDelta.x, 72 * (count + 1));
        for(int i = 0; i < count; i++)
        {
            FileElem fileElem = Instantiate(fileElemPrefab, content);
            fileElem.Init(i);
        }
    }
}
