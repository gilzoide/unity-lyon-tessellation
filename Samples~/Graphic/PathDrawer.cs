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

    protected PathBuilder PathBuilder => _pathBuilder ??= new PathBuilder();
    protected Tessellator<UIVertex, int> Tessellator => _tessellator ??= new Tessellator<UIVertex, int>();

    private PathBuilder _pathBuilder;
    private Tessellator<UIVertex, int> _tessellator;
    private JobHandle _jobHandle;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _jobHandle.Complete();
        _tessellator?.Dispose();
        _pathBuilder?.Dispose();
    }

    protected void Update()
    {
        Vector2 center = rectTransform.rect.center;
        PathBuilder.Clear()
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

        Tessellator.Clear();
        if (Fill)
        {
            _jobHandle = Tessellator.CreatePathFillJob(PathBuilder, FillOptions).Schedule();
        }
        else
        {
            _jobHandle = Tessellator.CreatePathStrokeJob(PathBuilder, StrokeOptions).Schedule();
        }
        _jobHandle = Tessellator.CreateUIVertexJob(this).Schedule(_jobHandle);
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
