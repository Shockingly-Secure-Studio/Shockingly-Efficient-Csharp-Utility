using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Scan;
using UnityEngine;
using UnityEngine.UI;

public class SetFiche : MonoBehaviour
{
    public GameObject fiche;
    private GameObject _input;
    private GameObject _alertBox;
    private Button _button;
    private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _button.onClick.AddListener(Save);
    }

    private void Awake()
    {
        _button=GameObject.Find("Save").GetComponent<Button>();
        _text = GameObject.Find("TextInput").GetComponent<Text>();
        _input=GameObject.Find("Input");
        _input.SetActive(false);
        _alertBox=GameObject.Find("AlertBox");
        _alertBox.SetActive(false);

    }

    public void SetActiveFiche(bool activate){
        fiche.SetActive(activate);
    }
    
    private void Save()
    {
        string text;
        text = _text.text; 
        if (!Directory.Exists(text))
            _alertBox.SetActive(true);
        else
        {
            gameObject.AddComponent<rapport>().NewDocument(text);
            _input.SetActive(false);
        }
    }
    public void AddRapport()
    {
        _input.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
