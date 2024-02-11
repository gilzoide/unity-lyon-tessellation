use std::convert::TryFrom;
use std::mem::size_of;
use std::vec::Vec;
use lyon_tessellation::{GeometryBuilder, FillGeometryBuilder, FillVertex, StrokeGeometryBuilder, StrokeVertex, VertexId};
use lyon_tessellation::geometry_builder::GeometryBuilderError;
use lyon_tessellation::math::Point;

pub struct UnityGeometryBuilder {
    pub vertices: Vec<u8>,
    pub indices: Vec<u8>,
    pub vertex_size: i32,
    pub index_size: i32,
    pub vertices_len: i32,
    pub indices_len: i32,
}

impl UnityGeometryBuilder {
    pub fn new(vertex_size: i32, index_size: i32) -> Self {
        return Self {
            vertices: Vec::new(),
            indices: Vec::new(),
            vertex_size,
            index_size,
            vertices_len: 0,
            indices_len: 0,
        }
    }

    pub fn clear(&mut self) {
        self.vertices.clear();
        self.indices.clear();
        self.vertices_len = 0;
        self.indices_len = 0;
    }

    fn add_vertex(&mut self, position: Point) -> Result<VertexId, GeometryBuilderError> {
        if self.vertices_len >= i32::MAX {
            Err(GeometryBuilderError::TooManyVertices)
        }
        else {
            let byte_len = self.vertices.len();
            let vertex_id = self.vertices_len;
            self.vertices_len += 1;
            self.vertices.resize(byte_len + self.vertex_size as usize, 0);
            unsafe {
                *(self.vertices.as_mut_ptr().add(byte_len) as *mut f32) = position.x;
                *(self.vertices.as_mut_ptr().add(byte_len) as *mut f32).add(1) = position.y;
            }
            Ok(VertexId(vertex_id as u32))
        }
    }

    fn add_index(&mut self, index: VertexId) {
        if self.indices_len < i32::MAX {
            let byte_len = self.indices.len();
            self.indices_len += 1;
            self.indices.resize(byte_len + self.index_size as usize, 0);
            match self.index_size {
                s if s as usize == size_of::<u16>() => {
                    match u16::try_from(index.0) {
                        Ok(i) => {
                            unsafe {
                                *(self.indices.as_mut_ptr().add(byte_len) as *mut u16) = i;
                            }
                        },
                        _ => {},
                    }
                },
                _ => {
                    unsafe {
                        *(self.indices.as_mut_ptr().add(byte_len) as *mut u32) = index.0;
                    }
                },
            }
        }
    }
}

impl GeometryBuilder for UnityGeometryBuilder {
    fn add_triangle(&mut self, a: VertexId, b: VertexId, c: VertexId) {
        self.add_index(a);
        self.add_index(b);
        self.add_index(c);
    }
}

impl FillGeometryBuilder for UnityGeometryBuilder {
    fn add_fill_vertex(
        &mut self,
        vertex: FillVertex<'_>
    ) -> Result<VertexId, GeometryBuilderError> {
        let position = vertex.position();
        self.add_vertex(position)
    }
}

impl StrokeGeometryBuilder for UnityGeometryBuilder {
    fn add_stroke_vertex(
        &mut self,
        vertex: StrokeVertex<'_, '_>
    ) -> Result<VertexId, GeometryBuilderError> {
        let position = vertex.position();
        self.add_vertex(position)
    }
}