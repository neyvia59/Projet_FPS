using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    


    public void OnClickPlay()
    {
        SceneManager.LoadScene("Gameplay");
    }


    public void OnClickCredits()
    {

    }
}
