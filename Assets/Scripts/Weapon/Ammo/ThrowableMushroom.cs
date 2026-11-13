using System.Collections;
using UnityEngine;

public class ThrowableMushroom : Ammo
{
	[SerializeField] private GameObject _lightZonePrefab;
	public float _zoneRadius;
	[SerializeField] private float _offsetPlacement = 2.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        
    }
    void Start()
	{
		StartCoroutine(DestroyAfter());
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Wall>() != null || collision.gameObject.GetComponent<Floor>() != null)
		{


            ContactPoint contact = collision.contacts[0];

            Vector3 pointCollision = contact.point;
            Vector3 normale = contact.normal;


            Vector3 newPos = pointCollision + normale * _offsetPlacement;

            GetComponentInChildren<Light>().gameObject.transform.position = newPos;
            Instantiate(_lightZonePrefab, newPos, Quaternion.identity);

            Destroy(gameObject);
		}
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(_destroyDelay);
        Instantiate(_lightZonePrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


}
