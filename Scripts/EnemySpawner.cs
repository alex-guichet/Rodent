using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector3 _cubeCheckerSize;
    [SerializeField] private Vector3 _cubeEraserSize;
    [SerializeField] private SpawnPoints[] _enemySpawnPoints;
    [SerializeField] private LayerMask _checkerLayerMask;
    [SerializeField] private LayerMask _eraserLayerMask;
    [SerializeField] private GameObject _cat;

    public static EnemySpawner _instance;
    private int _lastSpawnPointIndex = 0;

    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("More than one EnemySpawner instance !");
            return;
        }
        _instance = this;
    }

    private bool CubeChecker()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, _cubeCheckerSize / 2, transform.rotation, _checkerLayerMask);

        if (colliders.Length > 0)
        {
            return false;
        }

        CubeEraser();
        return true;
    }

    private void CubeEraser()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, _cubeEraserSize / 2, transform.rotation, _eraserLayerMask);

        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                c.gameObject.SetActive(false);
            }
        }

        Instantiate(_cat, transform.position, _cat.transform.rotation);
    }

    public void SpawnEnemy(int number)
    {
        StartCoroutine(MoveSpawnerMultipleTimes(number));
    }

    IEnumerator MoveSpawner()
    {
        int randomIndex = Random.Range(0, _enemySpawnPoints.Length);

        while(randomIndex == _lastSpawnPointIndex)
        {
            randomIndex = Random.Range(0, _enemySpawnPoints.Length);
        }

        _lastSpawnPointIndex = randomIndex;
        Vector3 EndPos = _enemySpawnPoints[randomIndex]._endPoint;
        transform.position = _enemySpawnPoints[randomIndex]._startPoint;

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
        Gizmos.DrawWireCube(transform.position, _cubeCheckerSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _cubeEraserSize);
    }
    [System.Serializable]
    public struct SpawnPoints
    {
        public Vector3 _startPoint;
        public Vector3 _endPoint;
    }
}
