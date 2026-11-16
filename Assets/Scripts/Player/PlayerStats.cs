using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    static public PlayerStats Instance;

    [SerializeField] Gauge SporeBar;
    public float _sporeContamination = 0f;
    [SerializeField] private float _careSporeContamination = 0.001f;
    [SerializeField] private float _timeBetweenCoughs = 4f;
    public Coroutine CoughingCoroutine;

    private void Awake()
    {

        Instance = this;
    }

    public void AddSporeContamination(float addSpore)
    {

            _sporeContamination += addSpore;
            if (_sporeContamination > 1)
            {
                PlayerStats.Instance.gameObject.GetComponent<PlayerCollisions>().KillPlayer();
            }
            else if (_sporeContamination >= 0)
                SporeBar.UpdateGauge(_sporeContamination);
        
    }

    public void CareSporeContamination()
    {
        if (_sporeContamination <=0) return;
        AddSporeContamination(-_careSporeContamination);
    }

    public IEnumerator CoughingDelay()
    {
        yield return new WaitForSeconds(_timeBetweenCoughs);
        System.Random rand = new System.Random();
        int index = rand.Next(0, SoundManager._coughingSounds.Count);
        SoundManager.PlaySound(SoundManager._coughingSounds[index], SoundManager._coughVolume);
        CoughingCoroutine = StartCoroutine(CoughingDelay());
    }
}
