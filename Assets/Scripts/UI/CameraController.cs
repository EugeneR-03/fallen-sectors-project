using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private GameObject _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position - _player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _player.transform.position + _offset;
    }
}
