using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    [SerializeField] private Image _gauge;

    public void UpdateGauge(float gaugeValue)
    {
        _gauge.fillAmount = gaugeValue;
    }
}
