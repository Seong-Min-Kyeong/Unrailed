using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : MonoBehaviour
{
    // ���� �� ���ϰž�

    public bool isEmpty = false; // ����̸� isEmpty true�� ����!

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    /*public void Show()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = true;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }

    public void UnShow()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = false;

        // ���� �ڽĵ��� �� �Ⱥ��̰� �Ѵ�. �Ⱥ��̰� �ϴ� ����� �ڽĵ��� ù��° �ڽ��� MeshObject�� setActive.false ���ش�.
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
    }*/
}
