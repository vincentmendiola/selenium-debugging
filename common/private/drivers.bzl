def _symlink_if_available(repository_ctx, driver_name):
    driver = repository_ctx.which(driver_name)

    if driver:
        repository_ctx.symlink(driver, driver_name)
    else:
        repository_ctx.file(driver_name, "")

    return "\n".join([
        "bool_setting(name = \"use_%s\", build_setting_default = %s)" % (driver_name, driver != None),
        "",
        "exports_files([\"%s\"])" % driver_name,
        "",
        """js_library(name = "%s-js", data = [":%s"])""" % (driver_name, driver_name),
        "",
    ])

def _local_drivers_impl(repository_ctx):
    contents = [
        "load(\"@bazel_skylib//rules:common_settings.bzl\", \"bool_setting\")",
        "load(\"@aspect_rules_js//js:defs.bzl\", \"js_library\")",
        "",
        "package(default_visibility = [\"//visibility:public\"])",
        _symlink_if_available(repository_ctx, "chromedriver"),
        _symlink_if_available(repository_ctx, "msedgedriver"),
        _symlink_if_available(repository_ctx, "geckodriver"),
    ]

    repository_ctx.file("BUILD.bazel", "\n".join(contents))

local_drivers = repository_rule(
    _local_drivers_impl,
)
