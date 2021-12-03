

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class BarGraph : MonoBehaviour {

    private RectTransform graphContainer;
    public GameObject other_gameObject;

    private void Start() {
        //graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        //graphContainer = GetComponent()

        //int widthInPoints = RectTransform.Width;
        //GameObject gameObject;
        int width = 100;
        int height = 90;
        int percentage = 10;
        //gameObject.rectTransform.sizeDelta = new Vector2(width, height);

        //graphContainer.rectTransform.sizeDelta = new Vector2(width * (int)(percentage/100),height);
    }
}
