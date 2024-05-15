using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform[] transorms;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lineRenderer.positionCount = transorms.Length;
        for (int i = 0; i < transorms.Length; i++)
        {
            lineRenderer.SetPosition(i, transorms[i].position);
        }
    }
}
