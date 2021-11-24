using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Database;
using UnityEngine.UIElements;
using System;

public class ChangeSceneWithButton : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        int time = DayData.GetNormTimeToday();
        //TODO how can I read the slider value?
        //TODO This Code should only be run when changing from slider view to home
        DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;
        db.Child("lol").SetValueAsync("test");
        db.Child("users/TEST_USER").Child(time.ToString()).Child("migraine").SetValueAsync("true");
        db.Child("users/TEST_USER").Child(time.ToString()).Child("severity").SetValueAsync("2");
        db.Child("users/TEST_USER").Child(time.ToString()).Child("timestamp").SetValueAsync(time.ToString());
    }



}
