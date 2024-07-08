<%--
=========================================================================================================
  Module      : マスタダウンロード系の出力コントローラ(MasterDownLoadPanel.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterDownLoadPanel.ascx.cs" Inherits="Form_Common_MasterDownLoadPanel" %>
<table cellspacing="1" cellpadding="2" border="0" runat="server" id="tMasterDownload" width='<%#string.IsNullOrEmpty(TableWidth) ? "758" : this.TableWidth%>'>
<tr>
	<td class="search_title_bg" width="110"><img alt="" class="search_title_arrow" src="../../Images/Common/arrow_01.gif" />データ出力</td>
	<td align="left" class="search_item_bg">
		<asp:DropDownList id="ddlExportSetting" Width="530" runat="server" ></asp:DropDownList>&nbsp;&nbsp;<asp:Button id="btnExport" runat="server" Text="  出力  " OnClick="btnExport_Click" />
	</td>
</tr>
</table>