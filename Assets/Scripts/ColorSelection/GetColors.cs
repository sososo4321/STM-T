using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * The tray that creates child objects which have the colours for the spheres (the generated gameObjects)
 * 
 */
public class GetColors : MonoBehaviour
{
    public GameObject objectColor; //for trayType 0 
    public GameObject grabColor; //for trayType 1 
    public GameObject stateManager;
    public float shiftAmount;
    public float shiftFactor;
    private List<Color> colors;
   
    /// <summary>
    /// creates a nr of colored spheres based on the colors list
    /// </summary>
    /// <param name="trayType">determines the functionality of the generated spheres</param>
    public void CreateTheObjects(int trayType)
    {
        //scales tray with shiftAmount
        int amount = 0;
        if ((colors.Count - 4) > 0 )
        {
            amount = (colors.Count - 4 + 1) / 2;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + amount * shiftAmount); 
        }

        int count = 0;
        for (int i = 0; i < colors.Count; i++)
        {
            Vector3 newPosition;
            GameObject newObjectCol;
            if(trayType == 0)
            {
                newObjectCol = Instantiate(objectColor, transform, true);
                newObjectCol.GetComponent<GiveColor>().myColor = colors[i];
                newObjectCol.GetComponent<GiveColor>().stateManager = stateManager;
            }
            else if(trayType == 1)
            {
                newObjectCol = Instantiate(grabColor, transform, true);
                newObjectCol.GetComponent<AmINearSpheres>().myColor = colors[i];
                newObjectCol.GetComponent<AmINearSpheres>().stateManager = stateManager;
            }
            else
            {
                Debug.LogError("A proper trayType hasn't be assigned, default is used");
                newObjectCol = Instantiate(objectColor, transform, true);
                newObjectCol.GetComponent<GiveColor>().myColor = colors[i];
                newObjectCol.GetComponent<GiveColor>().stateManager = stateManager;
            }

            if (count % 2 == 0) //we have two columns for the tray
            {
                newPosition = transform.position + new Vector3(- 0.092f, 0, i *shiftFactor - 0.085f + (amount*(-.07f)));
            }
            else
            {
                newPosition = transform.position + new Vector3(0.08599997f, 0, (i-1) * shiftFactor - 0.085f + (amount * (-.07f)));
            }
            newObjectCol.transform.position = newPosition;
            newObjectCol.GetComponent<MeshRenderer>().material.color = colors[i];
            newObjectCol.SetActive(true);
            count++;
        }
    }

    /// <summary>
    /// Destroys all its children besides its BottomOfTray part
    /// </summary>
    public void DestroyTheObjects()
    {
        bool skipOne = true; //to not destroy the bottom of the tray
        foreach (Transform child in transform)
        {
            if (skipOne)
            {
                skipOne = false;
                continue;            }
            else
            {
                Destroy(child.gameObject);
            }
        }
    }
    /// <summary>
    /// Assigns a Color list to its own "colors" list
    /// </summary>
    /// <param name="thecolors">the Color list</param>
    public void GiveColorList(List<Color> thecolors)
    {
        colors = thecolors;
        Debug.Log(" The colors are assigned");
    }

}
