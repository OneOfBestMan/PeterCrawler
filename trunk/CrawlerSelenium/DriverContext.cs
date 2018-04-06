// <copyright file="DriverContext.cs" company="Objectivity Bespoke Software Specialists">
// Copyright (c) Objectivity Bespoke Software Specialists. All rights reserved.
// </copyright>
// <license>
//     The MIT License (MIT)
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//     SOFTWARE.
// </license>

namespace CrawlerSelenium
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.PhantomJS;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Safari;
    using NLog;

    namespace CrawlerSelenium
    {
        /// <summary>
        /// Contains handle to driver and methods for web browser
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Driver is disposed on test end")]
        public class DriverContext
        {

            /// <summary>
            /// Gets or sets the handle to current driver.
            /// </summary>
            /// <value>
            /// The handle to driver.
            /// </value>
            private IWebDriver driver;
            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            /// <summary>
            /// Gets Sets Folder name for Download
            /// </summary>
            public string DownloadFolder
            {
                get
                {
                    return FilesHelper.GetFolder(BaseConfiguration.DownloadFolder, this.CurrentDirectory);
                }
            }

            /// <summary>
            /// Gets Sets Folder name for PageSource
            /// </summary>
            public string PageSourceFolder
            {
                get
                {
                    return FilesHelper.GetFolder(BaseConfiguration.PageSourceFolder, this.CurrentDirectory);
                }
            }


            /// <summary>
            /// Gets driver Handle
            /// </summary>
            public IWebDriver Driver
            {
                get
                {
                    return this.driver;
                }
            }

            /// <summary>
            /// Gets or sets directory where assembly files are located
            /// </summary>
            public string CurrentDirectory { get; set; }

            private FirefoxProfile FirefoxProfile
            {
                get
                {
                    FirefoxProfile profile;

                    if (BaseConfiguration.UseDefaultFirefoxProfile)
                    {
                        try
                        {
                            var pathToCurrentUserProfiles = BaseConfiguration.PathToFirefoxProfile; // Path to profile
                            var pathsToProfiles = Directory.GetDirectories(pathToCurrentUserProfiles, "*.default", SearchOption.TopDirectoryOnly);

                            profile = new FirefoxProfile(pathsToProfiles[0]);
                        }
                        catch (DirectoryNotFoundException e)
                        {
                            Logger.Info(CultureInfo.CurrentCulture, "problem with loading firefox profile {0}", e.Message);
                            profile = new FirefoxProfile();
                        }
                    }
                    else
                    {
                        profile = new FirefoxProfile();
                    }

                    // predefined preferences
                    // set browser proxy for firefox
                    if (!string.IsNullOrEmpty(BaseConfiguration.Proxy))
                    {
                        profile.SetProxyPreferences(this.CurrentProxy());
                    }

                    profile.SetPreference("toolkit.startup.max_resumed_crashes", "999999");
                    profile.SetPreference("network.automatic-ntlm-auth.trusted-uris", BaseConfiguration.Host ?? string.Empty);

                    // retrieving settings from config file
                    var firefoxPreferences = ConfigurationManager.GetSection("FirefoxPreferences") as NameValueCollection;
                    var firefoxExtensions = ConfigurationManager.GetSection("FirefoxExtensions") as NameValueCollection;

                    // preference for downloading files
                    profile.SetPreference("browser.download.dir", this.DownloadFolder);
                    profile.SetPreference("browser.download.folderList", 2);
                    profile.SetPreference("browser.download.managershowWhenStarting", false);
                    profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/vnd.ms-excel, application/x-msexcel, application/pdf, text/csv, text/html, application/octet-stream");

                    // disable Firefox's built-in PDF viewer
                    profile.SetPreference("pdfjs.disabled", true);

                    // disable Adobe Acrobat PDF preview plugin
                    profile.SetPreference("plugin.scan.Acrobat", "99.0");
                    profile.SetPreference("plugin.scan.plid.all", false);

                    // custom preferences
                    // if there are any settings
                    if (firefoxPreferences != null)
                    {
                        // loop through all of them
                        for (var i = 0; i < firefoxPreferences.Count; i++)
                        {
                            Logger.Trace(CultureInfo.CurrentCulture, "Set custom preference '{0},{1}'", firefoxPreferences.GetKey(i), firefoxPreferences[i]);

                            // and verify all of them
                            switch (firefoxPreferences[i])
                            {
                                // if current settings value is "true"
                                case "true":
                                    profile.SetPreference(firefoxPreferences.GetKey(i), true);
                                    break;

                                // if "false"
                                case "false":
                                    profile.SetPreference(firefoxPreferences.GetKey(i), false);
                                    break;

                                // otherwise
                                default:
                                    int temp;

                                    // an attempt to parse current settings value to an integer. Method TryParse returns True if the attempt is successful (the string is integer) or return False (if the string is just a string and cannot be cast to a number)
                                    if (int.TryParse(firefoxPreferences.Get(i), out temp))
                                    {
                                        profile.SetPreference(firefoxPreferences.GetKey(i), temp);
                                    }
                                    else
                                    {
                                        profile.SetPreference(firefoxPreferences.GetKey(i), firefoxPreferences[i]);
                                    }

                                    break;
                            }
                        }
                    }

                    // if there are any extensions
                    if (firefoxExtensions != null)
                    {
                        // loop through all of them
                        for (var i = 0; i < firefoxExtensions.Count; i++)
                        {
                            Logger.Trace(CultureInfo.CurrentCulture, "Installing extension {0}", firefoxExtensions.GetKey(i));
                            profile.AddExtension(firefoxExtensions.GetKey(i));
                        }
                    }

                    return profile;
                }
            }

            private ChromeOptions ChromeProfile
            {
                get
                {
                    ChromeOptions options = new ChromeOptions();

                    // retrieving settings from config file
                    var chromePreferences = ConfigurationManager.GetSection("ChromePreferences") as NameValueCollection;
                    var chromeExtensions = ConfigurationManager.GetSection("ChromeExtensions") as NameValueCollection;
                    var chromeArguments = ConfigurationManager.GetSection("ChromeArguments") as NameValueCollection;

                    options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
                    options.AddUserProfilePreference("download.default_directory", this.DownloadFolder);
                    options.AddUserProfilePreference("download.prompt_for_download", false);

                    // set browser proxy for chrome
                    if (!string.IsNullOrEmpty(BaseConfiguration.Proxy))
                    {
                        options.Proxy = this.CurrentProxy();
                    }

                    // custom preferences
                    // if there are any settings
                    if (chromePreferences != null)
                    {
                        // loop through all of them
                        for (var i = 0; i < chromePreferences.Count; i++)
                        {
                            Logger.Trace(CultureInfo.CurrentCulture, "Set custom preference '{0},{1}'", chromePreferences.GetKey(i), chromePreferences[i]);

                            // and verify all of them
                            switch (chromePreferences[i])
                            {
                                // if current settings value is "true"
                                case "true":
                                    options.AddUserProfilePreference(chromePreferences.GetKey(i), true);
                                    break;

                                // if "false"
                                case "false":
                                    options.AddUserProfilePreference(chromePreferences.GetKey(i), false);
                                    break;

                                // otherwise
                                default:
                                    int temp;

                                    // an attempt to parse current settings value to an integer. Method TryParse returns True if the attempt is successful (the string is integer) or return False (if the string is just a string and cannot be cast to a number)
                                    if (int.TryParse(chromePreferences.Get(i), out temp))
                                    {
                                        options.AddUserProfilePreference(chromePreferences.GetKey(i), temp);
                                    }
                                    else
                                    {
                                        options.AddUserProfilePreference(chromePreferences.GetKey(i), chromePreferences[i]);
                                    }

                                    break;
                            }
                        }
                    }

                    // if there are any extensions
                    if (chromeExtensions != null)
                    {
                        // loop through all of them
                        for (var i = 0; i < chromeExtensions.Count; i++)
                        {
                            Logger.Trace(CultureInfo.CurrentCulture, "Installing extension {0}", chromeExtensions.GetKey(i));
                            options.AddExtension(chromeExtensions.GetKey(i));
                        }
                    }

                    if (BaseConfiguration.ChromePath != null)
                    {
                        options.BinaryLocation = BaseConfiguration.ChromePath;
                    }

                    // if there are any arguments
                    if (chromeArguments != null)
                    {
                        // loop through all of them
                        for (var i = 0; i < chromeArguments.Count; i++)
                        {
                            options.AddArgument(chromeArguments.GetKey(i));
                        }
                    }

                    return options;
                }
            }

            private InternetExplorerOptions InternetExplorerProfile
            {
                get
                {
                    var options = new InternetExplorerOptions
                    {
                        EnsureCleanSession = true,
                        IgnoreZoomLevel = true,
                    };

                    // set browser proxy for IE
                    if (!string.IsNullOrEmpty(BaseConfiguration.Proxy))
                    {
                        options.Proxy = this.CurrentProxy();
                    }

                    return options;
                }
            }

            private SafariOptions SafariProfile
            {
                get
                {
                    var options = new SafariOptions();
                    options.AddAdditionalCapability("cleanSession", true);

                    // set browser proxy for Safari
                    if (!string.IsNullOrEmpty(BaseConfiguration.Proxy))
                    {
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Use command line to setup proxy"));
                    }

                    return options;
                }
            }


            /// <summary>
            /// Starts the specified Driver.
            /// </summary>
            /// <exception cref="NotSupportedException">When driver not supported</exception>
            [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Driver disposed later in stop method")]
            public void Start()
            {
                switch (BaseConfiguration.TestBrowser)
                {
                    case BrowserType.Firefox:
                        var fireFoxOptionsLegacy = new FirefoxOptions { Profile = this.FirefoxProfile, UseLegacyImplementation = BaseConfiguration.FirefoxUseLegacyImplementation };
                        this.driver = new FirefoxDriver(fireFoxOptionsLegacy);
                        break;
                    case BrowserType.FirefoxPortable:
                        var fireFoxOptions = new FirefoxOptions { BrowserExecutableLocation = BaseConfiguration.FirefoxPath, Profile = this.FirefoxProfile, UseLegacyImplementation = BaseConfiguration.FirefoxUseLegacyImplementation };
                        this.driver = new FirefoxDriver(fireFoxOptions);
                        break;
                    case BrowserType.InternetExplorer:
                        this.driver = new InternetExplorerDriver(this.InternetExplorerProfile);
                        break;
                    case BrowserType.Chrome:
                        //this.driver = new ChromeDriver(this.ChromeProfile);
                        this.driver = new ChromeDriver(BaseConfiguration.ChromePath);
                        break;
                    case BrowserType.Safari:
                        this.driver = new SafariDriver(this.SafariProfile);
                        break;
                    case BrowserType.PhantomJs:
                        this.driver = new PhantomJSDriver(this.CurrentDirectory + BaseConfiguration.PhantomJsPath);
                        break;
                    case BrowserType.RemoteWebDriver:
                        this.driver = new RemoteWebDriver(BaseConfiguration.RemoteWebDriverHub, this.SetCapabilities());
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format(CultureInfo.CurrentCulture, "Driver {0} is not supported", BaseConfiguration.TestBrowser));
                }

                this.driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(BaseConfiguration.LongTimeout);
                this.driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(BaseConfiguration.ShortTimeout);
                this.driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(BaseConfiguration.ImplicitlyWaitMilliseconds);
            }

            /// <summary>
            /// Maximizes the current window if it is not already maximized.
            /// </summary>
            public void WindowMaximize()
            {
                this.driver.Manage().Window.Maximize();
            }

            /// <summary>
            /// Deletes all cookies from the page.
            /// </summary>
            public void DeleteAllCookies()
            {
                this.driver.Manage().Cookies.DeleteAllCookies();
            }

            /// <summary>
            /// Stop browser instance.
            /// </summary>
            public void Stop()
            {
                if (this.driver != null)
                {
                    this.driver.Quit();
                }
            }


            /// <summary>
            /// Set web driver capabilities.
            /// </summary>
            /// <returns>Instance with set web driver capabilities.</returns>
            private DesiredCapabilities SetCapabilities()
            {
                DesiredCapabilities capabilities = new DesiredCapabilities();

                switch (BaseConfiguration.TestBrowserCapabilities)
                {
                    case BrowserType.Firefox:
                        capabilities = DesiredCapabilities.Firefox();
                        capabilities.SetCapability(FirefoxDriver.ProfileCapabilityName, this.FirefoxProfile.ToBase64String());
                        break;
                    case BrowserType.InternetExplorer:
                        capabilities = DesiredCapabilities.InternetExplorer();
                        break;
                    case BrowserType.Chrome:
                        capabilities = DesiredCapabilities.Chrome();
                        break;
                    case BrowserType.Safari:
                        capabilities = DesiredCapabilities.Safari();
                        break;
                    default:
                        throw new NotSupportedException(
                            string.Format(CultureInfo.CurrentCulture, "Driver {0} is not supported with Selenium Grid", BaseConfiguration.TestBrowser));
                }

                var driverCapabilitiesConf = ConfigurationManager.GetSection("DriverCapabilities") as NameValueCollection;

                // if there are any capability
                if (driverCapabilitiesConf != null)
                {
                    // loop through all of them
                    for (var i = 0; i < driverCapabilitiesConf.Count; i++)
                    {
                        string value = driverCapabilitiesConf.GetValues(i)[0];
                        Logger.Trace(CultureInfo.CurrentCulture, "Adding driver capability {0}", driverCapabilitiesConf.GetKey(i));
                        capabilities.SetCapability(driverCapabilitiesConf.GetKey(i), value);
                    }
                }
                return capabilities;
            }

            private Proxy CurrentProxy()
            {
                Proxy proxy = new Proxy
                {
                    HttpProxy = BaseConfiguration.Proxy,
                    FtpProxy = BaseConfiguration.Proxy,
                    SslProxy = BaseConfiguration.Proxy,
                    SocksProxy = BaseConfiguration.Proxy
                };
                return proxy;
            }
        }
    }
}
