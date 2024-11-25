using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using System.Security.Policy;
using System.Threading;
//using UnityEditor.IMGUI.Controls;

public class StateManager : MonoBehaviour
{
    [HideInInspector] public List<GameObject> sphereList; // contains all the generated gameObjects
    [HideInInspector] public Dictionary<int, Color> userSelection; //the colors user selects for the gameObjects
    [HideInInspector] public List<Color> correctColors; //the correct color order of the generated gameObjects
    [HideInInspector] public bool finishedGame = false; // to check if a game round has finished
    [HideInInspector] public float prevAccuracy;

    public static int MAX_NUM_SPHERES = 9;

    public Transform gridObjectCollectionTransform; //colors for the handmenu tray
    public GameObject MenuContent; // handmenu tray
    public float displayDuration; // how long the gameObjects display each round
    public GameObject nextRoundButton;
    public GameObject colorSelectButton;//color button for the handmenu tray
    public GameObject json;
    public GameObject colorTray;
    public int trayType;
    private Color selectedColor;
    public GameObject explain;
    private TextMeshPro theText;
    public GameObject startButton;
    public AudioClip clickSound;
    public AudioClip audioHelp;
    public AudioClip audioNext;
    public AudioClip audioIntroduction;
    public AudioClip audioHandmenu;
    private AudioSource clickSource;
    public GameObject coachPalmUp;
    public GameObject coachClick;

    private bool preview = true;


    private float accuracy;
    private int numCorrect;
    private float responseTime;
    private bool done = false;
    private int numColums;
    private float backPlateIncrementAmnt = 0.015f;

    private List<GameObject> palleteButtons;
    private bool trialRound = true;
    private bool trialHelp = false;
    private bool trialNext = false;
    private bool trialPlay = true;

    private int currentSelectedSphere = -1;
    private bool hasColor = false;
    private string filePath = "C:\\Users\\sophi\\Documents\\Mod11\\Game.json";

    // Start is called before the first frame update
    void Start()
    {
        palleteButtons = new List<GameObject>();
        userSelection = new Dictionary<int, Color>();
        theText = explain.GetComponent<TextMeshPro>();
        clickSource = GetComponent<AudioSource>();
        StartCoroutine(PlayIntroduction());
    }

    // Update is called once per frame
    void Update()
    {

        if (!done && sphereList.Count > 0)
        {
            StartCoroutine(DisplayBlocks());
            done = true;
        }
    }

    public Color GetColor()
    {
        return selectedColor;
    }
    public int GetTrayType()
    {
        return trayType;
    }

   
    public void SelectedSphere(int index)
    {
        if (currentSelectedSphere > -1)
        {
            sphereList[currentSelectedSphere].transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        }
        currentSelectedSphere = index;
        sphereList[currentSelectedSphere].transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        if (hasColor)
        {
            sphereList[currentSelectedSphere].GetComponent<MeshRenderer>().material.color = selectedColor;
            Debug.Log(" give exis col " + selectedColor + " to now selected sphere " + currentSelectedSphere);
        }
    }
    
    public void SelectColor(Color color)
    {
        selectedColor = color;
        hasColor = true;
        if (currentSelectedSphere > -1)
        {
            sphereList[currentSelectedSphere].GetComponent<MeshRenderer>().material.color = color;
            Debug.Log("select new col " + selectedColor + " for Already selected Cube " + currentSelectedSphere);
            if (userSelection.ContainsKey(currentSelectedSphere))
            {
                userSelection[currentSelectedSphere] = color;
            }
            else
            {
                userSelection.Add(currentSelectedSphere, color);
            }
            SelectedSphere(currentSelectedSphere);
        }
    }
    public bool HasSelectedColor()
    {
        return hasColor;
    }

    /**
     * Adds a color to the handmenu 
     */
    public void addButtonToGrid(Color color)
    {
        GridObjectCollection gridObjectCollection = gridObjectCollectionTransform.GetComponent<GridObjectCollection>();

        GameObject newButton = Instantiate(colorSelectButton, gridObjectCollectionTransform);

        Transform iconAndTextTransform = newButton.transform.Find("IconAndText");
        Transform UIButtonSquareIcon = iconAndTextTransform.Find("UIButtonSquareIcon");

        UIButtonSquareIcon.GetComponent<MeshRenderer>().material.color = color;

        PressableButtonHoloLens2 pressableButtonHoloLens2 = newButton.GetComponent<PressableButtonHoloLens2>();

        Interactable interactable = newButton.GetComponent<Interactable>();
        interactable.OnClick.AddListener(() =>
        {

            selectedColor = color;
            hasColor = true;
            coachPalmUp.SetActive(false);

        });

        palleteButtons.Add(newButton);
        gridObjectCollection.UpdateCollection();
    }

    /**
     * Sets size of handmenu backplate based on number of colors  
     * 
     */
    public void increaseBackPlateScale()
    {
        GridObjectCollection gridObjectCollection = gridObjectCollectionTransform.GetComponent<GridObjectCollection>();
        StartCoroutine(InvokeUpdateCollection());
        Transform BackPlate = MenuContent.transform.Find("BackPlate");
  
        float incrHeight = 0.0015f * (gridObjectCollection.Rows + 1);
        float incrWidth = 0.0015f * (gridObjectCollection.Columns);
        BackPlate.transform.localScale += new Vector3(incrWidth,incrHeight, 0f);
    }

    /*
     * Enables and disables the GameObject's script. 
     * Enables visibility and defaults the GameObject's color.
     */
    private IEnumerator DisplayBlocks()
    {
        explain.SetActive(true);
        for (int i = 0; i < sphereList.Count; i++)
        {
            sphereList[i].GetComponent<SphereInteractor>().enabled = false;
            sphereList[i].GetComponent<MeshRenderer>().enabled = true;
        }
        nextRoundButton.SetActive(false);
        if (trialRound)
        {
            yield return new WaitForSeconds(5);
        }
        else
        {
            yield return new WaitForSeconds(displayDuration);
        }

        for (int i = 0; i < sphereList.Count; i++)
        {
            sphereList[i].GetComponent<SphereInteractor>().enabled = true;
            sphereList[i].GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1); // Color.gray;
        }
        if(trayType != 1)
        {
            coachClick.GetComponent<TouchTheObjects>().TouchIt(sphereList[0]);
        }
        if (trayType == 0 || trayType == 1) 
        { 
            colorTray.SetActive(true);
            GiveTrayColors(correctColors); // Pass the correct colors to be shuffled and displayed in the tray
            colorTray.GetComponent<GetColors>().CreateTheObjects(trayType);
        }
        if(trayType == 2)
        {
            coachPalmUp.SetActive(true);
        }
        else
        {
            coachPalmUp.SetActive(false);
        }


        nextRoundButton.SetActive(true);
        if (trayType == 0 || trayType == 2)
        {
            theText.text = "Select the correct Color";
            coachClick.SetActive(true); 
        }
        else if (trayType == 1)
        {
            theText.text = "Drag the correct Color";
        }
        else
        {
            Debug.LogError("An existing trayType is not assigned, default message used.");
            theText.text = "Select the correct Color";
        }
        responseTime = -Time.time;
    }
    

    /*
     * Checks if the user finished selecting colors for all the generated GameOfbjects 
     * if yes, their accuracy and response time of this round is calculated.
     */
    public void onUserFinish()
    {
        if (!preview)
        {
            if (sphereList.Count == userSelection.Count && !finishedGame)
            {
                //calculates accuracy
                for (int i = 0; i < userSelection.Count; i++)
                {
                    Color selectedColor = userSelection[i];
                    Color correctColor = correctColors[i];
                    if (selectedColor == correctColor)
                    {
                        numCorrect += 1;
                    }
                    Destroy(sphereList[i]);
                    coachClick.SetActive(false);
                }
                if (json.GetComponent<SphereGenerator>().getCurrentNumberOfRounds() == 0)
                {
                    prevAccuracy = 0;
                    startButton.SetActive(true);
                    trialRound = false;
                    TextStart(); 
                    json.GetComponent<SphereGenerator>().setCurrentNumberOfRounds();
                    resetState();
                
                }
                else
                {
                    responseTime += Time.time;
                    accuracy = ((float)numCorrect / sphereList.Count) * 100;
                    Debug.Log("You have finished!" + " ACCURACY: " + accuracy.ToString("F2") + "%" + " RESPONSE TIME (Seconds): " + responseTime.ToString("F2"));
                    json.GetComponent<BetterJson>().AddRound(accuracy, responseTime);
                    prevAccuracy = accuracy;
                    if (accuracy < 50)
                    {
                        TextGameFinished();
                    }
                    else
                    {
                        finishedGame = true;
                        explain.SetActive(false);
                    }
                    
                }
       
                colorTray.GetComponent<GetColors>().DestroyTheObjects();
                colorTray.SetActive(false);
                currentSelectedSphere = -1;
                hasColor = false;
                nextRoundButton.SetActive(false);
            
            
            
            

            }//user didn't select a color for all the GameObjects
            else if (sphereList.Count != userSelection.Count)
            {
                theText.text = "You need to select all the objects!";
            }
            else //shouldn't occur
            {
                Debug.Log("You have finished already!");
            }
        }
        
    }

    private List<Color> ShuffleColors(List<Color> colors)
    {
        System.Random rng = new System.Random();
        return colors.OrderBy(a => rng.Next()).ToList();
    }

    public void GiveTrayColors(List<Color> theColors)
    {
        List<Color> shuffledColors = ShuffleColors(theColors);
        colorTray.GetComponent<GetColors>().GiveColorList(shuffledColors);
    }


    /*
     * Resets the values necessary for logging user's performance and starting the next game round
     */
    public void resetState()
    {
        numCorrect = 0;
        accuracy = 0;
        responseTime = 0;
        done = false;
        finishedGame = false;
        userSelection = new Dictionary<int, Color>();
        sphereList = new List<GameObject>();
        correctColors = new List<Color>();

        if (trayType == 2)
        {
            palleteButtons.RemoveAll(button =>
            {
                Destroy(button);
                return true;
            });
            StartCoroutine(InvokeUpdateCollection());

        }

        hasColor = false;
    }

    private IEnumerator InvokeUpdateCollection()
    {
        yield return null;
        gridObjectCollectionTransform.GetComponent<GridObjectCollection>().UpdateCollection();
    }


    public void noPreview()
    {
        preview = false;
    }

    public void GoHelp() 
    {
        StartCoroutine(PlayHelp());
    }
    IEnumerator PlayHelp()
    {
        if (preview)
        {
            trialHelp = true;
            if (trialHelp && trialNext)
            {
                startButton.SetActive(true);
                coachPalmUp.SetActive(false);
                clickSource.clip = audioHelp;
                clickSource.Play();
                yield return new WaitForSeconds(audioHelp.length + 1);
                clickSource.clip = clickSound;
                clickSource.Play();
            }
            else
            {
                clickSource.clip = audioHelp;
                clickSource.Play();
                yield return null;
            }
        }
    }
    public void GoNext()
    {
        StartCoroutine(PlayNext());
    }
    IEnumerator PlayNext()
    {
        if (preview)
        {
            trialNext = true;
            if (trialHelp && trialNext)
            {
                startButton.SetActive(true);
                coachPalmUp.SetActive(false);
                theText.text = "Click this or the blue next round button that will appear besides the objects to go to the next round.";
                clickSource.clip = audioNext;
                clickSource.Play();
                yield return new WaitForSeconds(audioNext.length + 1);
                clickSource.clip = clickSound;
                clickSource.Play();
            }
            else
            {
                theText.text = "Click this or the blue next round button that will appear besides the objects to go to the next round.";
                clickSource.clip = audioNext;
                clickSource.Play();
                yield return new WaitForSeconds(1);

            }
        }

    }

   
    IEnumerator PlayIntroduction()
    {
        clickSource.clip = audioIntroduction;
        clickSource.Play();
        yield return new WaitForSeconds(4);
        clickSource.clip = audioHandmenu;
        clickSource.Play();
        theText.text = "Click the HandMenu buttons!";
        coachPalmUp.SetActive(true);
    }

    //----------------TEXT------------------

    /*
     * Occurs before starting the trial round and the actual experiment
     */
    public void TextStart()
    {
        if (trialRound)
        {
            theText.text = "This is the trial round, click Start";
        }
        else
        {
            theText.text = "Let's start the actual experiment";
        }
    }
    /*
     * Occurs when clicking the help button on the handmenu
     */
    public void TextGiveHelp()
    {
        if (trialRound)
        {
            theText.text = "Here info about correct hand movements and instructions on how the game works are given";
        }
        else
        {
            if(trayType == 0)
            {
                theText.text = "Select colors/objects with far interaction click, the spectral hands show it.";
            }else if(trayType == 1)
            {
                theText.text = "Select and hold your click from above, towards the color, then drag it towards the objects.";
            }else if(trayType == 2)
            {
                theText.text = "See the spectral hand movements in front of the object as to how.";
            }
            else
            {
                theText.text = "Watch the spectral hand movements to understand";
            }
            
        }
    }
    /*
     * Occurs when the objects are generated and are showing their color
     */
    public void TextObjectsGenerated()
    {
        if (trialRound)
        {
            theText.text = "Multiple objects will appear of which the color order you have to remember";
        }
        else
        {
            theText.text = "Remember!";
        }
    }

    /*
     * Occurs when the final round has been played
     */
    public void TextGameFinished()
    {
        theText.text = "You finished the pre-experiment game! Good job, please remove the headset.";
        explain.SetActive(true);
        json.GetComponent<BetterJson>().SaveThisGameData(filePath);
    }
}
