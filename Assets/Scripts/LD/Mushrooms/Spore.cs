using System;
using UnityEngine;

public class Spore : MonoBehaviour
{
    [SerializeField] private float _sporeContamination = 0.001f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.name is "Player")
        {
           if(PlayerStats.Instance.CoughingCoroutine == null)
            {
                PlayerStats.Instance.CoughingCoroutine = StartCoroutine(PlayerStats.Instance.CoughingDelay());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name is "Player")
        {
            if (PlayerStats.Instance.CoughingCoroutine != null)
            {
                StopCoroutine(PlayerStats.Instance.CoughingDelay());
                PlayerStats.Instance.CoughingCoroutine = null;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name is "Player")
        {
            PlayerStats.Instance.AddSporeContamination(_sporeContamination);
        }
    }
}
