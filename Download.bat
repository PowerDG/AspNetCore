title "下载AspNetCore源码"
@echo off & setlocal enabledelayedexpansion
set version=2.2
set modules=MetaPackages BasicMiddleware WebSockets WebHooks StaticFiles Session Security ResponseCaching Mvc Options Caching Logging Localization KestrelHttpServer JavaScriptServices IISIntegration Identity HttpClientFactory HttpAbstractions HtmlAbstractions FileSystem Hosting Configuration CORS DataProtection DependencyInjection Diagnostics DotNetTools EntityFrameworkCore Extensions Routing DataProtection HttpSysServer

(for %%a in (%modules%) do ( 
echo 正在下载%%a
git clone https://github.com/aspnet/%%a.git Modules/%%a -b release/%version%
))

pause