using NUnit.Framework;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{

    public Transform _currentCheckpoint;


    private void Awake()
    {
        _currentCheckpoint = transform;
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Killzone>() != null)
        {
            gameObject.GetComponent<CharacterController>().enabled = false;
            gameObject.transform.position = _currentCheckpoint.position;
            gameObject.GetComponent<CharacterController>().enabled = true;
        }

        if(other.GetComponent<Checkpoint>() != null)
        {
            _currentCheckpoint = other.transform;
        }
    }
}
