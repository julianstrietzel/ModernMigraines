using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LocalDataManager;

public class ChangeRectWidth : MonoBehaviour
{

	GameObject backgroundGen;
	RectTransform backgroundGenRect;

	GameObject backgroundPers;
	RectTransform backgroundPersRect;

    public RectTransform BackgroundSize;
    public RectTransform BarGraph;

    private void SetPercent(float percent) {
        // percent is 0 - 1

        float maxWidth = BackgroundSize.sizeDelta.x;
        float width = Mathf.Lerp(0, maxWidth, percent);

        // Set Width
        BarGraph.sizeDelta = new Vector2(width, BarGraph.sizeDelta.y);
    }

    // Start is called before the first frame update
    void Start()
    {

        GameObject o = GameObject.Find("JulianStuff");
        o.GetComponent<LocalDataManager>();
      
        FactorTracker fs = LocalDataManager.fs;
        fs.updateRisks();

        Debug.Log(LocalDataManager.instance.ToString() + "fs to string");
        Debug.Log(fs.getGeneralRisk());
        int generalPercentage = fs.getGeneralRisk();
        int personalPercentage = fs.getPersonalRisk();
        


    	//make sure this is a whole number
        SetPercent(generalPercentage/100f);
    	backgroundGen = GameObject.Find("backgroundGen");
        // backgroundGenRect = backgroundGen.GetComponent<RectTransform>();
        // float genArea = (float)((generalPercentage)* backgroundGenRect.sizeDelta.x)/100f;
        // Debug.Log("Gen area.   " + genArea.ToString() + "and the " + generalPercentage.ToString() + " and the " + backgroundGenRect.sizeDelta.x);
        // backgroundGenRect.sizeDelta = new Vector2(100f, backgroundGenRect.sizeDelta.y);

        //float newWidth = (float)(personalPercentage * (double)backgroundPersRect.sizeDelta.x);
        // backgroundPers = GameObject.Find("backgroundPers");
        // backgroundPersRect = backgroundPers.GetComponent<RectTransform>();
        // float persArea = (float)((personalPercentage)* backgroundPersRect.sizeDelta.x);
        // Debug.Log("Personal area.   " + persArea.ToString() + "and the " + personalPercentage.ToString());
        // persArea = backgroundPersRect.sizeDelta.x;
        // backgroundPersRect.sizeDelta = new Vector2(persArea, backgroundPersRect.sizeDelta.y);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
