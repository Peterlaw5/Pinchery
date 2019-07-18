using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour {

    public RectTransform panel;
    public RectTransform[] bttn;
    public RectTransform center;
    public int startPanel = 1;

    private float[] distance;
    private bool dragging = false;
    private int bttnDistance;
    private int minButtonNum;
    private bool targetNearestButton = true;

    void Start()
    {
        int bttnLenght = bttn.Length;
        distance = new float[bttnLenght];
        bttnDistance = (int)Mathf.Abs(bttn[1].anchoredPosition.x- bttn[0].anchoredPosition.x);

        panel.anchoredPosition = new Vector2((startPanel - 1) * -bttnDistance,0f);
        
    }

    void Update()
    {
        for (int i=0 ; i < bttn.Length ; ++i)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - bttn[i].transform.position.x);
        }

        if (targetNearestButton)
        {
            float minDistance = Mathf.Min(distance);

            for (int j = 0; j < bttn.Length; ++j)
            {
                if (minDistance == distance[j])
                {
                    minButtonNum = j;
                }
            }
        }

        if (!dragging)
        {
            LerpToBttn(minButtonNum * -bttnDistance);
        }
    }

    void LerpToBttn(int position)
    {
        float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 10f);
        Vector2 newPosition = new Vector2(newX, panel.anchoredPosition.y);

        panel.anchoredPosition = newPosition;
    }

    public void StartDrag()
    {
        dragging = true;
        targetNearestButton = true;
    }

    public void EndDrag()
    {
        dragging = false;
    }

    public void GoToPanel (int panelIndex)
    {
        targetNearestButton = false;
        minButtonNum = panelIndex - 1;
    }
}
