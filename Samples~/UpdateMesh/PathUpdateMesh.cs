using Gilzoide.LyonTesselation;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LyonTessellation.Samples.RenderPrimitives
{
    public class PathUpdateMesh : MonoBehaviour
    {
        public FillOptions FillOptions = FillOptions.Default();
        public StrokeOptions StrokeOptions = StrokeOptions.Default();
        public bool Fill = true;

        private Mesh _mesh;
        private PathBuilder _pathBuilder;
        private GeometryBuilder<Vector3, ushort> _tessellator;
        private JobHandle _jobHandle;

        protected void Start()
        {
            _mesh = new Mesh();
            _mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        protected void OnEnable()
        {
            _pathBuilder = new();
            _tessellator = new();
        }

        protected void OnDisable()
        {
            _jobHandle.Complete();
            _tessellator?.Dispose();
            _tessellator = null;
            _pathBuilder?.Dispose();
            _pathBuilder = null;
        }

        protected void OnDestroy()
        {
            Destroy(_mesh);
        }

        protected void Update()
        {
            Vector2 center = Vector2.zero;
            _pathBuilder.Clear()
                .AddCircle(center, 1)
                .AddCircle(new Vector2(center.x - 0.2f, center.y + 0.2f), 0.2f)
                .AddCircle(new Vector2(center.x + 0.2f, center.y + 0.2f), 0.2f)
                .MoveTo(new Vector2(center.x - 0.3f, center.y - 0.05f))
                    .CubicBezierTo(
                        new Vector2(center.x - 0.3f, center.y - 0.45f),
                        new Vector2(center.x + 0.3f, center.y - 0.45f),
                        new Vector2(center.x + 0.3f, center.y - 0.05f)
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
        }

        protected void LateUpdate()
        {
            _jobHandle.Complete();
            _mesh.SetVertices(_tessellator.Vertices.AsArray());
            _mesh.SetIndices(_tessellator.Indices.AsArray(), MeshTopology.Triangles, 0);
            _mesh.UploadMeshData(false);
        }
    }
}
