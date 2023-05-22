using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public List<RailController> railCon = new List<RailController>();

    public void TrainGoal()
    {
        Debug.LogError(gameObject.name);
        foreach (RailController rail in railCon)
        {
            rail.isGoal = true;
            rail.isInstance = true;

            for (int i = 0; i < rail.trainComponents.Length; i++)
            {
               rail.trainComponents[i].isGoal = true;
            }
        }
        railCon[railCon.Count - 2].ResetLine();
    }
}
