const std = @import("std");

export fn sigil_println_integer(value: i64) void {
    std.debug.print("{d}\n", .{value});
}

export fn sigil_println_float(value: f64) void {
    std.debug.print("{d}\n", .{value});
}
