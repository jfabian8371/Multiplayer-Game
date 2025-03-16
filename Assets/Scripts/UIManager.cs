using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header(" Panels ")]
    [SerializeField] private GameObject connectionPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject gamePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowConnectionPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ShowConnectionPanel(){
        connectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);
    }

    private void ShowWaitingPanel(){
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private void ShowGamePanel(){
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void HostButtonCallBack(){
        NetworkManager.Singleton.StartHost();
        ShowWaitingPanel();
    }

    

    public void ClientButtonCallBack(){
        NetworkManager.Singleton.StartClient();
        ShowWaitingPanel();
    }
}
