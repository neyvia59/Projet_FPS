using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlareAmmo : Ammo
{
    private bool _isPlanted = false;
    public float _destroyDelayInAir;

    public float _offsetPlacement = 0.5f;

    [SerializeField] private GameObject _pointLightPrefab;
    [SerializeField] private float _distanceBetweenLights;
    private Vector3 _lastPointLightPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterInAir());
        _lastPointLightPos = transform.position;
    }

    private void Update()
    {
        if ( Vector3.Distance(_lastPointLightPos, transform.position) > _distanceBetweenLights)
        {
            Instantiate(_pointLightPrefab, transform.position, Quaternion.identity);
            _lastPointLightPos = transform.position;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Wall>() != null && _isPlanted == false || collision.gameObject.GetComponent<Floor>() != null && _isPlanted == false)
        {
            StopCoroutine(DestroyAfterInAir());

            gameObject.GetComponent<Rigidbody>().isKinematic = true;


            ContactPoint contact = collision.contacts[0];

            Vector3 pointCollision = contact.point;
            Vector3 normale = contact.normal;


            Vector3 newPos = pointCollision + normale * _offsetPlacement;

            GetComponentInChildren<Light>().gameObject.transform.position = newPos;

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
