using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject[] panels;

    public void SwitchPanel(int index)
    {
        panels[index].transform.SetAsLastSibling();
    }

    public void LoadSave(int index)
    {
        Debug.Log($" click index {index} ");
    }

    public void ExitGame()
    {
        GameObject go = new ();
        Application.Quit();
        Debug.Log("退出");
    }
}
