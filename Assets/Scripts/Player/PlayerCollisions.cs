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
    public TextMeshProUGUI CheckpointsText;
    public TextMeshProUGUI InteractText;

    public float CheckpointsDuration = 0.7f;
    public float InteractDuration = 0.2f;

    public Image VictoryImage;
    public TextMeshProUGUI VictoryText;
    public float VictoryDuration = 10f; 
    public bool IsInDeathAnim;
    [SerializeField] private float _deathDelay = 0.5f;

    private void Awake()
    {
        _currentCheckpoint = transform;
    }

    private void Start()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Killzone>() != null)
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
            CheckpointsText.DOFade(1, CheckpointsDuration).OnComplete(HideText);
        }


        if (other.GetComponent<Plant>() != null)
        {
            if (other.GetComponent<Plant>()._isFullyRetracted == false && other.GetComponent<Plant>()._canKillPlayer && other is not SphereCollider)
            {
                KillPlayer();
            }
            if (other.GetComponent<Plant>()._canKillPlayer == false && other.GetComponent<Plant>()._isEnlightened == false && other is BoxCollider)
            {
                GetComponent<PlayerMovement>().StopWallRun();
            }

        }
        if (other.gameObject.GetComponent<JumpPad>() != null && other is not SphereCollider)
        {
            gameObject.GetComponent<PlayerMovement>().Launch(other.gameObject.GetComponent<JumpPad>().Force, other.gameObject.transform.up);
        }


        if (other.gameObject.GetComponent<Victory>() != null)
        {
            Victory();
        }

        if (other.GetComponent<AmmoBox>() != null || other.gameObject.GetComponentInParent<PickableMushroom>() != null || other.gameObject.GetComponentInParent<NotePage>() != null)
        {
            InteractText.DOFade(1, InteractDuration);
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

        CheckpointsText.DOFade(0, CheckpointsDuration);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<LowZone>() != null)
        {
            GetComponent<PlayerMovement>().CanStandUp = true;
        }

        if (other.GetComponent<AmmoBox>() != null || other.gameObject.GetComponentInParent<PickableMushroom>() != null)
        {
            InteractText.DOFade(0, InteractDuration);
        }

        if (other.gameObject.GetComponentInParent<NotePage>() != null)
        {
            InteractText.DOFade(0, InteractDuration);

        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<AmmoBox>() != null && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E)))
        {
            gameObject.GetComponent<WeaponManager>().RefillAmmo();
        }

        if (other.gameObject.GetComponentInParent<PickableMushroom>() != null && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E)))
        {

            if (gameObject.GetComponent<WeaponManager>()._weaponIndex == 1)
            {
                return;
            }

            InteractText.DOFade(0, InteractDuration);
            gameObject.GetComponent<WeaponManager>().PickupMushroom();
            Destroy(other.gameObject.GetComponentInParent<PickableMushroom>().gameObject);
        }

        if (other.gameObject.GetComponentInParent<NotePage>() != null && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E)))
        {
            
            Notebook.Instance.CollectPage(other.gameObject.GetComponentInParent<NotePage>().PageIndex);
            InteractText.DOFade(0, InteractDuration);
            Destroy(other.gameObject.transform.parent.gameObject);
            //PageImage.SetActive(true);
            //PageImage.GetComponentInChildren<TextMeshProUGUI>().text = ;


        }

        if (Gamepad.current != null)
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



            if (other.gameObject.GetComponentInParent<NotePage>() != null && Gamepad.current.buttonWest.wasPressedThisFrame)
            {
               
                InteractText.DOFade(0, InteractDuration);
                Notebook.Instance.CollectPage(other.gameObject.GetComponentInParent<NotePage>().PageIndex);
                Destroy(other.gameObject.transform.parent.gameObject);
            }
        }


    }

    public void KillPlayer()
    {
        VictoryImage.gameObject.SetActive(true);
        VictoryImage.DOFade(1, 0.2f).OnComplete(GetUp);
        IsInDeathAnim = true;

    }


    private void GetUp()
    {
        gameObject.GetComponent<PlayerStats>()._sporeContamination = 0;
        gameObject.GetComponent<PlayerStats>().AddSporeContamination(0);
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.transform.position = _currentCheckpoint.position;

        gameObject.GetComponent<WeaponManager>().RefillNoAnim();
        Camera.main.GetComponent<PlayerCam>().enabled = false;
        
        Camera.main.GetComponent<Animator>().enabled = true;
        Camera.main.GetComponent<Animator>().SetTrigger("Death");
        VictoryImage.DOFade(0, _deathDelay * 0.8f).SetEase(Ease.InCirc);

        StartCoroutine(EnableAfterDelay());
    }

    private IEnumerator EnableAfterDelay()
    {
        VictoryImage.gameObject.SetActive(false);
        yield return new WaitForSeconds(_deathDelay);
        IsInDeathAnim = false;
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        Camera.main.GetComponent<Animator>().SetTrigger("DeathEnd");
        
        Camera.main.GetComponent<PlayerCam>().enabled = true;
        Camera.main.GetComponent<Animator>().enabled = false;
        

    }
}
