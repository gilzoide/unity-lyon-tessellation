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
    private Tessellator<UIVertex, int> _tessellator;
    private JobHandle _jobHandle;

    protected override void OnEnable()
    {
        _pathBuilder = new();
        _tessellator = new();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        _jobHandle.Complete();
        _tessellator?.Dispose();
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
            .BeginAt(new Vector2(center.x - 30, center.y - 5))
                .CubicBezierTo(
                    new Vector2(center.x - 30, center.y - 45),
                    new Vector2(center.x + 30, center.y - 45),
                    new Vector2(center.x + 30, center.y - 5)
                )
            .Close();

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
