
using System.Collections.Generic;
using UnityEngine;

public class FogLightSource : MonoBehaviour
{
    // Liste des sources présentes dans la scène.
    public static readonly List<FogLightSource> ActiveSources =
        new List<FogLightSource>();

    [Header("Fog Clearing")]
    [Min(0.1f)]
    public float clearingRadius = 4f;

    [Min(0f)]
    public float clearingStrength = 1f;

    [Header("Light Settings")]
    public bool isActive = true;

    public Vector3 Position => transform.position;

    private void OnEnable()
    {
        if (!ActiveSources.Contains(this))
            ActiveSources.Add(this);
    }

    private void OnDisable()
    {
        ActiveSources.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, clearingRadius);
    }
}
