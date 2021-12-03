using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static LocalDataManager;
 
public class PersonalRiskText : MonoBehaviour
{
   public GameObject dailyFactor;
   public FactorTracker factorTracker;
   public int personalRisk;
   public TextMeshProUGUI textMesh;
    
    void Start(){
      GameObject o = GameObject.Find("JulianStuff");
      o.GetComponent<LocalDataManager>();
      
      FactorTracker fs = LocalDataManager.fs;
      fs.updateRisks();

      Debug.Log(LocalDataManager.fs.ToString());
      Debug.Log(fs.getPersonalRisk());
      personalRisk = fs.getPersonalRisk();

      //Debug.Log(LocalDataManager.fs.ToString());
      //Debug.Log(fs.getPersonalRisk());
      personalRisk = fs.getPersonalRisk();
      if(personalRisk > 0)
        textMesh.text = "Your personal risk for getting a migraine today is " + personalRisk.ToString() + " %";
      else
        textMesh.text = "We're still getting enough data to be able to predict your migraine";
    return;
  
    }
    

}