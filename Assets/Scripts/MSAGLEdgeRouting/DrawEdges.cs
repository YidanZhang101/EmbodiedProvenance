using Microsoft.Msagl.Core.DataStructures;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Routing.Spline.Bundling;
using Microsoft.Msagl.Routing.Visibility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawEdges : MonoBehaviour
{
    public float LinkWidth = 0.015f;
    public Material LinkMaterial;
    public Color LinkColour;
    public Color LinkHighlightColour;

    public List<Metroline> MetroEdges = new List<Metroline>();

    private static DrawEdges _instance;
    public static DrawEdges Instance { get { return _instance; } }

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

    public void DrawPolylines()
    {
        List<Edge> graphEdges = EdgeRoutingProgram.Instance.GeoGraph.Edges.ToList();

        for (int i = 0; i < MetroEdges.Count; i++)
        {
            List<Vector3> polylinePoints = new List<Vector3>();
            foreach (var point in MetroEdges[i].Polyline.PolylinePoints)
            {
                Vector3 position = new Vector3((float)point.Point.X, (float)point.Point.Y, 0);
                polylinePoints.Add(position);
            }

            GameObject polyline = new GameObject(graphEdges[i].ToString());
            polyline.tag = "Edge";
            LineRenderer lineRenderer = polyline.AddComponent<LineRenderer>();

            lineRenderer.positionCount = polylinePoints.Count;
            lineRenderer.startWidth = LinkWidth;
            lineRenderer.endWidth = LinkWidth;

            Material[] materialArray = new Material[1];
            LinkMaterial.color = LinkColour;
            materialArray[0] = LinkMaterial;
            lineRenderer.materials = materialArray;

            lineRenderer.SetPositions(polylinePoints.ToArray());

            polyline.transform.SetParent(EdgeRoutingProgram.Instance.GraphContainer.transform);
        }
    }
}
