using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsButton : MonoBehaviour {


    public GameObject creditsImage; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void toggleCredits()
    {
        if(creditsImage.activeInHierarchy)
        {
            creditsImage.SetActive(false);
        }
        else
        {
            creditsImage.SetActive(true);
        }
    }
}
