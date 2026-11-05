using NUnit.Framework;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    public static Cheats Instance;
    public GameObject Player;
    public List<Vector3> Checkpoints = new List<Vector3>();
    public int _checkpointCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2) && _checkpointCount<Checkpoints.Count-1)
        {
            _checkpointCount++;
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = Checkpoints[_checkpointCount];
            Player.GetComponent<CharacterController>().enabled = true;
        }


        if (Input.GetKeyDown(KeyCode.F1) && _checkpointCount>0)
        {
            _checkpointCount--;
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = Checkpoints[_checkpointCount];
            Player.GetComponent<CharacterController>().enabled = true;

        }
    }
}
