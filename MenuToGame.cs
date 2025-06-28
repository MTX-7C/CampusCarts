using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuToGame : MonoBehaviour
{
    [SerializeField] private TMP_InputField u_Text, s_Text, i_Text;
    [SerializeField] internal string username;
    [SerializeField] internal string sessionID;
    [SerializeField] internal string ipAddress;
    [SerializeField] internal int cart;
    [SerializeField] internal int track;
    [SerializeField] internal bool inMenu = true;
    [SerializeField] private Button joinOrCreateSession;
    [SerializeField] private TrackSelect trackSelect;
    [SerializeField] private int scene;
    internal string pID;
    internal Guid _id;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (inMenu)
            joinOrCreateSession.onClick.AddListener(StartGame);
        try { pID = PlayerPrefs.GetString("pID"); }
        catch
        {
            _id = System.Guid.NewGuid();
            pID = _id.ToString();
            PlayerPrefs.SetString("pID", pID);
        }

        if (pID == "")
        {
            _id = System.Guid.NewGuid();
            pID = _id.ToString();
            PlayerPrefs.SetString("pID", pID);
        }

        if(GameObject.Find("MTG") != this.gameObject)
        {
            Destroy(GameObject.Find("MTG"));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void StartGame()
    {
        username = u_Text.text;
        sessionID = s_Text.text;
        ipAddress = i_Text.text;
        track = trackSelect.whichTrack;
        inMenu = false;
        SceneManager.LoadScene(scene);
    }
}
