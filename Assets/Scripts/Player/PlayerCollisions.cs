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

        if (other.GetComponent<Plant>() != null) 
        { 
            if(other.GetComponent<Plant>()._isEnlightened == false)
            {
                print("attacked");
            }
            else
            {
                print("safe");
            }
        }
        if (other.gameObject.GetComponent<JumpPad>() != null)
        {
            gameObject.GetComponent<PlayerController>().ApplyJumpPadBoost(other.gameObject.GetComponent<JumpPad>().Force, other.gameObject.GetComponent<JumpPad>().HorizontalBoost, other.gameObject.GetComponent<JumpPad>().BoostDuration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
