using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSource : MonoBehaviour, IDig
{
    [SerializeField] int ResourceHp = 3;
    [SerializeField] float Delay = 0.5f;
    [SerializeField] Transform ItemPrefab;
    private float ResourceScale = 1f;
    private float CurrentTime = 0f;
    private bool isDig = false;// ó���� false���� ���� : Player�� ������ �ٲ���� �Ǵ� �ɱ�?

  
    public void OnDig(Vector3 hitposition)
    {
        if (!isDig)
        {
            StartCoroutine(OnDig_co());
        }
    }

    private void DisableAllChildColliders(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if (childCollider != null)
            {
                childCollider.enabled = false;
            }

            // �ڽ� ��ü�� �ڽ� ��ü�鵵 ��������� Ž��
            DisableAllChildColliders(child);
        }
    }

    private void SpawnItem()
    {
        // 1. ������ �����ؼ� ������ �����Ѵ�
        Transform NewItem = Instantiate(ItemPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NewItem.SetParent(transform.parent);
        NewItem.localPosition = (new Vector3(0, 0.5f, 0));
        NewItem.localRotation = Quaternion.identity;

        DisableAllChildColliders(NewItem);
    }

    private IEnumerator OnDig_co()
    {
        isDig = true;

        yield return new WaitForSeconds(Delay);

        if (ResourceHp > 0)
        {
            ResourceHp--;
            ResourceScale -= 0.25f;
            transform.localScale = new Vector3(ResourceScale, ResourceScale, ResourceScale);
        }
        if (ResourceHp == 0)
        {
            SpawnItem();
            Destroy(gameObject, 0.5f);
        }
        isDig = false;

        /*CurrentTime = 0;
        while (true)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= Delay)
            {
                isDig = false;
                break;
            }

        }*/

    }
}