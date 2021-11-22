using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DayData
{
    public int timestamp;

    // Health Data
    public int stepcount;
    public bool migraine;
    public int severity; //only if migraine
    public bool symptoms; // only if not migraine

    //Weather Data
    public float temp;
    public float humidity;

    public DayData()
    {
        this.timestamp = getNormTimeToday();
    }


    public DayData(int unixTimestemp)
    {
        this.timestamp = getNormTimestamp(unixTimestemp);
    }

    public DayData(Dictionary<string, string> data)
    {
        this.timestamp = getNormTimestamp(GetUnixTime());
        SetData(data);
    }

    public DayData(int unixTimestemp, Dictionary<string, string> data)
    {
        this.timestamp = getNormTimestamp(unixTimestemp);
        SetData(data);
    }

    public void SetData(Dictionary<string, string> data)
    {
        string restimestamp;
        int resIntTimestamp;
        if(data.TryGetValue("timestamp", out restimestamp)) {
            int.TryParse(restimestamp, out resIntTimestamp);
            timestamp = getNormTimestamp(timestamp);
        }
        string resstepcount;
        if (data.TryGetValue("stepcount", out resstepcount))
        {
            int.TryParse(resstepcount, out stepcount);
        }
        string restemp;
        if (data.TryGetValue("temp", out restemp))
        {
            float.TryParse(restemp, out temp);
        }
        string reshumidity;
        if (data.TryGetValue("humidity", out reshumidity))
        {
            float.TryParse(reshumidity, out humidity);
        }
        string resmigraine;
        if (data.TryGetValue("migraine", out resmigraine))
        {
            bool.TryParse(resmigraine, out migraine);
        }
        string resseverity;
        if (data.TryGetValue("severity", out resseverity))
        {
            int.TryParse(resseverity, out severity);
        }
        string ressymptoms;
        if (data.TryGetValue("symptoms", out ressymptoms))
        {
            bool.TryParse(ressymptoms, out symptoms);
        }


    }

    public static int getNormTimestamp(int timestamp)
    {
        return timestamp - timestamp % (24 * 60 * 60 * 1) + (12 * 60 * 60 * 1);
    }

    public static int getNormTimeToday()
    {
        return getNormTimestamp(GetUnixTime());
    }

    public static int GetUnixTime()
    {
        return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public bool equals(DayData other)
    {
        return (getNormTimestamp(this.timestamp) == getNormTimestamp(other.timestamp));
    }

    override
    public string ToString()
    {
        string output = timestamp + ": ";
        if(stepcount != 0)
        {
            output += "stepcount: " + stepcount;
        }

        if (temp != 0)
        {
            output += "temp: " + temp;
        }

        output += "migraine: " + migraine;
        if (migraine)
        {
            output += "severity: " + severity;
        } else
        {
            output += "symptoms" + symptoms;
        }
        return output;
    }


}
