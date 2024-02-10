use std::iter::Iterator;
use lyon_path::Event;
use lyon_path::PathEvent;
use lyon_path::math::Point;

pub struct UnityPathEventIter {
    points: *const Point,
    verbs: *const u8,
    verbs_left: i32,
    current: Point,
    first: Point,
}

impl UnityPathEventIter {
    pub fn new(points: *const Point, verbs: *const u8, verbs_left: i32) -> Self {
        Self {
            points,
            verbs,
            verbs_left,
            current: Point::default(),
            first: Point::default(),
        }
    }

    unsafe fn pop_verb(&mut self) -> Option<u8> {
        if self.verbs_left < 0 {
            None
        }
        else {
            let verb = *self.verbs;
            self.verbs = self.verbs.add(1);
            self.verbs_left -= 1;
            Some(verb)
        }
    }

    unsafe fn pop_point(&mut self) -> Point {
        let point = *self.points;
        self.points = self.points.add(1);
        point
    }
}

impl Iterator for UnityPathEventIter {
    type Item = PathEvent;

    fn next(&mut self) -> Option<Self::Item> {
        unsafe {
            match self.pop_verb() {
                Some(0) => {
                    let at = self.pop_point();
                    self.first = at;
                    self.current = at;
                    Some(Event::Begin {
                        at,
                    })
                },
                Some(1) => {
                    let from = self.current;
                    let to = self.pop_point();
                    self.current = to;
                    Some(Event::Line {
                        from,
                        to,
                    })
                },
                Some(2) => {
                    let from = self.current;
                    let ctrl = self.pop_point();
                    let to = self.pop_point();
                    self.current = to;
                    Some(Event::Quadratic {
                        from,
                        ctrl,
                        to,
                    })
                },
                Some(3) => {
                    let from = self.current;
                    let ctrl1 = self.pop_point();
                    let ctrl2 = self.pop_point();
                    let to = self.pop_point();
                    self.current = to;
                    Some(Event::Cubic {
                        from,
                        ctrl1,
                        ctrl2,
                        to,
                    })
                },
                Some(4) => {
                    let last = self.current;
                    let first = self.first;
                    self.current = first;
                    Some(Event::End {
                        last,
                        first,
                        close: true,
                    })
                },
                Some(5) => {
                    let last = self.current;
                    let first = self.first;
                    Some(Event::End {
                        last,
                        first,
                        close: false,
                    })
                },
                _ => None
            }
        }
    }
}