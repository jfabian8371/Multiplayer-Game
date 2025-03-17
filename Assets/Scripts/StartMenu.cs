using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    //  Ensure this is static so it can be accessed from other scripts
    public static string SelectedMode = ""; 

    public void StartServer()
    {
        Debug.Log(" Server Selected. Loading GameScene...");
        SelectedMode = "Server";  // Store mode
        SceneManager.LoadScene("GameScene"); // Load game scene
    }

    public void JoinAsHost()
    {
        Debug.Log(" Host Selected. Loading GameScene...");
        SelectedMode = "Host";  // Store mode
        SceneManager.LoadScene("GameScene");
    }

    public void JoinAsClient()
    {
        Debug.Log(" Client Selected. Loading GameScene...");
        SelectedMode = "Client";  // Store mode
        SceneManager.LoadScene("GameScene");
    }
}
