using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLinesHandler : MonoBehaviour
{
    [SerializeField] private int _beginningLines = 3;
    [SerializeField] private LineRenderer _linePrefab;

    private LineRenderer _activeLine = null;
    private List<LineRenderer> _lines = new List<LineRenderer>();

    #region Static Data Members

    private static TargetingLinesHandler _Instance = null;

    public static TargetingLinesHandler Instance
    {
        get
        {
            if (!_Instance)
                _Instance = FindObjectOfType<TargetingLinesHandler>();
            return _Instance;
        }
    }

    #endregion

    private void Awake()
    {
        int count = FindObjectsOfType<TargetingLinesHandler>().Length;
        if (count > 1)
            Destroy(gameObject);
        else
            _Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _beginningLines; i++)
        {
            LineRenderer line = Instantiate(_linePrefab, transform);
            line.gameObject.SetActive(false);
            _lines.Add(line);
        }
    }

    public void EnableLine(Vector3 startPosition)
    {
        _activeLine = null;
        foreach (LineRenderer line in _lines)
        {
            if (!line.gameObject.activeInHierarchy)
            {
                Vector3[] linePositions = new Vector3[2];
                linePositions[0] = startPosition;
                line.SetPositions(linePositions);
                line.gameObject.SetActive(true);
                _activeLine = line;
                break;
            }
        }

        if (!_activeLine)
        {
            LineRenderer line = Instantiate(_linePrefab, transform);
            _lines.Add(line);
            _activeLine = line;
        }
    }

    public void SetLine(Vector3 endPosition)
    {
        if (_activeLine)
        {
            int count = _activeLine.positionCount;
            Vector3[] linePositions = new Vector3[count];
            _activeLine.GetPositions(linePositions);
            linePositions[count - 1] = endPosition;
            _activeLine.SetPositions(linePositions);
        }
    }

    public void DisableLines()
    {
        int n = _lines.Count;
        if (n > _beginningLines) 
        {
            for (int i = _beginningLines; i < n; i++)
                Destroy(_lines[i].gameObject);

            _lines.RemoveRange(_beginningLines, n);
        }

        foreach (LineRenderer line in _lines)
            line.gameObject.SetActive(false);

        _activeLine = null;
    }
}
