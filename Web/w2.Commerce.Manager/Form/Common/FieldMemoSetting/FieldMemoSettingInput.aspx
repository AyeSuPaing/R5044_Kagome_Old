<%--
=========================================================================================================
  Module      : 項目メモ編集ページ(FieldMemoSettingInput.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FieldMemoSettingInput.aspx.cs" Inherits="Form_Common_FieldMemoSetting_FieldMemoSettingInput" %>
<html>
<head>
<title></title>
<style type="text/css">
	html {
		overflow-y: hidden;
	}
	 body
	 {
		 margin: 0;
		 padding: 0;
	 }	
	.memo_edit_textarea {
		width: 255px;
		height: 177px;
		resize: none;
	}
</style>
</head>
<body>
<form id="memoEditForm" method="post" action="" runat="server">
	<asp:HiddenField id="hfTableName" runat="server"/>
	<asp:HiddenField id="hfFieldName" runat="server"/>
	<img height="5" alt="" src="../../../Images/Common/sp.gif" width="100%" border="0" />
	<div id="memo_content_edit" class="memo_area" runat="server">
		<asp:TextBox id="tbMemoEdit" TextMode="MultiLine" class="memo_edit_textarea" Text="" runat="server" />
	</div>
	<img height="5" alt="" src="../../../Images/Common/sp.gif" width="100%" border="0" />
	<div style="text-align:right">
		<asp:Button id="btnMemoUpdate" runat="server" Text="  更新する  " OnClientClick="if ( ! UserUpdateConfirmation()) return false;"  OnClick="btnMemoUpdate_Click" />
	</div>
</form>

<script type="text/javascript">
	function UserUpdateConfirmation() {
		return confirm("項目メモの内容を更新します。よろしいですか?");
	}
</script>

</body>
</html>