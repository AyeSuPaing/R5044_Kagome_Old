<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Landing/formlp/formlp.master" AutoEventWireup="true" CodeFile="~/Landing/formlp/Template/LpTemplate.aspx.cs" Inherits="Landing_formlp_Template_LpTemplate" Title="ランディングカート カスタムデザインテンプレート画面" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Register Src="~/SmartPhone/Landing/formlp/LpInputForm.ascx" TagPrefix="uc" TagName="LpInputForm" %>

<%-- ▽▽Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない▽▽ --%>
<script runat="server">
	public override PageAccessTypes PageAccessType
	{
		get { return PageAccessTypes.Https; }
	}
</script>
<%-- △△Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない△△ --%>

<asp:Content ContentPlaceHolderID="AffiliateTagHead" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagHead" Location="head" Datas="<%# this.CartList %>" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyTop" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyTop" Location="body_top" Datas="<%# this.CartList %>" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyBottom" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyBottom" Location="body_bottom" Datas="<%# this.CartList %>" runat="server"/>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
	<%-- ▽LPビルダーにて内容を更新するため、編集・削除しないでください▽ --%>
	<%-- ▽Meta Tag▽ --%>
	<meta property="og:title" content="[[@@og_title@@]]"/>
	<meta property="og:type" content="[[@@og_type@@]]"/>
	<meta property="og:url" content="[[@@og_url@@]]"/>
	<meta property="og:image" content="[[@@og_image@@]]"/>
	<meta property="og:site_name" content="[[@@og_site_name@@]]"/>
	<meta property="og:description" content="[[@@og_description@@]]"/>
	<%-- △Meta Tag△ --%>
	<%-- △LPビルダーにて内容を更新するため、編集・削除しないでください△ --%>

	<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/order.css") %>" rel="stylesheet" type="text/css" media="all"/>
	<link href="<%= WebSanitizer.HtmlEncode(Constants.PATH_ROOT + "SmartPhone/Css/user.css") %>" rel="stylesheet" type="text/css" media="all"/>
	<link rel="stylesheet" href="<%: Constants.PATH_ROOT %>SmartPhone/Landing/formlp/Css/front_formlp.css" type="text/css" media="screen"/>
	<link href="https://fonts.googleapis.com/css?family=Noto+Sans+JP:400,700&display=swap" rel="stylesheet">
	<link href="https://fonts.googleapis.com/css?family=Noto+Serif+JP:400,700&display=swap" rel="stylesheet">
	<link href="https://fonts.googleapis.com/css?family=Sawarabi+Mincho" rel="stylesheet">
	<script type="text/javascript" src="<%: Constants.PATH_ROOT %>SmartPhone/Landing/formlp/Js/formlp.js"></script>
	<link href="<%: Constants.PATH_ROOT %>SmartPhone/Css/order.css" rel="stylesheet" type="text/css" media="all"/>
	<link href="<%: Constants.PATH_ROOT %>SmartPhone/Css/user.css" rel="stylesheet" type="text/css" media="all"/>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	<script type="text/C#" runat="server">
	// ▽LPビルダーにて内容を更新するため、編集・削除しないでください▽
	// ▽System Code▽
		public new void Page_Init(Object sender, EventArgs e)
		{
			// 絶対に消さないでください▽
			base.Page_Init(sender, e);
			var padeId = "[[@@page_id@@]]";
			Initialize(padeId);
		}
	// △System Code△
	// △LPビルダーにて内容を更新するため、編集・削除しないでください△
	</script>

	<uc:LpInputForm ID="ucInputForm" runat="server"/>

</asp:Content>