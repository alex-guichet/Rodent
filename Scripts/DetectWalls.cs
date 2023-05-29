using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

public class DetectWalls : MonoBehaviour
{
    [SerializeField] private LayerMask layerDetection;
    [SerializeField] private float sphereRadius = 1f;
    [SerializeField] private float distanceSphere = 1f;
    [SerializeField] private List<SideCheck> sideCheckList;
    [SerializeField] private int timerStuck = 3;

    private EnemyMovement _enemyMovement;
    private int _sideCollided;
    private Coroutine _startTimer;
    private bool _isChecking;


    private void Awake()
    {
        _sideCollided = 0;
        _enemyMovement = this.GetComponent<EnemyMovement>();
        sideCheckList = new List<SideCheck>();
    }

    private void FixedUpdate()
    {
        var forward = transform.forward;
        CheckSide(forward, "Forward");
        CheckSide(-forward, "Backward");
        
        var right = transform.right;
        CheckSide(right, "Right");
        CheckSide(-right, "Left");
        
        CheckDetectors();
    }

    private void CheckSide(Vector3 direction, string sideName)
    {
        if (Physics.SphereCast(transform.position, sphereRadius, direction, out RaycastHit hit, distanceSphere, layerDetection, QueryTriggerInteraction.Collide))
        {
            if (!sideCheckList.Exists(sideCheck => sideCheck.sideName == sideName))
            {
                sideCheckList.Add(new SideCheck(sideName, true));
                _sideCollided++;
                return;
            }

            bool Match(SideCheck sideCheck) => sideCheck.sideName == sideName;

            int index = sideCheckList.FindIndex(check => Match(check));
            if (sideCheckList[index].isTouching == true)
                return;
            
            Updatelist(index, true);
            _sideCollided++;
        }
        else
        {
            if (!sideCheckList.Exists(sideCheck => sideCheck.sideName == sideName))
                return;

            bool Match(SideCheck sideCheck) => sideCheck.sideName == sideName;

            int index = sideCheckList.FindIndex(check => Match(check));
            if (sideCheckList[index].isTouching == false)
                return;

            Updatelist(index, false);
            _sideCollided--;
        }
    }

    private void Updatelist(int index, bool value)
    {
        SideCheck sideCheck = sideCheckList[index];
        sideCheck.isTouching = value;
        sideCheckList[index] = sideCheck;
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
        while (timer < timerStuck)
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
        public string sideName;
        public bool isTouching;

        public SideCheck(string sideName, bool isTouching)
        {
            this.sideName = sideName;
            this.isTouching = isTouching;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * distanceSphere, sphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + -transform.forward * distanceSphere, sphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.right * distanceSphere, sphereRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + -transform.right * distanceSphere, sphereRadius);
    }
}
