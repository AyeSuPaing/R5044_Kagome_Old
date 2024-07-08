<%--
=========================================================================================================
  Module      : Friend Referral (FriendReferral.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FriendReferral/FriendReferral.aspx.cs" Inherits="Form_FriendReferral" Title="お友達紹介コードページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div id="dvUserFltContents">
		<h2>紹介URLの発行</h2>
		<div id="dvUserReferralCode" class="unit">
			<table cellspacing="0">
				<tr>
					<th style="width: 18%"><%= ReplaceTag("@@User.referral_code.name@@") %></th>
					<td>
						<%: this.ReferralCode %>
					</td>
				</tr>
				<tr>
					<th style="width: 100px"><%= ReplaceTag("@@User.referral_url.name@@") %></th>
					<td>
						<%: string.Format(GetReferralUrl()) %>
						<asp:LinkButton ID="lbCopy" runat="server" OnClientClick="return ClipboardUrlCopy()" class="btn btn-small btn-inverse" Text="  コピー  " />
					</td>
				</tr>
			</table>
		</div>
	</div>
	<script type="text/javascript">
		// Clipboard url copy
		function ClipboardUrlCopy() {
			var copyText = '<%: GetReferralUrl() %>';

			if (window.clipboardData
				&& window.clipboardData.setData)
			{
				return clipboardData.setData("Text", copyText);
			}
			else if (document.queryCommandSupported
				&& document.queryCommandSupported("copy"))
			{
				// Create element save text copy
				var textarea = document.createElement("textarea");
				textarea.textContent = copyText;
				textarea.style.position = "fixed";
				document.body.appendChild(textarea);
				textarea.select();

				// Exec copy to clipboard and notification
				document.execCommand("copy");
				document.body.removeChild(textarea);
				alert("紹介URLをクリップボードにコピーしました。");
			}
		}
	</script>
</asp:Content>
