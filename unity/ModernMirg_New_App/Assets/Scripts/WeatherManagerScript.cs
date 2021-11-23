using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;
using System;

/**
 * This class will update the weather data of today everytime  
 */
public class WeatherManager : MonoBehaviour
{
    public string apiKey = "282e2ea449f833e3d567ef4e2d087815";
    public string currentWeatherApi = "api.openweathermap.org/data/2.5/weather?";
    [Header("UI")] public TextMeshProUGUI statusText;
    public TextMeshProUGUI location;
    public TextMeshProUGUI mainWeather;
    public TextMeshProUGUI description;
    public TextMeshProUGUI temp;
    public TextMeshProUGUI feels_like;
    public TextMeshProUGUI temp_min;
    public TextMeshProUGUI temp_max;
    public TextMeshProUGUI pressure;
    public TextMeshProUGUI humidity;
    public TextMeshProUGUI windspeed;

    public float vtemp_min = float.NaN;
    public float vtemp_max = float.NaN;
    public float vhumidity = float.NaN;
    public float vpressure = float.NaN;

    private LocationInfo lastLocation;
    void Start() { StartCoroutine(FetchLocationData()); }
    private IEnumerator FetchLocationData()
    {
        // First, check if user has location service enabled if (!Input.location.isEnabledByUser) yield break;
        // Start service before querying location
        Input.location.Start();
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            statusText.text = "Location Timed out";
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            //lastLocation = Input.location.lastData;
            UpdateWeatherData();
        }
        Input.location.Stop();
    }

    private void UpdateWeatherData()
    {
        StartCoroutine(FetchWeatherDataFromApi(lastLocation.latitude.ToString(), lastLocation.longitude.ToString()));
    }

    private IEnumerator FetchWeatherDataFromApi(string latitude, string longitude)
    {
        string url = currentWeatherApi + "lat=" + latitude + "&lon=" + longitude + "&appid=" + apiKey + "&units=metric";
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        yield return fetchWeatherRequest.SendWebRequest();
        if (fetchWeatherRequest.result != UnityWebRequest.Result.ConnectionError && fetchWeatherRequest.result != UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);
            location.text = response["name"];
            mainWeather.text = response["weather"][0]["main"];
            description.text = response["weather"][0]["description"];
            temp.text = response["main"]["temp"] + " C";
            feels_like.text = "Feels like " + response["main"]["feels_like"] + " C";
            temp_min.text = "Min is " + response["main"]["temp_min"] + " C";
            temp_max.text = "Max is " + response["main"]["temp_max"] + " C";
            pressure.text = "Pressure is " + response["main"]["pressure"] + " Pa";
            humidity.text = response["main"]["humidity"] + " % Humidity";
            windspeed.text = "Windspeed is " + response["wind"]["speed"] + " Km/h";

            vtemp_max = (float) response["temp_max"];
            vtemp_min = (float) response["temp_min"];
            vhumidity = (float) response["humidity"];
            vpressure = (float) response["pressure"];

            Dictionary<string, string> data_for_DB = new Dictionary<string, string>();
            data_for_DB.Add("temp_max", vtemp_max.ToString());
            data_for_DB.Add("temp_min", vtemp_min.ToString());
            data_for_DB.Add("humidity", vhumidity.ToString());
            data_for_DB.Add("pressure", vpressure.ToString());
            LocalDataManager.AddDataForToday(data_for_DB);
        }
        else
        {
            //Check and print error
            statusText.text = fetchWeatherRequest.error;
        }



    }
}