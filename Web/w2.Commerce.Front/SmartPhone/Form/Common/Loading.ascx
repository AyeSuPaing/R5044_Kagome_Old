<%--
=========================================================================================================
  Module      : ローディングジェスチャー コントローラ(Loading.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Loading.ascx.cs" Inherits="Form_Common_Loading" %>
<style>
	/* ローディング時バックグラウンド */
	.loadingBackground {
		position: fixed;
		width: 100%;
		height: 100%;
		top: 0;
		left: 0;
		background-color: #333;
		/* 背景に色を設定したい場合はopacityに0以上の値を設定してください。 */
		opacity: 0;
		cursor: wait;
		z-index: 100000;
	}

	/* ローディングアイコン */
	.loadingIcon{
		position: fixed;
		top: 50%;
		left: 50%;
		z-index: 100001;
		display: inline-block;
		width: 50px;
		height: 50px;
		background: #fff;
		text-align: center;
		line-height: 50px;
		border-radius: 10%;
		box-shadow: 0 0 8px gray;
	}

	/* ローディングメッセージ */
	.loadingMessage {
		font-weight:bold; 
	}

	/* ローディング画像 */
	.loadingImage{
		margin: 10px auto;
	}
</style>

<div id="loadingBackground" class="loadingBackground" style="display: none;"></div>
<div id="loadingIcon" class="loadingIcon" style="display: none;">
	<img id="loadingImage" class="loadingImage" src="<%= Constants.PATH_ROOT %>Contents/ImagesPkg/check_running.gif" alt="ローディング中"/>
	<div id="loadingMessage" class="loadingMessage" style="display: none;">
		ただいま決済処理中です。<br/>
		画面が切り替わるまで<br/>
		そのままお待ちください。
	</div>
</div>

<% if (this.UpdatePanelReload) { %>
	<script type="text/javascript">
		Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(function (sender, args) {
			var postbackElementId = args.get_postBackElement().id;
			if (postbackElementId.indexOf('lbComplete') != -1) {
				LoadingMessageSpShow();
			} else {
				LoadingShow();
			}
		});

		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
			LoadingHide();
		});
	</script>
<% } %>