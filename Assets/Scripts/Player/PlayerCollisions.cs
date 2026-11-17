using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollisions : MonoBehaviour
{

    public Transform _currentCheckpoint;
    public TextMeshProUGUI _checkpointsText;

    public float CheckpointsDuration = 0.7f;


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

        if (other.GetComponent<LowZone>() != null)
        {
            GetComponent<PlayerMovement>().CanStandUp = false;
        }

        if (other.GetComponent<Checkpoint>() != null)
        {
            _currentCheckpoint = other.transform;
            _checkpointsText.DOFade(1, CheckpointsDuration).OnComplete(HideText);
        }


        if (other.GetComponent<Plant>() != null) 
        {
            if (other.GetComponent<Plant>()._isFullyRetracted == false && other.GetComponent<Plant>()._canKillPlayer && other is not SphereCollider)
            {
                KillPlayer();
            }
            if(other.GetComponent<Plant>()._canKillPlayer == false && other is BoxCollider)
            {
                GetComponent<PlayerMovement>().StopWallRun();
            }

        }
        if (other.gameObject.GetComponent<JumpPad>() != null && other is not SphereCollider)
        {
            gameObject.GetComponent<PlayerMovement>().Launch(other.gameObject.GetComponent<JumpPad>().Force, other.gameObject.transform.up);
        }
    }

    private void HideText()
    {
 
        _checkpointsText.DOFade(0, CheckpointsDuration);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<LowZone>() != null)
        {
            GetComponent<PlayerMovement>().CanStandUp = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<AmmoBox>() != null && ( Input.GetKeyDown(KeyCode.F)|| Input.GetKeyDown(KeyCode.E)))
        {
            gameObject.GetComponent<WeaponManager>().RefillAmmo();
        }
        
        if(other.gameObject.GetComponentInParent<PickableMushroom>() != null && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E)))
        {
            if(gameObject.GetComponent<WeaponManager>()._weaponIndex == 1)
            {
                return;
            }
            gameObject.GetComponent<WeaponManager>().PickupMushroom();
            Destroy(other.gameObject.GetComponentInParent<PickableMushroom>().gameObject);
        }

        if(Gamepad.current != null)
        {
            if (other.gameObject.GetComponentInParent<PickableMushroom>() != null && Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                if (gameObject.GetComponent<WeaponManager>()._weaponIndex == 1)
                {
                    return;
                }
                gameObject.GetComponent<WeaponManager>().PickupMushroom();
                Destroy(other.gameObject.GetComponentInParent<PickableMushroom>().gameObject);
            }
        }

        if (other.GetComponent<AmmoBox>() != null && Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            gameObject.GetComponent<WeaponManager>().RefillAmmo();
        }
    }

    public void KillPlayer()
    {
        gameObject.GetComponent<PlayerStats>()._sporeContamination = 0;
        gameObject.GetComponent<PlayerStats>().AddSporeContamination(0);
        gameObject.GetComponent<CharacterController>().enabled = false;
        gameObject.transform.position = _currentCheckpoint.position;
        gameObject.GetComponent<CharacterController>().enabled = true;
        gameObject.GetComponent<WeaponManager>().RefillNoAnim();
        print("Player has died");
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
