use std::boxed::Box;
use lyon_tessellation::FillOptions;
use lyon_tessellation::FillTessellator;
use lyon_tessellation::StrokeTessellator;
use lyon_tessellation::StrokeOptions;
use lyon_tessellation::VertexBuffers;
use lyon_tessellation::geometry_builder::simple_builder;
use lyon_tessellation::math::Point;
use crate::path_event_iter::{ UnityPathEvent, UnityPathEventIter };

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
pub unsafe extern "C" fn lyon_unity_triangulate_fill(buffer: &mut Buffer, path_events: *const UnityPathEvent, path_events_len: i32) -> i32 {
    let mut vertex_builder = simple_builder(buffer);
    let mut tessellator = FillTessellator::new();
    let result = tessellator.tessellate(
        UnityPathEventIter::new(path_events, path_events_len),
        &FillOptions::default(),
        &mut vertex_builder
    );
    match result {
        Ok(()) => return 0,
        Err(_) => return -1,
    }
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_stroke(buffer: &mut Buffer, path_events: *const UnityPathEvent, path_events_len: i32) -> i32 {
    let mut vertex_builder = simple_builder(buffer);
    let mut tessellator = StrokeTessellator::new();
    let result = tessellator.tessellate(
        UnityPathEventIter::new(path_events, path_events_len),
        &StrokeOptions::default(),
        &mut vertex_builder
    );
    match result {
        Ok(()) => return 0,
        Err(_) => return -1,
    }
}