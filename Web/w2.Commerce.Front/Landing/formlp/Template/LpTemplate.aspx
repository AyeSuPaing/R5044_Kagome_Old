<%@ Page Language="C#" MasterPageFile="~/Landing/formlp/formlp.master" AutoEventWireup="true" CodeFile="~/Landing/formlp/Template/LpTemplate.aspx.cs" Inherits="Landing_formlp_Template_LpTemplate" Title="ランディングカートテンプレート画面" MaintainScrollPositionOnPostback="true" %>
<%@ Register TagPrefix="uc" TagName="AffiliateTag" Src="~/Form/Common/AffiliateTag.ascx" %>
<%@ Register TagPrefix="uc" TagName="Efo" Src="~/Form/Common/Efo/EfoTagManager.ascx" %>
<%-- ▽▽Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない▽▽ --%>
<script runat="server">
	public override PageAccessTypes PageAccessType
	{
		get { return PageAccessTypes.Https; }
	}
</script>
<%-- △△Amazonペイメントを使う場合はウィジェットを配置するページは必ずSSLでなければいけない△△ --%>
<%@ Register Src="~/Landing/formlp/LpInputForm.ascx" TagPrefix="uc" TagName="LpInputForm" %>

<asp:Content ContentPlaceHolderID="AffiliateTagHead" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagHead" Location="head" Datas="<%# this.CartList %>" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyTop" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyTop" Location="body_top" Datas="<%# this.CartList %>" runat="server"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="AffiliateTagBodyBottom" Runat="Server">
	<uc:AffiliateTag ID="AffiliateTagBodyBottom" Location="body_bottom" Datas="<%# this.CartList %>" runat="server"/>
</asp:Content>

<asp:Content ID="meta" ContentPlaceHolderID="head" Runat="Server">
	<%-- ▽LPビルダーにて内容を更新するため、編集・削除しないでください▽ --%>
	<%-- ▽Meta Tag▽ --%>
	<meta property="og:title" content="[[@@og_title@@]]"/>
	<meta property="og:type" content="[[@@og_type@@]]"/>
	<meta property="og:url" content="[[@@og_url@@]]"/>
	<meta property="og:image" content="[[@@og_image@@]]"/>
	<meta property="og:site_name" content="[[@@og_site_name@@]]"/>
	<meta property="og:description" content="[[@@og_description@@]]" />
	<%-- △Meta Tag△ --%>
	<%-- △LPビルダーにて内容を更新するため、編集・削除しないでください△ --%>

	<link rel="stylesheet" href="<%: Constants.PATH_ROOT %>Landing/formlp/Css/front_formlp.css" type="text/css" media="screen" />	
	<link href="https://fonts.googleapis.com/css?family=Noto+Sans+JP:400,700&display=swap" rel="stylesheet">
	<link href="https://fonts.googleapis.com/css?family=Noto+Serif+JP:400,700&display=swap" rel="stylesheet">
	<link href="https://fonts.googleapis.com/css?family=Sawarabi+Mincho" rel="stylesheet">
	<style>
		.main {
			background-image: none !important;
		}
	</style>

	<!-- ▼▼ EFO CUBE ▼▼ -->
	<% if (this.LpPageDesign.IsEfoEnabled) { %>
		<uc:Efo id="ucEfo" runat="server" />
	<% } %>
	<!-- ▲▲ EFO CUBE ▲▲ -->

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/C#" runat="server">
	// ▽System Code▽
	public new void Page_Init(Object sender, EventArgs e)
	{
		// 絶対に消さないでください▽
		base.Page_Init(sender, e);

		var pageId = "[[@@page_id@@]]";
		Initialize(pageId);
	}
	// △System Code△
</script>
<script type="text/javascript" src="<%: Constants.PATH_ROOT %>Landing/formlp/Js/formlp.js"></script>

	<%--▼▼ ブロックループ ▼▼--%>
	<% foreach (var setting in this.PageDesignInput.BlockSettings) %> <% { %>

		<%--▼▼ ブロック：テンプレート ▼▼--%>
		<% if (setting.BlockClassName == "template") { %>
			<div class="template">
			</div>
		<% } %>
		<%--▲▲ ブロック：テンプレート ▲▲--%>

		<%--▼▼ ブロック：フリーHTML ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--sfree") { %>
			<div class="formlp-front-section formlp-front-section--sfree">
				<div class="formlp-front-section--sfree-inner" data-edit-prop-placeholder="HTML" data-edit-prop="html-src"><%= setting.GetAttributeValue("HTML", "html-src") %></div>
			</div>
		<% } %>
		<%--▲▲ ブロック：フリーHTML ▲▲--%>

		<%--▼▼ ブロック：ヘッダー ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--header") { %>
			<div class="formlp-front-section formlp-front-section--header">
				<div class="formlp-front-section--header-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--header-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--header-logo">
							<a href="<%: setting.GetAttributeValue("ロゴリンク", "href") %>" target="<%: setting.GetAttributeValue("ロゴリンク","target") %>" data-edit-prop="href" data-edit-prop-placeholder="ロゴリンク">
								<img class="formlp-front-section--header-logo-img" src='<%: setting.GetAttributeValue("ロゴ", "src") %>' alt="" data-edit-prop="src" data-edit-prop-placeholder="ロゴ">
							</a>
						</div>
						<div class="formlp-front-section--header-telnum">
							<a class='<%: setting.GetAttributeValue("電話番号", "font") %>' href='<%: setting.GetAttributeValue("電話番号", "href") %>' target='<%: setting.GetAttributeValue("電話番号","target") %>' data-edit-prop="text,href" style='<%: setting.GetAttributeValueStyleFormat("電話番号", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="電話番号"><%= setting.GetAttributeValue("電話番号", "text") %></a>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：ヘッダー ▲▲--%>

		<%--▼▼ ブロック：ファーストビュー ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001a1") { %>
			<div class="formlp-front-section formlp-front-section--s0001a1">
				<div class="formlp-front-section--s0001a1-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001a1-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<p class="formlp-front-section--s0001a1-txt1">
							<span class="formlp-front-section--s0001a1-txt1-label <%: setting.GetAttributeValue("テキスト１", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("テキスト１", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト１"><%= setting.GetAttributeValue("テキスト１", "text") %></span>
						</p>
						<p class="formlp-front-section--s0001a1-txt2 f-notoserifjp <%: setting.GetAttributeValue("テキスト２", "font") %>" data-edit-prop="text,background" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト２", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト２"><%= setting.GetAttributeValue("テキスト２", "text") %></p>
						<div class="formlp-front-section--s0001a1-block1">
							<div class="formlp-front-section--s0001a1-item">
								<img src='<%: setting.GetAttributeValue("商品画像", "src") %>' alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
							<ul class="formlp-front-section--s0001a1-points">
								<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント背景").Length; index++) { %>
									<li class="formlp-front-section--s0001a1-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="4">
										<span class="formlp-front-section--s0001a1-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント背景">
											<span class="txt1 <%: setting.GetAttributeValue("ポイントテキスト１", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト１", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト１"><%= setting.GetAttributeValue("ポイントテキスト１", "text", index) %></span>
											<span class="txt2 <%: setting.GetAttributeValue("ポイントテキスト２", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト２", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト２"><%= setting.GetAttributeValue("ポイントテキスト２", "text", index) %></span>
											<span class="txt3 <%: setting.GetAttributeValue("ポイントテキスト３", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト３", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト３"><%= setting.GetAttributeValue("ポイントテキスト３", "text", index) %></span>
										</span>
									</li>
								<% } %>
							</ul>
						</div>
					</div>
							</div>
			</div>
		<% } %>
		
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001a2") { %>
			<div class="formlp-front-section formlp-front-section--s0001a2">
				<div class="formlp-front-section--s0001a2-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001a2-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<p class="formlp-front-section--s0001a2-txt1">
							<span class="formlp-front-section--s0001a2-txt1-label <%: setting.GetAttributeValue("テキスト１", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("テキスト１", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト１"><%= setting.GetAttributeValue("テキスト１", "text") %></span>
						</p>
						<p class="formlp-front-section--s0001a2-txt2 f-notoserifjp <%: setting.GetAttributeValue("テキスト２", "font") %>" data-edit-prop="text,background" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト２", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト２"><%= setting.GetAttributeValue("テキスト２", "text") %></p>
						<div class="formlp-front-section--s0001a2-block1">
							<div class="formlp-front-section--s0001a2-item">
								<img src='<%: setting.GetAttributeValue("商品画像", "src") %>' alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
							<ul class="formlp-front-section--s0001a2-points">
								<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント背景").Length; index++) { %>
									<li class="formlp-front-section--s0001a2-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="4">
										<span class="formlp-front-section--s0001a2-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント背景">
											<span class="txt1 <%: setting.GetAttributeValue("ポイントテキスト１", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト１", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト１"><%= setting.GetAttributeValue("ポイントテキスト１", "text", index) %></span>
											<span class="txt2 <%: setting.GetAttributeValue("ポイントテキスト２", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト２", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト２"><%= setting.GetAttributeValue("ポイントテキスト２", "text", index) %></span>
											<span class="txt3 <%: setting.GetAttributeValue("ポイントテキスト３", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト３", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト３"><%= setting.GetAttributeValue("ポイントテキスト３", "text", index) %></span>
										</span>
									</li>
								<% } %>
							</ul>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001a3") { %>
			<div class="formlp-front-section formlp-front-section--s0001a3">
				<div class="formlp-front-section--s0001a3-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001a3-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<p class="formlp-front-section--s0001a3-txt1">
							<span class="formlp-front-section--s0001a3-txt1-label <%: setting.GetAttributeValue("テキスト１", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("テキスト１", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト１"><%= setting.GetAttributeValue("テキスト１", "text") %></span>
						</p>
						<p class="formlp-front-section--s0001a3-txt2 f-notoserifjp <%: setting.GetAttributeValue("テキスト２", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("テキスト２", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト２"><%= setting.GetAttributeValue("テキスト２", "text") %></p>
						<div class="formlp-front-section--s0001a3-block1">
							<div class="formlp-front-section--s0001a3-item">
								<img src='<%: setting.GetAttributeValue("商品画像", "src") %>' alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
							<ul class="formlp-front-section--s0001a3-points">
								<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント背景").Length; index++) { %>
									<li class="formlp-front-section--s0001a3-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="4">
										<span class="formlp-front-section--s0001a3-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント背景">
											<span class="txt1 <%: setting.GetAttributeValue("ポイントテキスト１", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト１", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト１"><%= setting.GetAttributeValue("ポイントテキスト１", "text", index) %></span>
											<span class="txt2 <%: setting.GetAttributeValue("ポイントテキスト２", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト２", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト２"><%= setting.GetAttributeValue("ポイントテキスト２", "text", index) %></span>
											<span class="txt3 <%: setting.GetAttributeValue("ポイントテキスト３", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト３", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト３"><%= setting.GetAttributeValue("ポイントテキスト３", "text", index) %></span>
										</span>
									</li>
								<% } %>
							</ul>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001b1") { %>
			<div class="formlp-front-section formlp-front-section--s0001b1">
				<div class="formlp-front-section--s0001b1-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001b1-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--s0001b1-txt1" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("斜めブロック背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="斜めブロック背景">
							<span class="formlp-front-section--s0001b1-txt1-sub <%: setting.GetAttributeValue("斜めブロックテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("斜めブロックテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="斜めブロックテキスト1"><%= setting.GetAttributeValue("斜めブロックテキスト1", "text") %></span>
							<span class="formlp-front-section--s0001b1-txt1-title <%: setting.GetAttributeValue("斜めブロックテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("斜めブロックテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="斜めブロックテキスト2"><%= setting.GetAttributeValue("斜めブロックテキスト2", "text") %></span>
						</div>
						<div class="formlp-front-section--s0001b1-item">
							<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
						</div>
						<div class="formlp-front-section--s0001b1-txt2">
							<p class="formlp-front-section--s0001b1-txt2-title <%: setting.GetAttributeValue("テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト1"><%= setting.GetAttributeValue("テキスト1", "text") %></p>
							<p class="formlp-front-section--s0001b1-txt2-text <%: setting.GetAttributeValue("テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト2"><%= setting.GetAttributeValue("テキスト2", "text") %></p>
						</div>
						<div class="formlp-front-section--s0001b1-label" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="上部ポイント">
							<div class="formlp-front-section--s0001b1-label-inner">
								<span class="txt1 <%: setting.GetAttributeValue("上部ポイント テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部ポイント テキスト1"><%= setting.GetAttributeValue("上部ポイント テキスト1", "text") %></span>
								<span class="txt2 <%: setting.GetAttributeValue("上部ポイント テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト2", new[] { "color", "font-size", "font-weight" }) %>'data-edit-prop-placeholder="上部ポイント テキスト2"><%= setting.GetAttributeValue("上部ポイント テキスト2", "text") %></span>
								<span class="txt3 <%: setting.GetAttributeValue("上部ポイント テキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト3", new[] { "color", "font-size", "font-weight" }) %>'data-edit-prop-placeholder="上部ポイント テキスト3"><%= setting.GetAttributeValue("上部ポイント テキスト3", "text") %></span>
							</div>
						</div>
						<ul class="formlp-front-section--s0001b1-points">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント背景").Length; index++) { %>
								<li class="formlp-front-section--s0001b1-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="4">
									<span class="formlp-front-section--s0001b1-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント背景">
										<span class="txt1 <%: setting.GetAttributeValue("ポイントテキスト1", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト1", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト1"><%= setting.GetAttributeValue("ポイントテキスト1", "text", index) %></span>
										<span class="txt2 <%: setting.GetAttributeValue("ポイントテキスト2", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト2", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト2"><%= setting.GetAttributeValue("ポイントテキスト2", "text", index) %></span>
										<span class="txt3 <%: setting.GetAttributeValue("ポイントテキスト3", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト3", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト3"><%= setting.GetAttributeValue("ポイントテキスト3", "text", index) %></span>
									</span>
								</li>
							<% } %>
						</ul>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001b2") { %>
			<div class="formlp-front-section formlp-front-section--s0001b2">
			<div class="formlp-front-section--s0001b2-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
				<div class="formlp-front-section--s0001b2-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
					<div class="formlp-front-section--s0001b2-txt1" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("斜めブロック背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="斜めブロック背景">
						<span class="formlp-front-section--s0001b2-txt1-sub <%: setting.GetAttributeValue("斜めブロックテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("斜めブロックテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="斜めブロックテキスト1"><%= setting.GetAttributeValue("斜めブロックテキスト1", "text") %></span>
						<span class="formlp-front-section--s0001b2-txt1-title <%: setting.GetAttributeValue("斜めブロックテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("斜めブロックテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="斜めブロックテキスト2"><%= setting.GetAttributeValue("斜めブロックテキスト2", "text") %></span>
					</div>
					<div class="formlp-front-section--s0001b2-item">
						<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
					</div>
					<div class="formlp-front-section--s0001b2-txt2">
						<p class="formlp-front-section--s0001b2-txt2-title <%: setting.GetAttributeValue("テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト1"><%= setting.GetAttributeValue("テキスト1", "text") %></p>
						<p class="formlp-front-section--s0001b2-txt2-text <%: setting.GetAttributeValue("テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト2"><%= setting.GetAttributeValue("テキスト2", "text") %></p>
					</div>
					<div class="formlp-front-section--s0001b2-label" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="上部ポイント">
						<div class="formlp-front-section--s0001b2-label-inner">
							<span class="txt1 <%: setting.GetAttributeValue("上部ポイント テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部ポイント テキスト1"><%= setting.GetAttributeValue("上部ポイント テキスト1", "text") %></span>
							<span class="txt2 <%: setting.GetAttributeValue("上部ポイント テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト2", new[] { "color", "font-size", "font-weight" }) %>'data-edit-prop-placeholder="上部ポイント テキスト2"><%= setting.GetAttributeValue("上部ポイント テキスト2", "text") %></span>
							<span class="txt3 <%: setting.GetAttributeValue("上部ポイント テキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト3", new[] { "color", "font-size", "font-weight" }) %>'data-edit-prop-placeholder="上部ポイント テキスト3"><%= setting.GetAttributeValue("上部ポイント テキスト3", "text") %></span>
						</div>
					</div>
					<ul class="formlp-front-section--s0001b2-points">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント背景").Length; index++) { %>
							<li class="formlp-front-section--s0001b2-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="4">
								<span class="formlp-front-section--s0001b2-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント背景">
									<span class="txt1 <%: setting.GetAttributeValue("ポイントテキスト1", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト1", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト1"><%= setting.GetAttributeValue("ポイントテキスト1", "text", index) %></span>
									<span class="txt2 <%: setting.GetAttributeValue("ポイントテキスト2", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト2", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト2"><%= setting.GetAttributeValue("ポイントテキスト2", "text", index) %></span>
									<span class="txt3 <%: setting.GetAttributeValue("ポイントテキスト3", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト3", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト3"><%= setting.GetAttributeValue("ポイントテキスト3", "text", index) %></span>
								</span>
							</li>
						<% } %>
					</ul>
				</div>
			</div>
		</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001b3") { %>
			<div class="formlp-front-section formlp-front-section--s0001b3">
				<div class="formlp-front-section--s0001b3-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001b3-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--s0001b3-txt1" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("斜めブロック背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="斜めブロック背景">
							<span class="formlp-front-section--s0001b3-txt1-sub <%: setting.GetAttributeValue("斜めブロックテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("斜めブロックテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="斜めブロックテキスト1"><%= setting.GetAttributeValue("斜めブロックテキスト1", "text") %></span>
							<span class="formlp-front-section--s0001b3-txt1-title <%: setting.GetAttributeValue("斜めブロックテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("斜めブロックテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="斜めブロックテキスト2"><%= setting.GetAttributeValue("斜めブロックテキスト2", "text") %></span>
						</div>
						<div class="formlp-front-section--s0001b3-item">
							<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
						</div>
						<div class="formlp-front-section--s0001b3-txt2">
							<p class="formlp-front-section--s0001b3-txt2-title <%: setting.GetAttributeValue("テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト1"><%= setting.GetAttributeValue("テキスト1", "text") %></p>
							<p class="formlp-front-section--s0001b3-txt2-text <%: setting.GetAttributeValue("テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト2"><%= setting.GetAttributeValue("テキスト2", "text") %></p>
						</div>
						<div class="formlp-front-section--s0001b3-label" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="上部ポイント">
							<div class="formlp-front-section--s0001b3-label-inner">
								<span class="txt1 <%: setting.GetAttributeValue("上部ポイント テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部ポイント テキスト1"><%= setting.GetAttributeValue("上部ポイント テキスト1", "text") %></span>
								<span class="txt2 <%: setting.GetAttributeValue("上部ポイント テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部ポイント テキスト2"><%= setting.GetAttributeValue("上部ポイント テキスト2", "text") %></span>
								<span class="txt3 <%: setting.GetAttributeValue("上部ポイント テキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部ポイント テキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部ポイント テキスト3"><%= setting.GetAttributeValue("上部ポイント テキスト3", "text") %></span>
							</div>
						</div>
						<ul class="formlp-front-section--s0001b3-points">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント背景").Length; index++) { %>
								<li class="formlp-front-section--s0001b3-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="4">
									<span class="formlp-front-section--s0001b3-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント背景">
										<span class="txt1 <%: setting.GetAttributeValue("ポイントテキスト1", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト1", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト1"><%= setting.GetAttributeValue("ポイントテキスト1", "text", index) %></span>
										<span class="txt2 <%: setting.GetAttributeValue("ポイントテキスト2", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト2", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト2"><%= setting.GetAttributeValue("ポイントテキスト2", "text", index) %></span>
										<span class="txt3 <%: setting.GetAttributeValue("ポイントテキスト3", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ポイントテキスト3", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ポイントテキスト3"><%= setting.GetAttributeValue("ポイントテキスト3", "text", index) %></span>
									</span>
								</li>
							<% } %>
						</ul>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001c1") { %>
			<div class="formlp-front-section formlp-front-section--s0001c1">
				<div class="formlp-front-section--s0001c1-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001c1-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--s0001c1-block1">
							<p class="formlp-front-section--s0001c1-txt1 <%: setting.GetAttributeValue("上部テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部テキスト"><%= setting.GetAttributeValue("上部テキスト", "text") %></p>
							<h1 class="formlp-front-section--s0001c1-txt2">
								<span class="formlp-front-section--s0001c1-txt2-txt1 <%: setting.GetAttributeValue("メインテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト1"><%= setting.GetAttributeValue("メインテキスト1", "text") %></span>
								<span class="formlp-front-section--s0001c1-txt2-txt2 <%: setting.GetAttributeValue("メインテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト2"><%= setting.GetAttributeValue("メインテキスト2", "text") %></span>
								<span class="formlp-front-section--s0001c1-txt2-txt3 <%: setting.GetAttributeValue("メインテキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト3"><%= setting.GetAttributeValue("メインテキスト3", "text") %></span>
							</h1>
							<p class="formlp-front-section--s0001c1-txt3 <%: setting.GetAttributeValue("サブテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("サブテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="サブテキスト"><%= setting.GetAttributeValue("サブテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0001c1-points">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント上部テキスト").Length; index++) { %>
								<div class="formlp-front-section--s0001c1-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="3">
									<p class="formlp-front-section--s0001c1-point-txt1" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント上部テキスト", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント上部テキスト">
										<span class="formlp-front-section--s0001c1-point-txt1-txt1 <%: setting.GetAttributeValue("上部テキスト1", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト1", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="上部テキスト1"><%= setting.GetAttributeValue("上部テキスト1", "text", index) %></span>
										<span class="formlp-front-section--s0001c1-point-txt1-txt2 <%: setting.GetAttributeValue("上部テキスト2", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト2", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="上部テキスト2"><%= setting.GetAttributeValue("上部テキスト2", "text", index) %></span>
										<span class="formlp-front-section--s0001c1-point-txt1-txt3 <%: setting.GetAttributeValue("上部テキスト3", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト3", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="上部テキスト3"><%= setting.GetAttributeValue("上部テキスト3", "text", index) %></span>
									</p>
									<h2 class="formlp-front-section--s0001c1-point-txt2 <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></h2>
									<p class="formlp-front-section--s0001c1-point-txt3 <%: setting.GetAttributeValue("説明テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text", index) %></p>
								</div>
							<% } %>
						</div>
					</div>
					<div class="formlp-front-section--s0001c1-block2">
						<p class="formlp-front-section--s0001c1-txt4">
							<span class="formlp-front-section--s0001c1-txt4-txt1 <%: setting.GetAttributeValue("下部テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("下部テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="下部テキスト1"><%= setting.GetAttributeValue("下部テキスト1", "text") %></span>
							<span class="formlp-front-section--s0001c1-txt4-txt2 <%: setting.GetAttributeValue("下部テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("下部テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="下部テキスト2"><%= setting.GetAttributeValue("下部テキスト2", "text") %></span>
						</p>
					</div>
							</div>
						</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001c2") { %>
			<div class="formlp-front-section formlp-front-section--s0001c2">
				<div class="formlp-front-section--s0001c2-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001c2-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--s0001c2-block1">
							<p class="formlp-front-section--s0001c2-txt1 <%: setting.GetAttributeValue("上部テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部テキスト"><%= setting.GetAttributeValue("上部テキスト", "text") %></p>
							<h1 class="formlp-front-section--s0001c2-txt2">
								<span class="formlp-front-section--s0001c2-txt2-txt1 <%: setting.GetAttributeValue("メインテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト1"><%= setting.GetAttributeValue("メインテキスト1", "text") %></span>
								<span class="formlp-front-section--s0001c2-txt2-txt2 <%: setting.GetAttributeValue("メインテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト2"><%= setting.GetAttributeValue("メインテキスト2", "text") %></span>
								<span class="formlp-front-section--s0001c2-txt2-txt3 <%: setting.GetAttributeValue("メインテキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト3"><%= setting.GetAttributeValue("メインテキスト3", "text") %></span>
							</h1>
							<p class="formlp-front-section--s0001c2-txt3 <%: setting.GetAttributeValue("サブテキスト", "font") %>" data-edit-prop="text" data-edit-prop-placeholder="サブテキスト"><%= setting.GetAttributeValue("サブテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0001c2-points">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント上部背景").Length; index++) { %>
								<div class="formlp-front-section--s0001c2-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="3">
									<div class="formlp-front-section--s0001c2-point-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント上部背景", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント上部背景">
										<div class="formlp-front-section--s0001c2-point-inner2">
											<p class="formlp-front-section--s0001c2-point-txt1" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント上部テキスト", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント上部テキスト">
												<span class="formlp-front-section--s0001c2-point-txt1-txt1 <%: setting.GetAttributeValue("上部テキスト1", "font", index) %>" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト1", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop="text" data-edit-prop-placeholder="上部テキスト1"><%= setting.GetAttributeValue("上部テキスト1", "text", index) %></span>
												<span class="formlp-front-section--s0001c2-point-txt1-txt2 <%: setting.GetAttributeValue("上部テキスト2", "font", index) %>" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト2", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop="text" data-edit-prop-placeholder="上部テキスト2"><%= setting.GetAttributeValue("上部テキスト2", "text", index) %></span>
												<span class="formlp-front-section--s0001c2-point-txt1-txt3 <%: setting.GetAttributeValue("上部テキスト3", "font", index) %>" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト3", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop="text" data-edit-prop-placeholder="上部テキスト3"><%= setting.GetAttributeValue("上部テキスト3", "text", index) %></span>
											</p>
											<h2 class="formlp-front-section--s0001c2-point-txt2 <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></h2>
											<p class="formlp-front-section--s0001c2-point-txt3 <%: setting.GetAttributeValue("説明テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text", index) %></p>
										</div>
									</div>
								</div>
							<% } %>
						</div>
						<div class="formlp-front-section--s0001c2-block2">
							<p class="formlp-front-section--s0001c2-txt4">
								<span class="formlp-front-section--s0001c2-txt4-txt1 <%: setting.GetAttributeValue("下部テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("下部テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="下部テキスト1"><%= setting.GetAttributeValue("下部テキスト1", "text") %></span>
								<span class="formlp-front-section--s0001c2-txt4-txt2 <%: setting.GetAttributeValue("下部テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("下部テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="下部テキスト2"><%= setting.GetAttributeValue("下部テキスト2", "text") %></span>
							</p>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0001c3") { %>
			<div class="formlp-front-section formlp-front-section--s0001c3">
				<div class="formlp-front-section--s0001c3-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0001c3-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--s0001c3-block1">
							<p class="formlp-front-section--s0001c3-txt1 <%: setting.GetAttributeValue("上部テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="上部テキスト"><%= setting.GetAttributeValue("上部テキスト", "text") %></p>
							<h1 class="formlp-front-section--s0001c3-txt2">
								<span class="formlp-front-section--s0001c3-txt2-txt1 <%: setting.GetAttributeValue("メインテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト1"><%= setting.GetAttributeValue("メインテキスト1", "text") %></span>
								<span class="formlp-front-section--s0001c3-txt2-txt2 <%: setting.GetAttributeValue("メインテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト2"><%= setting.GetAttributeValue("メインテキスト2", "text") %></span>
								<span class="formlp-front-section--s0001c3-txt2-txt3 <%: setting.GetAttributeValue("メインテキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メインテキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メインテキスト3"><%= setting.GetAttributeValue("メインテキスト3", "text") %></span>
							</h1>
							<p class="formlp-front-section--s0001c3-txt3 <%: setting.GetAttributeValue("サブテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("サブテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="サブテキスト"><%= setting.GetAttributeValue("サブテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0001c3-points">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ポイント上部テキスト").Length; index++) { %>
								<div class="formlp-front-section--s0001c3-point" data-edit-group="1" data-edit-group-placeholder="ポイント" data-edit-group-max="3">
									<p class="formlp-front-section--s0001c3-point-txt1" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ポイント上部テキスト", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="ポイント上部テキスト">
										<span class="formlp-front-section--s0001c3-point-txt1-txt1 <%: setting.GetAttributeValue("上部テキスト1", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト1", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="上部テキスト1"><%= setting.GetAttributeValue("上部テキスト1", "text", index) %></span>
										<span class="formlp-front-section--s0001c3-point-txt1-txt2 <%: setting.GetAttributeValue("上部テキスト2", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト2", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="上部テキスト2"><%= setting.GetAttributeValue("上部テキスト2", "text", index) %></span>
										<span class="formlp-front-section--s0001c3-point-txt1-txt3 <%: setting.GetAttributeValue("上部テキスト3", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("上部テキスト3", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="上部テキスト3"><%= setting.GetAttributeValue("上部テキスト3", "text", index) %></span>
									</p>
									<h2 class="formlp-front-section--s0001c3-point-txt2 <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></h2>
									<p class="formlp-front-section--s0001c3-point-txt3 <%: setting.GetAttributeValue("説明テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text", index) %></p>
								</div>
							<% } %>
						</div>
					</div>
					<div class="formlp-front-section--s0001c3-block2">
						<p class="formlp-front-section--s0001c3-txt4">
							<span class="formlp-front-section--s0001c3-txt4-txt1 <%: setting.GetAttributeValue("下部テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("下部テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="下部テキスト1"><%= setting.GetAttributeValue("下部テキスト1", "text") %></span>
							<span class="formlp-front-section--s0001c3-txt4-txt2 <%: setting.GetAttributeValue("下部テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("下部テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="下部テキスト2"><%= setting.GetAttributeValue("下部テキスト2", "text") %></span>
						</p>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：ファーストビュー ▲▲--%>

		<%--▼▼ ブロック：購入エリア ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002a1") { %>
			<div class="formlp-front-section formlp-front-section--s0002a1">
				<div class="formlp-front-section--s0002a1-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002a1-inner">
						<div class="formlp-front-section--s0002a1-header" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ヘッダー", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="ヘッダー">
							<p class="formlp-front-section--s0002a1-header-txt <%: setting.GetAttributeValue("ヘッダーテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ヘッダーテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="ヘッダーテキスト"><%= setting.GetAttributeValue("ヘッダーテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0002a1-body" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="背景（内側）">
							<h2 class="formlp-front-section--s0002a1-name <%: setting.GetAttributeValue("商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="商品名"><%= setting.GetAttributeValue("商品名", "text") %></h2>
							<p class="formlp-front-section--s0002a1-txt1 <%: setting.GetAttributeValue("説明テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text") %></p>
							<div class="formlp-front-section--s0002a1-priceblock">
								<p class="formlp-front-section--s0002a1-priceblock-txt1 <%: setting.GetAttributeValue("特別価格", "font") %>" data-edit-prop="text,background,border" style='<%: setting.GetAttributeValueStyleFormat("特別価格", new[] { "background-color", "background-image", "border-color", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="特別価格"><%= setting.GetAttributeValue("特別価格", "text") %></p>
								<p class="formlp-front-section--s0002a1-priceblock-price">
									<span class="formlp-front-section--s0002a1-priceblock-price-value <%: setting.GetAttributeValue("金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="金額テキスト"><%= setting.GetAttributeValue("金額テキスト", "text") %></span>
									<span class="formlp-front-section--s0002a1-priceblock-price-unit <%: setting.GetAttributeValue("単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="単位テキスト"><%= setting.GetAttributeValue("単位テキスト", "text") %></span>
									<span class="formlp-front-section--s0002a1-priceblock-price-tax <%: setting.GetAttributeValue("税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="税込テキスト"><%= setting.GetAttributeValue("税込テキスト", "text") %></span></p>
								<p class="formlp-front-section--s0002a1-priceblock-price-free" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("送料無料", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="送料無料">
									<span class="formlp-front-section--s0002a1-priceblock-price-free-txt <%: setting.GetAttributeValue("送料無料テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("送料無料テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="送料無料テキスト"><%= setting.GetAttributeValue("送料無料テキスト", "text") %></span>
								</p>
							</div>
							<div class="formlp-front-section--s0002a1-btns">
								<a class="formlp-front-section--s0002a1-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target='<%: setting.GetAttributeValue("購入ボタン","target") %>' data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002a1-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
							<div class="formlp-front-section--s0002a1-notes">
								<p class="formlp-front-section--s0002a1-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002a1-img">
								<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002a2") { %>
			<div class="formlp-front-section formlp-front-section--s0002a2">
				<div class="formlp-front-section--s0002a2-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002a2-inner">
						<div class="formlp-front-section--s0002a2-header" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ヘッダー", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="ヘッダー">
							<p class="formlp-front-section--s0002a2-header-txt <%: setting.GetAttributeValue("ヘッダーテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ヘッダーテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="ヘッダーテキスト"><%= setting.GetAttributeValue("ヘッダーテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0002a2-body" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="背景（内側）">
							<h2 class="formlp-front-section--s0002a2-name <%: setting.GetAttributeValue("商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="商品名"><%= setting.GetAttributeValue("商品名", "text") %></h2>
							<p class="formlp-front-section--s0002a2-txt1 <%: setting.GetAttributeValue("説明テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text") %></p>
							<div class="formlp-front-section--s0002a2-priceblock">
								<p class="formlp-front-section--s0002a2-priceblock-txt1 <%: setting.GetAttributeValue("特別価格", "font") %>" data-edit-prop="text,background,border" style='<%: setting.GetAttributeValueStyleFormat("特別価格", new[] { "background-color", "background-image", "border-color", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="特別価格"><%= setting.GetAttributeValue("特別価格", "text") %></p>
								<p class="formlp-front-section--s0002a2-priceblock-price">
									<span class="formlp-front-section--s0002a2-priceblock-price-value <%: setting.GetAttributeValue("金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="金額テキスト"><%= setting.GetAttributeValue("金額テキスト", "text") %></span>
									<span class="formlp-front-section--s0002a2-priceblock-price-unit <%: setting.GetAttributeValue("単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="単位テキスト"><%= setting.GetAttributeValue("単位テキスト", "text") %></span>
									<span class="formlp-front-section--s0002a2-priceblock-price-tax <%: setting.GetAttributeValue("税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="税込テキスト"><%= setting.GetAttributeValue("税込テキスト", "text") %></span></p>
								<p class="formlp-front-section--s0002a2-priceblock-price-free" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("送料無料", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="送料無料">
									<span class="formlp-front-section--s0002a2-priceblock-price-free-txt <%: setting.GetAttributeValue("送料無料テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("送料無料テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="送料無料テキスト"><%= setting.GetAttributeValue("送料無料テキスト", "text") %></span>
								</p>
							</div>
							<div class="formlp-front-section--s0002a2-btns">
								<a class="formlp-front-section--s0002a2-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target='<%: setting.GetAttributeValue("購入ボタン","target") %>' data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002a2-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
							<div class="formlp-front-section--s0002a2-notes">
								<p class="formlp-front-section--s0002a2-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002a2-img">
								<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002a3") { %>
			<div class="formlp-front-section formlp-front-section--s0002a3">
				<div class="formlp-front-section--s0002a3-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002a3-inner">
						<div class="formlp-front-section--s0002a3-header" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ヘッダー", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="ヘッダー">
							<p class="formlp-front-section--s0002a3-header-txt <%: setting.GetAttributeValue("ヘッダーテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ヘッダーテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="ヘッダーテキスト"><%= setting.GetAttributeValue("ヘッダーテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0002a3-body" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="背景（内側）">
							<h2 class="formlp-front-section--s0002a3-name <%: setting.GetAttributeValue("商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="商品名"><%= setting.GetAttributeValue("商品名", "text") %></h2>
							<p class="formlp-front-section--s0002a3-txt1 <%: setting.GetAttributeValue("説明テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text") %></p>
							<div class="formlp-front-section--s0002a3-priceblock">
								<p class="formlp-front-section--s0002a3-priceblock-txt1 <%: setting.GetAttributeValue("特別価格", "font") %>" data-edit-prop="text,background,border" style='<%: setting.GetAttributeValueStyleFormat("特別価格", new[] { "background-color", "background-image", "border-color", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="特別価格"><%= setting.GetAttributeValue("特別価格", "text") %></p>
								<p class="formlp-front-section--s0002a3-priceblock-price">
									<span class="formlp-front-section--s0002a3-priceblock-price-value <%: setting.GetAttributeValue("金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="金額テキスト"><%= setting.GetAttributeValue("金額テキスト", "text") %></span>
									<span class="formlp-front-section--s0002a3-priceblock-price-unit <%: setting.GetAttributeValue("単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="単位テキスト"><%= setting.GetAttributeValue("単位テキスト", "text") %></span>
									<span class="formlp-front-section--s0002a3-priceblock-price-tax <%: setting.GetAttributeValue("税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="税込テキスト"><%= setting.GetAttributeValue("税込テキスト", "text") %></span></p>
								<p class="formlp-front-section--s0002a3-priceblock-price-free" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("送料無料", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="送料無料">
									<span class="formlp-front-section--s0002a3-priceblock-price-free-txt <%: setting.GetAttributeValue("送料無料テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("送料無料テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="送料無料テキスト"><%= setting.GetAttributeValue("送料無料テキスト", "text") %></span>
								</p>
							</div>
							<div class="formlp-front-section--s0002a3-btns">
								<a class="formlp-front-section--s0002a3-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target='<%: setting.GetAttributeValue("購入ボタン","target") %>' data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002a3-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
							<div class="formlp-front-section--s0002a3-notes">
								<p class="formlp-front-section--s0002a3-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002a3-img">
								<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002b1") { %>
			<div class="formlp-front-section formlp-front-section--s0002b1">
				<div class="formlp-front-section--s0002b1-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002b1-inner">
						<div class="formlp-front-section--s0002b1-header" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ヘッダー", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="ヘッダー">
							<p class="formlp-front-section--s0002b1-header-txt <%: setting.GetAttributeValue("ヘッダーテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ヘッダーテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="ヘッダーテキスト"><%= setting.GetAttributeValue("ヘッダーテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0002b1-body" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="背景（内側）">
							<h2 class="formlp-front-section--s0002b1-name <%: setting.GetAttributeValue("商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="商品名"><%= setting.GetAttributeValue("商品名", "text") %></h2>
							<p class="formlp-front-section--s0002b1-txt1 <%: setting.GetAttributeValue("説明テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text") %></p>
							<div class="formlp-front-section--s0002b1-priceblock">
								<div class="formlp-front-section--s0002b1-priceblock-before">
									<p class="formlp-front-section--s0002b1-priceblock-before-price">
										<span class="formlp-front-section--s0002b1-priceblock-before-price-txt1 <%: setting.GetAttributeValue("通常金額ラベル", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額ラベル", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額ラベル"><%= setting.GetAttributeValue("通常金額ラベル", "text") %></span>
										<span class="formlp-front-section--s0002b1-priceblock-before-price-value <%: setting.GetAttributeValue("通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額テキスト"><%= setting.GetAttributeValue("通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b1-priceblock-before-price-unit <%: setting.GetAttributeValue("通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常単位テキスト"><%= setting.GetAttributeValue("通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b1-priceblock-before-price-tax <%: setting.GetAttributeValue("通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常税込テキスト"><%= setting.GetAttributeValue("通常税込テキスト", "text") %></span>
									</p>
								</div>
								<div class="formlp-front-section--s0002b1-priceblock-arrow"></div>
								<div class="formlp-front-section--s0002b1-priceblock-after">
									<p class="formlp-front-section--s0002b1-priceblock-after-txt1 <%: setting.GetAttributeValue("割引金額ラベル", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引金額ラベル", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引金額ラベル"><%= setting.GetAttributeValue("割引金額ラベル", "text") %></p>
									<p class="formlp-front-section--s0002b1-priceblock-after-price">
										<span class="formlp-front-section--s0002b1-priceblock-after-price-value <%: setting.GetAttributeValue("割引金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引金額テキスト"><%= setting.GetAttributeValue("割引金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b1-priceblock-after-price-unit <%: setting.GetAttributeValue("割引単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引単位テキスト"><%= setting.GetAttributeValue("割引単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b1-priceblock-after-price-tax <%: setting.GetAttributeValue("割引税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引税込テキスト"><%= setting.GetAttributeValue("割引税込テキスト", "text") %></span>
									</p>
									<p class="formlp-front-section--s0002b1-priceblock-after-price-free" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("送料無料", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="送料無料">
										<span class="formlp-front-section--s0002b1-priceblock-after-price-free-txt <%: setting.GetAttributeValue("送料無料テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("送料無料テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="送料無料テキスト"><%= setting.GetAttributeValue("送料無料テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002b1-btns">
								<a class="formlp-front-section--s0002b1-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002b1-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
							<div class="formlp-front-section--s0002b1-notes">
								<p class="formlp-front-section--s0002b1-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002b1-img">
								<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002b2") { %>
			<div class="formlp-front-section formlp-front-section--s0002b2">
				<div class="formlp-front-section--s0002b2-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002b2-inner">
						<div class="formlp-front-section--s0002b2-header" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ヘッダー", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="ヘッダー">
							<p class="formlp-front-section--s0002b2-header-txt <%: setting.GetAttributeValue("ヘッダーテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ヘッダーテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="ヘッダーテキスト"><%= setting.GetAttributeValue("ヘッダーテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0002b2-body" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="背景（内側）">
							<h2 class="formlp-front-section--s0002b2-name <%: setting.GetAttributeValue("商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="商品名"><%= setting.GetAttributeValue("商品名", "text") %></h2>
							<p class="formlp-front-section--s0002b2-txt1 <%: setting.GetAttributeValue("説明テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text") %></p>
							<div class="formlp-front-section--s0002b2-priceblock">
								<div class="formlp-front-section--s0002b2-priceblock-before">
									<p class="formlp-front-section--s0002b2-priceblock-before-price">
										<span class="formlp-front-section--s0002b2-priceblock-before-price-txt1 <%: setting.GetAttributeValue("通常金額ラベル", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額ラベル", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額ラベル"><%= setting.GetAttributeValue("通常金額ラベル", "text") %></span>
										<span class="formlp-front-section--s0002b2-priceblock-before-price-value <%: setting.GetAttributeValue("通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額テキスト"><%= setting.GetAttributeValue("通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b2-priceblock-before-price-unit <%: setting.GetAttributeValue("通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常単位テキスト"><%= setting.GetAttributeValue("通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b2-priceblock-before-price-tax <%: setting.GetAttributeValue("通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常税込テキスト"><%= setting.GetAttributeValue("通常税込テキスト", "text") %></span>
									</p>
								</div>
								<div class="formlp-front-section--s0002b2-priceblock-arrow"></div>
								<div class="formlp-front-section--s0002b2-priceblock-after">
									<p class="formlp-front-section--s0002b2-priceblock-after-txt1 <%: setting.GetAttributeValue("割引金額ラベル", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引金額ラベル", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引金額ラベル"><%= setting.GetAttributeValue("割引金額ラベル", "text") %></p>
									<p class="formlp-front-section--s0002b2-priceblock-after-price">
										<span class="formlp-front-section--s0002b2-priceblock-after-price-value <%: setting.GetAttributeValue("割引金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引金額テキスト"><%= setting.GetAttributeValue("割引金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b2-priceblock-after-price-unit <%: setting.GetAttributeValue("割引単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引単位テキスト"><%= setting.GetAttributeValue("割引単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b2-priceblock-after-price-tax <%: setting.GetAttributeValue("割引税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引税込テキスト"><%= setting.GetAttributeValue("割引税込テキスト", "text") %></span>
									</p>
									<p class="formlp-front-section--s0002b2-priceblock-after-price-free" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("送料無料", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="送料無料">
										<span class="formlp-front-section--s0002b2-priceblock-after-price-free-txt <%: setting.GetAttributeValue("送料無料テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("送料無料テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="送料無料テキスト"><%= setting.GetAttributeValue("送料無料テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002b2-btns">
								<a class="formlp-front-section--s0002b2-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002b2-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
							<div class="formlp-front-section--s0002b2-notes">
								<p class="formlp-front-section--s0002b2-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002b2-img">
								<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002b3") { %>
			<div class="formlp-front-section formlp-front-section--s0002b3">
				<div class="formlp-front-section--s0002b3-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002b3-inner">
						<div class="formlp-front-section--s0002b3-header" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("ヘッダー", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="ヘッダー">
							<p class="formlp-front-section--s0002b3-header-txt <%: setting.GetAttributeValue("ヘッダーテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("ヘッダーテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="ヘッダーテキスト"><%= setting.GetAttributeValue("ヘッダーテキスト", "text") %></p>
						</div>
						<div class="formlp-front-section--s0002b3-body" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="背景（内側）">
							<h2 class="formlp-front-section--s0002b3-name <%: setting.GetAttributeValue("商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="商品名"><%= setting.GetAttributeValue("商品名", "text") %></h2>
							<p class="formlp-front-section--s0002b3-txt1 <%: setting.GetAttributeValue("説明テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("説明テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="説明テキスト"><%= setting.GetAttributeValue("説明テキスト", "text") %></p>
							<div class="formlp-front-section--s0002b3-priceblock">
								<div class="formlp-front-section--s0002b3-priceblock-before">
									<p class="formlp-front-section--s0002b3-priceblock-before-price">
										<span class="formlp-front-section--s0002b3-priceblock-before-price-txt1 <%: setting.GetAttributeValue("通常金額ラベル", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額ラベル", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額ラベル"><%= setting.GetAttributeValue("通常金額ラベル", "text") %></span>
										<span class="formlp-front-section--s0002b3-priceblock-before-price-value <%: setting.GetAttributeValue("通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額テキスト"><%= setting.GetAttributeValue("通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b3-priceblock-before-price-unit <%: setting.GetAttributeValue("通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常単位テキスト"><%= setting.GetAttributeValue("通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b3-priceblock-before-price-tax <%: setting.GetAttributeValue("通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常税込テキスト"><%= setting.GetAttributeValue("通常税込テキスト", "text") %></span>
									</p>
								</div>
								<div class="formlp-front-section--s0002b3-priceblock-arrow"></div>
								<div class="formlp-front-section--s0002b3-priceblock-after">
									<p class="formlp-front-section--s0002b3-priceblock-after-txt1 <%: setting.GetAttributeValue("割引金額ラベル", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引金額ラベル", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引金額ラベル"><%= setting.GetAttributeValue("割引金額ラベル", "text") %></p>
									<p class="formlp-front-section--s0002b3-priceblock-after-price">
										<span class="formlp-front-section--s0002b3-priceblock-after-price-value <%: setting.GetAttributeValue("割引金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引金額テキスト"><%= setting.GetAttributeValue("割引金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b3-priceblock-after-price-unit <%: setting.GetAttributeValue("割引単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引単位テキスト"><%= setting.GetAttributeValue("割引単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002b3-priceblock-after-price-tax <%: setting.GetAttributeValue("割引税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("割引税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="割引税込テキスト"><%= setting.GetAttributeValue("割引税込テキスト", "text") %></span>
									</p>
									<p class="formlp-front-section--s0002b3-priceblock-after-price-free" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("送料無料", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="送料無料">
										<span class="formlp-front-section--s0002b3-priceblock-after-price-free-txt <%: setting.GetAttributeValue("送料無料テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("送料無料テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="送料無料テキスト"><%= setting.GetAttributeValue("送料無料テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002b3-btns">
								<a class="formlp-front-section--s0002b3-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002b3-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
							<div class="formlp-front-section--s0002b3-notes">
							</div>
							<div class="formlp-front-section--s0002b3-img">
								<img src="<%: setting.GetAttributeValue("商品画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="商品画像">
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002c1") { %>
			<div class="formlp-front-section formlp-front-section--s0002c1">
				<div class="formlp-front-section--s0002c1-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002c1-inner">
						<div class="formlp-front-section--s0002c1-teiki" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("定期商品ブロック", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="定期商品ブロック">
							<div class="formlp-front-section--s0002c1-teiki-tag" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("割引タグ", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="割引タグ">
								<span class="formlp-front-section--s0002c1-teiki-tag-txt">
									<span class="formlp-front-section--s0002c1-teiki-tag-txt1 <%: setting.GetAttributeValue("テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト1"><%= setting.GetAttributeValue("テキスト1", "text") %></span>
									<span class="formlp-front-section--s0002c1-teiki-tag-txt2 <%: setting.GetAttributeValue("テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト2"><%= setting.GetAttributeValue("テキスト2", "text") %></span>
									<span class="formlp-front-section--s0002c1-teiki-tag-txt3 <%: setting.GetAttributeValue("テキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト3"><%= setting.GetAttributeValue("テキスト3", "text") %></span>
								</span>
							</div>
							<h2 class="formlp-front-section--s0002c1-teiki-name <%: setting.GetAttributeValue("定期商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期商品名"><%= setting.GetAttributeValue("定期商品名", "text") %></h2>
							<div class="formlp-front-section--s0002c1-teiki-priceblock">
								<div class="formlp-front-section--s0002c1-teiki-priceblock-before">
									<p class="formlp-front-section--s0002c1-teiki-priceblock-before-price">
										<span class="formlp-front-section--s0002c1-teiki-priceblock-before-price-txt1 <%: setting.GetAttributeValue("定期通常価格", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常価格", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常価格"><%= setting.GetAttributeValue("定期通常価格", "text") %></span>
										<span class="formlp-front-section--s0002c1-teiki-priceblock-before-price-value <%: setting.GetAttributeValue("定期通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常金額テキスト"><%= setting.GetAttributeValue("定期通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c1-teiki-priceblock-before-price-unit <%: setting.GetAttributeValue("定期通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常単位テキスト"><%= setting.GetAttributeValue("定期通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c1-teiki-priceblock-before-price-tax <%: setting.GetAttributeValue("定期通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常税込テキスト"><%= setting.GetAttributeValue("定期通常税込テキスト", "text") %></span>
									</p>
								</div>
								<div class="formlp-front-section--s0002c1-teiki-priceblock-after">
									<p class="formlp-front-section--s0002c1-teiki-priceblock-after-txt1 <%: setting.GetAttributeValue("定期限定価格", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("定期限定価格", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定価格"><%= setting.GetAttributeValue("定期限定価格", "text") %></p>
									<p class="formlp-front-section--s0002c1-teiki-priceblock-after-price">
										<span class="formlp-front-section--s0002c1-teiki-priceblock-after-price-value <%: setting.GetAttributeValue("定期限定金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定金額テキスト"><%= setting.GetAttributeValue("定期限定金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c1-teiki-priceblock-after-price-unit <%: setting.GetAttributeValue("定期限定単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定単位テキスト"><%= setting.GetAttributeValue("定期限定単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c1-teiki-priceblock-after-price-tax <%: setting.GetAttributeValue("定期限定税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定税込テキスト"><%= setting.GetAttributeValue("定期限定税込テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002c1-teiki-notes">
								<p class="formlp-front-section--s0002c1-teiki-note <%: setting.GetAttributeValue("定期注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期注釈"><%= setting.GetAttributeValue("定期注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002c1-teiki-btns">
								<a class="formlp-front-section--s0002c1-teiki-btn btn-hover-scale" href="<%: setting.GetAttributeValue("定期購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("定期購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="定期購入ボタン">
									<span class="formlp-front-section--s0002c1-teiki-btn-label1 <%: setting.GetAttributeValue("定期購入ボタンテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタンテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期購入ボタンテキスト1"><%= setting.GetAttributeValue("定期購入ボタンテキスト1", "text") %></span>
									<span class="formlp-front-section--s0002c1-teiki-btn-label2 <%: setting.GetAttributeValue("定期購入ボタンテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタンテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期購入ボタンテキスト2"><%= setting.GetAttributeValue("定期購入ボタンテキスト2", "text") %></span>
								</a>
							</div>
						</div>
						<div class="formlp-front-section--s0002c1-tanpin">
							<h2 class="formlp-front-section--s0002c1-tanpin-name <%: setting.GetAttributeValue("通常商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常商品名"><%= setting.GetAttributeValue("通常商品名", "text") %></h2>
							<div class="formlp-front-section--s0002c1-tanpin-priceblock">
								<div class="formlp-front-section--s0002c1-tanpin-priceblock-after">
									<p class="formlp-front-section--s0002c1-tanpin-priceblock-after-txt1 <%: setting.GetAttributeValue("通常価格", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("通常価格", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常価格"><%= setting.GetAttributeValue("通常価格", "text") %></p>
									<p class="formlp-front-section--s0002c1-tanpin-priceblock-after-price">
										<span class="formlp-front-section--s0002c1-tanpin-priceblock-after-price-value <%: setting.GetAttributeValue("通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額テキスト"><%= setting.GetAttributeValue("通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c1-tanpin-priceblock-after-price-unit <%: setting.GetAttributeValue("通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常単位テキスト"><%= setting.GetAttributeValue("通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c1-tanpin-priceblock-after-price-tax <%: setting.GetAttributeValue("通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常税込テキスト"><%= setting.GetAttributeValue("通常税込テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002c1-tanpin-notes">
								<p class="formlp-front-section--s0002c1-tanpin-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002c1-tanpin-btns">
								<a class="formlp-front-section--s0002c1-tanpin-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002c1-tanpin-btn-label1 <%: setting.GetAttributeValue("購入ボタンテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト1"><%= setting.GetAttributeValue("購入ボタンテキスト1", "text") %></span>
									<span class="formlp-front-section--s0002c1-tanpin-btn-label2 <%: setting.GetAttributeValue("購入ボタンテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト2"><%= setting.GetAttributeValue("購入ボタンテキスト2", "text") %></span>
								</a>
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002c2") { %>
			<div class="formlp-front-section formlp-front-section--s0002c2">
				<div class="formlp-front-section--s0002c2-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002c2-inner">
						<div class="formlp-front-section--s0002c2-teiki" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("定期商品ブロック", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="定期商品ブロック">
							<h2 class="formlp-front-section--s0002c2-teiki-name" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("定期商品名背景", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="定期商品名背景">
								<span class="formlp-front-section--s0002c2-teiki-name-text <%: setting.GetAttributeValue("定期商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期商品名"><%= setting.GetAttributeValue("定期商品名", "text") %></span>
							</h2>
							<div class="formlp-front-section--s0002c2-teiki-priceblock">
								<div class="formlp-front-section--s0002c2-teiki-priceblock-before">
									<p class="formlp-front-section--s0002c2-teiki-priceblock-before-price">
										<span class="formlp-front-section--s0002c2-teiki-priceblock-before-price-txt1 <%: setting.GetAttributeValue("定期通常価格", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常価格", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常価格"><%= setting.GetAttributeValue("定期通常価格", "text") %></span>
										<span class="formlp-front-section--s0002c2-teiki-priceblock-before-price-value <%: setting.GetAttributeValue("定期通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常金額テキスト"><%= setting.GetAttributeValue("定期通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c2-teiki-priceblock-before-price-unit <%: setting.GetAttributeValue("定期通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常単位テキスト"><%= setting.GetAttributeValue("定期通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c2-teiki-priceblock-before-price-tax <%: setting.GetAttributeValue("定期通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常税込テキスト"><%= setting.GetAttributeValue("定期通常税込テキスト", "text") %></span>
									</p>
								</div>
								<div class="formlp-front-section--s0002c2-teiki-priceblock-after">
									<p class="formlp-front-section--s0002c2-teiki-priceblock-after-txt1 <%: setting.GetAttributeValue("定期限定価格", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("定期限定価格", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>'  data-edit-prop-placeholder="定期限定価格"><%= setting.GetAttributeValue("定期限定価格", "text") %></p>
									<p class="formlp-front-section--s0002c2-teiki-priceblock-after-price">
										<span class="formlp-front-section--s0002c2-teiki-priceblock-after-price-value <%: setting.GetAttributeValue("定期限定金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定金額テキスト"><%= setting.GetAttributeValue("定期限定金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c2-teiki-priceblock-after-price-unit <%: setting.GetAttributeValue("定期限定単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定単位テキスト"><%= setting.GetAttributeValue("定期限定単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c2-teiki-priceblock-after-price-tax <%: setting.GetAttributeValue("定期限定税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定税込テキスト"><%= setting.GetAttributeValue("定期限定税込テキスト", "text") %></span>
									</p>
									<div class="formlp-front-section--s0002c2-teiki-priceblock-after-tag" data-edit-prop="background" data-edit-prop-placeholder="定期割引タグ">
										<span class="formlp-front-section--s0002c2-teiki-priceblock-after-tag-txt">
											<span class="formlp-front-section--s0002c2-teiki-priceblock-after-tag-txt1 <%: setting.GetAttributeValue("定期割引テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期割引テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期割引テキスト1"><%= setting.GetAttributeValue("定期割引テキスト1", "text") %></span>
											<span class="formlp-front-section--s0002c2-teiki-priceblock-after-tag-txt2 <%: setting.GetAttributeValue("定期割引テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期割引テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期割引テキスト2"><%= setting.GetAttributeValue("定期割引テキスト2", "text") %></span>
											<span class="formlp-front-section--s0002c2-teiki-priceblock-after-tag-txt3 <%: setting.GetAttributeValue("定期割引テキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期割引テキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期割引テキスト3"><%= setting.GetAttributeValue("定期割引テキスト3", "text") %></span>
										</span>
									</div>
								</div>
							</div>
							<div class="formlp-front-section--s0002c2-teiki-notes">
								<p class="formlp-front-section--s0002c2-teiki-note <%: setting.GetAttributeValue("定期注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期注釈"><%= setting.GetAttributeValue("定期注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002c2-teiki-btns">
								<a class="formlp-front-section--s0002c2-teiki-btn btn-hover-scale" href="<%: setting.GetAttributeValue("定期購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("定期購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="定期購入ボタン">
									<span class="formlp-front-section--s0002c2-teiki-btn-label1 <%: setting.GetAttributeValue("定期購入ボタンテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタンテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期購入ボタンテキスト1"><%= setting.GetAttributeValue("定期購入ボタンテキスト1", "text") %></span>
									<span class="formlp-front-section--s0002c2-teiki-btn-label2 <%: setting.GetAttributeValue("定期購入ボタンテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタンテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期購入ボタンテキスト2"><%= setting.GetAttributeValue("定期購入ボタンテキスト2", "text") %></span>
								</a>
							</div>
						</div>
						<div class="formlp-front-section--s0002c2-tanpin">
							<h2 class="formlp-front-section--s0002c2-tanpin-name" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("通常商品名背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="通常商品名背景">
								<span class="formlp-front-section--s0002c2-tanpin-name-text <%: setting.GetAttributeValue("通常商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常商品名"><%= setting.GetAttributeValue("通常商品名", "text") %></span>
							</h2>
							<div class="formlp-front-section--s0002c2-tanpin-priceblock">
								<div class="formlp-front-section--s0002c2-tanpin-priceblock-after">
									<p class="formlp-front-section--s0002c2-tanpin-priceblock-after-txt1 <%: setting.GetAttributeValue("通常価格", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("通常価格", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常価格"><%= setting.GetAttributeValue("通常価格", "text") %></p>
									<p class="formlp-front-section--s0002c2-tanpin-priceblock-after-price">
										<span class="formlp-front-section--s0002c2-tanpin-priceblock-after-price-value <%: setting.GetAttributeValue("通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額テキスト"><%= setting.GetAttributeValue("通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c2-tanpin-priceblock-after-price-unit <%: setting.GetAttributeValue("通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常単位テキスト"><%= setting.GetAttributeValue("通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c2-tanpin-priceblock-after-price-tax <%: setting.GetAttributeValue("通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常税込テキスト"><%= setting.GetAttributeValue("通常税込テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002c2-tanpin-notes">
								<p class="formlp-front-section--s0002c2-tanpin-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002c2-tanpin-btns">
								<a class="formlp-front-section--s0002c2-tanpin-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop="background,href" data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002c2-tanpin-btn-label1 <%: setting.GetAttributeValue("購入ボタンテキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト1"><%= setting.GetAttributeValue("購入ボタンテキスト1", "text") %></span>
									<span class="formlp-front-section--s0002c2-tanpin-btn-label2 <%: setting.GetAttributeValue("購入ボタンテキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト2"><%= setting.GetAttributeValue("購入ボタンテキスト2", "text") %></span>
								</a>
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002c3") { %>
			<div class="formlp-front-section formlp-front-section--s0002c3">
				<div class="formlp-front-section--s0002c3-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002c3-inner">
						<div class="formlp-front-section--s0002c3-teiki" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("定期商品ブロック", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="定期商品ブロック">
							<div class="formlp-front-section--s0002c3-teiki-tag" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("割引タグ", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="割引タグ">
								<span class="formlp-front-section--s0002c3-teiki-tag-txt">
									<span class="formlp-front-section--s0002c3-teiki-tag-txt1 <%: setting.GetAttributeValue("テキスト1", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト1", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト1"><%= setting.GetAttributeValue("テキスト1", "text") %></span>
									<span class="formlp-front-section--s0002c3-teiki-tag-txt2 <%: setting.GetAttributeValue("テキスト2", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト2", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト2"><%= setting.GetAttributeValue("テキスト2", "text") %></span>
									<span class="formlp-front-section--s0002c3-teiki-tag-txt3 <%: setting.GetAttributeValue("テキスト3", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト3", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト3"><%= setting.GetAttributeValue("テキスト3", "text") %></span>
								</span>
							</div>
							<h2 class="formlp-front-section--s0002c3-teiki-name <%: setting.GetAttributeValue("定期商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期商品名"><%= setting.GetAttributeValue("定期商品名", "text") %></h2>
							<div class="formlp-front-section--s0002c3-teiki-priceblock">
								<div class="formlp-front-section--s0002c3-teiki-priceblock-before">
									<p class="formlp-front-section--s0002c3-teiki-priceblock-before-price">
										<span class="formlp-front-section--s0002c3-teiki-priceblock-before-price-txt1 <%: setting.GetAttributeValue("定期通常価格", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常価格", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常価格"><%= setting.GetAttributeValue("定期通常価格", "text") %></span>
										<span class="formlp-front-section--s0002c3-teiki-priceblock-before-price-value <%: setting.GetAttributeValue("定期通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常金額テキスト"><%= setting.GetAttributeValue("定期通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c3-teiki-priceblock-before-price-unit <%: setting.GetAttributeValue("定期通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常単位テキスト"><%= setting.GetAttributeValue("定期通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c3-teiki-priceblock-before-price-tax <%: setting.GetAttributeValue("定期通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期通常税込テキスト"><%= setting.GetAttributeValue("定期通常税込テキスト", "text") %></span>
									</p>
								</div>
								<div class="formlp-front-section--s0002c3-teiki-priceblock-after">
									<p class="formlp-front-section--s0002c3-teiki-priceblock-after-txt1 <%: setting.GetAttributeValue("定期限定価格", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("定期限定価格", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定価格"><%= setting.GetAttributeValue("定期限定価格", "text") %></p>
									<p class="formlp-front-section--s0002c3-teiki-priceblock-after-price">
										<span class="formlp-front-section--s0002c3-teiki-priceblock-after-price-value <%: setting.GetAttributeValue("定期限定金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定金額テキスト"><%= setting.GetAttributeValue("定期限定金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c3-teiki-priceblock-after-price-unit <%: setting.GetAttributeValue("定期限定単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定単位テキスト"><%= setting.GetAttributeValue("定期限定単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c3-teiki-priceblock-after-price-tax <%: setting.GetAttributeValue("定期限定税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期限定税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期限定税込テキスト"><%= setting.GetAttributeValue("定期限定税込テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002c3-teiki-notes">
								<p class="formlp-front-section--s0002c3-teiki-note <%: setting.GetAttributeValue("定期注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期注釈"><%= setting.GetAttributeValue("定期注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002c3-teiki-btns">
								<a class="formlp-front-section--s0002c3-teiki-btn btn-hover-scale" href="<%: setting.GetAttributeValue("定期購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("定期購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="定期購入ボタン">
									<span class="formlp-front-section--s0002c3-teiki-btn-label <%: setting.GetAttributeValue("定期購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("定期購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="定期購入ボタンテキスト"><%= setting.GetAttributeValue("定期購入ボタンテキスト", "text") %></span>
								</a>
							</div>
						</div>
						<div class="formlp-front-section--s0002c3-tanpin">
							<h2 class="formlp-front-section--s0002c3-tanpin-name <%: setting.GetAttributeValue("通常商品名", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常商品名", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常商品名"><%= setting.GetAttributeValue("通常商品名", "text") %></h2>
							<div class="formlp-front-section--s0002c3-tanpin-priceblock">
								<div class="formlp-front-section--s0002c3-tanpin-priceblock-after">
									<p class="formlp-front-section--s0002c3-tanpin-priceblock-after-txt1 <%: setting.GetAttributeValue("通常価格", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("通常価格", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常価格"><%= setting.GetAttributeValue("通常価格", "text") %></p>
									<p class="formlp-front-section--s0002c3-tanpin-priceblock-after-price">
										<span class="formlp-front-section--s0002c3-tanpin-priceblock-after-price-value <%: setting.GetAttributeValue("通常金額テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常金額テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常金額テキスト"><%= setting.GetAttributeValue("通常金額テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c3-tanpin-priceblock-after-price-unit <%: setting.GetAttributeValue("通常単位テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常単位テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常単位テキスト"><%= setting.GetAttributeValue("通常単位テキスト", "text") %></span>
										<span class="formlp-front-section--s0002c3-tanpin-priceblock-after-price-tax <%: setting.GetAttributeValue("通常税込テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("通常税込テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="通常税込テキスト"><%= setting.GetAttributeValue("通常税込テキスト", "text") %></span>
									</p>
								</div>
							</div>
							<div class="formlp-front-section--s0002c3-tanpin-notes">
								<p class="formlp-front-section--s0002c3-tanpin-note <%: setting.GetAttributeValue("注釈", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("注釈", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="注釈"><%= setting.GetAttributeValue("注釈", "text") %></p>
							</div>
							<div class="formlp-front-section--s0002c3-tanpin-btns">
								<a class="formlp-front-section--s0002c3-tanpin-btn btn-hover-scale" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="購入ボタン">
									<span class="formlp-front-section--s0002c3-tanpin-btn-label <%: setting.GetAttributeValue("購入ボタンテキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("購入ボタンテキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタンテキスト"><%= setting.GetAttributeValue("購入ボタンテキスト", "text") %></span>
								</a>
							</div>
						</div>
					</div>
				</div>
			</div>
		<% } %>

		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0002") { %>
			<div class="formlp-front-section formlp-front-section--s0002">
				<div class="formlp-front-section--s0002-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0002-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0002-text <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0002-btns">
							<a class="formlp-front-section--s0002-btn btn-action <%: setting.GetAttributeValue("購入ボタン", "font") %>" href="<%: setting.GetAttributeValue("購入ボタン", "href") %>" target="<%: setting.GetAttributeValue("購入ボタン","target") %>" data-edit-prop="text,background,href" style='<%: setting.GetAttributeValueStyleFormat("購入ボタン", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="購入ボタン"><%= setting.GetAttributeValue("購入ボタン", "text") %></a>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：購入エリア ▲▲--%>

		<%--▼▼ ブロック：お悩み ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0003") { %>
			<div class="formlp-front-section formlp-front-section--s0003">
				<div class="formlp-front-section--s0003-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0003-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0003-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0003-body">
							<div class="formlp-front-section--s0003-body-img">
								<img src="<%: setting.GetAttributeValue("画像", "src") %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="画像">
							</div>
							<ul class="formlp-front-section--s0003-body-list">
								<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("見出しテキスト").Length; index++) { %>
									<li data-edit-group="1" data-edit-group-placeholder="リスト">
										<span class="<%: setting.GetAttributeValue("見出しテキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出しテキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出しテキスト"><%= setting.GetAttributeValue("見出しテキスト", "text", index) %></span>
									</li>
								<% } %>
							</ul>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：お悩み ▲▲--%>

		<%--▼▼ ブロック：選ばれる理由 ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0004") { %>
			<div class="formlp-front-section formlp-front-section--s0004">
				<div class="formlp-front-section--s0004-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0004-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0004-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>

						<div class="formlp-front-section--s0004-items">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("画像").Length; index++) { %>
								<div class="formlp-front-section--s0004-item" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<div class="formlp-front-section--s0004-item-img">
										<img src="<%: setting.GetAttributeValue("画像", "src", index) %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="画像">
									</div>
									<div class="formlp-front-section--s0004-item-list">
										<p class="formlp-front-section--s0004-item-list-cap <%: setting.GetAttributeValue("理由タグ", "font", index) %>" data-edit-prop="text,border" style='<%: setting.GetAttributeValueStyleFormat("理由タグ", new[] { "color", "font-size", "font-weight", "border-color" }, index) %>' data-edit-prop-placeholder="理由タグ"><%= setting.GetAttributeValue("理由タグ", "text", index) %></p>
										<h3 class="formlp-front-section--s0004-item-list-title <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></h3>
										<p class="formlp-front-section--s0004-item-list-text <%: setting.GetAttributeValue("テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="テキスト"><%= setting.GetAttributeValue("テキスト", "text", index) %></p>
									</div>
								</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：選ばれる理由 ▲▲--%>

		<%--▼▼ ブロック：3つの特徴 ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0006") { %>
			<div class="formlp-front-section formlp-front-section--s0006">
				<div class="formlp-front-section--s0006-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0006-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0006-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0006-items">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("タグ").Length; index++) { %>
							<div class="formlp-front-section--s0006-item" data-edit-group="1" data-edit-group-placeholder="ブロック">
								<h3 class="formlp-front-section--s0006-item-title">
										<span class="formlp-front-section--s0006-item-title-cap <%: setting.GetAttributeValue("タグ", "font") %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("タグ", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="タグ"><%= setting.GetAttributeValue("タグ", "text", index) %></span>
										<span class="formlp-front-section--s0006-item-title-value <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></span>
								</h3>
								<div class="formlp-front-section--s0006-item-img">
									<img src="<%: setting.GetAttributeValue("画像", "src", index) %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="画像">
								</div>
									<p class="formlp-front-section--s0006-item-text <%: setting.GetAttributeValue("テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="テキスト"><%= setting.GetAttributeValue("テキスト", "text", index) %></p>
							</div>
						<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：3つの特徴 ▲▲--%>

		<%--▼▼ ブロック：メリットデメリット ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0007") { %>
			<div class="formlp-front-section formlp-front-section--s0007">
				<div class="formlp-front-section--s0007-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0007-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0007-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0007-merit" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("メリットブロック", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="メリットブロック">
							<h3 class="formlp-front-section--s0007-merit-title <%: setting.GetAttributeValue("メリット見出し", "font") %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("メリット見出し", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="メリット見出し"><%= setting.GetAttributeValue("メリット見出し", "text") %></h3>
							<ul class="formlp-front-section--s0007-merit-list">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("メリットテキスト").Length; index++) { %>
								<li class="formlp-front-section--s0007-merit-list-item" data-edit-group="1" data-edit-group-placeholder="メリット">
										<span class="<%: setting.GetAttributeValue("メリットテキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("メリットテキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="メリットテキスト"><%= setting.GetAttributeValue("メリットテキスト", "text", index) %></span>
								</li>
							<% } %>
							</ul>
						</div>
						<div class="formlp-front-section--s0007-demerit" data-edit-prop="background,border" style='<%: setting.GetAttributeValueStyleFormat("デメリットブロック", new[] { "background-color", "background-image", "border-color" }) %>' data-edit-prop-placeholder="・デメリットブロック">
							<h3 class="formlp-front-section--s0007-demerit-title <%: setting.GetAttributeValue("デメリット見出し", "font") %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("デメリット見出し", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="デメリット見出し"><%= setting.GetAttributeValue("デメリット見出し", "text") %></h3>
							<ul class="formlp-front-section--s0007-demerit-list">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("デメリットテキスト").Length; index++) { %>
								<li class="formlp-front-section--s0007-demerit-list-item" data-edit-group="2" data-edit-group-placeholder="デメリット">
										<span class="<%: setting.GetAttributeValue("デメリットテキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("デメリットテキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="デメリットテキスト"><%= setting.GetAttributeValue("デメリットテキスト", "text", index) %></span>
								</li>
							<% } %>
							</ul>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：メリットデメリット ▲▲--%>

		<%--▼▼ ブロック：お客様の声 ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0008") { %>
			<div class="formlp-front-section formlp-front-section--s0008">
				<div class="formlp-front-section--s0008-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0008-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0008-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0008-voices">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("アイコン").Length; index++) { %>
							<div class="formlp-front-section--s0008-voice" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<div class="formlp-front-section--s0008-voice-icon" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("アイコン", new[] { "background-color", "background-image" }, index) %>' data-edit-prop-placeholder="アイコン"></div>
									<h3 class="formlp-front-section--s0008-voice-title <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></h3>
									<p class="formlp-front-section--s0008-voice-profile <%: setting.GetAttributeValue("プロフィール", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("プロフィール", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="プロフィール"><%= setting.GetAttributeValue("プロフィール", "text", index) %></p>
									<p class="formlp-front-section--s0008-voice-text <%: setting.GetAttributeValue("本文テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("本文テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="本文テキスト"><%= setting.GetAttributeValue("本文テキスト", "text", index) %></p>
							</div>
						<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：お客様の声 ▲▲--%>

		<%--▼▼ ブロック：よくあるご質問 ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0009") { %>
			<div class="formlp-front-section formlp-front-section--s0009">
				<div class="formlp-front-section--s0009-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0009-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0009-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0009-faq-items">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("質問アイコン").Length; index++) { %>
							<div class="formlp-front-section--s0009-faq-item" data-edit-group="1" data-edit-group-placeholder="質問">
								<div class="formlp-front-section--s0009-faq-q">
										<div class="formlp-front-section--s0009-faq-q-icon <%: setting.GetAttributeValue("質問アイコン", "font", index) %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("質問アイコン", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="質問アイコン"><%= setting.GetAttributeValue("質問アイコン", "text", index) %></div>
										<h3 class="formlp-front-section--s0009-faq-q-text <%: setting.GetAttributeValue("質問テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("質問テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="質問テキスト"><%= setting.GetAttributeValue("質問テキスト", "text", index) %></h3>
								</div>
								<div class="formlp-front-section--s0009-faq-a">
										<div class="formlp-front-section--s0009-faq-a-icon <%: setting.GetAttributeValue("回答アイコン", "font", index) %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("回答アイコン", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="回答アイコン"><%= setting.GetAttributeValue("回答アイコン", "text", index) %></div>
										<p class="formlp-front-section--s0009-faq-a-text <%: setting.GetAttributeValue("回答テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("回答テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="回答テキスト"><%= setting.GetAttributeValue("回答テキスト", "text", index) %></p>
								</div>
							</div>
						<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：よくあるご質問 ▲▲--%>

		<%--▼▼ ブロック：購入ステップ ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0010") { %>
			<div class="formlp-front-section formlp-front-section--s0010">
				<div class="formlp-front-section--s0010-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0010-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0010-title" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0010-steps">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("タグ").Length; index++) { %>
							<div class="formlp-front-section--s0010-step" data-edit-group="1" data-edit-group-placeholder="ステップ">
									<p class="formlp-front-section--s0010-step-cap <%: setting.GetAttributeValue("タグ", "font", index) %>" data-edit-prop="background,text" style='<%: setting.GetAttributeValueStyleFormat("タグ", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="タグ"><%= setting.GetAttributeValue("タグ", "text", index) %></p>
									<h3 class="formlp-front-section--s0010-step-title <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></h3>
								<div class="formlp-front-section--s0010-step-body">
									<div class="formlp-front-section--s0010-step-img">
										<img src="<%: setting.GetAttributeValue("画像", "src", index) %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="画像">
									</div>
										<p class="formlp-front-section--s0010-step-text <%: setting.GetAttributeValue("本文", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("本文", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="本文"><%= setting.GetAttributeValue("本文", "text", index) %></p>
								</div>
							</div>
						<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：購入ステップ ▲▲--%>

		<%--▼▼ ブロック：アクセス ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0011") { %>
			<div class="formlp-front-section formlp-front-section--s0011">
				<div class="formlp-front-section--s0011-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0011-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0011-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0011-items">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("名前").Length; index++) { %>
							<div class="formlp-front-section--s0011-item" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<h3 class="formlp-front-section--s0011-item-title <%: setting.GetAttributeValue("名前", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("名前", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="名前"><%= setting.GetAttributeValue("名前", "text", index) %></h3>
								<div class="formlp-front-section--s0011-item-img">
									<img src="<%: setting.GetAttributeValue("画像", "src", index) %>" alt="" data-edit-prop="src" data-edit-prop-placeholder="画像">
								</div>
									<p class="formlp-front-section--s0011-item-text <%: setting.GetAttributeValue("テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="テキスト">
									<%= setting.GetAttributeValue("テキスト", "text", index) %>
								</p>
							</div>
						<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：アクセス ▲▲--%>

		<%--▼▼ ブロック：会社情報 ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0012") { %>
			<div class="formlp-front-section formlp-front-section--s0012">
				<div class="formlp-front-section--s0012-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0012-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0012-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0012-items">
						<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("タグ").Length; index++) { %>
							<div class="formlp-front-section--s0012-item" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<h3 class="formlp-front-section--s0012-item-title <%: setting.GetAttributeValue("タグ", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("タグ", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="タグ"><%= setting.GetAttributeValue("タグ", "text", index) %></h3>
									<p class="formlp-front-section--s0012-item-descrirption <%: setting.GetAttributeValue("内容", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("内容", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="内容"><%= setting.GetAttributeValue("内容", "text", index) %></p>
									<p class="formlp-front-section--s0012-item-text <%: setting.GetAttributeValue("テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="テキスト">
									<%= setting.GetAttributeValue("テキスト", "text", index) %>
								</p>
							</div>
						<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：会社情報 ▲▲--%>

		<%--▼▼ ブロック：募集要項 ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0013") { %>
			<div class="formlp-front-section formlp-front-section--s0013">
				<div class="formlp-front-section--s0013-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0013-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0013-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0013-item">
							<p class="formlp-front-section--s0013-item-descrirption <%: setting.GetAttributeValue("テキスト", "font") %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="テキスト"><%= setting.GetAttributeValue("テキスト", "text") %></p>
							<table class="formlp-front-section--s0013-item-table">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("項目名").Length; index++) { %>
								<tr data-edit-group="1" data-edit-group-placeholder="ブロック">
										<th class="<%: setting.GetAttributeValue("項目名", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("項目名", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="項目名"><%= setting.GetAttributeValue("項目名", "text", index) %></th>
										<td class="<%: setting.GetAttributeValue("内容", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("内容", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="内容"><%= setting.GetAttributeValue("内容", "text", index) %></td>
								</tr>
							<% } %>
							</table>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：募集要項 ▲▲--%>

		<%--▼▼ ブロック：旧SNS ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0014") { %>
			<div class="formlp-front-section formlp-front-section--s0014">
				<div class="formlp-front-section--s0014-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0014-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0014-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0014-items">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ボタン").Length; index++) { %>
								<div class="formlp-front-section--s0014-item formlp-front-section--s0014-item-facebook" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<a href="<%: setting.GetAttributeValue("ボタン", "href", index) %>" target="<%: setting.GetAttributeValue("ボタン", "target", index) %>" data-edit-prop="background,text,href" style='<%: setting.GetAttributeValueStyleFormat("ボタン", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ボタン"><%= setting.GetAttributeValue("ボタン", "text", index) %></a>
								</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：旧SNS ▲▲--%>
	
		<%--▼▼ ブロック：SNS ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--s0015") { %>
			<div class="formlp-front-section formlp-front-section--s0015">
				<div class="formlp-front-section--s0015-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--s0015-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<h2 class="formlp-front-section--s0015-title <%: setting.GetAttributeValue("タイトル", "font") %>" data-edit-prop="text,background" style='<%: setting.GetAttributeValueStyleFormat("タイトル", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="タイトル"><%= setting.GetAttributeValue("タイトル", "text") %></h2>
						<div class="formlp-front-section--s0015-items">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("ボタン").Length; index++) { %>
								<div class="formlp-front-section--s0015-item formlp-front-section--s0015-item-facebook" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<a href="<%: setting.GetAttributeValue("ボタン", "href", index) %>" target="<%: setting.GetAttributeValue("ボタン", "target", index) %>" data-edit-prop="background,text,href" style='<%: setting.GetAttributeValueStyleFormat("ボタン", new[] { "background-color", "background-image", "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="ボタン"><%= setting.GetAttributeValue("ボタン", "text", index) %></a>
								</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：SNS ▲▲--%>

		<%--▼▼ ブロック：カートエリア ▼▼--%>
		<% if(setting.BlockClassName == "formlp-front-section-form") { %>
			<uc:LpInputForm id="ucInputForm" runat="server" />
		<% } %>
		<%--▲▲ ブロック：カートエリア ▲▲--%>

		<%--▼▼ ブロック：フッター ▼▼--%>
		<% if (setting.BlockClassName == "formlp-front-section formlp-front-section--footer") { %>
			<div class="formlp-front-section formlp-front-section--footer">
				<div class="formlp-front-section--footer-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--footer-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<div class="formlp-front-section--footer-items">
							<% for (var index = 0; index < setting.GetListElementsByPlaceHolder("見出し").Length; index++) { %>
								<div class="formlp-front-section--footer-item" data-edit-group="1" data-edit-group-placeholder="ブロック">
									<p class="formlp-front-section--footer-item-cap <%: setting.GetAttributeValue("見出し", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("見出し", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="見出し"><%= setting.GetAttributeValue("見出し", "text", index) %></p>
									<p class="formlp-front-section--footer-item-text <%: setting.GetAttributeValue("テキスト", "font", index) %>" data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("テキスト", new[] { "color", "font-size", "font-weight" }, index) %>' data-edit-prop-placeholder="テキスト"><%= setting.GetAttributeValue("テキスト", "text", index) %></p>
								</div>
							<% } %>
						</div>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：フッター ▲▲--%>

		<%--▼▼ ブロック：コピーライト ▼▼--%>
		<% if(setting.BlockClassName == "formlp-front-section formlp-front-section--copyright" ) { %>
			<div class="formlp-front-section formlp-front-section--copyright">
				<div class="formlp-front-section--copyright-outer" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景">
					<div class="formlp-front-section--copyright-inner" data-edit-prop="background" style='<%: setting.GetAttributeValueStyleFormat("背景（内側）", new[] { "background-color", "background-image" }) %>' data-edit-prop-placeholder="背景（内側）">
						<p class='<%: setting.GetAttributeValue("コピーライト", "font") %>' data-edit-prop="text" style='<%: setting.GetAttributeValueStyleFormat("コピーライト", new[] { "color", "font-size", "font-weight" }) %>' data-edit-prop-placeholder="コピーライト"><%= setting.GetAttributeValue("コピーライト", "text") %></p>
					</div>
				</div>
			</div>
		<% } %>
		<%--▲▲ ブロック：コピーライト ▲▲--%>

	<% } %>
	<%--▲▲ ブロック：ループ ▲▲--%>

</asp:Content>
