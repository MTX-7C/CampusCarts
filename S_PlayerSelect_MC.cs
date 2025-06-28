using UnityEngine;

public class S_PlayerSelect_MC : MonoBehaviour
{
    public MenuToGame mTG;

    public GameObject[] selection;

    public GameObject currentSelection;

    // Update is called once per frame
    public void Right()
    {
        mTG.cart++;
        if (mTG.cart > 3)
        {
            mTG.cart = 0;
        }
        currentSelection.SetActive(false);
        selection[mTG.cart].SetActive(true);
        currentSelection = selection[mTG.cart];
    }

    public void Left() 
    {
        mTG.cart--;
        if(mTG.cart < 0)
        {
            mTG.cart = 3;
        }
        currentSelection.SetActive(false);
        selection[mTG.cart].SetActive(true);
        currentSelection = selection[mTG.cart];
    }
}
