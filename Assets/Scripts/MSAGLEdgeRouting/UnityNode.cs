using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.DebugHelpers.Persistence;
using Microsoft.Msagl.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityNode : MonoBehaviour
{
    public List<Edge> NodeEdges = new List<Edge>();

    private List<Edge> startEdges = new List<Edge>();
    private List<Edge> endEdges = new List<Edge>();

    private BundlingSettings bundlingSettings;

    private Vector3 prevPosition;
    private bool edgeRouted = true;

    private bool isTiming = false;
    private float timer = 0;
    private float dwellTime = 0.5f;

    private void Start()
    {
        prevPosition = transform.position;

        foreach (var edge in NodeEdges)
        {
            if (name == edge.Source.ToString())
                startEdges.Add(edge);
            if (name == edge.Target.ToString())
                endEdges.Add(edge);
        }

        bundlingSettings = new BundlingSettings();
    }

    private void Update()
    {
        if (prevPosition != transform.position)
        {
            foreach (var edge in startEdges)
            {
                List<Vector3> straightlinePoints = new List<Vector3>();

                straightlinePoints.Add(transform.position);

                LineRenderer lineRenderer = GameObject.Find(edge.ToString()).GetComponent<LineRenderer>();
                int linePointCount = lineRenderer.positionCount;
                Vector3[] polylinePositions = new Vector3[linePointCount];
                lineRenderer.GetPositions(polylinePositions);
                straightlinePoints.Add(polylinePositions[linePointCount - 1]);

                lineRenderer.positionCount = straightlinePoints.Count;
                lineRenderer.SetPositions(straightlinePoints.ToArray());
            }

            foreach (var edge in endEdges)
            {
                List<Vector3> straightlinePoints = new List<Vector3>();

                LineRenderer lineRenderer = GameObject.Find(edge.ToString()).GetComponent<LineRenderer>();
                straightlinePoints.Add(lineRenderer.GetPosition(0));

                straightlinePoints.Add(transform.position);

                lineRenderer.positionCount = straightlinePoints.Count;
                lineRenderer.SetPositions(straightlinePoints.ToArray());
            }

            edgeRouted = false;
            isTiming = false;
            timer = 0;
        }
        else if (!edgeRouted)
        {
            if (!isTiming)
            {
                isTiming = true;
                timer += Time.deltaTime;
            }
            else
            {
                timer += Time.deltaTime;
            }

            if (timer > dwellTime)
            {
                isTiming = false;
                timer = 0;

                edgeRouted = true;
                UpdateEdgeRouting();
            }
        }

        prevPosition = transform.position;
    }

    private void UpdateEdgeRouting()
    {
        var graph = GeometryGraphReader.CreateFromFile("Assets/Files/channel.msagl.ellipse.geom");
        foreach (var edge in graph.Edges)
        {
            if (edge.SourcePort == null)
                edge.SourcePort = new FloatingPort(edge.Source.BoundaryCurve, edge.Source.Center);
            if (edge.TargetPort == null)
                edge.TargetPort = new FloatingPort(edge.Target.BoundaryCurve, edge.Target.Center);
        }

        var router = new SplineRouter(graph, 3, 3, Math.PI / 6, bundlingSettings);
        router.Run();

        foreach (var edge in GameObject.FindGameObjectsWithTag("Edge"))
            Destroy(edge);

        EdgeRoutingProgram.Instance.GeoGraph = graph;
        DrawEdges.Instance.DrawPolylines();
    }

    public void HighlightEdges()
    {
        foreach (var edge in NodeEdges)
        {
            LineRenderer lineRenderer = GameObject.Find(edge.ToString()).GetComponent<LineRenderer>();
            lineRenderer.material.color = DrawEdges.Instance.LinkHighlightColour;
        }
    }

    public void DehighlightEdges()
    {
        foreach (var edge in NodeEdges)
        {
            LineRenderer lineRenderer = GameObject.Find(edge.ToString()).GetComponent<LineRenderer>();
            lineRenderer.material.color = DrawEdges.Instance.LinkColour;
        }
    }
}
