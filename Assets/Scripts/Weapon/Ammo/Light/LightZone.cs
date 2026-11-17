using UnityEngine;

public class LightZone : Ammo
{
    
    private void Start()
    {
        GetComponent<SphereCollider>().radius = GetComponent<Light>().range;
    }


    private void OnTriggerStay(Collider other)
    {

    }
}
