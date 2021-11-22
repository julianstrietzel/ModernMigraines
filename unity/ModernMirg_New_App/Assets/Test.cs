using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LocalDataManager.SetUpLocalDataManager();
        foreach (KeyValuePair<int, DayData> day in LocalDataManager.dayDatas)
        {
            print(day.Value.ToString());

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        LocalDataManager.saveLocally();
    }
}
