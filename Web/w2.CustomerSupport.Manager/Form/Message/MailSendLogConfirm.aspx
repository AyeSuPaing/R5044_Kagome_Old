<%--
=========================================================================================================
  Module      : メール送信ログ確認ページ(MailSendLogConfirm.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
--%>
<%@ Import Namespace="w2.Common.Web" %>
<%@ Import Namespace="w2.App.Common.Cs.Message" %>
<%@ Import Namespace="w2.App.Common.Cs.SummarySetting" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="MailSendLogConfirm.aspx.cs" Inherits="Form_Message_MailSendLogConfirm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<script type="text/javascript">
	// HTML文書表示用フレームのウィンドウ幅調整
	(function (window, $) {
		$(window).bind('load', function () {
			$('iframe.HtmlPreview').each(function () {
				var doc = $(this).get(0).contentWindow.document;
				var innerHeight = Math.max(
					doc.body.scrollHeight, doc.documentElement.scrollHeight,
					doc.body.offsetHeight, doc.documentElement.offsetHeight,
					doc.body.clientHeight, doc.documentElement.clientHeight);
				$(this).removeAttr("height").css('height', innerHeight + 'px');
			});
		});
	})(window, jQuery);
</script>
<img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" />
<% if (this.ExistMailSendLog == false){ %>
<table class="error_table" width="500" border="0" cellspacing="1" cellpadding="3">
	<tr class="error_title_bg">
		<td align="left">下記の内容についてエラーが発生しました。</td>
	</tr>
	<tr class="error_item_bg">
		<td align="left">
			<div class="error_text">
				<asp:Label id="lNotExistErrorMessage" runat="server" />
			</div>
		</td>
	</tr>
</table>
<% } %>
<% if (this.ExistMailSendLog){ %>
<table class="detail_table" cellspacing="1" cellpadding="3" width="96%" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">基本情報</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="120">メール送信ログNo</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lLogNo" runat="server" /></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">既読フラグ</td>
		<td class="detail_item_bg" align="left">
			<asp:CheckBox ID="checkBoxReadFlg" runat="server" Enabled="false" CssClass="checkBoxZoomNormal"/>
		</td>
	</tr>
	<% if (this.IsMailTemplate == false) { %>
	<tr>
		<td class="detail_title_bg" align="left">メール文章名&nbsp(ID)</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailTextName" runat="server" /> ( <asp:Literal ID="lMailTextId" runat="server" /> )</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">メール配信設定名&nbsp(ID)</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailDistName" runat="server"/> ( <asp:Literal ID="lMailDistId" runat="server" /> )</td>
	</tr>
	<% } %>
	<% if (this.IsMailTemplate) { %>
	<tr>
		<td class="detail_title_bg" align="left">メールテンプレート名&nbsp(ID)</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailName" runat="server" /> ( <asp:Literal ID="lMailId" runat="server" /> )</td>
	</tr>
	<% } %>
	<tr>
		<td class="detail_title_bg" align="left">宛先</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailTo" runat="server" /></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">Cc</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailCc" runat="server" /></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">Bcc</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailBcc" runat="server" /></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">送信元</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailFrom" runat="server" /></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">送信日時</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lDateSendMail" runat="server" /></td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left">既読日時</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lDateReadMail" runat="server" /></td>
	</tr>
</table>
<img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" />

<div class="text_top btns-filter" style="margin-top:30px;">
<span class="btn-group">
	<asp:LinkButton ID="lbDisplayText" runat="server" OnCommand="lbDisplay_Click" CommandName="DisplayText">テキスト表示</asp:LinkButton>
	<asp:LinkButton ID="lbDisplayHtml" runat="server" OnCommand="lbDisplay_Click" CommandName="DisplayHtml">HTML表示</asp:LinkButton>
</span>
<div id="dMessage" style="white-space:nowrap;margin-top:10px" runat="server">
	表示されているユーザー情報は、現時点（<%: DateTimeUtility.ToStringForManager(DateTime.Now, DateTimeUtility.FormatType.ShortDateHourMinute2Letter) %>）の情報です。送信時と異なる場合がありますのでご注意ください。
</div>
</div>

<table class="detail_table" cellspacing="1" cellpadding="3" width="96%" border="0">
	<tr>
		<td class="detail_title_bg" align="center" colspan="2">内容</td>
	</tr>
	<tr>
		<td class="detail_title_bg" align="left" width="120">メール件名</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailSubject" runat="server" /></td>
	</tr>
	<tr id="trMailBody" runat="server">
		<td class="detail_title_bg" align="left" width="120">メール本文</td>
		<td class="detail_item_bg" align="left"><asp:Literal ID="lMailBody" runat="server" /></td>
	</tr>
	<tr id="trMailBodyHtml" runat="server">
		<td class="detail_title_bg" align="left" width="120">メール本文</td>
		<td class="detail_item_bg" align="left">
			<iframe class="HtmlPreview" src="<%: new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_HTML_PREVIEW_FORM).AddParam(Constants.HTML_PREVIEW_NO, "1").CreateUrl() %>" style="width: 100%; height: 100%;" frameborder="0" scrolling="no">
			</iframe>
		</td>
	</tr>
</table>
<% } %>
<img alt="" src="../../Images/Common/sp.gif" border="0" height="10" width="5" />

</asp:Content>