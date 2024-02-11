using System.Collections.Generic;
using Gilzoide.LyonTesselation;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class PathFill : Graphic
{
    public FillOptions FillOptions = FillOptions.Default();

    private Tessellator<UIVertex, int> _tessellator;
    private JobHandle _jobHandle;
    private PathBuilder _pathBuilder;
    private Vector3[] _corners = new Vector3[4];

    protected override void OnEnable()
    {
        _tessellator = Tessellator<UIVertex, int>.Create();
        _pathBuilder = new PathBuilder(Allocator.Persistent);
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
        var center = rectTransform.rect.center;
        rectTransform.GetLocalCorners(_corners);
        _pathBuilder.Clear()
            .BeginAt(_corners[0])
                .QuadraticTo(center, _corners[1])
                .QuadraticTo(center, _corners[2])
                .QuadraticTo(center, _corners[3])
                .QuadraticTo(center, _corners[0])
            .End()
            .AddEllipse(new Vector2(_corners[0].x, center.y), new Vector2(10, 20))
            .AddEllipse(new Vector2(_corners[2].x, center.y), new Vector2(10, 20))
            ;

        _tessellator.Clear();
        JobHandle fillJobHandle = _tessellator.CreatePathFillJob(_pathBuilder, FillOptions).Schedule();
        _jobHandle = _tessellator.CreateUIVertexJob(this).Schedule(fillJobHandle);
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
