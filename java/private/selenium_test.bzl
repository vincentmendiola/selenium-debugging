load(
    "//common:browsers.bzl",
    "COMMON_TAGS",
    "chrome_data",
    "edge_data",
    "firefox_beta_data",
    "firefox_data",
)
load(
    "//java:browsers.bzl",
    "chrome_jvm_flags",
    "edge_jvm_flags",
    "firefox_beta_jvm_flags",
    "firefox_jvm_flags",
)
load(":junit5_test.bzl", "junit5_test")

DEFAULT_BROWSER = "firefox"

BROWSERS = {
    "chrome": {
        "deps": ["//java/src/org/openqa/selenium/chrome"],
        "jvm_flags": ["-Dselenium.browser=chrome"] + chrome_jvm_flags,
        "data": chrome_data,
        "tags": COMMON_TAGS + ["chrome"],
    },
    "edge": {
        "deps": ["//java/src/org/openqa/selenium/edge"],
        "jvm_flags": ["-Dselenium.browser=edge"] + edge_jvm_flags,
        "data": edge_data,
        "tags": COMMON_TAGS + ["edge"],
    },
    "firefox": {
        "deps": ["//java/src/org/openqa/selenium/firefox"],
        "jvm_flags": ["-Dselenium.browser=ff"] + firefox_jvm_flags,
        "data": firefox_data,
        "tags": COMMON_TAGS + ["firefox"],
    },
    "firefox-beta": {
        "deps": ["//java/src/org/openqa/selenium/firefox"],
        "jvm_flags": ["-Dselenium.browser=ff"] + firefox_beta_jvm_flags,
        "data": firefox_beta_data,
        "tags": COMMON_TAGS + ["firefox", "firefox-beta"],
    },
    "ie": {
        "deps": ["//java/src/org/openqa/selenium/ie"],
        "jvm_flags": ["-Dselenium.browser=ie"] +
                     select({
                         "@selenium//common:windows": ["-Dselenium.skiptest=false"],
                         "@selenium//conditions:default": ["-Dselenium.skiptest=true"],
                     }),
        "data": [],
        "tags": COMMON_TAGS + ["exclusive-if-local", "ie", "skip-remote"],
    },
    "safari": {
        "deps": ["//java/src/org/openqa/selenium/safari"],
        "jvm_flags": ["-Dselenium.browser=safari"] +
                     select({
                         "@selenium//common:macos": ["-Dselenium.skiptest=false"],
                         "@selenium//conditions:default": ["-Dselenium.skiptest=true"],
                     }),
        "data": [],
        "tags": COMMON_TAGS + ["exclusive-if-local", "safari", "skip-remote"],
    },
}

DEFAULT_BROWSERS = [b for b in BROWSERS.keys() if b != "ie"]

def selenium_test(name, test_class, size = "medium", browsers = DEFAULT_BROWSERS, **kwargs):
    if len(browsers) == 0:
        fail("At least one browser must be specified.")

    default_browser = DEFAULT_BROWSER if DEFAULT_BROWSER in browsers else browsers[0]

    test_name = test_class.rpartition(".")[2]

    data = kwargs["data"] if "data" in kwargs else []
    jvm_flags = kwargs["jvm_flags"] if "jvm_flags" in kwargs else []
    tags = kwargs["tags"] if "tags" in kwargs else []

    remote = False
    if "selenium-remote" in tags:
        tags.remove("selenium-remote")
        remote = True

    stripped_args = dict(**kwargs)
    stripped_args.pop("data", None)
    stripped_args.pop("jvm_flags", None)
    stripped_args.pop("tags", None)
    inherited_env = stripped_args.pop("env_inherit", []) + ["REMOTE_BUILD"]

    all_tests = []

    for browser in browsers:
        if not browser in BROWSERS:
            fail("Unrecognized browser: " + browser)

        test = name if browser == default_browser else "%s-%s" % (name, browser)

        junit5_test(
            name = test,
            test_class = test_class,
            size = size,
            jvm_flags = BROWSERS[browser]["jvm_flags"] + jvm_flags,
            # Only allow linting on the default test
            tags = BROWSERS[browser]["tags"] + tags + ([] if test == name else ["no-lint"]),
            data = BROWSERS[browser]["data"] + data,
            env_inherit = inherited_env,
            **stripped_args
        )
        if browser == default_browser:
            native.alias(
                name = "%s-%s" % (name, browser),
                actual = test,
            )
        all_tests.append(":%s" % test)

        if remote:
            junit5_test(
                name = "%s-remote" % test,
                test_class = test_class,
                size = size,
                jvm_flags = BROWSERS[browser]["jvm_flags"] + jvm_flags + [
                    "-Dselenium.browser.remote=true",
                ],
                # No need to lint remote tests as the code for non-remote is the same and they get linted
                tags = BROWSERS[browser]["tags"] + tags + ["remote-browser", "no-lint"],
                data = BROWSERS[browser]["data"] + data + [
                    "@selenium//java/src/org/openqa/selenium/grid:selenium_server",
                ],
                **stripped_args
            )
            all_tests.append(":%s-remote" % test)

    # Handy way to run everything
    native.test_suite(name = "%s-all-browsers" % name, tests = all_tests, tags = tags + ["manual"])
