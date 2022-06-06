using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawNodes : MonoBehaviour
{
    public float NodeDepth = 1;
    public GameObject SpherePrefab;
    public GameObject DiamondPrefab;

    private static DrawNodes _instance;
    public static DrawNodes Instance { get { return _instance; } }

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

    public void DrawShapes()
    {
        foreach (var node in EdgeRoutingProgram.Instance.GeoGraph.Nodes)
        {
            GameObject nodeObj;
            if (node.BoundaryCurve.ToString() == "Microsoft.Msagl.Core.Geometry.Curves.Ellipse")
                nodeObj = Instantiate(SpherePrefab);
            else
                nodeObj = Instantiate(DiamondPrefab);

            nodeObj.transform.localScale = new Vector3((float)node.Width, (float)node.Height, NodeDepth);
            Vector3 position = new Vector3((float)node.Center.X, (float)node.Center.Y, 0);
            nodeObj.transform.position = position;
            nodeObj.name = node.ToString();

            UnityNode unityNode = nodeObj.GetComponent<UnityNode>();
            unityNode.NodeEdges = node.Edges.ToList();

            nodeObj.transform.SetParent(EdgeRoutingProgram.Instance.GraphContainer.transform);
        }
    }
}
