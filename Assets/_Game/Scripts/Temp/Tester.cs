using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tester : MonoBehaviour
{
    [SerializeField] private Canvas _baseCanvas;
    [SerializeField] private Canvas _testCanvas;

    private RectTransform _testRect;
    private RectTransform _testOgRect;

    private void Awake()
    {
        _testRect = _testCanvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) 
            Change();
        if (Input.GetKeyDown(KeyCode.R)) 
            Revert();
    }

    private void Change()
    {
        PrintRect(_testRect);

        _testCanvas.transform.SetParent(_baseCanvas.transform);
        _testCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _testRect.position = Vector3.one;
        _testRect.sizeDelta = new Vector2(350, 345);
    }

    private void Revert()
    {
        _testCanvas.transform.SetParent(null);
        _testCanvas.renderMode = RenderMode.WorldSpace;
        _testRect.position = new Vector3(0, -2, 235);
        _testRect.sizeDelta = new Vector2(420, 69);
    }

    private void PrintRect(RectTransform rect)
    {
        print($"Pos: {rect.position}\n" +
              $"Width: {rect.rect.width} Height: {rect.rect.height}\n" +
              $"Rotation: {rect.rotation}\nScale: {rect.localScale}");
    }
}
