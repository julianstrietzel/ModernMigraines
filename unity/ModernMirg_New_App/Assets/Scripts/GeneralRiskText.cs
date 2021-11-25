using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class GeneralRiskText : MonoBehaviour
{
   public GameObject dailyFactor; //probably not necessary

   public FactorTracker factorTracker;
   public int generalRisk;
   public TextMeshProUGUI textMesh;
    
    void Start(){

    	//dailyFactor = GameObject.Find("DailyFactor");
    	//factorTracker = dailyFactor.Find("FactorTracker");
        //factorTracker = GetComponent<FactorTracker>();

    	
    	//factorTracker.getGeneralRisk(); 
    	//TODO
    	//Find how to get var from classes w/o getting errors
    	generalRisk = 5;



		textMesh.text = "Your general risk for getting a migraine today is " + generalRisk.ToString() + "%";
		return;
	
    }


}