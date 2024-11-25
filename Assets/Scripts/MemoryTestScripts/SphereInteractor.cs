using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
public class SphereInteractor : MonoBehaviour, IMixedRealityPointerHandler
{

    [HideInInspector] public int currentMaterialIndex;
    [HideInInspector] public List<Color> colors;
    [HideInInspector] public int sphereIdx;
    [HideInInspector] public GameObject stateManager;
    public AudioClip clickSound;
    private AudioSource clickSource;
    
    private Dictionary<int, Color> userSelection;
    private MeshRenderer meshRenderer;
    private StateManager StateManager_;
 
    // Start is called before the first frame update
    void Start()
    {
        StateManager_ = stateManager.GetComponent<StateManager>();
        currentMaterialIndex = -1;
        meshRenderer = GetComponent<MeshRenderer>();
        clickSource = GetComponent<AudioSource>();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
    }

    private void OnTriggerEnter(Collider other) //for trayType 1
    {
        Debug.Log("This Sphere entered an coll detecting objcet "); // + count);
        //count++;

        meshRenderer.material.color = other.gameObject.GetComponent<AmINearSpheres>().myColor;
        if (StateManager_.userSelection.ContainsKey(sphereIdx))
        {
            StateManager_.userSelection[sphereIdx] = meshRenderer.material.color;
        }
        else
        {
            StateManager_.userSelection.Add(sphereIdx,
            meshRenderer.material.color);
        }
        

    }

    public void OnPointerUp(MixedRealityPointerEventData eventData) { }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { SphereSelected(); }
    

    /*
     * Changes color of the GameObject to the next color within the colors list
     * and adds/changes the color in userSelection Dictionary.
     */
    public void ChangeColor()
    {
        if (colors.Count == 0)
        {
            Debug.LogWarning("No materials assigned!");
            return;
        }
        if (currentMaterialIndex == -1)
        {
            currentMaterialIndex = 1;
        }
        else
        {
            currentMaterialIndex = (currentMaterialIndex + 1) % colors.Count;
        }

        meshRenderer.material.color = colors[currentMaterialIndex];

        if (StateManager_.userSelection.ContainsKey(sphereIdx))
        {
            StateManager_.userSelection[sphereIdx] = meshRenderer.material.color;
        }
        else
        {
            StateManager_.userSelection.Add(sphereIdx,
            meshRenderer.material.color);
        }

    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }


    public void SphereSelected()
    {
        if (StateManager_.GetTrayType() == 0 || StateManager_.GetTrayType() == 2) 
        {
            StateManager_.coachPalmUp.SetActive(false);
            clickSource.PlayOneShot(clickSound);
            if (colors.Count == 0)
            {
                Debug.LogWarning("No materials assigned!");
                return;
            }

            if (StateManager_.HasSelectedColor())
            {
                meshRenderer.material.color = StateManager_.GetColor();

                if(StateManager_.userSelection.ContainsKey(sphereIdx))
                {
                    StateManager_.userSelection[sphereIdx] = meshRenderer.material.color;
                }
                else
                {
                    StateManager_.userSelection.Add(sphereIdx,
                    meshRenderer.material.color);
                }
                StateManager_.SelectedSphere(sphereIdx);
            }
            else
            {
                StateManager_.SelectedSphere(sphereIdx);
            }
        }
       
        
    }


}

