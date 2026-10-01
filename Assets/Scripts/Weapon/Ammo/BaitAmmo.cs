using System.Collections;
using UnityEngine;

public class BaitAmmo : Ammo
{
	private bool _isPlanted = false;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		StartCoroutine(DestroyAfter());
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Floor>() != null && _isPlanted == false)
		{
			StopCoroutine(DestroyAfter());

			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			_isPlanted = true;
			StartCoroutine(DestroyAfter());
		}
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(_destroyDelay);
		Destroy(gameObject);
	}
}
