using UnityEngine;

public class PlantCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ammo>() != null)
        {
            GetComponentInChildren<Plant>().CollisionEnter();
        }
    }
}
