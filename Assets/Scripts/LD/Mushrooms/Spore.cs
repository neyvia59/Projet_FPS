using System;
using UnityEngine;

public class Spore : MonoBehaviour
{
    [SerializeField] private float _sporeContamination = 0.001f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.name is "Player")
        {
            PlayerStats.Instance.IsInSpore = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name is "Player")
        {
            PlayerStats.Instance.IsInSpore = false;
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
