using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileElem : MonoBehaviour
{
    [Header("Color")]
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color grayColor;

    [Header("Image")]
    [SerializeField] private Image fileName;
    [SerializeField] private Image fileOpen;
    [SerializeField] private Image fileDelete;

    [Header("Text")]
    [SerializeField] private Text fileNameText;

    [Header("Button")]
    [SerializeField] private Button fileOpenButton;
    [SerializeField] private Button fileDeleteButton;

    private MapEditorMK2 mapEditor;
    private OpenUI openUI;

    private void Awake()
    {
        mapEditor = FindObjectOfType<MapEditorMK2>();
        openUI = FindObjectOfType<OpenUI>();
    }

    public void Init(int index)
    {
        MapData mapData = FileManager.MapsData.mapsData[index];

        if(index % 2 == 0)  // 흰색
        {
            fileName.color = whiteColor;
            fileOpen.color = whiteColor;
            fileDelete.color = whiteColor;
        }
        else  // 검은색
        {
            fileName.color = grayColor;
            fileOpen.color = grayColor;
            fileDelete.color = grayColor;
        }

        // 저장 이름
        fileNameText.text = mapData.mapDataName;

        // 버튼 onClick 할당
        fileOpenButton.onClick.AddListener(() => mapEditor.LoadMap(mapData));
        fileDeleteButton.onClick.AddListener(() => FileManager.MapsData.mapsData.RemoveAt(index));
        fileDeleteButton.onClick.AddListener(() => FileManager.SaveGame());
        fileDeleteButton.onClick.AddListener(() => openUI.gameObject.SetActive(false));
        fileDeleteButton.onClick.AddListener(() => openUI.gameObject.SetActive(true));
    }
}
