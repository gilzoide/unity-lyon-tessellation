use std::boxed::Box;
use lyon_tessellation::{FillTessellator, StrokeTessellator, VertexBuffers};
use lyon_tessellation::geometry_builder::simple_builder;
use lyon_tessellation::math::Point;
use crate::fill_options::UnityFillOptions;
use crate::path_iterator::UnityPathIterator;
use crate::stroke_options::UnityStrokeOptions;

type Buffer = VertexBuffers<Point, u16>;

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_new() -> Box<Buffer> {
    return Box::new(VertexBuffers::new());
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_destroy(_buffer: Box<Buffer>) {
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_clear(buffer: &mut Buffer) {
    buffer.vertices.clear();
    buffer.indices.clear();
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_get_vertices(buffer: &Buffer, vertices_ptr: *mut *const Point, vertices_length: *mut i32) {
    let vertices = &buffer.vertices;
    *vertices_ptr = vertices.as_ptr();
    *vertices_length = vertices.len() as i32;
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_get_indices(buffer: &Buffer, indices_ptr: *mut *const u16, indices_length: *mut i32) {
    let indices = &buffer.indices;
    *indices_ptr = indices.as_ptr();
    *indices_length = indices.len() as i32;
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_fill(buffer: &mut Buffer, points: *const Point, verbs: *const u8, verbs_len: i32, options: &UnityFillOptions) -> i32 {
    let mut vertex_builder = simple_builder(buffer);
    let mut tessellator = FillTessellator::new();
    let result = tessellator.tessellate(
        UnityPathIterator::new(points, verbs, verbs_len),
        &options.to_lyon(),
        &mut vertex_builder
    );
    match result {
        Ok(()) => return 0,
        Err(_) => return -1,
    }
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_stroke(buffer: &mut Buffer, points: *const Point, verbs: *const u8, verbs_len: i32, options: &UnityStrokeOptions) -> i32 {
    let mut vertex_builder = simple_builder(buffer);
    let mut tessellator = StrokeTessellator::new();
    let result = tessellator.tessellate(
        UnityPathIterator::new(points, verbs, verbs_len),
        &options.to_lyon(),
        &mut vertex_builder
    );
    match result {
        Ok(()) => return 0,
        Err(_) => return -1,
    }
}