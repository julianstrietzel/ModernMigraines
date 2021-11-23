using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitDB : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LocalDataManager.SetUpLocalDataManager();
        foreach (KeyValuePair<int, DayData> day in LocalDataManager.dayDatas)
        {
            Debug.Log(day.Value.ToString() + "at initDB");

        }
    }

    // Update is called once per frame
    void Update()
    {
        //LocalDataManager.GetToday();
        //LocalDataManager.GetDay(1237634);
    }

    private void OnApplicationQuit()
    {
        LocalDataManager.SaveLocally();
    }
}
