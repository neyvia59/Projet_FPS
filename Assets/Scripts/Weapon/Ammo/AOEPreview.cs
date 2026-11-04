
using UnityEngine;

public class AOEPreview : MonoBehaviour
{

    private void Awake()
    {
        Hide();
    }

    public void Show(Vector3 position, float radius)
    {
        transform.position = position;

        transform.localScale = Vector3.one * (radius * 2f);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
