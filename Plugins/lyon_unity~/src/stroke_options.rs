use lyon_tessellation::LineCap;
use lyon_tessellation::LineJoin;
use lyon_tessellation::StrokeOptions;

#[repr(C)]
pub struct UnityStrokeOptions {
    pub start_cap: i32,
    pub end_cap: i32,
    pub line_join: i32,
    pub line_width: f32,
    pub miter_limit: f32,
    pub tolerance: f32,
}

impl UnityStrokeOptions {
    pub fn to_lyon(&self) -> StrokeOptions {
        let start_cap = match self.start_cap {
            1 => LineCap::Square,
            2 => LineCap::Round,
            _ => LineCap::Butt,
        };
        let end_cap = match self.end_cap {
            1 => LineCap::Square,
            2 => LineCap::Round,
            _ => LineCap::Butt,
        };
        let line_join = match self.line_join {
            1 => LineJoin::MiterClip,
            2 => LineJoin::Round,
            3 => LineJoin::Bevel,
            _ => LineJoin::Miter,
        };
        let line_width = self.line_width;
        let miter_limit = self.miter_limit;
        let tolerance = self.tolerance;
        StrokeOptions::tolerance(tolerance)
            .with_start_cap(start_cap)
            .with_end_cap(end_cap)
            .with_line_join(line_join)
            .with_line_width(line_width)
            .with_miter_limit(miter_limit)
    }
}