using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStats>() != null)
        {
            PlayerStats.Instance.CareSporeContamination();
        }
    }
}
