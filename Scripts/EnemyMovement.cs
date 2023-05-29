using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    //Hidden Fields
    private static Transform _playerTransform;
    private NavMeshAgent _agent;
    private Vector3 _lastPositionPlayer;
    private Vector3 _currentPositionPlayer;
    private Coroutine _walkBackAndForth;
    private bool _walk;
    public bool _remainingDistance;
    public Vector3 _directionMove;
    private Transform _nearestTransformPlayer;
    private bool _isStopped;
    public Vector3 _destination;
    private DetectWalls _detectWalls;
    private Coroutine _meow;

    //Visible Fields
    [SerializeField] private LayerMask _cubeLayerMask;
    [SerializeField] private Rigidbody _catRb;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private GameObject _cheesePrefab;
    [SerializeField] private Animator _catAnimator;

    public float remainingDistance;

    private void Awake()
    {
        _detectWalls = GetComponent<DetectWalls>();
        _playerTransform = GameObject.Find("Player").transform;
        _walk = false;
        _agent = GetComponent<NavMeshAgent>();
    }


    private void Start()
    {
        SelectNearestTransform();
        _currentPositionPlayer = _nearestTransformPlayer.position;
        _lastPositionPlayer = _currentPositionPlayer;
        _agent.SetDestination(_lastPositionPlayer);
        _agent.updateRotation = false;
        _meow = StartCoroutine(Meow(Random.Range(5, 10)));
    }

    private void FixedUpdate()
    {
        if (_isStopped)
            return;

        if (_agent.pathPending)
            return;

        if (_agent.velocity.normalized != Vector3.zero)
        {
            Quaternion LookRotation = Quaternion.LookRotation(_agent.velocity.normalized);
            _catRb.rotation = Quaternion.RotateTowards(_catRb.rotation, LookRotation, _rotationSpeed);
        }

        MoveTowardsPlayer();
    }

    IEnumerator WalkBackAndForth()
    {
        while (_agent.pathPending)
            yield return new WaitForEndOfFrame();

        while (_agent.remainingDistance > 0.3f)
            yield return new WaitForSeconds(0.5f);

        _agent.SetDestination(_lastPositionPlayer);
        _walk = false;
    }

    private void SelectNearestTransform()
    {
        Transform pathFinder = _playerTransform.Find("PathFinder");
        float nearestDistance = Vector3.Distance(pathFinder.GetChild(0).position, transform.position);
        _nearestTransformPlayer = pathFinder.GetChild(0);

        for (int i=1; i < pathFinder.childCount; i++)
        {
            float localDistance = Vector3.Distance(pathFinder.GetChild(i).position, transform.position);
            if (localDistance < nearestDistance)
            {
                nearestDistance = localDistance;
                _nearestTransformPlayer = pathFinder.GetChild(i);
            }
        }
    }

    private void MoveTowardsPlayer()
    {

        if (_walk)
            return;

        if (_lastPositionPlayer != _nearestTransformPlayer.position)
        {
            _currentPositionPlayer = _nearestTransformPlayer.position;
            _lastPositionPlayer = _currentPositionPlayer;
            _agent.SetDestination(_lastPositionPlayer);
        }

        if (_agent.remainingDistance < 0.1f && _agent.path.status == NavMeshPathStatus.PathPartial)
        {
            _walk = true;
            _directionMove = transform.position + ((transform.position - _nearestTransformPlayer.position).normalized *2f);

            if (_walkBackAndForth != null)
                StopCoroutine(_walkBackAndForth);

            _agent.SetDestination(_directionMove);
            _walkBackAndForth = StartCoroutine(WalkBackAndForth());
        }

        remainingDistance = _agent.remainingDistance;
        _destination = _agent.destination;
    }

    public void Stop()
    {
        StopCoroutine(_meow);
        _agent.isStopped = true;
        _isStopped = true;
        _catAnimator.SetBool("IsSitting", true);
    }
    public void Blocked()
    {
        Stop();
        GameManager._instance.IncrementCatLocked();
    }

    public void Restart()
    {
        _agent.isStopped = false;
        _isStopped = false;
        GameManager._instance.DecrementCatLocked();
        _catAnimator.SetBool("IsSitting", false);
        _meow = StartCoroutine(Meow(Random.Range(5, 10)));
    }

    public bool IsStopped()
    {
        return _isStopped;
    }
    public void TurnIntoCheese()
    {
        Instantiate(_cheesePrefab, transform.position, _cheesePrefab.transform.rotation);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !IsStopped())
        {
            _detectWalls.enabled = false;
            Stop();
            GameManager._instance.Defeat();
        }
    }

    IEnumerator Meow(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        SoundManager._instance.PlayAudio(AudioName.Cat_Meow);
        _meow = StartCoroutine(Meow(Random.Range(5, 10)));
    }
}
