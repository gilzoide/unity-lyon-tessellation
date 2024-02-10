use lyon_tessellation::FillOptions;
use lyon_tessellation::FillRule;
use lyon_tessellation::Orientation;

#[repr(C)]
pub struct UnityFillOptions {
    pub tolerance: f32,
    pub fill_rule: i32,
    pub sweep_orientation: i32,
    pub handle_intersections: i32,
}

impl UnityFillOptions {
    pub fn to_lyon(&self) -> FillOptions {
        let tolerance = self.tolerance;
        let fill_rule = match self.fill_rule {
            1 => FillRule::NonZero,
            _ => FillRule::EvenOdd,
        };
        let sweep_orientation = match self.sweep_orientation {
            0 => Orientation::Horizontal,
            _ => Orientation::Vertical,
        };
        let handle_intersections = self.handle_intersections != 0;
        FillOptions::tolerance(tolerance)
            .with_fill_rule(fill_rule)
            .with_sweep_orientation(sweep_orientation)
            .with_intersections(handle_intersections)
    }
}