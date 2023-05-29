using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeChecker : MonoBehaviour
{
    public List<Collider> CheckedGameObjectList { get; private set; }

    private void Awake()
    {
        CheckedGameObjectList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CheckedGameObjectList.Contains(other))
        {
            CheckedGameObjectList.Add(other);
        }
    }
}
