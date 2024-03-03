using System.Collections.Generic;
using Gilzoide.LyonTesselation;
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

    private PathBuilder _pathBuilder;
    private GeometryBuilder<UIVertex, int> _geometryBuilder;
    private JobHandle _jobHandle;

    protected override void OnEnable()
    {
        _pathBuilder = new();
        _geometryBuilder = new();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        _jobHandle.Complete();
        _geometryBuilder?.Dispose();
        _pathBuilder?.Dispose();
        base.OnDisable();
    }

    protected void Update()
    {
        Vector2 center = rectTransform.rect.center;
        _pathBuilder.Clear()
            .AddCircle(center, 100)
            .AddCircle(new Vector2(center.x - 20, center.y + 20), 20)
            .AddCircle(new Vector2(center.x + 20, center.y + 20), 20)
            .MoveTo(new Vector2(center.x - 30, center.y - 5))
                .CubicBezierTo(
                    new Vector2(center.x - 30, center.y - 45),
                    new Vector2(center.x + 30, center.y - 45),
                    new Vector2(center.x + 30, center.y - 5)
                )
            .Close();

        _geometryBuilder.Clear();
        if (Fill)
        {
            _jobHandle = _geometryBuilder.CreatePathFillJob(_pathBuilder, FillOptions).Schedule();
        }
        else
        {
            _jobHandle = _geometryBuilder.CreatePathStrokeJob(_pathBuilder, StrokeOptions).Schedule();
        }
        _jobHandle = _geometryBuilder.CreateUIVertexJob(this).Schedule(_jobHandle);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        _jobHandle.Complete();
        using (ListPool<UIVertex>.Get(out List<UIVertex> vertices))
        using (ListPool<int>.Get(out List<int> indices))
        {
            vertices.AddRange(_geometryBuilder.Vertices.AsArray());
            indices.AddRange(_geometryBuilder.Indices.AsArray());
            vh.AddUIVertexStream(vertices, indices);
        }
    }
}
