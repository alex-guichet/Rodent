using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeChecker : MonoBehaviour
{
    public List<Collider> _checkedGameObjectList;

    private void Awake()
    {
        _checkedGameObjectList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_checkedGameObjectList.Contains(other))
        {
            _checkedGameObjectList.Add(other);
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (_checkedGameObjectList.Contains(other))
        {
            _checkedGameObjectList.Remove(other);
        }
    }
    */

}
