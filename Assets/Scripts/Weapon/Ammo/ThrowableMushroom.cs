using System.Collections;
using UnityEngine;

public class ThrowableMushroom : Ammo
{
	[SerializeField] private GameObject _lightZonePrefab;
	public float _zoneRadius;
    public float MinDistance = 2f;
    public float CurrentDistance = 4f;
    public float MaxDistance = 8f;
    public float DistanceIncrement = 0.01f;
    [SerializeField] private float _offsetPlacement = 2.5f;

    [SerializeField] private float _destroyDistanceOffset;

    private Vector3 _spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        
    }
    void Start()
	{
        _spawnPosition = transform.position;
		StartCoroutine(DestroyAfter());
	}


    private void Update()
    {
        if (Vector3.Distance(_spawnPosition, transform.position) > CurrentDistance * _destroyDistanceOffset)
        {

            DestroySelf();
        } 
    }

    private void OnCollisionEnter(Collision collision)
	{
        print(collision.gameObject.name);
		if ((collision.gameObject.GetComponent<Wall>() != null || collision.gameObject.GetComponent<Floor>() != null) && collision.gameObject.GetComponent<PlayerMovement>() == null)
		{
            ContactPoint contact = collision.contacts[0];

            Vector3 pointCollision = contact.point;
            Vector3 normale = contact.normal;


            Vector3 newPos = pointCollision + normale * _offsetPlacement;

            GetComponentInChildren<Light>().gameObject.transform.position = newPos;

            DestroySelf();
            
		}
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(_destroyDelay);
        DestroySelf();
    }


    private void DestroySelf()
    {

        

        Instantiate(_lightZonePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


}
