use std::boxed::Box;
use std::ptr::drop_in_place;
use lyon_tessellation::{FillTessellator, StrokeTessellator};
use lyon_tessellation::math::Point;
use crate::fill_options::UnityFillOptions;
use crate::geometry_builder::UnityGeometryBuilder;
use crate::path_iterator::UnityPathIterator;
use crate::stroke_options::UnityStrokeOptions;

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_new(vertex_size: i32, index_size: i32) -> *mut UnityGeometryBuilder {
    let value = Box::new(UnityGeometryBuilder::new(vertex_size, index_size));
    Box::into_raw(value)
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_destroy(buffer: *mut UnityGeometryBuilder) {
    if !buffer.is_null() {
        drop_in_place(buffer);
    }
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_clear(buffer: &mut UnityGeometryBuilder) {
    buffer.clear();
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_get_vertices(buffer: &UnityGeometryBuilder, vertices_ptr: *mut *const u8, vertices_length: *mut i32) {
    *vertices_ptr = buffer.vertices.as_ptr();
    *vertices_length = buffer.vertices_len as i32;
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_buffer_get_indices(buffer: &UnityGeometryBuilder, indices_ptr: *mut *const u8, indices_length: *mut i32) {
    *indices_ptr = buffer.indices.as_ptr();
    *indices_length = buffer.indices_len as i32;
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_fill(buffer: &mut UnityGeometryBuilder, points: *const Point, verbs: *const u8, verbs_len: i32, options: &UnityFillOptions) -> i32 {
    let mut tessellator = FillTessellator::new();
    let result = tessellator.tessellate(
        UnityPathIterator::new(points, verbs, verbs_len),
        &options.to_lyon(),
        buffer,
    );
    match result {
        Ok(()) => return 0,
        Err(_) => return -1,
    }
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_stroke(buffer: &mut UnityGeometryBuilder, points: *const Point, verbs: *const u8, verbs_len: i32, options: &UnityStrokeOptions) -> i32 {
    let mut tessellator = StrokeTessellator::new();
    let result = tessellator.tessellate(
        UnityPathIterator::new(points, verbs, verbs_len),
        &options.to_lyon(),
        buffer,
    );
    match result {
        Ok(()) => return 0,
        Err(_) => return -1,
    }
}