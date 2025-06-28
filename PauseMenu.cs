using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public PlayerManager playerManager;
    public ConnectionManager connectionManager;
    public GameObject menu;
    public GameObject connectMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(menu.activeInHierarchy)
            {
                menu.SetActive(false);
            }
            else
            {
                menu.SetActive(true);
            }
        }
    }

    public void Disconnect()
    {
        connectionManager.SignOutPlayer();
        NetworkManager.Singleton.Shutdown();
    }

    public void DisconnectFully()
    {
        playerManager.RemovePlayer(connectionManager.PID, connectionManager.ClientID);
        connectionManager.SignOutPlayer();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        connectionManager.SignOutPlayer();
        playerManager.RemovePlayer(connectionManager.PID, connectionManager.ClientID);
        NetworkManager.Singleton.Shutdown();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
        Application.Quit();
    }
}
