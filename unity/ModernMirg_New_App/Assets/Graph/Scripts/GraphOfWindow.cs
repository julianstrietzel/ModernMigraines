
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using static LocalDataManager;
using System.Linq;

public class GraphOfWindow : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();


      GameObject o = GameObject.Find("JulianStuff");
      o.GetComponent<LocalDataManager>();

      List<KeyValuePair<int, DayData>> list = LocalDataManager.dayDatas.ToList();
      List<KeyValuePair<int, DayData>> sortd = new List<KeyValuePair<int, DayData>>(list.OrderBy(kv => kv.Key));

      List<int> valueList = new List<int>(){5, 98, 56, 45, 30, 22, 17};

      int j = 1;
      for (int i = 7; i --> 0; )
        {
            if(list.Count - j > 0)
                valueList[i] = sortd[(list.Count - j)].Value.severity * 10;
            
            j = j+1;
        }

        //List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
        ShowGraph(valueList);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void ShowGraph(List<int> valueList) {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 100f;
        float xSize = 80.5f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = xSize + i * xSize -25;
            float yPosition = (valueList[i] / yMaximum) * graphHeight + 42;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1,1,1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
    }

    /*
    private void ChangeChildrenOreder() {
        int index = 0;
        for (int i = 0; i < ChildrenRectTransforms.Count; i++) {
            if (ChildrenRectTransforms[i].name == dotName) {
                ChildrenRectTransforms[i].SetSiblingIndex(index);
                index++;
            }
        }
    }
    */

}
