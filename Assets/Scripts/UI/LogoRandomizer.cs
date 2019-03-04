using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoRandomizer : MonoBehaviour {
    public List<Sprite> orbs;
    Image imageComponent;
    int currentOrbImageIndex;
    
	// Use this for initialization
	void Start () {
        imageComponent = GetComponent<Image>();
        currentOrbImageIndex = Random.Range(0, orbs.Count);
        imageComponent.sprite = orbs[currentOrbImageIndex];
        StartCoroutine(orbRandomizer());
	}
	
	// Update is called once per frame
	void Update () {

	}

    IEnumerator orbRandomizer()
    {
        int prev = currentOrbImageIndex;
        while(prev == currentOrbImageIndex)
        {
            currentOrbImageIndex = Random.Range(0, orbs.Count);
        }
        imageComponent.sprite = orbs[currentOrbImageIndex];

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(orbRandomizer());
    }
}
