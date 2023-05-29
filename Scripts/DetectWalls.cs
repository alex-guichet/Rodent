using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DetectWalls : MonoBehaviour
{
    [SerializeField] private LayerMask _layerDetection;
    [SerializeField] private float _sphereRadius = 1f;
    [SerializeField] private float _distanceSphere = 1f;
    [SerializeField] private List<SideCheck> _sideCheckList;
    [SerializeField] private int _timerStuck = 3;

    private EnemyMovement _enemyMovement;
    private int _sideCollided;
    private Coroutine _startTimer;
    private bool _isChecking;


    private void Awake()
    {
        _sideCollided = 0;
        _enemyMovement = this.GetComponent<EnemyMovement>();
        _sideCheckList = new List<SideCheck>();
    }

    private void FixedUpdate()
    {
        CheckSide(transform.forward, "Forward");
        CheckSide(-transform.forward, "Backward");
        CheckSide(transform.right, "Right");
        CheckSide(-transform.right, "Left");
        CheckDetectors();
    }

    private void CheckSide(Vector3 direction, string sideName)
    {
        if (Physics.SphereCast(transform.position, _sphereRadius, direction, out RaycastHit hit, _distanceSphere, _layerDetection, QueryTriggerInteraction.Collide))
        {
            if (!_sideCheckList.Exists(SideCheck => SideCheck._sideName == sideName))
            {
                _sideCheckList.Add(new SideCheck(sideName, true));
                _sideCollided++;
                return;
            }

            int index = _sideCheckList.FindIndex(SideCheck => SideCheck._sideName == sideName);
            if (_sideCheckList[index]._isTouching == true)
                return;
            
            Updatelist(index, true);
            _sideCollided++;
        }
        else
        {
            if (!_sideCheckList.Exists(SideCheck => SideCheck._sideName == sideName))
                return;

            int index = _sideCheckList.FindIndex(SideCheck => SideCheck._sideName == sideName);
            if (_sideCheckList[index]._isTouching == false)
                return;

            Updatelist(index, false);
            _sideCollided--;
        }
    }

    private void Updatelist(int index, bool value)
    {
        SideCheck sideCheck = _sideCheckList[index];
        sideCheck._isTouching = value;
        _sideCheckList[index] = sideCheck;
    }

    private void CheckDetectors()
    {
        if(_sideCollided == 4 && !_isChecking && !_enemyMovement.IsStopped())
        {
            _isChecking = true;

            if(_startTimer != null)
                StopCoroutine(_startTimer);

            _startTimer = StartCoroutine(StartTimer());
        }

        if(_sideCollided != 4 && _enemyMovement.IsStopped())
        {
            _enemyMovement.Restart();
        }

        if (_sideCollided != 4 && !_enemyMovement.IsStopped() && _isChecking)
        {
            StopCoroutine(_startTimer);
            _isChecking = false;
        }
    }

    IEnumerator StartTimer()
    {
        int timer = 0;
        while (timer < _timerStuck)
        {
            timer++;
            yield return new WaitForSeconds(1f);
        }

        _enemyMovement.Blocked();
        _isChecking = false;
    }

    [System.Serializable]
    public struct SideCheck
    {
        public string _sideName;
        public bool _isTouching;

        public SideCheck(string sideName, bool isTouching)
        {
            _sideName = sideName;
            _isTouching = isTouching;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * _distanceSphere, _sphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + -transform.forward * _distanceSphere, _sphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * _distanceSphere, _sphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + -transform.right * _distanceSphere, _sphereRadius);
    }
}
