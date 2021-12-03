using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static LocalDataManager;
 
public class RiskText : MonoBehaviour
{
   public GameObject dailyFactor;
   public FactorTracker factorTracker;
   public int generalRisk;
   public int personalRisk;
   public TextMeshProUGUI GeneralRiskText;
    
    void Start(){
    	GameObject o = GameObject.Find("JulianStuff");
      LocalDataManager ld = o.GetComponent<LocalDataManager>();
      ld = LocalDataManager.instance;
      FactorTracker fs = LocalDataManager.fs;
      fs.updateRisks();

      Debug.Log(LocalDataManager.fs.ToString());
      Debug.Log(fs.getGeneralRisk());
    	generalRisk = fs.getGeneralRisk();



		GeneralRiskText.text = "Your general risk for getting a migraine today is " + generalRisk.ToString() + "%";
		return;
	
    }


}