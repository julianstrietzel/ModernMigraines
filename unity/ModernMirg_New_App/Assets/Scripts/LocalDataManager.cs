using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Firebase.Database;

using System.Collections;
using System;

public class LocalDataManager : MonoBehaviour
{
    //use GetData with unix timestamp of a day to acces DayData Object for that day
    //U can also use the public dayDatas Dict to iterate over all the data to calc stuff (averages)

    public static LocalDataManager instance;
    public static Dictionary<int, DayData> dayDatas;
    private static string path;
    private static BinaryFormatter formatter;
    private static bool locAccess;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            path = Application.persistentDataPath + "db.file";
            setUp();
            
        }
        

    }

    private void OnApplicationQuit()
    {
        LocalDataManager.SaveLocally();
    }

    private void setUp() 
    {

        if(formatter != null)
        {
            return;
        }
        formatter = new BinaryFormatter();
        locAccess = StaticWeatherManager.FetchLocationData();

        Debug.Log("reached 1here");

        if (File.Exists(path)) //mocking allways new pull from db
        //if(false)
        {
            Debug.Log("read from local file");
            FileStream stream = File.OpenRead(path);
            dayDatas = (Dictionary<int, DayData>)formatter.Deserialize(stream);
            stream.Close();
        }
        else
        {
            dayDatas = new Dictionary<int, DayData>();
        }

        
        /**
        Debug.Log("reached 2 here");

        AddWeather(1637668800);
        AddWeather(11111111);**/
        

        foreach (KeyValuePair<int, DayData> day in LocalDataManager.dayDatas)
        {
            Debug.Log(day.Value.ToString() + " : SetUp in Local DataManager -> what is saved in local file");

        }

        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance
            .GetReference("users/TEST_USER");

        dbRef.ChildAdded += DbRef_ChildAdded;
        dbRef.ChildChanged += DbRef_ChildAdded;
    }

    private static void DbRef_ChildAdded(object sender, ChildChangedEventArgs e)
    {
        if(e.DatabaseError != null)
        {
            Debug.Log(e.DatabaseError.ToString() + "while reading messanges from DB");
            return;
        }

        int eTimeCode = int.Parse(e.Snapshot.Key);
        Dictionary<string, string> snapDataDict = new Dictionary<string, string>();

        foreach (DataSnapshot data in e.Snapshot.Children)
        {
            snapDataDict.Add(data.Key, (string)data.Value);
        }

        
        DayData day =  AddData(eTimeCode, snapDataDict);
        Debug.Log(day.timestamp + "added or changed from db in LocalDM");
        instance.AddWeather(eTimeCode);
    }

    

    public static void SaveLocally()
    {
        FileStream stream = File.OpenWrite(path);
        formatter.Serialize(stream, dayDatas);
        stream.Close();
        Debug.Log("saved locally in LDM");
    }



    public static DayData GetToday()
    {
        return GetDay(DayData.GetUnixTime());
    }



    public static DayData GetDay(int timestamp)
    {
        DayData res;
        int normTime = DayData.getNormTimestamp(timestamp);
        if (!dayDatas.TryGetValue(normTime, out res))
        {
            res = new DayData(timestamp);
            dayDatas.Add(res.timestamp, res);
        }
        return res;
    }

    public static DayData AddData(int timestamp, Dictionary<string, string> data)
    {
        timestamp = DayData.getNormTimestamp(timestamp);
        DayData day;
        if (dayDatas.TryGetValue(timestamp, out day))
        {
            day.SetData(data);
            Debug.Log(day.ToString() + " updated");

        }
        else
        {
            day = new DayData(timestamp, data);
            dayDatas.Add(timestamp, day);
            Debug.Log(day.ToString() + " added");

        }

        return day;
    }


    public static DayData AddDataForToday(Dictionary<string, string> data)
    {
        return AddData(DayData.getNormTimeToday(), data);
    }

    /**
     * can so far only fetch weather for today
     */
    public void AddWeather(int ptimestamp)
    {
        if (!locAccess)
        {
            Debug.Log("no location access");
        }
        if (GetDay(ptimestamp).HasWeather())
        {
            return;
        }
        int timestamp = DayData.getNormTimestamp(ptimestamp);
        Debug.Log("trying to fetch weather data for " + ptimestamp + "being norm " + timestamp);

        if (timestamp == DayData.getNormTimeToday())
        //if (false)
        {
            StaticWeatherManager.FetchWeatherDataNow((dict) =>
            {
                AddDataForToday(dict);


                Debug.Log("added weather data for " + timestamp + "in Callback FUnction LDM");

                string debugLog = "weather today for timestamp" + timestamp;
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    debugLog += " key:  " + pair.Key + " value: " + pair.Value;
                }

                Debug.Log(debugLog);

            }
            );
        }
        else
        {
            StartCoroutine(
                StaticWeatherManager.FetchWeatherHistory(timestamp, (dict) =>
                { 
                Debug.Log("added weather data" + timestamp);
                AddData(timestamp, dict);
                string debugLog = "weather for timestamp" + timestamp + " added via fetch historical ";
                foreach (KeyValuePair<string, string> pair in dict)
                {
                    debugLog += " key:  " + pair.Key + " value: " + pair.Value;
                }

                Debug.Log(debugLog);
            })
            );
            
            

        }

        

    }

    
}
