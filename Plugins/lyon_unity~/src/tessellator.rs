use lyon_tessellation::{FillTessellator, StrokeTessellator};
use lyon_tessellation::math::Point;
use crate::fill_options::UnityFillOptions;
use crate::geometry_builder::UnityGeometryBuilder;
use crate::path_iterator::UnityPathIterator;
use crate::stroke_options::UnityStrokeOptions;

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