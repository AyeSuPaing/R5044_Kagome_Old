<%--
=========================================================================================================
  Module      : メールマガジン登録画面(BodyMailMagazineRegistration.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="BodyMailMagazineRegistrationControl" %>
<%@ Register TagPrefix="uc" TagName="MailDomains" Src="~/Form/Common/MailDomains.ascx" %>
<%-- 各種Js読み込み --%>
<uc:MailDomains id="MailDomains" runat="server"></uc:MailDomains>
<%-- ▽編集可能領域：コンテンツ▽ --%>
<style>
	.user-unit {
		margin-bottom: 2em;
	}

		.user-unit .user-form dd input {
			padding: .5em;
		}

		.user-unit .user-form {
			margin: 1em auto;
		}

			.user-unit .user-form dt,
			.user-unit .user-form dd {
				padding: .5em;
				line-height: 1.5;
			}

			.user-unit .user-form dt {
				background-color: #ccc;
			}

		.user-unit .msg {
			padding: .5em;
			line-height: 1.5;
			font-size: 12px;
		}

		.user-unit .require {
			color: #f00;
			font-weight: bold;
			margin-left: 3px;
		}

		.user-unit .attention {
			font-size: 11px;
			color: #f00;
			line-height: 1.5;
		}

	.user-footer {
		padding: 0 1em 1em 1em;
	}

		.user-footer .button-next .btn {
			padding: 1.5em 0;
			background-color: #000;
			color: #fff;
		}

	.user-form .mail input {
		width: 100%;
		margin-bottom: .2em;
	}
</style>
<%-- ▽ミニカート（UpdatePanel）▽ --%>
<asp:UpdatePanel ID="upMailMagazine" runat="server">
<ContentTemplate>
<div class="user-unit">
	<div runat="server" id="dvMailMagazineRegistInput">
		<section class="wrap-user mailmagazine-regist-input" runat="server">
			<div class="user-unit" runat="server">
				<dl class="user-form">
					<dt class="title">メールマガジン登録</dt>
				</dl>
				<p class="msg">
					メールマガジンを登録される方は、下記のフォームにメールアドレスをご入力の上、「登録する」ボタンをクリックして下さい。<br />
					<span class="attention">※は必須入力となります。</span>
				</p>

				<dl class="user-form">
					<dt class="title">メールアドレス<span class="require">※</span>
					</dt>
					<dd class="mail">
						<p class="attention">
							<asp:CustomValidator ID="cvUserMailAddr" runat="server"
								ControlToValidate="tbUserMailAddr"
								ValidationGroup="MailMagazineRegist"
								ValidateEmptyText="true"
								SetFocusOnError="true"
								ClientValidationFunction="ClientValidate"
								EnableClientScript="false" />
						</p>
						<asp:TextBox ID="tbUserMailAddr" Type="email" runat="server" MaxLength="256" />
					</dd>
				</dl>
				<div class="user-footer user-unit">
					<div class="button-next">
						<asp:LinkButton CssClass="btn" ValidationGroup="MailMagazineRegist" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click">登録する</asp:LinkButton>
					</div>
				</div>
			</div>
		</section>
	</div>

	<div runat="server" id="dvUserModifyConfirm">
		<section class="wrap-user mailmagazine-regist-complete" runat="server">
			<div class="user-unit" runat="server">
				<dl class="user-form">
					<dt class="title">登録完了</dt>
				</dl>
				<p class="msg">
					メールマガジン配信の登録が完了しました。<br />
					今後とも、「<%: ShopMessage.GetMessage("ShopName") %>」をどうぞ宜しくお願い申し上げます。<br />
					<br />
					<%: ShopMessage.GetMessage("ContactCenterInfo") %>
				</p>
			</div>
		</section>
	</div>
</div>
</ContentTemplate>
</asp:UpdatePanel>
<%-- △ミニカート（UpdatePanel）△ --%>
<%-- △編集可能領域△ --%>