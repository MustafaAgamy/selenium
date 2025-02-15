// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SFC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

package org.openqa.selenium.testing;

import java.io.File;
import java.io.IOException;
import java.io.UncheckedIOException;
import java.nio.file.Files;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import org.openqa.selenium.Capabilities;
import org.openqa.selenium.HasCapabilities;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.Platform;
import org.openqa.selenium.WebDriver;

public class TestUtilities {

  public static String getUserAgent(WebDriver driver) {
    try {
      return (String) ((JavascriptExecutor) driver).executeScript("return navigator.userAgent;");
    } catch (Throwable e) {
      // Some drivers will only execute JS once a page has been loaded. Since those
      // drivers aren't Firefox or IE, we don't worry about that here.
      return "";
    }
  }

  public static boolean isFirefox(WebDriver driver) {
    return getUserAgent(driver).contains("Firefox");
  }

  public static boolean isFirefoxVersionOlderThan(int version, WebDriver driver) {
    return isFirefox(driver) && getFirefoxVersion(driver) < version;
  }

  public static boolean isInternetExplorer(WebDriver driver) {
    String userAgent = getUserAgent(driver);
    return userAgent != null && (userAgent.contains("MSIE") || userAgent.contains("Trident"));
  }

  public static boolean isChrome(WebDriver driver) {
    return getUserAgent(driver).contains("Chrome");
  }

  public static int getChromeVersion(WebDriver driver) {
    if (!(driver instanceof HasCapabilities)) {
      // Driver does not support capabilities -- not a chromedriver at all.
      return 0;
    }
    Capabilities caps = ((HasCapabilities) driver).getCapabilities();
    String chromedriverVersion = (String) caps.getCapability("chrome.chromedriverVersion");
    if (chromedriverVersion == null) {
      Object chrome = caps.getCapability("chrome");
      if (chrome != null) {
        chromedriverVersion = (String) ((Map<?, ?>) chrome).get("chromedriverVersion");
      }
    }
    if (chromedriverVersion != null) {
      String[] versionMajorMinor = chromedriverVersion.split("\\.", 2);
      if (versionMajorMinor.length > 1) {
        try {
          return Integer.parseInt(versionMajorMinor[0]);
        } catch (NumberFormatException e) {
          // First component of the version is not a number -- not a chromedriver.
          return 0;
        }
      }
    }
    return 0;
  }

  /**
   * Finds the Firefox version of the given webdriver and returns it as an integer. For instance,
   * '14.0.1' will translate to 14.
   *
   * @param driver The driver to find the version for.
   * @return The found version, or 0 if no version could be found.
   */
  public static int getFirefoxVersion(WebDriver driver) {
    // extract browser string
    Pattern browserPattern = Pattern.compile("Firefox/\\d+.");
    Matcher browserMatcher = browserPattern.matcher(getUserAgent(driver));
    if (!browserMatcher.find()) {
      return 0;
    }
    String browserStr = browserMatcher.group();

    // extract version string
    Pattern versionPattern = Pattern.compile("\\d+");
    Matcher versionMatcher = versionPattern.matcher(browserStr);
    if (!versionMatcher.find()) {
      return 0;
    }
    return Integer.parseInt(versionMatcher.group());
  }

  /**
   * Finds the IE major version of the given webdriver and returns it as an integer. For instance,
   * '10.6' will translate to 10.
   *
   * @param driver The driver to find the version for.
   * @return The found version, or 0 if no version could be found.
   */
  public static int getIEVersion(WebDriver driver) {
    String userAgent = getUserAgent(driver);
    // extract browser string
    Pattern browserPattern = Pattern.compile("MSIE\\s+\\d+\\.");
    Matcher browserMatcher = browserPattern.matcher(userAgent);
    // IE dropped the "MSIE" token from its user agent string starting with IE11.
    Pattern tridentPattern = Pattern.compile("Trident/\\d+\\.");
    Matcher tridentMatcher = tridentPattern.matcher(userAgent);

    Matcher versionMatcher;
    if (browserMatcher.find()) {
      versionMatcher = Pattern.compile("(\\d+)").matcher(browserMatcher.group());
    } else if (tridentMatcher.find()) {
      versionMatcher = Pattern.compile("rv:(\\d+)").matcher(userAgent);
    } else {
      return Integer.MAX_VALUE; // Because people check to see if we're at this version or less
    }

    // extract version string
    if (!versionMatcher.find()) {
      return 0;
    }
    return Integer.parseInt(versionMatcher.group(1));
  }

  public static Platform getEffectivePlatform() {
    return Platform.getCurrent();
  }

  /** Returns Platform where the browser (driven by given WebDriver) runs on. */
  public static Platform getEffectivePlatform(WebDriver driver) {
    if (!(driver instanceof HasCapabilities)) {
      throw new RuntimeException("WebDriver must implement HasCapabilities");
    }

    Capabilities caps = ((HasCapabilities) driver).getCapabilities();
    return caps.getPlatformName();
  }

  public static boolean isLocal() {
    return !(Boolean.getBoolean("selenium.browser.remote")
        || Boolean.getBoolean("selenium.browser.grid"));
  }

  public static boolean isOnTravis() {
    return Boolean.parseBoolean(System.getenv("TRAVIS"));
  }

  public static boolean isOnGitHubActions() {
    return Boolean.parseBoolean(System.getenv("GITHUB_ACTIONS"));
  }

  public static File createTmpFile(String content) {
    try {
      File f = File.createTempFile("webdriver", "tmp");
      f.deleteOnExit();
      Files.writeString(f.toPath(), content);
      return f;
    } catch (IOException e) {
      throw new UncheckedIOException(e);
    }
  }
}
