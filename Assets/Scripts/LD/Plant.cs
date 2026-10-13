using UnityEngine;

public class Plant : Mushroom
{

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnlightened)
        {
            print("caché");
        }
        else
        {
            print("pas caché");
        }
    }
}
