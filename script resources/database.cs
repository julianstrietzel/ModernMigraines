using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;
        db.Child("users/new_unity").SetValueAsync("nice");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
