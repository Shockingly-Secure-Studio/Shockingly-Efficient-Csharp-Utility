using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFiche : MonoBehaviour
{
    public GameObject fiche;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetActiveFiche(bool activate){
        fiche.SetActive(activate);
    }
    public void AddRapport(){
        gameObject.AddComponent<rapport>().NewDocument();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
