using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
//using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.DebugHelpers.Persistence;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Routing;
using Microsoft.Msagl.Routing.Rectilinear;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.MixedReality.Toolkit.Input;

public class EdgeRoutingProgram : MonoBehaviour
{
    public GameObject CameraRig;
    public GameObject GraphContainer;
    public bool Scaled = false;
    
    public GeometryGraph GeoGraph;

    private bool leftHandInited = false;
    private bool rightHandInited = false;

    private static EdgeRoutingProgram _instance;
    public static EdgeRoutingProgram Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        //DisplayGeometryGraph.SetShowFunctions();

        var graph = GeometryGraphReader.CreateFromFile("Assets/Files/channel.msagl.ellipse.geom");
        foreach (var edge in graph.Edges)
        {
            if (edge.SourcePort == null)
                edge.SourcePort = new FloatingPort(edge.Source.BoundaryCurve, edge.Source.Center);
            if (edge.TargetPort == null)
                edge.TargetPort = new FloatingPort(edge.Target.BoundaryCurve, edge.Target.Center);
        }

        InitNodes(graph);

        Vector3 camPosition = new Vector3(950, 350, -200);
        CameraRig.transform.position = camPosition;
        GraphContainer.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        Scaled = true;

        graph = GeometryGraphReader.CreateFromFile("Assets/Files/channel.msagl.ellipse.geom");
        foreach (var edge in graph.Edges)
        {
            if (edge.SourcePort == null)
                edge.SourcePort = new FloatingPort(edge.Source.BoundaryCurve, edge.Source.Center);
            if (edge.TargetPort == null)
                edge.TargetPort = new FloatingPort(edge.Target.BoundaryCurve, edge.Target.Center);
        }

        DemoEdgeRouterHelper(graph);
    }

    private void InitNodes(GeometryGraph graph)
    {
        GeoGraph = graph;
        DrawNodes.Instance.DrawShapes();
    }

    private void DemoEdgeRouterHelper(GeometryGraph graph)
    {

        //DemoRoutingFromPortToPort(graph);

        var bundlingSettings = new BundlingSettings();
        // set bundling settings to null to disable the bundling of the edges

        var router = new SplineRouter(graph, 3, 3, Math.PI / 6, bundlingSettings);
        router.Run();
#if TEST
            LayoutAlgorithmSettings.ShowGraph(graph);
#endif

        //var rectRouter = new RectilinearEdgeRouter(graph, 3, 3, true);
        //rectRouter.Run();
#if TEST
            LayoutAlgorithmSettings.ShowGraph(graph);
#endif

        GeoGraph = graph;
        DrawEdges.Instance.DrawPolylines();
    }

    private void Update()
    {
        if (!leftHandInited)
        {
            GameObject leftHandPointer = GameObject.Find("MixedRealityPlayspace/Left_ShellHandRayPointer(Clone)");
            if (leftHandPointer)
            {
                ShellHandRayPointer pointer = leftHandPointer.GetComponent<ShellHandRayPointer>();
                pointer.PointerExtent = 1000;
                pointer.DefaultPointerExtent = 1000;

                leftHandInited = true;
            }
        }

        if (!rightHandInited)
        {
            GameObject rightHandPointer = GameObject.Find("MixedRealityPlayspace/Right_ShellHandRayPointer(Clone)");
            if (rightHandPointer)
            {
                ShellHandRayPointer pointer = rightHandPointer.GetComponent<ShellHandRayPointer>();
                pointer.PointerExtent = 1000;
                pointer.DefaultPointerExtent = 1000;

                rightHandInited = true;
            }
        }
    }

    static void DemoRoutingFromPortToPort(GeometryGraph graph)
    {
        var edges = graph.Edges.ToArray();
        SetCurvesToNull(edges);
        var portRouter = new InteractiveEdgeRouter(graph.Nodes.Select(n => n.BoundaryCurve), 3, 0.65 * 3, 0);
        portRouter.Run(); //calculates the whole visibility graph, takes a long time
        DrawEdgeWithPort(edges[0], portRouter, 0.3, 0.4);
        DrawEdgeWithPort(edges[1], portRouter, 0.7, 1.5 * Math.PI);
        //I know here that my node boundary curves are ellipses so the parameters run from 0 to 2Pi
        //otherwise the curve parameter runs from curve.ParStart, to curve.ParEnd

#if TEST
            LayoutAlgorithmSettings.ShowGraph(graph);
#endif
    }

    static void SetCurvesToNull(Edge[] edges)
    {
        foreach (var edge in edges)
            edge.Curve = null;
    }

    static void DrawEdgeWithPort(Edge edge, InteractiveEdgeRouter portRouter, double par0, double par1)
    {
        var port0 = new CurvePort(edge.Source.BoundaryCurve, par0);
        var port1 = new CurvePort(edge.Target.BoundaryCurve, par1);

        SmoothedPolyline sp;
        var spline = portRouter.RouteSplineFromPortToPortWhenTheWholeGraphIsReady(port0, port1, true, out sp);

        Arrowheads.TrimSplineAndCalculateArrowheads(edge.EdgeGeometry,
                                                     edge.Source.BoundaryCurve,
                                                     edge.Target.BoundaryCurve,
                                                     spline, true);
    }
}
