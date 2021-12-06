using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
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

      

		  textMesh.text = epoch2string(today.Key) + "" + today.Value.ToString();
		return;

    }


      private string epoch2string(int epoch) {
    return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToShortDateString();
}


}
