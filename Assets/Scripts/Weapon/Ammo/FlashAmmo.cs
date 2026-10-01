using System.Collections;
using UnityEngine;

public class FlashAmmo : Ammo
{

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		StartCoroutine(DestroyAfter());
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Wall>() != null || collision.gameObject.GetComponent<Floor>() != null)
		{
			FlashBang();
		}
	}

	private IEnumerator DestroyAfter()
	{
		yield return new WaitForSeconds(_destroyDelay);
		FlashBang();
	}


	private void FlashBang() 
	{
		print("Think fast Chucklenuts");
		Destroy(gameObject);
	}
}
