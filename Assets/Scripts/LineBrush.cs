using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBrush : SimplePooling
{
    //private LineRenderer lineRenderer;

    private float lineThreshold = 2f;
    private float elapsedTimeTouchRelease = 0f;
    private bool isTouchReleased = false;
    private int frame = 1;
    private float elapsedFrameTime;

    private List<Vector3> prePositionList = new List<Vector3>();
    private Vector3[] positionList;

    private List<LineRenderer> lineList = new List<LineRenderer>();

    public Vector3 screenOffset;
    public float width = 1;
    public float lineSegmentSize = 0.25f;
    public Transform lineParent;

    protected override void Start()
    {
        base.Start();
        Input.multiTouchEnabled = false;
        //lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.startWidth = width;
        //lineRenderer.endWidth = width;
        StartCoroutine(SyncLinePosition(frame));
        elapsedFrameTime = 0f;

        EventManager.onClearLine += ClearLine;
        //GameManager.Instance.controlType = GameManager.ControlType.SHAKEDRAW;
    }

    private LineRenderer SpawnLine(Vector3 position)
    {
        //Debug.Log("spawn line");
        GameObject lineObj = GetItem("line");
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        lineRenderer.transform.parent = lineParent;
        lineRenderer.transform.position = position;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineList.Add(lineRenderer);
        return lineRenderer;
    }

    private List<Vector3> GetLinePositions(LineRenderer line)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < line.positionCount; i++)
        {
            positionList.Add(line.GetPosition(i));
        }
        return positionList;
    }

    private Vector3[] Smoothing()
    {
        LineRenderer line = lineList[lineList.Count - 1];
        Vector3[] smoothedPoints = LineSmoother.SmoothLine(prePositionList.ToArray(), lineSegmentSize);

        //set line settings
        line.positionCount = smoothedPoints.Length;
        line.SetPositions(smoothedPoints);
        line.startWidth = width;
        line.endWidth = width;
        //Debug.LogWarning("line position count : " + line.positionCount);
        return smoothedPoints;
    }

    private void OnDestroy()
    {
        EventManager.onClearLine -= ClearLine;
    }

    private void Update()
    {
        if (EventManager.onGetIsConnected != null)
        {
            if (!EventManager.onGetIsConnected.Invoke())
            {
                return;
            }
        }
        if (GameManager.Instance.IsServer)
        {
            CheckElapsedTime();
        }
        else if(GameManager.Instance.controlType == GameManager.ControlType.SHAKEDRAW)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("touch down");
                isTouchReleased = false;
                elapsedTimeTouchRelease = 0;
                //ClearLine();
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

                LineRenderer lineRenderer = SpawnLine(worldPos);
                worldPos.z = 0;
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, worldPos);

                prePositionList.Add(worldPos);
                //positionList.Add(worldPos);

                ParticleSystem touchEffect = ((FootballController)GameMatchController.Instance).touchEffect;
                touchEffect.transform.position = worldPos;
                touchEffect.Play();

            }

            if (elapsedFrameTime >= frame / 120f)
            {
                if (Input.GetMouseButton(0))
                {
                    //if (positionList.Length < 5000)
                    {
                        Vector3 mousePos = Input.mousePosition;
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                        worldPos.z = 0;

                        //Debug.Log("linelist count : " + lineList.Count, this);

                        prePositionList.Add(worldPos);

                        positionList = Smoothing();

                        //lineList[lineList.Count - 1].positionCount += 1;
                        //lineList[lineList.Count - 1].SetPosition(lineList[lineList.Count - 1].positionCount - 1, worldPos);

                        //positionList.Add(worldPos);
                        Transform touchEffect = ((FootballController)GameMatchController.Instance).touchEffect.transform;
                        touchEffect.position = worldPos;
                    }
                }
                elapsedFrameTime = 0f;
            }
            else
            {
                elapsedFrameTime += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(0))
            {
                //Reflect();
                isTouchReleased = true;
                positionList = null;
                prePositionList.Clear();
                ((FootballController)GameMatchController.Instance).touchEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            //if (isTouchReleased)
            //{
            //    CheckElapsedTime();
            //}
        }
    }

    public void Reflect()
    {
        Vector3 offset = new Vector3(2f, 0f, 0f);
        LineRenderer line = SpawnLine(Vector3.zero);
        line.startWidth = width / 2f;
        line.endWidth = width / 2f;
        for (int i = 0; i < positionList.Length; i++)
        {
            line.positionCount += 1;
            line.SetPosition(line.positionCount - 1, positionList[i] / 2 + offset);
        }
    }

    private void CheckElapsedTime()
    {
        //elapsedTimeTouchRelease += Time.deltaTime;

        //if (elapsedTimeTouchRelease > lineThreshold)
        //{
        //    elapsedTimeTouchRelease = 0f;
        //    ClearLine();
        //}
    }

    private IEnumerator SyncLinePosition(float rate)
    {
        while (true)
        {
            if (!isTouchReleased)
            {
                yield return null;
                if (!GameManager.Instance.IsServer)
                {
                    if (GameManager.Instance.controlType == GameManager.ControlType.SHAKEDRAW && positionList != null)
                    {
                        EventManager.onDrawingLine?.Invoke(GameManager.Instance.GetClientId(), lineList.Count - 1, positionList);
                    }
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void ClearLine()
    {
        for (int i = 0; i < lineList.Count; i++)
        {
            lineList[i].positionCount = 0;
            lineList[i].gameObject.SetActive(false);
        }

        //lineRenderer.positionCount = 0;
        //positionList.Clear();
    }

    public void SetLine(int index, List<Vector3> points)
    {
        if (index >= lineList.Count)
        {
            SpawnLine(points[0] + screenOffset);
        }
        elapsedTimeTouchRelease = 0f;
        if (lineList.Count > index)
        {
            for (int i = 0; i < points.Count; i++)
            {
                lineList[index].startWidth = width / 2;
                lineList[index].endWidth = width / 2;
                lineList[index].positionCount = points.Count;
                lineList[index].SetPosition(i, points[i] / 2 + screenOffset);
            }
        }
    }
}
