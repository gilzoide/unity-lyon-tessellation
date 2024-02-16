use std::convert::TryFrom;
use std::mem::size_of;
use lyon_tessellation::{GeometryBuilder, FillGeometryBuilder, FillVertex, StrokeGeometryBuilder, StrokeVertex, VertexId};
use lyon_tessellation::geometry_builder::GeometryBuilderError;
use lyon_tessellation::math::Point;

#[repr(C)]
pub struct UnityGeometryBuilder {
    vertices_list_ptr: *const u8,
    indices_list_ptr: *const u8,

    push_bytes_func: extern "C" fn(*const u8, i32) -> *mut u8,
    get_length_func: extern "C" fn(*const u8) -> i32,

    vertex_size: i32,
    index_size: i32,
}

impl UnityGeometryBuilder {
    unsafe fn add_vertex(&mut self, position: Point) -> Result<VertexId, GeometryBuilderError> {
        let byte_len = (self.get_length_func)(self.vertices_list_ptr);
        if byte_len >= i32::MAX - self.vertex_size {
            Err(GeometryBuilderError::TooManyVertices)
        }
        else {
            let vertex_id = byte_len / self.vertex_size;
            let vertex_ptr = (self.push_bytes_func)(self.vertices_list_ptr, self.vertex_size);
            *(vertex_ptr as *mut f32) = position.x;
            *(vertex_ptr as *mut f32).add(1) = position.y;
            Ok(VertexId(vertex_id as u32))
        }
    }

    unsafe fn add_index(&mut self, index: VertexId) {
        let index_ptr = (self.push_bytes_func)(self.indices_list_ptr, self.index_size);
        match self.index_size {
            s if (s as usize) < size_of::<u32>() => {
                match u16::try_from(index.0) {
                    Ok(i) => {
                        *(index_ptr as *mut u16) = i;
                    },
                    _ => {},
                }
            },
            _ => {
                *(index_ptr as *mut u32) = index.0;
            },
        }
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