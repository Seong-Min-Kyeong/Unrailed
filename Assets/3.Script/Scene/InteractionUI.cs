using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionUI : MonoBehaviour
{
    public void GameStart()
    {
        //goto �Ŀ� �ٲ� ��
        SceneManager.LoadScene("TrainScene");
    }
    public void GameExit()
    {
        //goto �Ŀ� �ٲ� ��
        Application.Quit();
    }

}
