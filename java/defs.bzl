load(
    "@contrib_rules_jvm//java:defs.bzl",
    _JUNIT5_DEPS = "JUNIT5_DEPS",
)
load(
    "@rules_java//java:defs.bzl",
    _java_binary = "java_binary",
    _java_import = "java_import",
)
load("@rules_jvm_external//:defs.bzl", _artifact = "artifact", _javadoc = "javadoc", _maven_bom = "maven_bom")
load("//java/private:dist_zip.bzl", _java_dist_zip = "java_dist_zip")
load("//java/private:java_test_suite.bzl", _java_test_suite = "java_test_suite")
load("//java/private:library.bzl", _java_export = "java_export", _java_library = "java_library", _java_test = "java_test")
load("//java/private:merge_jars.bzl", _merge_jars = "merge_jars")
load("//java/private:module.bzl", _java_module = "java_module")
load("//java/private:selenium_test.bzl", _selenium_test = "selenium_test")
load("//java/private:suite.bzl", _java_selenium_test_suite = "java_selenium_test_suite")

def java_test_suite(name, runner = "junit5", **kwargs):
    _java_test_suite(name = name, runner = runner, **kwargs)

artifact = _artifact
java_binary = _java_binary
java_dist_zip = _java_dist_zip
java_export = _java_export
java_import = _java_import
java_library = _java_library
java_module = _java_module
java_selenium_test_suite = _java_selenium_test_suite
java_test = _java_test
javadoc = _javadoc
maven_bom = _maven_bom
merge_jars = _merge_jars
selenium_test = _selenium_test
JUNIT5_DEPS = _JUNIT5_DEPS
