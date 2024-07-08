<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Register TagPrefix="uc" TagName="GlobalChangeList" Src="~/Form/Common/Global/GlobalChangeList.ascx" %>
<%@ Register TagPrefix="uc" TagName="GlobalChangeIcon" Src="~/Form/Common/Global/GlobalChangeIcon.ascx" %>

<% if (Constants.GLOBAL_OPTION_ENABLE)
   { %>

<link id="lGlobalChangeMenuCss" rel="stylesheet" href="<%: Constants.PATH_ROOT %>Css/globalChangeMenu.css" type="text/css" media="screen" />

<div class="region_change_menu" id="region_change_menu">
	<div class="user_global_status_icon" id="user_global_status_icon">
		<uc:GlobalChangeIcon ID="GlobalChangeIcon" runat="server" />
	</div>
	<div class="popup_window" id="popup_window">
		<uc:GlobalChangeList ID="GlobalChangeList" runat="server" />
	</div>
</div>

<script type="text/javascript">
	// グローバル切り替え
	function switchGlobalFunction() {
		$('#user_global_status_icon').on({
			'mouseenter': function () {
				$('#user_global_status_icon').addClass("active");
				$('#popup_window').fadeIn(20);
			}
		});
		$('#region_change_menu').on({
			'mouseleave': function () {
				$('#user_global_status_icon').removeClass("active");
				$('#popup_window').fadeOut(20);
			}
		});
	}
</script>

<% } %>
