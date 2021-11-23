using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{
    public TMPro.TMP_Text text;
    private bool gpsEnabled;
    private double startLocLongitude;
    private double startLocLatitude;
    public Button mybutton;

    /// <summary>
    /// Remember, Start gets executed once the game starts / object instantiated.
    /// </summary>
    void Start()
    {
        // Call CheckPermission function
        StartCoroutine(CheckPermissions());

        mybutton.onClick.AddListener(update2);
        // (Coroutines are async functions)
    }

    public void update2()
    {
        startLocLatitude = UnityEngine.Input.location.lastData.latitude;
        startLocLongitude = UnityEngine.Input.location.lastData.longitude;
    }


    /// <summary>
    /// Initialize GPS service
    /// On Android
    ///     Request permission to access location
    ///     Start GPS service
    /// On iOS
    ///     (permission in settings)
    ///     Start GPS service
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckPermissions()
    {
        yield return null;

        // If we're using Android, we request GPS permission.
        // On iOS, we enable in Project Settings > Player > iOS > Location Usage Description
//#if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
        }
//#endif

        // Let's make sure we have GPS enabled.
//if UNITY_IOS || UNITY_ANDROID
        if (!UnityEngine.Input.location.isEnabledByUser)
        {
            text.text = "Location is not enabled";
            yield break;
        }
//#endif

        // Start the location services and set accuracy
        UnityEngine.Input.location.Start(1f, 1f); // 1 meter accuracy
        // It takes time for the GPS to initialize, so wait until it's ready.
        int timeout = 15;
        while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && timeout > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            timeout--;
        }
        if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
        {
            text.text = "Failed to initialize Location Service";
            yield break;
        }

        // Call our function
        text.text = "GPS Online";
        StartCoroutine(LoopEvery10Seconds());
    }


    /// <summary>
    /// Create a loop that requests GPS data every 10 seconds.
    /// If you want a different update frequency, change the 10 to another number.
    /// Or only get GPS coords once.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoopEvery10Seconds()
    {
        float updateFrequency = 1f; // 10 seconds
        yield return null;

        while (Application.isPlaying)
        {
            GetLocation();
            yield return new WaitForSecondsRealtime(updateFrequency);
        }

        UnityEngine.Input.location.Stop();
    }


    /// <summary>
    /// Get GPS Data and do something with it (just display as text)
    /// Edit this function to do something else with the data
    /// </summary>
    private void GetLocation()
    {
        // Here's all of the data Unity gives you
        double latitude = UnityEngine.Input.location.lastData.latitude;
        double longitude = UnityEngine.Input.location.lastData.longitude;
        double altitude = UnityEngine.Input.location.lastData.altitude;
        double horizontalAccuracy = UnityEngine.Input.location.lastData.horizontalAccuracy;
        double verticalAccuracy = UnityEngine.Input.location.lastData.verticalAccuracy;
        double timestamp = UnityEngine.Input.location.lastData.timestamp;
        // Now go do something cool with it!
        double distance = Math.Sqrt(Math.Pow(longitude - startLocLongitude, 2)+ Math.Pow(latitude - startLocLatitude,2));
        text.text = "Distance = " + distance;

    }
}
