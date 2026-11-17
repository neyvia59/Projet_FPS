using UnityEngine;

public class PlantActivator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponentInChildren<Plant>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
