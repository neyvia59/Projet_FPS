using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefabBasicProjectile;
    [SerializeField] private Transform _SpawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
    }

    private void CheckInputs() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Shoot();
        }
    }

    private void Shoot() 
    {
        GameObject go = Instantiate(_prefabBasicProjectile, _SpawnPosition.position, Quaternion.identity);
        go.GetComponent<Rigidbody>().AddForce(_SpawnPosition.forward.normalized, ForceMode.Impulse);
    }
}
