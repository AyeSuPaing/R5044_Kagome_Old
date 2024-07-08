<%--
=========================================================================================================
  Module      : WYSIWYGエディタ画面(WysiwygEditor.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/Form/Common/PopupPage.master" AutoEventWireup="true" CodeFile="WysiwygEditor.aspx.cs" Inherits="Form_Common_WysiwygEditor" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderHead" Runat="Server">
	<script src="https://cdn.ckeditor.com/4.4.6/full/ckeditor.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
<table cellSpacing="0" cellPadding="0" width="98%" border="0">
	<tbody>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"></td>
		</tr>
		<tr>
			<td align="left"><h2 class="cmn-hed-h2">WYSIWYGエディタ</h2></td>
		</tr>
		<tr>
			<td>
			<table class="box_border" cellSpacing="1" cellPadding="3" width="100%" border="0">
					<tr>
						<td>
							<table class="info_box_bg" cellSpacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td align="center">
										<table cellspacing="0" cellpadding="0" width="98%" border="0">
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
											<tr>
												<td>
													 <textarea cols="80" name="editor1" id="editor1" rows="24" style="height:100%"></textarea>
												</td>
											</tr>
											<tr valign="bottom" height="30">
												<td align="right"><input type="button" id="btnFlush" value="　編集内容を反映します　" onclick="flushTextareaBinded();" /></td>
											</tr>
											<tr>
												<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0" /></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td><img height="10" alt="" src="../../Images/Common/sp.gif" width="100%" border="0"></td>
		</tr>
	</tbody>
</table>

<script type="text/javascript">
	// エディタと紐づけされた親ウィンドウのtextarea
	var textAreaWysiwygBinded;
	// ウィンドウロード時実行イベント処理（OnLoadEventメソッドは、popup.masterでbody要素のonloadに紐づけられている。）
	OnLoadEvent = function () 
	{
		// 親ウィンドウの変数は変化する可能性があるので、openerとは別に、このエディタと紐づけされたtextareaを保持しておく
		textAreaWysiwygBinded = window.opener.textAreaWysiwygBinded;
		// Setting auto paragraph off to avoid add tag <p> automaticaly
		CKEDITOR.config.autoParagraph = false;
		// 初期化
		$('#editor1').val(textAreaWysiwygBinded.value.replace(new RegExp("</@@", "gm"), '&lt;/@@'));

		// フルページモード
		var fullPageFlg = window.opener.global_fullPageFlg;
		
	

		CKEDITOR.replace('editor1', {
			resize_enabled: true,
			customConfig: '<%= ResolveUrl("~/Js/ckeditor-config.js") %>',
			//removePlugins: 'elementspath',
			fullPage: fullPageFlg,
			
			toolbarGroups: [
				{ name: 'document', groups: ['mode', 'document', 'doctools'] },
				{ name: 'clipboard', groups: ['clipboard', 'undo'] },
				{ name: 'editing', groups: ['find', 'selection'] },
				'/',
				{ name: 'styles' },
				{ name: 'colors' },
				{ name: 'basicstyles', groups: ['basicstyles', 'cleanup', 'list', 'indent', 'blocks', 'align', 'paragraph'] },
				{ name: 'links', groups: ['links', 'insert'] },
				{ name: 'tools' }
			],
			removePlugins: 'save, print, pagebreak, templates, smiley, specialchar, flash, iframe',  // 要素パスを非表示
			height: 370,
			width: "100%"
		});

		setTimeout(function () {
			if (CKEDITOR.instances.editor1) {
				// PopupPage.masterにある閉じるボタンのイベントを書き換える
				document.getElementById('btnClose').onclick = function () {
					if (textAreaWysiwygBinded.value !== CKEDITOR.instances.editor1.getData()) {
						if (confirm('編集内容を反映しますか？')) {
							flushTextareaBinded();
						}
						else if (confirm('エディタを閉じてもよろしいですか？') === false) {
							return;
						}
					}

					window.close();
				}
			}
			
			else {
				setTimeout(arguments.callee, 100);
			}
		}, 100);

		// ウィンドウが閉じる時、バインドされたtextareaを有効にする。
		window.onunload = function () {
			textAreaWysiwygBinded.removeAttribute('disabled');
		};
	}

	// 親ウィンドウのtextareaを更新。
	flushTextareaBinded = function () {
		textAreaWysiwygBinded.value = CKEDITOR.instances.editor1.getData().replace(/&lt;/g, "<").replace(/&gt;/g, ">");
	}
</script>

<%-- セッションタイムアウト防止 --%>
<script type="text/javascript" src="<%: Constants.PATH_ROOT %>Js/UpdateSessionTimeout.aspx?<%: Constants.QUERY_STRING_FOR_UPDATE_EXTERNAL_FILE_URLENCODED %>"></script>

</asp:Content>

