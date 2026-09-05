const std = @import("std");

pub fn build(b: *std.Build) void {
    const target = b.standardTargetOptions(.{});
    const optimize = b.standardOptimizeOption(.{});

    const runtime = b.addLibrary(.{
        .name = "sigil_runtime",
        .root_module = b.createModule(.{
            .root_source_file = b.path("src/runtime.zig"),
            .target = target,
            .optimize = optimize,
        }),
        .linkage = .static,
    });

    b.installArtifact(runtime);

    const runtime_tests = b.addTest(.{
        .root_module = runtime.root_module,
    });

    const run_tests = b.addRunArtifact(runtime_tests);

    const test_step = b.step("test", "Run runtime tests");
    test_step.dependOn(&run_tests.step);
}
