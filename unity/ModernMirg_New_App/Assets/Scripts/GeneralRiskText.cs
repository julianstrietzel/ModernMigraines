using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static LocalDataManager;
 
public class GeneralRiskText : MonoBehaviour
{
   public GameObject dailyFactor;
   public FactorTracker factorTracker;
   public int generalRisk;
   public TextMeshProUGUI textMesh;
    
    void Start(){
      
    	GameObject o = GameObject.Find("JulianStuff");
      o.GetComponent<LocalDataManager>();
      
      FactorTracker fs = LocalDataManager.fs;
      fs.updateRisks();

      Debug.Log(LocalDataManager.instance.ToString() + "fs to string");
      Debug.Log(fs.getGeneralRisk());
    	generalRisk = fs.getGeneralRisk();



		textMesh.text = "Your general risk for getting a migraine today is " + generalRisk.ToString() + "%";
		return;
	
    }


}