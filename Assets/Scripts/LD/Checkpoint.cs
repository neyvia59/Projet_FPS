using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private void Start()
    {
        Cheats.Instance.Checkpoints.Add(transform.position);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerStats>() != null)
        {
            PlayerStats.Instance.CareSporeContamination();
        }
    }
}
