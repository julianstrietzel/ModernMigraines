using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRectWidth : MonoBehaviour
{

	GameObject backgroundGen;
	RectTransform backgroundGenRect;

	GameObject backgroundPers;
	RectTransform backgroundPersRect;

    // Start is called before the first frame update
    void Start()
    {
    	int generalPercentage = 90; //make sure this is a whole number
    	backgroundGen = GameObject.Find("backgroundGen");
        backgroundGenRect = backgroundGen.GetComponent<RectTransform>();
        backgroundGenRect.sizeDelta = new Vector2((int)(200), backgroundGenRect.sizeDelta.y);

        //double personalPercentage = (double)90/(double)100; //make sure this is a whole number
        //float newWidth = (float)(personalPercentage * (double)backgroundPersRect.sizeDelta.x);
        backgroundPers = GameObject.Find("backgroundPers");
        backgroundPersRect = backgroundPers.GetComponent<RectTransform>();
        backgroundPersRect.sizeDelta = new Vector2(90, backgroundPersRect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
