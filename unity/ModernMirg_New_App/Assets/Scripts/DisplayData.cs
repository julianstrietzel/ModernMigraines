using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class DisplayData : MonoBehaviour
{
   public FactorTracker factorTracker;
   public TextMeshProUGUI textMesh;
    
    void Start(){


    	//was too out of it to remember the line Julian told me
    	string info= "Sorry Julian I didn't write down what you told me :("; //some toString func
		textMesh.text = info;
		return;
	
    }


}