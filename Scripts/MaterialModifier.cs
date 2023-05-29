using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialModifier : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    void Awake()
    {
        ChangeGameObject();
    }


    private void ChangeGameObject()
    {
        Instantiate(_gameObjects[Random.Range(0, _gameObjects.Length)], transform.position, transform.rotation, transform);
        GetComponent<Renderer>().enabled = false;
    }

}
