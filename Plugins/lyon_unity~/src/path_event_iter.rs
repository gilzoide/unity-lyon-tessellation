use std::iter::Iterator;
use lyon_path::Event;
use lyon_path::PathEvent;
use lyon_path::math::Point;

#[repr(C)]
pub struct UnityPathEvent {
    event: i32,
    close: i32,
    from: Point,
    to: Point,
    ctrl1: Point,
    ctrl2: Point,
}

pub struct UnityPathEventIter {
    events: *const UnityPathEvent,
    len: usize,
    current: usize,
}

impl UnityPathEventIter {
    pub fn new(events: *const UnityPathEvent, len: i32) -> Self {
        Self {
            events: events,
            len: len as usize,
            current: 0,
        }
    }
}

impl Iterator for UnityPathEventIter {
    type Item = PathEvent;

    fn next(&mut self) -> Option<Self::Item> {
        if self.current >= self.len {
            None
        }
        else {
            unsafe {
                let current_event = &*self.events.add(self.current);
                match current_event.event {
                    0 => Some(Event::Begin {
                        at: current_event.from,
                    }),
                    1 => Some(Event::Line {
                        from: current_event.from,
                        to: current_event.to,
                    }),
                    2 => Some(Event::Quadratic {
                        from: current_event.from,
                        ctrl: current_event.ctrl1,
                        to: current_event.to,
                    }),
                    3 => Some(Event::Cubic {
                        from: current_event.from,
                        ctrl1: current_event.ctrl1,
                        ctrl2: current_event.ctrl2,
                        to: current_event.to,
                    }),
                    4 => Some(Event::End {
                        last: current_event.from,
                        first: current_event.to,
                        close: current_event.close != 0,
                    }),
                    _ => None,
                }
            }
        }
    }
}