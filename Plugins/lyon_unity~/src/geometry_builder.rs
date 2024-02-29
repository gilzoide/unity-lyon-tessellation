use std::convert::TryFrom;
use lyon_tessellation::{GeometryBuilder, FillGeometryBuilder, FillVertex, StrokeGeometryBuilder, StrokeVertex, VertexId};
use lyon_tessellation::geometry_builder::GeometryBuilderError;
use lyon_tessellation::math::Point;

#[repr(C)]
pub struct UnityUnsafeList {
    pub(crate) ptr: *mut u8,
    pub(crate) length: i32,
    pub(crate) capacity: i32,
}

impl UnityUnsafeList {
    pub unsafe fn push(&mut self, size: i32, push_func: extern "C" fn(&mut UnityUnsafeList, i32) -> *mut u8) -> *mut u8 {
        if self.length + size <= self.capacity {
            let ptr = self.ptr.add(self.length as usize);
            self.length += size;
            ptr
        }
        else {
            push_func(self, size)
        }
    }
}

#[repr(C)]
pub struct UnityGeometryBuilder {
    vertices_list_ptr: *mut UnityUnsafeList,
    indices_list_ptr: *mut UnityUnsafeList,

    push_bytes_func: extern "C" fn(&mut UnityUnsafeList, i32) -> *mut u8,

    vertex_size: i32,
    index_size: i32,

    is_index_16: u8,
}

impl UnityGeometryBuilder {
    unsafe fn add_vertex(&mut self, position: Point) -> Result<VertexId, GeometryBuilderError> {
        let byte_len = (*self.vertices_list_ptr).length;
        if byte_len >= i32::MAX - self.vertex_size {
            Err(GeometryBuilderError::TooManyVertices)
        }
        else {
            let vertex_id = byte_len / self.vertex_size;
            let vertex_ptr = (*self.vertices_list_ptr).push(self.vertex_size, self.push_bytes_func);
            *(vertex_ptr as *mut f32) = position.x;
            *(vertex_ptr as *mut f32).add(1) = position.y;
            Ok(VertexId(vertex_id as u32))
        }
    }

    unsafe fn add_index(&mut self, index: VertexId) {
        let index_ptr = (*self.indices_list_ptr).push(self.index_size, self.push_bytes_func);
        if self.is_index_16 != 0 {
            if let Ok(i) = u16::try_from(index.0) {
                *(index_ptr as *mut u16) = i;
            }
        }
        else {
            *(index_ptr as *mut u32) = index.0;
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