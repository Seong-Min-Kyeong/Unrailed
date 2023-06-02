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

    public void Show()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = true;
    }

    public void UnShow()
    {
        if (isEmpty)
            return;

        _meshRenderer.enabled = false;
    }
}
