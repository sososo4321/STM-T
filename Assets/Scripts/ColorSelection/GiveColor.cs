using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

/*
 * A child within the tray that gives it color to StateManager when clicked upon
 * 
 */
public class GiveColor : MonoBehaviour, IMixedRealityPointerHandler
{
    public Color myColor;
    public GameObject stateManager;
    public AudioClip clickSound;
    private AudioSource clickSource;

    void Start()
    {
        clickSource = GetComponent<AudioSource>();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        GiveManagerColor(myColor);
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData) {  }

    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }

    public void OnPointerDragged(MixedRealityPointerEventData eventData) {}

    public void GiveManagerColor(Color myColor)
    {
        clickSource.PlayOneShot(clickSound);
        stateManager.GetComponent<StateManager>().SelectColor(myColor);

        Debug.Log("GIVES color");
    }

}
