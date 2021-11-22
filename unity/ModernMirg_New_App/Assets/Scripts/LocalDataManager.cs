using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine;
using Firebase.Database;

public static class LocalDataManager 
{
    public static Dictionary<int, DayData> dayDatas;
    private static string path = Application.persistentDataPath + "db.file";
    private static BinaryFormatter formatter;

    public static void SetUpLocalDataManager()
    {
        if(formatter != null)
        {
            return;
        }
        formatter = new BinaryFormatter();
        

        if (File.Exists(path))
        {
            Debug.Log("read from local file");
            FileStream stream = File.OpenRead(path);
            dayDatas = (Dictionary<int, DayData>)formatter.Deserialize(stream);
            stream.Close();
        } else
        {
            dayDatas = new Dictionary<int, DayData>();
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

        DayData day =  addData(eTimeCode, snapDataDict);

    }

    

    public static void saveLocally()
    {
        FileStream stream = File.OpenWrite(path);
        formatter.Serialize(stream, dayDatas);
        stream.Close();
        
    }



    public static DayData getToday()
    {
        return getDay(DayData.GetUnixTime());
    }



    public static DayData getDay(int timestamp)
    {
        DayData res;
        int normTime = DayData.getNormTimestamp(timestamp);
        if (!dayDatas.TryGetValue(normTime, out res))
        {
            res = new DayData();
            dayDatas.Add(res.timestamp, res);
        }
        return res;
    }

    public static DayData addData(int timestamp, Dictionary<string, string> data)
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

    public static DayData addDataForToday(Dictionary<string, string> data)
    {
        return addData(DayData.getNormTimeToday(), data);
    }
}
