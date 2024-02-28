use std::boxed::Box;
use std::ffi::CString;
use std::panic;
use std::panic::catch_unwind;
use lyon_tessellation::{FillTessellator, StrokeTessellator};
use lyon_tessellation::math::Point;
use crate::fill_options::UnityFillOptions;
use crate::geometry_builder::UnityGeometryBuilder;
use crate::path_iterator::UnityPathIterator;
use crate::stroke_options::UnityStrokeOptions;

extern "C" {
    pub fn lyon_unity_log_error(cstr: *const u8);
    pub fn c_UnityPluginLoad(unity_interfaces: *const u8);
}

#[no_mangle]
pub unsafe extern "C" fn UnityPluginLoad(unity_interfaces: *const u8) {
    c_UnityPluginLoad(unity_interfaces);
    panic::set_hook(Box::new(|panic_info| {
        if let Ok(cstr) = CString::new(panic_info.to_string()) {
            lyon_unity_log_error(cstr.as_ptr() as *const u8);
        }
    }));
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_fill(buffer: *mut UnityGeometryBuilder, points: *const Point, verbs: *const u8, verbs_len: i32, options: &UnityFillOptions) -> i32 {
    match catch_unwind(|| {
        match FillTessellator::new().tessellate(
            UnityPathIterator::new(points, verbs, verbs_len),
            &options.to_lyon(),
            &mut *buffer,
        ) {
            Ok(()) => return 0,
            Err(_) => return -1,
        }
    }) {
        Ok(i) => i,
        Err(_) => -2,
    }
}

#[no_mangle]
pub unsafe extern "C" fn lyon_unity_triangulate_stroke(buffer: *mut UnityGeometryBuilder, points: *const Point, verbs: *const u8, verbs_len: i32, options: &UnityStrokeOptions) -> i32 {
    match catch_unwind(|| {
        match StrokeTessellator::new().tessellate(
            UnityPathIterator::new(points, verbs, verbs_len),
            &options.to_lyon(),
            &mut *buffer,
        ) {
            Ok(()) => return 0,
            Err(_) => return -1,
        }
    }) {
        Ok(i) => i,
        Err(_) => -2,
    }
}