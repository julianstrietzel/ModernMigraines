using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using static LocalDataManager;
using System.Linq;
 
public class DisplayData : MonoBehaviour
{

   public DayData today;
   public TextMeshProUGUI textMesh;

    void Start(){

      GameObject o = GameObject.Find("JulianStuff");
      o.GetComponent<LocalDataManager>();
 
      List<KeyValuePair<int, DayData>> list = LocalDataManager.dayDatas.ToList();
      List<KeyValuePair<int, DayData>> sortd = new List<KeyValuePair<int, DayData>>(list.OrderBy(kv => kv.Key));

      KeyValuePair<int, DayData> today = list[list.Count - 1];

		  textMesh.text = today.Value.ToString();
		return;

    }


}
