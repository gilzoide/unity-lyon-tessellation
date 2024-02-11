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

    private Tessellator<UIVertex, int> tessellator;
    private JobHandle jobHandle;
    private PathBuilder pathBuilder;
    private Vector3[] corners = new Vector3[4];

    protected override void OnEnable()
    {
        tessellator = Tessellator<UIVertex, int>.Create();
        pathBuilder = new PathBuilder(Allocator.Persistent);
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        jobHandle.Complete();
        tessellator.Dispose();
        pathBuilder.Dispose();
    }

    private void Update()
    {
        var center = rectTransform.rect.center;
        rectTransform.GetLocalCorners(corners);
        pathBuilder.Clear()
            .BeginAt(corners[0])
            .QuadraticTo(center, corners[1])
            .QuadraticTo(center, corners[2])
            .QuadraticTo(center, corners[3])
            .QuadraticTo(center, corners[0])
            .End();

        tessellator.Clear();
        JobHandle fillJobHandle = tessellator.CreatePathFillJob(pathBuilder, FillOptions).Schedule();
        jobHandle = tessellator.CreateUIVertexJob(this).Schedule(fillJobHandle);
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        jobHandle.Complete();
        using (ListPool<UIVertex>.Get(out List<UIVertex> vertices))
        using (ListPool<int>.Get(out List<int> indices))
        {
            vertices.AddRange(tessellator.Vertices);
            indices.AddRange(tessellator.Indices);
            vh.AddUIVertexStream(vertices, indices);
        }
    }
}
