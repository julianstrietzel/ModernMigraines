using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Firebase.Database;

public class SliderValueToText : MonoBehaviour {
  public Slider sliderUI;
  private Text textSliderValue;
  public Button button;

  void Start (){
    textSliderValue = GetComponent<Text>();
    ShowSliderValue();

    //Button btn = button.GetComponent<Button>();
    //btn.onClick.AddListener(TaskOnClick);
  }

  public void ShowSliderValue () {
    
    string sliderMessage = "" + sliderUI.value;
    textSliderValue.text = sliderMessage;
  }

  public int GetSliderValue(){
    return (int)sliderUI.value;
  }

  public static int getNormTimestamp(int timestamp)
    {
        return timestamp - timestamp % (24 * 60 * 60 * 1) + (12 * 60 * 60 * 1);
    }

  public static int GetUnixTime()
    {
        return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }
    
  public static int getNormTimeToday()
    {
        return getNormTimestamp(GetUnixTime());
    }

  
  public void TaskOnClick(){
    Debug.Log ("You have clicked the button!");
    //get component LocalDataManager and DayData
    
    int timestamp = getNormTimeToday();
    
    FirebaseDatabase.DefaultInstance.RootReference.Child("users/TEST_USER/" + timestamp.ToString()).Child("migraine").SetValueAsync("true");
    FirebaseDatabase.DefaultInstance.RootReference.Child("users/TEST_USER/" + timestamp.ToString()).Child("timestamp").SetValueAsync(timestamp.ToString());
    FirebaseDatabase.DefaultInstance.RootReference.Child("users/TEST_USER/" + timestamp.ToString()).Child("severity").SetValueAsync(GetSliderValue().ToString());

    }

}