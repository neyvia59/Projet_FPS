using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    static public PlayerStats Instance;
    public bool IsAlive = true;
    [SerializeField] Gauge SporeBar;
    public float _sporeContamination = 0f;
    [SerializeField] private float _careSporeContamination = 0.001f;

    private void Awake()
    {
        IsAlive = true;
        Instance = this;
    }

    public void AddSporeContamination(float addSpore)
    {
        if (IsAlive)
        {
            _sporeContamination += addSpore;
            if (_sporeContamination > 1)
            {
                Debug.Log("player death");
                IsAlive = false;
            }
            else if (_sporeContamination >= 0)
                SporeBar.UpdateGauge(_sporeContamination);
        }
    }

    public void CareSporeContamination()
    {
        AddSporeContamination(-_careSporeContamination);
    }
}
