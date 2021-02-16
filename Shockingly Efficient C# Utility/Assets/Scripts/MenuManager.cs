using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    

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
    public void Spawn_( bool spawn)
    {
        menu.SetActive(spawn);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
