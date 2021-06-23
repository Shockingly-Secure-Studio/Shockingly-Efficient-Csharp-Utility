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
    void OnDisable()
    {
        _button.onClick.RemoveListener(Save);
    }

    private void Awake()
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            switch (go.name)
            {
                case "Save":
                    _button = go.GetComponent<Button>();
                    break;
                case "TextInput":
                    _text = go.GetComponent<Text>();
                    break;
                case "Input":
                    _input = go;
                    _input.SetActive(false);
                    break;
                case "AlertBox":
                    _alertBox = go;
                    _alertBox.SetActive(false);
                    break;
            }
        }

            /*
        _button=GameObject.Find("Save").GetComponent<Button>();
        _text = GameObject.Find("TextInput").GetComponent<Text>();
        _input=GameObject.Find("Input");
        _input.SetActive(false);
        _alertBox=GameObject.Find("AlertBox");
        _alertBox.SetActive(false);
        */

    }

    public void SetActiveFiche(bool activate){
        fiche.SetActive(activate);
    }

    private void Save()
    {
        string text;
        if (fiche!=null && fiche.activeInHierarchy && fiche.activeSelf)
        {
            text = _text.text; 
            if (!Directory.Exists(text))
                _alertBox.SetActive(true);
            else
            {
                gameObject.AddComponent<rapport>().NewDocument(text);
                _input.SetActive(false);
            }
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
