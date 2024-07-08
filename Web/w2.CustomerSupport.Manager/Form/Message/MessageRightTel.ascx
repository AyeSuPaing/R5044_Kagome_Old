<%--
=========================================================================================================
  Module      : メッセージページ電話フォーム出力コントローラ(MessageRightTel.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control	Language="C#" AutoEventWireup="true" CodeFile="MessageRightTel.ascx.cs" Inherits="Form_Message_MessageRightTel" %>

<a id="aTelTitle" href="#" runat="server"></a>

<asp:Label ID="lErrorMessages" CssClass="notice" runat="server"></asp:Label>

<div class="dataresult larger">
	<table>
	<tr>
		<td width="15%" class="alt">媒体<span class="notice">*</span></td>
		<td width="85%" colspan="3">
			<asp:RadioButtonList ID="rblMediaKbn" CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
		</td>
	</tr>
	<tr>
		<td width="15%" class="alt">受信/発信<span class="notice">*</span></td>
		<td width="85%" colspan="3">
			<asp:RadioButtonList ID="rblDirectionKbn"  CssClass="radio_button_list" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
		</td>
	</tr>
	<tr>
		<td class="alt">問合せ日時</td>
		<td colspan="3">
			<asp:TextBox ID="tbInquiryDateTime" Width="120" runat="server"></asp:TextBox>
			<asp:Button ID="btnSetMessageInquiryDateTime" Text="  現在日時  " runat="server" OnClick="btnSetMessageInquiryDateTime_Click" />
		</td>
	</tr>
	<tr id="trBcc">
		<td class="alt">氏名</td>
		<td colspan="3">
			&nbsp;&nbsp;姓&nbsp;<asp:TextBox ID="tbUserName1" runat="server"></asp:TextBox>
			&nbsp;&nbsp;&nbsp;名&nbsp;<asp:TextBox ID="tbUserName2" runat="server"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td class="alt"><%: ReplaceTag("@@User.name_kana.name@@") %></td>
		<td colspan="3">
			&nbsp;&nbsp;姓&nbsp;<asp:TextBox ID="tbUserNameKana1" runat="server"></asp:TextBox>
			&nbsp;&nbsp;&nbsp;名&nbsp;<asp:TextBox ID="tbUserNameKana2" runat="server"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td width="20%" class="alt">電話番号</td>
		<td width="30%"><asp:TextBox ID="tbUserTel1" runat="server"></asp:TextBox></td>
		<td width="20%" class="alt">メールアドレス</td>
		<td width="30%"><asp:TextBox ID="tbUserMail" Width="180" runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="alt">件名</td>
		<td colspan="3">
			<span id="spTilteArea">
				<asp:TextBox ID="tbInquiryTitle"  Width="90%" runat="server"></asp:TextBox>
			</span>
		</td>
	</tr>
	<tr>
		<td colspan="4" class="alt">内容<span class="notice">*</span></td>
	</tr>
	<tr>
		<td colspan="4"><asp:TextBox ID="tbInquiryText" Rows="5" Width="90%" TextMode="MultiLine" runat="server" CssClass="larger"></asp:TextBox></td>
	</tr>
	<tr>
		<td colspan="4" class="alt">回答 </td>
	</tr>
	<tr>
		<td colspan="4"><asp:TextBox ID="tbReplyText" Rows="5" Width="90%" TextMode="MultiLine" runat="server" CssClass="larger"></asp:TextBox></td>
	</tr>
	<tr>
		<td width="15%" class="alt">回答者<span class="notice">*</span></td>
		<td colspan="3" style="overflow: visible">
			<asp:DropDownList ID="ddlReplyOperators" CssClass="select2-select" Width="20%" runat="server"></asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td class="alt">作成者</td>
		<td colspan="3"><asp:Literal id="lUpdateOperator" runat="server"></asp:Literal></td>
	</tr>
	</table>
	<script type="text/javascript">
		// 氏名（姓・名）自動振り仮名変換
		function autoKanaWithKanaType() {
			<%-- 氏名（姓・名）の自動振り仮名変換を実行する --%>
			execAutoKanaWithKanaType(
				$("#<%= tbUserName1.ClientID %>"),
				$("#<%= tbUserNameKana1.ClientID %>"),
				$("#<%= tbUserName2.ClientID %>"),
				$("#<%= tbUserNameKana2.ClientID %>"));

			<%-- フリガナ（姓・名）の自動かな←→カナ変換を実行する --%>
			execAutoChangeKanaWithKanaType(
				$("#<%= tbUserNameKana1.ClientID %>"),
				$("#<%= tbUserNameKana2.ClientID %>"));
		}
		
		// 件名カウンターセット
		function set_inquirytitle_testbox_count()
		{
			set_text_count('<%= tbInquiryTitle.ClientID %>', '50', true, '文字数');
		}

		$(function () {
			autoKanaWithKanaType();

			set_inquirytitle_testbox_count();

			// 入力フォームのみ更新の場合（入力エラーメッセージ表示など）、メソッド実行しないことの対策
			// PageRequestManagerクラスをインスタンス化
			var mng = Sys.WebForms.PageRequestManager.getInstance();
			// 非同期ポストバックの完了後、メソッド再実行
			mng.add_endRequest(
				function (sender, args) {
					autoKanaWithKanaType();

					set_inquirytitle_testbox_count();
				}
			);
		});
	</script>
</div>
