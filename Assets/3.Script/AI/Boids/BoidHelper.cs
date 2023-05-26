using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidHelper
{
    const int numViewDirections = 100;
    public static readonly Vector3[] Directions;

    static BoidHelper()
    {
        //X, Z�� �������� ��ֹ��� ������ �ʴ� ��� Ž���� �����ִ�
        //RayCast�� ��� ���� Directions
        Directions = new Vector3[BoidHelper.numViewDirections];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        //1.6
        float angleIncrement = Mathf.PI * 2 * goldenRatio;
        
        for (int i = 0; i < numViewDirections; i++)
        {

   

            float t = (float)i / numViewDirections;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = 0.5f;
            //float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            Directions[i] = new Vector3(x, y, z);
        }
    }
}
