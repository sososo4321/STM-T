using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class SphereGenerator : MonoBehaviour
{
    public GameObject initialSphere;
    public int shiftAmount;
    public float shiftFactor;
    public GameObject stateManager;
    public int initNumberOfSpheresToGenerate;
    public int numberOfRounds;
    public int maxNumberOfSpheresToGenerate;


    private int currentNumberOfRounds = 0;
    private Transform initialTransform;
    private List<GameObject> sphereList;
    private List<Color> colors;
    private StateManager stateManagerScript;
    private List<Color> availableColors;
    void Start()
    {
        stateManagerScript = stateManager.GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stateManagerScript.finishedGame) { 
            if(currentNumberOfRounds < numberOfRounds)
            {
                if(initNumberOfSpheresToGenerate < maxNumberOfSpheresToGenerate && stateManagerScript.prevAccuracy >= 50)
                {
                    initNumberOfSpheresToGenerate++;
                }
                
                currentNumberOfRounds++;
                stateManagerScript.resetState();
                generateSpheres();
            }
            else
            {
                stateManagerScript.TextGameFinished();
                stateManager.SetActive(false);
            }
        }
    }

    public int getCurrentNumberOfRounds()
    {
        return currentNumberOfRounds;
    }
    public void setCurrentNumberOfRounds()
    {
        currentNumberOfRounds = currentNumberOfRounds + 1;
    }
    public void generateSpheres()
    {
        stateManagerScript.TextObjectsGenerated();
        sphereList = new List<GameObject>();
        initialTransform = initialSphere.transform;
  
        colors = new List<Color>();

        pickColor();

        for (int i = 0; i < initNumberOfSpheresToGenerate; i++)
        {
            GameObject newCube = Instantiate(initialSphere);
            Vector3 newPosition = initialTransform.position + new Vector3(shiftFactor * (shiftAmount + i), 0, 0);
            newCube.transform.position = newPosition;
            newCube.GetComponent<SphereInteractor>().stateManager = stateManager; 
            newCube.GetComponent<SphereInteractor>().colors = new List<Color>(colors);
            newCube.GetComponent<SphereInteractor>().sphereIdx = i;
            newCube.GetComponent<MeshRenderer>().material.color = colors[i];

            newCube.SetActive(true);
            sphereList.Add(newCube);
            if (currentNumberOfRounds == 0)
            {
                newCube.transform.position = initialTransform.position + new Vector3(shiftFactor * (2 + i), 0, 0);
                break;
            }
        }
        stateManagerScript.correctColors = new List<Color>(colors);
        stateManagerScript.sphereList = new List<GameObject>(sphereList);

        if (stateManagerScript.GetTrayType() == 0 ||stateManagerScript.GetTrayType() == 1)
        {
            stateManagerScript.GiveTrayColors(new List<Color>(colors));
        }
        else if (stateManagerScript.GetTrayType() == 2)
        {
            int[] indexes = new int[colors.Count];
            for (int i = 0; i < colors.Count; i++)
            {
                indexes[i] = i;
            }
            shuffle(indexes);   
            for(int i = 0; i < colors.Count; i++)
            {
                stateManagerScript.addButtonToGrid(colors[indexes[i]]);
            }
            stateManagerScript.increaseBackPlateScale();
        }
        else
        {
            Debug.Log("not proper tray type selected, default generated");
            //TODO
        }
        
    }
    private void shuffle(int[] indexes)
    { 
        for (int i = 0; i < indexes.Length; i++)
        {
            int j = Random.Range(0, colors.Count-i);
            int temp = indexes[i];
            indexes[i] = indexes[j];
            indexes[j] = temp;
        }
    }

    private void pickColor()
    {
        List<Color> col = ChangeColors();
        int rand;
        for (int i = 0; i < initNumberOfSpheresToGenerate; i++)
        {
            rand = Random.Range(0, col.Count);
            Color newColor = col[rand];
            colors.Add(newColor);
            col.Remove(newColor);
            if (currentNumberOfRounds == 0)
            {
                break;
            }
        }
    }

    /*
     * Hardcoded list of distinct colors
     */
    private List<Color> ChangeColors()
    {
        List<Color> lColors = new List<Color>();

        //lColors.Add(new Color32(,,,1));
        lColors.Add(new Color32(0, 0, 0, 1));
        lColors.Add(new Color32(128,128,128, 1));
        lColors.Add(new Color32(255, 0, 0, 1));
        lColors.Add(new Color32(255, 128, 0, 1));
        lColors.Add(new Color32(255, 255, 0, 1));
        lColors.Add(new Color32(0, 255, 0, 1));
        lColors.Add(new Color32(0, 255, 255, 1));
        lColors.Add(new Color32(0, 0, 255, 1));
        lColors.Add(new Color32(127, 0, 255, 1));

        return lColors;

    }

}


