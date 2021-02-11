using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void ExitAPP()
    {
        Debug.Log("Exit App");
        Application.Quit();
    }
    public void ChangeScene_(string sceanename)
    {
        SceneManager.LoadScene(sceanename);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
