<%--
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証用モーダル(PaymentYamatoKaSmsAuthModal.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Order/PaymentYamatoKaSmsAuthModal.ascx.cs" Inherits="Form_Common_Order_PaymentYamatoKaSmsAuthModal" %>
<div id="modal">
	<div id="modal_content">
		<div id="authcodeinput">
			<p>電話番号<span id="spantelnum"><%: this.TelNum %></span>宛てに<br/>SMSを送信いたしました。</p>
			<p>SMSに記載の4桁の認証コードを<br/>入力してください。</p>
			<p>SMSが届かない場合は<a href="javascript:void(0)" class="btn_resend">こちら</a>をクリックし、電話番号を入力して下さい。</p>
			<p class="input_code"><asp:TextBox MaxLength="4" placeholder="認証コード" style="width:6em" TextMode="Number" min="0" Columns="6" ID="tbAuthCode" runat="server"/></p>
			<p class="error-msg" style="display:none;">認証コードが正しくありません、再度入力をお願いします。</p>
			<asp:LinkButton Text="認証する" class="btn btn_submit" ID="lbAuthorize" OnClick="lbAuthorize_OnClick" OnClientClick="return lbAuthorize_ClientClick()" runat="server"/>
			<p class="progress" style="display:none;">認証中．．．</p>
			<asp:HiddenField ID="hfIsNg" runat="server"/>
		</div>
		<div id="telnuminput" style="display:none">
			<p>4桁の認証コードが記載されたSMSを送信いたします。</p>
			<p>SMSが受信可能な携帯電話番号をハイフンなしで入力してください。</p>
			<p><input id="tbTelNum" type="tel" placeholder="携帯電話番号" size="8" maxlength="11" /></p>
			<a href="javascript:void(0)" class="btn btn_submit"onclick="return smsSendClick();">SMS送信</a>
			<p class="error-msg" style="display:none;">SMS送信に失敗しました、再度電話番号入力してください。</p>
		</div>
		<div class="btn_close_wrap"><a href="javascript:void(0);" class="btn_close">キャンセル</a></div>
	</div>
</div>
<script type="text/javascript">
	var lbAuthorizeClicked = false;

	$(function() {
		var modal = $('#modal'),
			modalContent = $('#modal_content'),
			btnClose = $(".btn_close"),
			btnResend = $(".btn_resend");
		btnResend.on('click', function () {
			$('#telnuminput').show();
			$('#authcodeinput').hide();
			$("#telnuminput .error-msg").hide();
		});
		$(modal).on('click', function (event) {
			if ((lbAuthorizeClicked === false) && (!($(event.target).closest(modalContent).length) || ($(event.target).closest(btnClose).length))) {
				location.reload();
			}
		});
	});

	function lbAuthorize_ClientClick() {
		var result = false;
		lbAuthorizeClicked = true;
		$("#<%: lbAuthorize.ClientID %>").hide();
		$("#modal_content .progress").show();
		authorize($("#<%: tbAuthCode.ClientID %>").val(),
			function(response) {
				var data = JSON.parse(response.d);

				switch (data.result) {
				case 'Ok':
					$("#<%: hfIsNg.ClientID %>").val(false);
					result = true;

					break;
				case "Invalid":
					$("#modal_content .error-msg").html("認証コードは4桁の正しい数字を入力してください。");
				case 'Reenter':
					result = false;
					$("#modal_content .error-msg").show();
					$("#<%: lbAuthorize.ClientID %>").show();
					$("#modal_content .progress").hide();
					lbAuthorizeClicked = false;

					break;
				case 'Ng':
					$("#<%: hfIsNg.ClientID %>").val(true);
					result = true;

					break;
				default:
				}
			});

		return result;
	}

	function authorize(code, callback) {
		$.ajax({
			type: "POST",
			url: "<%=Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL%>/Authorize",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				ninCode:code
			}),
			async: false,
			success: callback
		});
	}

	function smsSendClick() {
		var tel = $('#tbTelNum').val();

		smssend(tel,
			function(response) {
				var data = JSON.parse(response.d);

				switch (data.result) {
					case "Ok":
						$('#telnuminput').hide();
						$('#authcodeinput').show();
						$("#telnuminput .error-msg").hide();

						$('#spantelnum').text(tel);
					case "Ng":
						$("#telnuminput .error-msg").show();
						break;
				}
			});
	}

	function smssend(tel, callback) {
		$.ajax({
			type: "POST",
			url: "<%=Constants.PATH_ROOT + "SmartPhone/" + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL%>/SmsSend",
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			data: JSON.stringify({
				telNum: tel
			}),
			async: false,
			success: callback
		});
	}

</script>
