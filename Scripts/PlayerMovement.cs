using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Transform _graphics;

    private Rigidbody _playerRb;
    private Vector3 _direction;


    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _direction.x = Input.GetAxis("Horizontal");
        _direction.z = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        _playerRb.velocity = _direction * _speed;

        if (_direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_direction);
            _graphics.rotation = lookRotation;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cheese"))
        {
            SoundManager.Instance.PlayAudio(AudioName.EatCheese);
            GameManager.Instance.IncrementScore();
            other.gameObject.SetActive(false);
        }
    }

}
