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
            KillPlayer();
        }

        if(other.GetComponent<Checkpoint>() != null)
        {
            _currentCheckpoint = other.transform;
        }

        if (other.GetComponent<Plant>() != null) 
        {
            if (other.GetComponent<Plant>()._isEnlightened == false && other.GetComponent<Plant>()._canKillPlayer)
            {
                KillPlayer();
            }

        }
        if (other.gameObject.GetComponent<JumpPad>() != null)
        {
            gameObject.GetComponent<PlayerMovement>().Launch(other.gameObject.GetComponent<JumpPad>().Force, other.gameObject.transform.forward);
        }
    }

    private void KillPlayer()
    {
        gameObject.GetComponent<CharacterController>().enabled = false;
        gameObject.transform.position = _currentCheckpoint.position;
        gameObject.GetComponent<CharacterController>().enabled = true;
        print("Player has died");
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
