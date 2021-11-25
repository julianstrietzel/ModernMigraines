using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class PersonalRiskText : MonoBehaviour
{
   public GameObject dailyFactor;
   public FactorTracker factorTracker;
   public int personalRisk;
   public TextMeshProUGUI textMesh;
    
    void Start(){

    	//dailyFactor = GameObject.Find("DailyFactor");
    	//factorTracker = dailyFactor.Find("FactorTracker");
        //factorTracker = GetComponent<FactorTracker>();

    	
    	//factorTracker.getPersonalRisk(); 
    	//TODO
    	//Find how to get var from classes w/o getting errors
    	personalRisk = 5;


    	if(personalRisk == -1)
    		textMesh.text = "We're still gathering data to be able to predict your personal risk for getting a migraine";
    	else
			textMesh.text = "Your personal risk for getting a migraine today is " + personalRisk.ToString() + "%";
		return;
	
    }


}