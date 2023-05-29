using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

public class CubeMover : MonoBehaviour
{

    [SerializeField] private Transform cubeFolder;
    [SerializeField] private Transform graphics;
    [SerializeField] private Transform cube;

    [SerializeField] private float sphereRadius = 1f;
    [SerializeField] private float distanceSphere = 1f;

    [SerializeField] private Vector3 boxCastSize;

    [SerializeField] private LayerMask cubeLayerMask;

    private const string _cheeseTag = "Cheese";
    private const string _cubeTag = "Cube";

    private BoxCollider _cubeCollider;
    private bool _sphereCastDisabled;
    private Vector3 _directionMove;

    private List<Transform> _cubeList;

    private Coroutine _moveCoroutine;
    private Coroutine _checkNearestCube;
    
    private void Awake()
    {
        _cubeList = new List<Transform>();
    }

    private void FixedUpdate()
    {
        if (_sphereCastDisabled)
            return;

        if (!Physics.SphereCast(graphics.position, sphereRadius, graphics.forward, out RaycastHit hit, distanceSphere, cubeLayerMask))
            return;

        _sphereCastDisabled = true;

        Vector3 directionContact = -hit.normal;

        _directionMove = new Vector3(Mathf.Round(directionContact.x), 0f, Mathf.Round(directionContact.z));

        if (Mathf.Abs(_directionMove.x) == 1 && Mathf.Abs(_directionMove.z) == 1)
        {
            if (Mathf.Abs(_directionMove.x) >= Mathf.Abs(_directionMove.z))
            {
                _directionMove = new Vector3(Mathf.Round(directionContact.x), 0f, 0f);
            }
            else
            {
                _directionMove = new Vector3(0f, 0f, Mathf.Round(directionContact.z));
            }
        }
        cube = hit.transform;
        _cubeList.Add(hit.transform);

        if (_checkNearestCube != null)
        {
            StopCoroutine(_checkNearestCube);
        }
        _checkNearestCube = StartCoroutine(CheckNearestCube(hit.transform.position));

    }

    IEnumerator CheckNearestCube(Vector3 pos)
    {
        if (Physics.BoxCast(pos, boxCastSize / 2f, _directionMove, out RaycastHit hit, Quaternion.identity, 1f))
        {
            if (hit.collider.CompareTag(_cheeseTag))
            {
                hit.collider.gameObject.SetActive(false);
                StartCoroutine(CheckNearestCube(pos));
                yield break;
            }

            if (hit.collider.CompareTag(_cubeTag))
            {
                var collider_transform = hit.collider.transform;
                cube = collider_transform;
                _cubeList.Add(collider_transform);
                StartCoroutine(CheckNearestCube(hit.collider.transform.position));
                yield break;
            }

            _sphereCastDisabled = false;
            _cubeList.Clear();
            yield break;
        }

        if (_cubeList.Count > 1)
        {
            for(int i  = 1; i < _cubeList.Count; i++)
            {
                _cubeList[i].transform.parent = _cubeList.First();
            }
        }
        SoundManager.Instance.PlayAudio(AudioName.RockSlide);
        StartCoroutine(CubeMove(_cubeList.First(), _directionMove));
        yield return null;
    }


    IEnumerator CubeMove(Transform cubeTransform, Vector3 direction)
    {
        Vector3 endPos = cubeTransform.position + direction;
        while (Vector3.Distance(cubeTransform.position, endPos) > 0.01f)
        {
            cubeTransform.position = Vector3.Lerp(cubeTransform.position, endPos, 0.25f);
            yield return new WaitForSeconds(0.01f);
        }
        cubeTransform.position = endPos;

        if (_cubeList.Count > 1)
        {
            for (int i = 1; i < _cubeList.Count; i++)
            {
                _cubeList[i].transform.parent = cubeFolder;
            }
        }
        _cubeList.Clear();
        _sphereCastDisabled = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * distanceSphere, sphereRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(cube.position + _directionMove, boxCastSize);
    }
}
