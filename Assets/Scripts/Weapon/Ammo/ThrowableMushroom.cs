using System.Collections;
using UnityEngine;

public class ThrowableMushroom : Ammo
{
	[SerializeField] private GameObject _lightZonePrefab;
	public float _zoneRadius;
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
            Explode();
		}
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(_destroyDelay);
        Explode();
	}


	private void Explode() 
	{
		Instantiate(_lightZonePrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
