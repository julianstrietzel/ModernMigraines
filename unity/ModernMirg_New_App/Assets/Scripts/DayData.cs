using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DayData
{
    public int timestamp;
    public bool hasWeather;

    // Health Data
    public int stepcount;
    //TODO add sleep and get health data from apple

    public bool migraine;
    public int severity; //only if migraine
    public bool symptoms; // only if not migraine

    //Weather Data
    public float temp_min = float.NegativeInfinity; //celsius
    public float temp_max = float.NegativeInfinity;
    public float pressure = float.NegativeInfinity; //pascal
    public float humidity = float.NegativeInfinity; //%



    public DayData()
    {
        this.timestamp = GetNormTimeToday();
    }


    public DayData(int unixTimestemp)
    {
        this.timestamp = GetNormTimestamp(unixTimestemp);
    }

    public DayData(Dictionary<string, string> data)
    {
        this.timestamp = GetNormTimestamp(GetUnixTime());
        SetData(data);
    }

    public DayData(int unixTimestemp, Dictionary<string, string> data)
    {
        this.timestamp = GetNormTimestamp(unixTimestemp);
        SetData(data);
    }

    public void SetData(Dictionary<string, string> data)
    {
        string restimestamp;
        int resIntTimestamp;
        if(data.TryGetValue("timestamp", out restimestamp)) {
            int.TryParse(restimestamp, out resIntTimestamp);
            timestamp = GetNormTimestamp(timestamp);
        }
        string resstepcount;
        if (data.TryGetValue("stepcount", out resstepcount))
        {
            int.TryParse(resstepcount, out stepcount);
        }


        string restemp_min;
        if (data.TryGetValue("temp_min", out restemp_min))
        {
            float.TryParse(restemp_min, out temp_min);
            hasWeather = true;
        }
        string restemp_max;
        if (data.TryGetValue("temp_max", out restemp_max))
        {
            float.TryParse(restemp_max, out temp_max);
        }
        string reshumidity;
        if (data.TryGetValue("humidity", out reshumidity))
        {
            float.TryParse(reshumidity, out humidity);
        }
        string respressure;
        if (data.TryGetValue("pressure", out respressure))
        {
            float.TryParse(respressure, out pressure);
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

    public static int GetNormTimestamp(int timestamp)
    {
        return timestamp - timestamp % (24 * 60 * 60 * 1) + (12 * 60 * 60 * 1);
    }

    public static int GetNormTimeToday()
    {
        return GetNormTimestamp(GetUnixTime());
    }

    public static int GetLocalTime()
    {
        return GetUnixTime() - 8 * 60 * 60;
    }

    public static int GetUnixTime()
    {
        return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public static int GetUnixTimeDaysAgo(int days)
    {
        return GetNormTimestamp(GetUnixTime()) - days * (24 * 60 * 60);
    }

    public bool Equals(DayData other)
    {
        return (GetNormTimestamp(this.timestamp) == GetNormTimestamp(other.timestamp));
    }

    public
    override string ToString()
    {
        string output = timestamp + ": ";
        if(stepcount != 0)
        {
            output += " stepcount: " + stepcount;
        }

        if (!(float.IsNegativeInfinity(temp_min)))
        {
            output += " tempmin: " + temp_min;
        }
        if (!(float.IsNegativeInfinity(temp_max)))
        {
            output += " tempmax: " + temp_max;
        }
        if (!(float.IsNegativeInfinity(humidity)))
        {
            output += " humididty: " + humidity;
        }
        if (!(float.IsNegativeInfinity(pressure)))
        {
            output += " pressure: " + pressure;
        }

        output += " migraine: " + migraine;
        if (migraine)
        {
            output += " severity: " + severity;
        } else
        {
            output += " symptoms" + symptoms;
        }
        return output;
    }

    public bool HasWeather()
    {
        return hasWeather;
    }


}
