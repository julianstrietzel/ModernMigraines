using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LocalDataManager;
using System.Linq;

public class DisplayHistory : MonoBehaviour
{
	public TextMeshProUGUI textMesh;
    void Start(){

      GameObject o = GameObject.Find("JulianStuff");
      o.GetComponent<LocalDataManager>();
 
      List<KeyValuePair<int, DayData>> list = LocalDataManager.dayDatas.ToList();
      List<KeyValuePair<int, DayData>> sortd = new List<KeyValuePair<int, DayData>>(list.OrderBy(kv => kv.Key));

      KeyValuePair<int, DayData> today = list[list.Count - 1];

      string output = "";
      int j = 1;
      for (int i = 7; i --> 0; )
        {
            if(list.Count - j > 0)
                output = output + "\n" + epoch2string(sortd[(list.Count - j)].Key) + " migraine: " + sortd[(list.Count - j)].Value.ToString();
            
            j = j+1;
        }
		  textMesh.text = output;

    }


      private string epoch2string(int epoch) {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToShortDateString();
      }

    }