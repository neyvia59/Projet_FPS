using System;
using UnityEngine;

public class Spore : MonoBehaviour
{
    [SerializeField] private float _sporeContamination = 0.001f;
    private void OnTriggerStay(Collider other)
    {
        if (other.name is "Player")
        {
            PlayerStats.Instance.AddSporeContamination(_sporeContamination);
        }
    }
}
