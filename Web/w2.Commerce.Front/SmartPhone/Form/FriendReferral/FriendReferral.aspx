<%--
=========================================================================================================
  Module      : Friend Referral (FriendReferral.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/FriendReferral/FriendReferral.aspx.cs" Inherits="Form_FriendReferral" Title="お友達紹介コードページ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<section class="wrap-user user-mofify-input">
			<div class="user-unit">
				<h2>紹介URLの発行</h2>
				<dl class="user-form">
					<dt><%= ReplaceTag("@@User.referral_code.name@@") %></dt>
					<dd>
						<%: this.ReferralCode %>
					</dd>
					<dt><%= ReplaceTag("@@User.referral_url.name@@") %></dt>
					<dd class="userReferralCode">
						<p>
							<%: string.Format(GetReferralUrl()) %>
							<asp:LinkButton ID="lbCopy" runat="server" CssClass="btn-copy" OnClientClick="return ClipboardUrlCopy()" Text="  コピー  " />
						</p>
					</dd>
				</dl>
			</div>
	</section>

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
