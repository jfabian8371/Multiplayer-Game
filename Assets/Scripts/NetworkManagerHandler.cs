using UnityEngine;
using Unity.Netcode;

public class NetworkManagerHandler : MonoBehaviour
{
    private void Start()
    {
        // Check if NetworkManager exists in GameScene
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError(" NetworkManager not found in GameScene!");
            return;
        }

        //  Ensure NetworkManager persists across scenes
        DontDestroyOnLoad(NetworkManager.Singleton.gameObject);

        //  Check SelectedMode and start network mode accordingly
        if (StartMenu.SelectedMode == "Server")
        {
            Debug.Log(" Starting Server...");
            NetworkManager.Singleton.StartServer();
        }
        else if (StartMenu.SelectedMode == "Host")
        {
            Debug.Log(" Starting Host...");
            NetworkManager.Singleton.StartHost();
        }
        else if (StartMenu.SelectedMode == "Client")
        {
            Debug.Log(" Connecting as Client...");
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.LogWarning(" No network mode selected, staying idle.");
        }
    }
}
