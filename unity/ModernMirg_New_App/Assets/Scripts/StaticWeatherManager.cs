using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Collections;

public static class StaticWeatherManager
{
    public static string apiKey = "f5029e208c2c76e6b91d38f371586fc7";
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

    

    public static IEnumerator FetchWeatherDataNow(System.Action<Dictionary<string, string>> callBackFunction)
    {
        Dictionary<string, string> data_dict = new Dictionary<string, string>();

        string url = "https://" + currentWeatherApi + "lat=" + lastLocation.latitude + "&lon=" + lastLocation.longitude + "&appid=" + apiKey + "&units=metric";
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);

        yield return fetchWeatherRequest.SendWebRequest();
        
        if (fetchWeatherRequest.result != UnityWebRequest.Result.ConnectionError && fetchWeatherRequest.result != UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);

            data_dict.Add("temp_max", ((float)response["main"]["temp_max"]).ToString());
            data_dict.Add("temp_min", ((float)response["main"]["temp_mxin"]).ToString());
            data_dict.Add("humidity", ((float)response["main"]["humidity"]).ToString());
            data_dict.Add("pressure", ((float)response["main"]["pressure"]).ToString());
        }
        else
        {
            //Check and print error
            statusText = fetchWeatherRequest.error;
            
        }
        callBackFunction(data_dict);
    }

    public static IEnumerator FetchWeatherHistory(int timestamp, System.Action<Dictionary<string, string>> callBackFunction)
    {
        Debug.Log("FETCHING data");
        Dictionary<string, string> data_dict = new Dictionary<string, string>();
        //string url = "https://history.openweathermap.org/data/2.5/history/city?lat=32.71574&lon=-117.1611&type=hour&cnt=" + timestamp + "&appid=f5029e208c2c76e6b91d38f371586fc7&units=metric";
        string url = "https://" + historyWeatherApi + "lat=" + lastLocation.latitude + "&lon=" + lastLocation.longitude + "&type=hour&cnt=" + timestamp.ToString() +  "&appid=" + apiKey + "&units=metric";
        Debug.Log(url);
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        int before = DayData.GetUnixTime();
        yield return fetchWeatherRequest.SendWebRequest();

        Debug.Log("waited for weather for " + (DayData.GetUnixTime() - before) + " secs");
        
        if (fetchWeatherRequest.result != UnityWebRequest.Result.ConnectionError && fetchWeatherRequest.result != UnityWebRequest.Result.ProtocolError)
        {

            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);

            data_dict.Add("temp_max", ((float)response["main"]["temp_max"]).ToString());
            data_dict.Add("temp_min", ((float)response["main"]["temp_mxin"]).ToString());
            data_dict.Add("humidity", ((float)response["main"]["humidity"]).ToString());
            data_dict.Add("pressure", ((float)response["main"]["pressure"]).ToString());
        }
        else
        {
            //Check and print error
            statusText = fetchWeatherRequest.error;
            Debug.Log("error in FetchweatherHistory static weathermanag: " + statusText);

        }
        callBackFunction(data_dict);
       }
}

