using TMPro;
using UnityEngine;

public class TrackSelect : MonoBehaviour
{
    public int numberOfTracks;
    public int whichTrack;
    public TMP_Text trackName;

    public void NextTrack()
    {
        whichTrack++;
        if(whichTrack >= numberOfTracks) { whichTrack = 0; }
        ShowTrackName();
    }

    public void PrevTrack() 
    { 
        whichTrack--; 
        if(whichTrack < 0) { whichTrack = numberOfTracks - 1; }
        ShowTrackName();
    }

    void ShowTrackName()
    {
        switch (whichTrack)
        {
            case 0:
                trackName.text = "Tutorial";
                break;
            case 1:
                trackName.text = "COBA";
                break;
            case 2:
                trackName.text = "Lunds";
                break;
            case 3:
                trackName.text = "Campus";
                break;
            default:
                trackName.text = "Tutorial";
                break;
        }
    }
}
