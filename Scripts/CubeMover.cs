using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeMover : MonoBehaviour
{
    private List<Transform> _cubeList;

    private Coroutine _moveCoroutine;
    private Coroutine _checkNearestCube;

    [SerializeField] private Transform _cubeFolder;
    [SerializeField] private Transform _graphics;

    [SerializeField] private float _sphereRadius = 1f;
    [SerializeField] private float _distanceSphere = 1f;

    [SerializeField] private Vector3 _boxCastSize;

    [SerializeField] private LayerMask _cubeLayerMask;

    [SerializeField] private Transform _cube;

    private BoxCollider _cubeCollider;
    private bool _sphereCastDisabled;
    private Vector3 _directionMove;

    private void Awake()
    {
        _cubeList = new List<Transform>();
    }

    private void FixedUpdate()
    {
        if (_sphereCastDisabled)
            return;

        if (!Physics.SphereCast(_graphics.position, _sphereRadius, _graphics.forward, out RaycastHit hit, _distanceSphere, _cubeLayerMask))
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
        _cube = hit.transform;
        _cubeList.Add(hit.transform);
        _checkNearestCube = StartCoroutine(CheckNearestCube(hit.transform.position));

    }

    IEnumerator CheckNearestCube(Vector3 pos)
    {
        /*
        Physics.Raycast(pos, _directionMove, out RaycastHit hit, 1f, _cubeCheckLayerMask, QueryTriggerInteraction.Collide)
        Debug.DrawRay(pos, _directionMove, Color.red, 1f);
        */
        if (Physics.BoxCast(pos, _boxCastSize / 2f, _directionMove, out RaycastHit hit, Quaternion.identity, 1f))
        {
            if (hit.collider.CompareTag("Cheese"))
            {
                hit.collider.gameObject.SetActive(false);
                StartCoroutine(CheckNearestCube(pos));
                yield break;
            }

            if (hit.collider.CompareTag("Cube"))
            {
                _cube = hit.collider.transform;
                _cubeList.Add(hit.collider.transform);
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
        SoundManager._instance.PlayAudio(AudioName.Rock_Slide);
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
                _cubeList[i].transform.parent = _cubeFolder;
            }
        }
        _cubeList.Clear();
        _sphereCastDisabled = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _distanceSphere, _sphereRadius);


        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_cube.position + _directionMove, _boxCastSize);

    }
}
