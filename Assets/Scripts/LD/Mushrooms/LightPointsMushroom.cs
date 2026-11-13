using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightPointsMushroom : Mushroom
{

    [SerializeField] private List<GameObject> _prefabsList = new List<GameObject>();

    [SerializeField] private float _delayBetweenStates = 0.3f;
        public int _counter = 0;

    private bool _isChanging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            _isEnlightened = true;
        }
        if (_isEnlightened && _counter < _prefabsList.Count && _isChanging == false)
        {
            StartCoroutine(LightenUp());
        }
        else if(_counter > 0 && _isEnlightened == false && _isChanging == false && SceneManager.GetActiveScene().name != "MainMenu")
        {
            StartCoroutine(LightenDown());
        }

    }


    private IEnumerator LightenUp()
    {
        _isChanging = true;
        
        if (_counter < _prefabsList.Count-1)
        {
            _prefabsList[_counter].gameObject.SetActive(false);
            _prefabsList[_counter+1].gameObject.SetActive(true);
            _counter++;
            yield return new WaitForSeconds(_delayBetweenStates);
            StartCoroutine(LightenUp());
        }
        else
        {
            _prefabsList[_prefabsList.Count - 1].gameObject.SetActive(true);
            _prefabsList[_prefabsList.Count - 2].gameObject.SetActive(false);
            _isChanging = false;
        }
    }

    private IEnumerator LightenDown()
    {
        _isChanging = true;
        
        if (_counter > 0)
        {
            
            _prefabsList[_counter].gameObject.SetActive(false);
            _prefabsList[_counter -1 ].gameObject.SetActive(true);
            _counter--;
            yield return new WaitForSeconds(_delayBetweenStates);
            StartCoroutine(LightenDown());
        }
        else
        {
            _prefabsList[0].gameObject.SetActive(true);
            _prefabsList[1].gameObject.SetActive(false);
            _isChanging = false;
        }
    }



}
