using System.Collections.Generic;
using Gilzoide.LyonTesselation;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class PathDrawer : Graphic
{
    public FillOptions FillOptions = FillOptions.Default();
    public StrokeOptions StrokeOptions = StrokeOptions.Default();
    public bool Fill = true;

    private Tessellator<UIVertex, int> _tessellator;
    private JobHandle _jobHandle;
    private PathBuilder _pathBuilder;
    private Vector3[] _corners = new Vector3[4];

    protected override void OnEnable()
    {
        _tessellator = Tessellator<UIVertex, int>.Allocate();
        _pathBuilder = new PathBuilder();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _jobHandle.Complete();
        _tessellator.Dispose();
        _pathBuilder.Dispose();
    }

    private void Update()
    {
        Rect rect = rectTransform.rect;
        Vector2 center = rect.center;
        rectTransform.GetLocalCorners(_corners);
        _pathBuilder.Clear()
            .AddEllipse(new Vector2(_corners[0].x, center.y), new Vector2(10, 20))
            .AddEllipse(new Vector2(_corners[2].x, center.y), new Vector2(10, 20))
            .AddRoundedRect(rectTransform.rect, 10)
            ;

        _tessellator.Clear();
        if (Fill)
        {
            _jobHandle = _tessellator.CreatePathFillJob(_pathBuilder, FillOptions).Schedule();
        }
        else
        {
            _jobHandle = _tessellator.CreatePathStrokeJob(_pathBuilder, StrokeOptions).Schedule();
        }
        _jobHandle = _tessellator.CreateUIVertexJob(this).Schedule(_jobHandle);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        _jobHandle.Complete();
        using (ListPool<UIVertex>.Get(out List<UIVertex> vertices))
        using (ListPool<int>.Get(out List<int> indices))
        {
            vertices.AddRange(_tessellator.Vertices);
            indices.AddRange(_tessellator.Indices);
            vh.AddUIVertexStream(vertices, indices);
        }
    }
}
