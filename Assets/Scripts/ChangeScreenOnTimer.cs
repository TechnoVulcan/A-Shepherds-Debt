using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScreenOnTimer : MonoBehaviour
{
    public float ChangeTime;
    public string SceneName;
    void Update()
    {
        ChangeTime-= Time.deltaTime;
        if(ChangeTime<=0)SceneManager.LoadScene(SceneName);
    }
}
