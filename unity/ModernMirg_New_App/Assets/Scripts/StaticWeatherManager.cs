using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public static class StaticWeatherManager
{
    public static string apiKey = "282e2ea449f833e3d567ef4e2d087815";
    private static string currentWeatherApi = "api.openweathermap.org/data/2.5/weather?";
    private static string historyWeatherApi = "history.openweathermap.org/data/2.5/history/city?";
    public static bool status;
    public static string statusText;


    private static LocationInfo lastLocation;
    
    public static bool FetchLocationData()
    {

        //Mocking Location INfo

        lastLocation = new LocationInfo();
        lastLocation.latitude = 32.715736f;
        lastLocation.longitude = -117.161087f;

        return true;

        // First, check if user has location service enabled if (!Input.location.isEnabledByUser) yield break;
        // Start service before querying location
        Input.location.Start();
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            statusText = "Location Timed out";
            
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText = "Unable to determine device location";

        }
        else
        {
            //lastLocation = Input.location.lastData;
            Input.location.Stop();
            return true;
        }
        Input.location.Stop();
        return false;
    }

    

    public static Dictionary<string, string> FetchWeatherDataNow()
    {
        Dictionary<string, string> data_dict = new Dictionary<string, string>();

        string url = currentWeatherApi + "lat=" + lastLocation.latitude + "&lon=" + lastLocation.longitude + "&appid=" + apiKey + "&units=metric";
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        fetchWeatherRequest.SendWebRequest();
        int maxWait = 10;
        while(!fetchWeatherRequest.isDone && maxWait > 0)
        {
            new WaitForSeconds(1);
            maxWait--;
        }
        if (fetchWeatherRequest.result != UnityWebRequest.Result.ConnectionError && fetchWeatherRequest.result != UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);

            data_dict.Add("temp_max", ((float)response["temp_max"]).ToString());
            data_dict.Add("temp_min", ((float)response["temp_mxin"]).ToString());
            data_dict.Add("humidity", ((float)response["humidity"]).ToString());
            data_dict.Add("pressure", ((float)response["pressure"]).ToString());
        }
        else
        {
            //Check and print error
            statusText = fetchWeatherRequest.error;
            
        }
        return data_dict;
    }

    public static  Dictionary<string,string> FetchWeatherHistory(int timestamp)
    {
        Dictionary<string, string> data_dict = new Dictionary<string, string>();

        string url = historyWeatherApi + "lat=" + lastLocation.latitude + "&lon=" + lastLocation.longitude + "&type=hour&cnt=" + timestamp.ToString() +  "&appid=" + apiKey + "&units=metric";
        Debug.Log(url);
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        int before = DayData.GetUnixTime();
        fetchWeatherRequest.SendWebRequest();

        Debug.Log("waited for weather for " + (DayData.GetUnixTime() - before) + " secs");
        int maxWait = 10;
        
        while (!fetchWeatherRequest.isDone && maxWait > 0)
        {
            //await fetchWeatherRequest.ta
            new WaitForSeconds(1);
            maxWait--;
            Debug.Log("waiting");
        }
        if (fetchWeatherRequest.result != UnityWebRequest.Result.ConnectionError && fetchWeatherRequest.result != UnityWebRequest.Result.ProtocolError)
        {

            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);

            data_dict.Add("temp_max", ((float)response["temp_max"]).ToString());
            data_dict.Add("temp_min", ((float)response["temp_mxin"]).ToString());
            data_dict.Add("humidity", ((float)response["humidity"]).ToString());
            data_dict.Add("pressure", ((float)response["pressure"]).ToString());
        }
        else
        {
            //Check and print error
            statusText = fetchWeatherRequest.error;

        }
        return data_dict;
    }
}
