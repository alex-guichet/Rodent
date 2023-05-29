using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 cubeCheckerSize;
    [SerializeField] private Vector3 cubeEraserSize;
    [SerializeField] private SpawnPoints[] enemySpawnPoints;
    [SerializeField] private LayerMask checkerLayerMask;
    [SerializeField] private LayerMask eraserLayerMask;
    [SerializeField] private GameObject cat;

    public static EnemySpawner Instance;
    
    private int _lastSpawnPointIndex = 0;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one EnemySpawner instance !");
            return;
        }
        Instance = this;
    }

    private bool CubeChecker()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, cubeCheckerSize / 2, transform.rotation, checkerLayerMask);

        if (colliders.Length > 0)
        {
            return false;
        }

        CubeEraser();
        return true;
    }

    private void CubeEraser()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, cubeEraserSize / 2, transform.rotation, eraserLayerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                c.gameObject.SetActive(false);
            }
        }

        Instantiate(cat, transform.position, cat.transform.rotation);
    }

    public void SpawnEnemy(int number)
    {
        StartCoroutine(MoveSpawnerMultipleTimes(number));
    }

    IEnumerator MoveSpawner()
    {
        int randomIndex = Random.Range(0, enemySpawnPoints.Length);

        while(randomIndex == _lastSpawnPointIndex)
        {
            randomIndex = Random.Range(0, enemySpawnPoints.Length);
        }

        _lastSpawnPointIndex = randomIndex;
        Vector3 EndPos = enemySpawnPoints[randomIndex]._endPoint;
        transform.position = enemySpawnPoints[randomIndex]._startPoint;

        float distance = Vector3.Distance(transform.position, EndPos);

        while (distance > 0.5f)
        {
            if (CubeChecker())
                yield break;

            transform.position = Vector3.Lerp(transform.position, EndPos, 3f/distance);
            distance = Vector3.Distance(transform.position, EndPos);
        }

        StartCoroutine(MoveSpawner());
    }

    IEnumerator MoveSpawnerMultipleTimes(int number)
    {
        int currentNumber = 0;
        while (currentNumber < number)
        {
            StartCoroutine(MoveSpawner());
            currentNumber++;
            yield return new WaitForEndOfFrame();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var position = transform.position;
        Gizmos.DrawWireCube(position, cubeCheckerSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(position, cubeEraserSize);
    }
    [System.Serializable]
    public struct SpawnPoints
    {
        public Vector3 _startPoint;
        public Vector3 _endPoint;
    }
}
