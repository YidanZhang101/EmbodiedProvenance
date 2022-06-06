using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeShaderProgram : MonoBehaviour
{
    private int linkPointNum = 64;

    public float LinkWidth = 0.015f;
    public Material LinkMaterial;
    public Color LinkColour;

    public ComputeShader LinkShader;
    [Range(0, 5000.0f)]
    public float springConstant;

    private int calculatePointsKernelIndex;
    private int smoothCurveKernelIndex;
    private ComputeBuffer nodesBuffer;
    private ComputeBuffer pointsBuffer;
    private ComputeBuffer sourceNodesBuffer;
    private ComputeBuffer targetNodesBuffer;

    private GameObject[] nodes;
    private List<LineRenderer> linkRenderers = new List<LineRenderer>();
    private int linkNum = 0;
    private bool initDone = false;

    private Vector3[] nodePositions;
    private Vector3[] prevNodePositions;
    private Vector3[] allLinkPoints;
    private List<int> sourceNodes = new List<int>();
    private List<int> targetNodes = new List<int>();

    public GameObject pointControllerPrefab;

    private ComputeBuffer pointControllerPositionsBuffer;
    private ComputeBuffer draggedPointsBuffer;

    private GameObject[] pointControllers;
    private Vector3[] pointControllerPositions;
    private List<int> draggedPoints = new List<int>();    

    private void Start()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");

        nodePositions = new Vector3[nodes.Length];
        prevNodePositions = new Vector3[nodes.Length];

        InitLinks();
        InitShaders();
        initDone = true;
    }

    private void Update()
    {
        if (initDone)
            UpdateLinks();
    }

    private void InitLinks()
    {
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            for ( int j = i + 1; j < nodes.Length; j++)
            {
                GameObject link = new GameObject("Link" + i + j);

                LineRenderer lineRenderer = link.AddComponent<LineRenderer>();
                lineRenderer.positionCount = linkPointNum;
                lineRenderer.startWidth = LinkWidth;
                lineRenderer.endWidth = LinkWidth;

                Material[] materialArray = new Material[1];
                LinkMaterial.color = LinkColour;
                materialArray[0] = LinkMaterial;
                lineRenderer.materials = materialArray;

                linkRenderers.Add(lineRenderer);

                sourceNodes.Add(i);
                targetNodes.Add(j);

                linkNum++;
            }
        }

        allLinkPoints = Enumerable.Repeat(new Vector3(0, 0, 0), linkPointNum * linkNum).ToArray();
        pointControllers = Enumerable.Repeat(0, linkPointNum * linkNum).Select(h => Instantiate(pointControllerPrefab)).ToArray();
        pointControllerPositions = Enumerable.Repeat(new Vector3(0, 0, 0), linkPointNum * linkNum).ToArray();
    }

    private void InitShaders()
    {
        calculatePointsKernelIndex = LinkShader.FindKernel("CalculatePoints");
        
        nodesBuffer = new ComputeBuffer(nodePositions.Length, 3 * sizeof(float));
        nodesBuffer.SetData(nodePositions);
        pointsBuffer = new ComputeBuffer(allLinkPoints.Length, 3 * sizeof(float));
        pointsBuffer.SetData(allLinkPoints);
        sourceNodesBuffer = new ComputeBuffer(sourceNodes.Count, sizeof(int));
        sourceNodesBuffer.SetData(sourceNodes);
        targetNodesBuffer = new ComputeBuffer(targetNodes.Count, sizeof(int));
        targetNodesBuffer.SetData(targetNodes);

        pointControllerPositionsBuffer = new ComputeBuffer(allLinkPoints.Length, 3 * sizeof(float));
        pointControllerPositionsBuffer.SetData(pointControllerPositions);

        LinkShader.SetBuffer(calculatePointsKernelIndex, "nodes", nodesBuffer);
        LinkShader.SetBuffer(calculatePointsKernelIndex, "points", pointsBuffer);
        LinkShader.SetBuffer(calculatePointsKernelIndex, "sourceNodes", sourceNodesBuffer);
        LinkShader.SetBuffer(calculatePointsKernelIndex, "targetNodes", targetNodesBuffer);

        LinkShader.SetBuffer(calculatePointsKernelIndex, "pointControllerPositions", pointControllerPositionsBuffer);

        smoothCurveKernelIndex = LinkShader.FindKernel("SmoothCurve");

        LinkShader.SetBuffer(smoothCurveKernelIndex, "points", pointsBuffer);

        LinkShader.SetFloat("nodeRadius", nodes[0].transform.localScale.x);
        LinkShader.SetFloat("k", springConstant);
    }

    private void UpdateLinks()
    {
        for (int i = 0; i < pointControllers.Length; i++)
        {
            pointControllerPositions[i] = pointControllers[i].transform.position;

            if (pointControllers[i].GetComponent<PointController>().dragged)
            {
                draggedPoints.Add(i);
            }
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            nodePositions[i] = nodes[i].transform.position;

            if (nodePositions[i] != prevNodePositions[i])
            {
                foreach (GameObject pointController in pointControllers)
                    pointController.GetComponent<PointController>().dragged = false;
            }
            prevNodePositions[i] = nodePositions[i];
        }

        nodesBuffer.SetData(nodePositions);
        LinkShader.SetBuffer(calculatePointsKernelIndex, "nodes", nodesBuffer);

        pointControllerPositionsBuffer.SetData(pointControllerPositions);
        LinkShader.SetBuffer(calculatePointsKernelIndex, "pointControllerPositions", pointControllerPositionsBuffer);

        if (draggedPoints.Count > 0)
        {
            draggedPointsBuffer = new ComputeBuffer(draggedPoints.Count, sizeof(int));
            draggedPointsBuffer.SetData(draggedPoints);

            LinkShader.SetBuffer(calculatePointsKernelIndex, "draggedPoints", draggedPointsBuffer);
        }
        else
        {
            draggedPointsBuffer = new ComputeBuffer(1, sizeof(int));
            int[] dummyPoint = new int[1];
            dummyPoint[0] = 0;
            draggedPointsBuffer.SetData(dummyPoint);

            LinkShader.SetBuffer(calculatePointsKernelIndex, "draggedPoints", draggedPointsBuffer);
        }

        LinkShader.Dispatch(calculatePointsKernelIndex, linkNum, 1, 1);

        LinkShader.Dispatch(smoothCurveKernelIndex, linkNum, 1, 1);

        DrawLinks();

        draggedPointsBuffer.Release();
        draggedPointsBuffer = null;
    }

    private void DrawLinks()
    {
        pointsBuffer.GetData(allLinkPoints);

        for (int i = 0; i < linkRenderers.Count; i++)
        {
            Vector3[] linkPoints = new Vector3[linkPointNum];
            for (int j = 0; j < linkPointNum; j++)
            {
                if (!pointControllers[i * linkPointNum + j].GetComponent<PointController>().dragged)
                {
                    linkPoints[j] = allLinkPoints[i * linkPointNum + j];
                    pointControllers[i * linkPointNum + j].transform.position = linkPoints[j];
                }
                else
                {
                    linkPoints[j] = pointControllers[i * linkPointNum + j].transform.position;
                }
                    
            }
            linkRenderers[i].SetPositions(linkPoints);
        }
    }

    private void OnDisable()
    {
        nodesBuffer.Release();
        nodesBuffer = null;

        pointsBuffer.Release();
        pointsBuffer = null;

        sourceNodesBuffer.Release();
        sourceNodesBuffer = null;

        targetNodesBuffer.Release();
        targetNodesBuffer = null;

        pointControllerPositionsBuffer.Release();
        pointControllerPositionsBuffer = null;
    }
}
