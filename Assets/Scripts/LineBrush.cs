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

    private List<Vector3> positionList = new List<Vector3>();

    private List<LineRenderer> lineList = new List<LineRenderer>();

    public Vector3 screenOffset;
    public float width = 1;
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
        Debug.Log("spawn line");
        GameObject lineObj = GetItem("line");
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        lineRenderer.transform.parent = lineParent;
        lineRenderer.transform.position = position;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineList.Add(lineRenderer);
        return lineRenderer;
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
                Debug.Log("touch down");
                isTouchReleased = false;
                elapsedTimeTouchRelease = 0;
                //ClearLine();
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                LineRenderer lineRenderer = SpawnLine(worldPos);
                worldPos.z = 0;
                lineRenderer.positionCount = 1;
                lineRenderer.SetPosition(0, worldPos);

                positionList.Add(worldPos);
                ParticleSystem touchEffect = FootballController.Instance.touchEffect;
                touchEffect.transform.position = worldPos;
                touchEffect.Play();

            }

            if (elapsedFrameTime >= frame / 120f)
            {
                if (Input.GetMouseButton(0))
                {
                    if (positionList.Count < 5000)
                    {
                        Vector3 mousePos = Input.mousePosition;
                        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                        worldPos.x = float.Parse(worldPos.x.ToString("0.000"));
                        worldPos.y = float.Parse(worldPos.y.ToString("0.000"));
                        worldPos.z = 0;
                        Debug.Log("linelist count : " + lineList.Count, this);
                        lineList[lineList.Count - 1].positionCount += 1;
                        lineList[lineList.Count - 1].SetPosition(lineList[lineList.Count - 1].positionCount - 1, worldPos);

                        positionList.Add(worldPos);
                        Transform touchEffect = FootballController.Instance.touchEffect.transform;
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
                positionList.Clear();
                FootballController.Instance.touchEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
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
        for (int i = 0; i < positionList.Count; i++)
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
                    if (GameManager.Instance.controlType == GameManager.ControlType.SHAKEDRAW && positionList.Count > 0)
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
        for (int i = 0; i < points.Count; i++)
        {
            lineList[index].positionCount = points.Count;
            lineList[index].SetPosition(i, points[i] / 2 + screenOffset);
        }
    }
}
