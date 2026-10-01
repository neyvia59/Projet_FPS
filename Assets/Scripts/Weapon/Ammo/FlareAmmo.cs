using System.Collections;
using UnityEngine;

public class FlareAmmo : Ammo
{
	private bool _isPlanted = false;
	public float _destroyDelayInAir;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		StartCoroutine(DestroyAfterInAir());
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Wall>() != null && _isPlanted == false || collision.gameObject.GetComponent<Floor>() != null && _isPlanted == false)
		{
			StopCoroutine(DestroyAfterInAir());

			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			_isPlanted = true;
			StartCoroutine(DestroyAfter());
		}
	}
	private IEnumerator DestroyAfterInAir()
	{
		yield return new WaitForSeconds(_destroyDelayInAir);
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(_destroyDelay);
		Destroy(gameObject);
	}

}
