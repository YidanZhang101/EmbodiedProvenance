using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClipShape
{
    Circle,
    Rectangle
}

public class ClipController : MonoBehaviour
{
    public Transform center;

    [Header("Rectangle")]
    public float width = 0.1f;
    public float height = 0.1f;

    public Action OnClipReady = delegate { };
    public Action OnUpdate = delegate { };

    public AbstractMap abstractMap;
    protected Material material;
    protected float prevRadius;
    protected float prevWidth;
    protected float prevHeight;
    protected float prevBorder;
    protected float prevBaseHeight;

    protected Vector3 prevPosition;
    protected Quaternion prevRotation;
    protected Material clipMaterial;
    protected bool isClipReady = false;
    protected List<Material> mapMaterials;
    protected List<Material> extraClipMaterials;


    #region MONOBEHAVIOURS
    private void Awake()
    {
        extraClipMaterials = new List<Material>();
        mapMaterials = new List<Material>();
    }

    private void Update()
    {
        if (prevHeight != height || prevWidth != width || prevPosition != transform.position || 
            prevRotation != transform.rotation )
        {
            if (mapMaterials.Count > 0)
            {
                foreach (Material m in mapMaterials)
                {
                    ApplyParameters(m);
                    UpdatePrevValues();
                    OnUpdate.Invoke();                   
                }
                foreach(Material m in extraClipMaterials)
                {
                   if(m) UpdateClipVolumeGlobalParams(m);
                }
                if (!isClipReady)
                {
                    OnClipReady.Invoke();
                    isClipReady = true;
                }
            }
            else
            {
               UpdateMaterials();
            }
        }
    }

    #endregion

    #region PRIVATES

    private void UpdateClipVolumeGlobalParams(Material m)
    {
        Vector3[] p = GetRectanglePoints();
        m.SetVector("_P1", p[0]);
        m.SetVector("_P2", p[1]);
        m.SetVector("_P4", p[2]);
        m.SetVector("_P5", p[3]);
        m.SetFloat("_Width", width);
        m.SetFloat("_Length", height);
    }

    private void UpdateMaterials()
    {
        mapMaterials = new List<Material>();
        foreach (MeshRenderer mr in abstractMap.GetComponentsInChildren<MeshRenderer>())
        {
            mapMaterials.Add(mr.material);
        }
    }


    private void UpdatePrevValues()
    {
        prevHeight = height;
        prevWidth = width;
        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    protected void ApplyParameters(Material m)
    {
        if(m) UpdateClipVolumeGlobalParams(m);
    }

    #endregion

    /// <summary>
    /// Set map materials
    /// </summary>
    /// <param name="mats"></param>
    public void SetMaterials(List<Material> mats)
    {
        mapMaterials = mats;
    }

    /// <summary>
    /// Add extra material to be clipped. Material must use ClipVolumeGlobal Shader!
    /// </summary>
    /// <param name="mat"></param>
    public void AddExtraClipMaterial(Material mat)
    {
        extraClipMaterials.Add(mat);
        UpdateClipVolumeGlobalParams(mat);
    }

    public void RemoveExtraClipMaterial(Material mat)
    {
        extraClipMaterials.Remove(mat);
    }

    public bool IsExtraMaterial(Material mat)
    {
        bool r = false;
        foreach(Material m in extraClipMaterials)
        {
            if (m.Equals(m))
            {
                r = true;
                break;
            }
        }

        return r;
    }



    /// <summary>
    /// Get corner points of the rectangle in world space.
    ///  0 -> top right,
    ///  1 -> top left,
    ///  2 -> bottom left,
    ///  3 -> bottom right
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetRectanglePoints()
    {
        Vector3 tR = center.position + (center.right * 0.5f * width) + (center.forward * 0.5f * height);
        Vector3 tL = center.position - (center.right * 0.5f * width) + (center.forward * 0.5f * height);
        Vector3 bL = center.position + (center.right * 0.5f * width) - (center.forward * 0.5f * height);
        Vector3 bR = center.position - (center.right * 0.5f * width) - (center.forward * 0.5f * height);

        Vector3[] r = new Vector3[4];
        r[0] = tR;
        r[1] = tL;
        r[2] = bR;
        r[3] = bL;

        return r;
    }

    /// <summary>
    /// Return the closest rectangle point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 GetClosestRectanglePoint(Vector3 point)
    {
        Vector3[] ps = GetRectanglePoints();
        Vector3 closestPoint = ps[0];
        
        foreach(Vector3 p in ps)
        {
            if(Vector3.Distance(p, point) < Vector3.Distance(closestPoint, point))
            {
                closestPoint = p;
            }
        }

        return closestPoint;
    }



    /// <summary>
    /// Get the closest edge point from position point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 GetClosestEdgePoint(Vector3 point)
    {
        Vector3 closest = transform.position;

        Vector3 up = transform.forward * height * 0.5f;
        Vector3 right = transform.right * width * 0.5f;
        Vector3 direction = point - transform.position;

        Vector3 upProject = Vector3.Project(direction, transform.forward);
        Vector3 rightProject = Vector3.Project(direction, transform.right);

        float diffUp = up.magnitude - upProject.magnitude;
        float diffRight = right.magnitude - rightProject.magnitude;

        Vector3 moveUpVector = upProject.normalized * diffUp;
        Vector3 moveRighVector = rightProject.normalized * diffRight;

        if (moveUpVector.magnitude < moveRighVector.magnitude)
        {
            closest = point + moveUpVector;
        }
        else
        {
            closest = point + moveRighVector;
        }

        return closest;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsClosestToHeight(Vector3 point)
    {
        bool closest = false;

        Vector3 up = transform.forward * height * 0.5f;
        Vector3 right = transform.right * width * 0.5f;
        Vector3 direction = point - transform.position;

        Vector3 upProject = Vector3.Project(direction, transform.forward);
        Vector3 rightProject = Vector3.Project(direction, transform.right);

        float diffUp = up.magnitude - upProject.magnitude;
        float diffRight = right.magnitude - rightProject.magnitude;

        Vector3 moveUpVector = upProject.normalized * diffUp;
        Vector3 moreRighVector = rightProject.normalized * diffRight;

        if (moveUpVector.magnitude < moreRighVector.magnitude)
        {
            closest = true;
        }
        else
        {
            closest = false;
        }

        return closest;
    }
}
