@rem プラグインDLLのテスト用配置ツール
@rem IIS再起動してから配置するので気を付けてね

set targetdir=C:\inetpub\wwwroot\Solutions\P0011_Intercomv5.3\Plugins\P0011_Intercom\w2.Plugin.P0011_Intercom\bin\Debug
set copydir=C:\inetpub\wwwroot\Solutions\P0011_Intercomv5.3\Commons\w2.CustomerResources\Plugins
set xmltargetdir=C:\inetpub\wwwroot\Solutions\P0011_Intercomv5.3\Plugins\P0011_Intercom\w2.Plugin.P0011_Intercom\bin\Debug\xml
set xmlcopydir=C:\inetpub\wwwroot\Solutions\P0011_Intercomv5.3\Commons\w2.CustomerResources\Plugins\xml
set copedir=C:\inetpub\wwwroot\Solutions\P0011_Intercomv5.3\Plugins\P0011_Intercom\w2.Plugin.P0011_Intercom.CooperationWebService\bin\Debug\


iisreset
mkdir %xmlcopydir%
copy /Y %xmltargetdir%\P0011_IntercomPluginConfig.xml %xmlcopydir%\P0011_IntercomPluginConfig.xml
copy /Y %xmltargetdir%\SqlSettings.xml %xmlcopydir%\SqlSettings.xml
copy /Y %targetdir%\w2.Crypto.dll %copydir%\w2.Crypto.dll
copy /Y %targetdir%\w2.Plugin.P0011_Intercom.dll %copydir%\w2.Plugin.P0011_Intercom.dll
copy /Y %copedir%w2.Plugin.P0011_Intercom.CooperationWebService.dll %copydir%\w2.Plugin.P0011_Intercom.CooperationWebService.dll

exit

