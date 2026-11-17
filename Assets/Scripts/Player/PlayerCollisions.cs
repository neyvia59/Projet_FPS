using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCollisions : MonoBehaviour
{

    public Transform _currentCheckpoint;
    public TextMeshProUGUI _checkpointsText;

    public float CheckpointsDuration = 0.7f;

    public Image VictoryImage;
    public TextMeshProUGUI VictoryText;
    public float VictoryDuration = 10f;

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
            if (_currentCheckpoint == other.transform) return;
            _currentCheckpoint = other.transform;
            _checkpointsText.DOFade(1, CheckpointsDuration).OnComplete(HideText);
        }


        if (other.GetComponent<Plant>() != null) 
        {
            if (other.GetComponent<Plant>()._isFullyRetracted == false && other.GetComponent<Plant>()._canKillPlayer && other is not SphereCollider)
            {
                KillPlayer();
            }
            if(other.GetComponent<Plant>()._canKillPlayer == false && other.GetComponent<Plant>()._isEnlightened == false && other is BoxCollider)
            {
                print("plant cancel wallrun");
                GetComponent<PlayerMovement>().StopWallRun();
            }

        }
        if (other.gameObject.GetComponent<JumpPad>() != null && other is not SphereCollider)
        {
            gameObject.GetComponent<PlayerMovement>().Launch(other.gameObject.GetComponent<JumpPad>().Force, other.gameObject.transform.up);
        }


        if(other.gameObject.GetComponent<Victory>() != null)
        {
            Victory();
        }
    }

    private void Victory()
    {
        VictoryImage.gameObject.SetActive(true);
        VictoryImage.DOFade(0, 0.001f);
        VictoryImage.DOFade(1, 0.7f);
        VictoryText.gameObject.SetActive(true);
        VictoryText.DOFade(1, 0.7f).OnComplete(StartDelayMenu);
    }

    private void StartDelayMenu()
    {
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(VictoryDuration);
        SceneManager.LoadScene("MainMenu");
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
            if (other.GetComponent<AmmoBox>() != null && Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                gameObject.GetComponent<WeaponManager>().RefillAmmo();
            }
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

        
    
}
