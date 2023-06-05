using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public TrainType itemType;
    public int itemCost;
    public int itemIdx;
    public bool _isSpawn = true;
    BoxCollider box;

    private Transform _originParent;
    private Vector3 _originPos;

    void Awake()
    {
        _originParent = transform.parent;
        _originPos = transform.localPosition;

        TryGetComponent(out box);
        if (itemType == TrainType.StationDir || itemType == TrainType.Dynamite)
            ShopManager.Instance.newCarList.Add(this);

    }
    private void OnEnable()
    {
        _isSpawn = true;
        box.enabled = true;
    }


    public void TradeItem(Collider col)
    {
        ShopManager.Instance.trainCoin--;

        TrainMovement train = col.GetComponent<TrainMovement>();

        if (train.trainType != TrainType.Spare)
        {
            train.trainUpgradeLevel++;

            if (train.trainType == itemType)
            {
                train.TrainUpgrade();
                box.enabled = false;
            }
        }

        else
        {
            TrainSpare spare = col.GetComponent<TrainSpare>();
            spare.ChangeTrain(itemIdx);
            box.enabled = false;
        }

        ShopManager.Instance.TrainCost();
        _isSpawn = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // ������ ��ġ�� �ǵ��ư���
    public void SetInitPosition()
    {
        // ��ġ �����ϰ�
        transform.SetParent(_originParent);
        transform.localPosition = _originPos;
        transform.localRotation = Quaternion.identity;

        // ���� ��������
        ShopManager.Instance.trainCoin += itemCost;
    }

    public void SetPosition(Transform parent)
    {
        // ��ġ �����ϰ�
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // ���� ��������
        ShopManager.Instance.trainCoin -= itemCost;
    }

    public bool TryBuyItem()
    {
        return ShopManager.Instance.trainCoin >= itemCost;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train"))
        {
            if (_isSpawn && (itemType == TrainType.StationDir || itemType == TrainType.Dynamite))
            {
                TrainSpare trains = other.GetComponent<TrainSpare>();

                if (trains != null && trains.trainIndex == 0)
                {
                    trains.ResetTrain(1);
                }
                else if (trains != null && trains.trainIndex == 1)
                {
                    trains.ResetTrain(0);
                }
            }

            TradeItem(other);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Train"))
        {
            if (_isSpawn && (itemType == TrainType.StationDir || itemType == TrainType.Dynamite))
            {
                TrainSpare trains = other.GetComponent<TrainSpare>();

                if (trains != null && trains.trainIndex == 0)
                {
                    trains.ResetTrain(1);
                }
                else if (trains != null && trains.trainIndex == 1)
                {
                    trains.ResetTrain(0);
                }
            }

            TradeItem(other);
        }

        else if (other.CompareTag("ShopItem"))
        {
            ShopItem items = other.GetComponent<ShopItem>();

            if (items._isSpawn)
            {
                transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }*/
}
