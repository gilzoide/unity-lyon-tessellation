use lyon_tessellation::{GeometryBuilder, FillGeometryBuilder, FillVertex, StrokeGeometryBuilder, StrokeVertex, VertexId};
use lyon_tessellation::geometry_builder::GeometryBuilderError;
use lyon_tessellation::math::Point;

const INVALID_VERTEX: u32 = u32::MAX - 1;
const TOO_MANY_VERTICES: u32 = u32::MAX;

#[repr(C)]
pub struct UnityGeometryBuilder {
    add_vertex_ptr: extern "C" fn(&mut UnityGeometryBuilder, f32, f32) -> u32,
    add_index_ptr: extern "C" fn(&mut UnityGeometryBuilder, u32),
}

impl UnityGeometryBuilder {
    unsafe fn add_vertex(&mut self, position: Point) -> Result<VertexId, GeometryBuilderError> {
        match (self.add_vertex_ptr)(self, position.x, position.y) {
            TOO_MANY_VERTICES => Err(GeometryBuilderError::TooManyVertices),
            INVALID_VERTEX => Err(GeometryBuilderError::InvalidVertex),
            vertex_id => Ok(VertexId(vertex_id)),
        }
    }

    unsafe fn add_index(&mut self, index: VertexId) {
        (self.add_index_ptr)(self, index.0)
    }
}

impl GeometryBuilder for UnityGeometryBuilder {
    fn add_triangle(&mut self, a: VertexId, b: VertexId, c: VertexId) {
        unsafe {
            self.add_index(a);
            self.add_index(b);
            self.add_index(c);
        }
    }
}

impl FillGeometryBuilder for UnityGeometryBuilder {
    fn add_fill_vertex(
        &mut self,
        vertex: FillVertex<'_>
    ) -> Result<VertexId, GeometryBuilderError> {
        let position = vertex.position();
        unsafe {
            self.add_vertex(position)
        }
    }
}

impl StrokeGeometryBuilder for UnityGeometryBuilder {
    fn add_stroke_vertex(
        &mut self,
        vertex: StrokeVertex<'_, '_>
    ) -> Result<VertexId, GeometryBuilderError> {
        let position = vertex.position();
        unsafe {
            self.add_vertex(position)
        }
    }
}