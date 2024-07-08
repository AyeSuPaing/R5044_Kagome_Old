<%--
=========================================================================================================
  Module      : 枠保証登録情報(FrameGurantee.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>

<%@ Register TagPrefix="uc" TagName="BodyMyPageMenu" Src="~/SmartPhone/Form/Common/BodyMyPageMenu.ascx" %>
<%@ Page Language="C#" MasterPageFile="~/SmartPhone/Form/Common/UserPage.master" AutoEventWireup="true" CodeFile="~/Form/User/FrameGuarantee.aspx.cs" Inherits="Form_User_FrameGuarantee" Title="枠保証登録情報" %>
<%@ Register TagPrefix="uc" TagName="MemberRankUpDisplay" Src="~/SmartPhone/Form/Common/MemberRankUpDisplay.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<section class="wrap-user user-credit-card-input">
		<div class="user-unit">
			<h2>枠保証登録情報</h2>
			<div class="msg">
					<% if (this.errorMessage.Count > 0)
					{ %>
					<div class="dvContentsInfo" style="color:#f00">
						<% foreach (var item in this.errorMessage)
						{ %>
						<p class="attention"><%: item  %></p>
						<% } %>   
					</div>
				<%} %>
			</div>
			<div id="divCreditCardInputForm">
				<dl class="user-form">
					<dt>
						審査状況
					</dt>
					<dd>
						<p>
							 <asp:Literal ID="lcreditStatus" runat="server"></asp:Literal>
						</p>
					</dd>
					<dt>
						<%: ReplaceTag("@@User.OwnerName1.name@@") %>
						<span class="require">※</span>

					</dt>
					<dd>
						<asp:TextBox id="tbOwnerName1" Runat="server" MaxLength="21"  width="100%"
							CssClass="shortTel">
						</asp:TextBox>
						<asp:CustomValidator ID="cvpresidentNameFamily" runat="Server"
							ControlToValidate="tbOwnerName1" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true"
							ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</dd>
					<dt>
							<%: ReplaceTag("@@User.OwnerName2.name@@") %>
						<span class="require">※</span>

					</dt>
					<dd>
						<asp:TextBox id="tbOwnerName2" Runat="server" MaxLength="21"  width="100%">
						</asp:TextBox>
						<asp:CustomValidator ID="cvpresidentName" runat="Server"
							ControlToValidate="tbOwnerName2" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true"
							ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</dd>
					<dt>
						<%: ReplaceTag("@@User.OwnerNameKana1.name@@") %>
						<span class="require">※</span>

					</dt>
					<dd>
						<asp:TextBox id="tbOwnerNameKana1" Runat="server" MaxLength="25" 
							width="100%">
						</asp:TextBox>
						<asp:CustomValidator ID="cvpresidentNameFamilyKana" runat="Server"
							ControlToValidate="tbOwnerNameKana1" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true"
							ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</dd>

					<dt>
						<%: ReplaceTag("@@User.OwnerNameKana2.name@@") %>
						<span class="require">※</span>

					</dt>
					<dd>
						<asp:TextBox id="tbOwnerNameKana2" Runat="server" MaxLength="25" 
							width="100%">
						</asp:TextBox>
						<asp:CustomValidator ID="cvpresidentNameKana" runat="Server"
							ControlToValidate="tbOwnerNameKana2" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true"
							ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</dd>

					<dt>
						<%: ReplaceTag("@@User.OwnerBirth.name@@") %>
						<span class="require">※</span>

					</dt>
					<dd class="birth">
						<asp:DropDownList id="ddlOwnerBirthYear" runat="server" CssClass="year"
							onchange="changeGmoDropDownDays()"></asp:DropDownList>年
						<asp:DropDownList id="ddlOwnerBirthMonth" runat="server" CssClass="month"
							onchange="changeGmoDropDownDays()"></asp:DropDownList>月
						<asp:DropDownList id="ddlOwnerBirthDay" runat="server" CssClass="date">
						</asp:DropDownList>日

						<asp:CustomValidator ID="cvOwnerBirthYear" runat="Server"
							ControlToValidate="ddlOwnerBirthYear" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true" EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator ID="cvOwnerBirthMonth" runat="Server"
							ControlToValidate="ddlOwnerBirthMonth" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true" EnableClientScript="false"
							CssClass="error_inline" />
						<asp:CustomValidator ID="cvOwnerBirthDay" runat="Server"
							ControlToValidate="ddlOwnerBirthDay" ValidationGroup="UserBusinessOwner"
							ValidateEmptyText="true" SetFocusOnError="true" EnableClientScript="false"
							CssClass="error_inline" />
					</dd>

					<dt>
						<%: ReplaceTag("@@User.RequestBudget.name@@") %>
						<span class="require">※</span>

					</dt>
					<dd>
						<div>
								<asp:TextBox id="tbRequestBudget" Runat="server" MaxLength="8"  width="30%" style="text-align: right">
							</asp:TextBox>
							<p class="limit-unit">円</p>
						</div>
							<asp:CustomValidator ID="cvreqUpperLimit" runat="Server"
								ControlToValidate="tbRequestBudget" ValidationGroup="UserBusinessOwner"
								ValidateEmptyText="true" SetFocusOnError="true"
								ClientValidationFunction="ClientValidate" CssClass="error_inline" />
					</dd>
                   
				</dl>
			</div>

		</div>
		<div class="user-footer">
			<div class="button-next">
				<asp:LinkButton ID="lbConfirm" ValidationGroup="UserBusinessOwner" OnClientClick="return exec_submit();" runat="server" OnClick="lbConfirm_Click" class="btn">確認する</asp:LinkButton>

			</div>
		</div>
	</section>
	<script type="text/javascript">
		changeGmoDropDownDays()
		<%-- 日付リスト変更 --%>
		function changeGmoDropDownDays() {
			var year = $("#<%= ddlOwnerBirthYear.ClientID %>").val();
			var month = $("#<%= ddlOwnerBirthMonth.ClientID %>").val();

			var select = document.getElementById('<%= ddlOwnerBirthDay.ClientID %>');

			var maxDay = new Date(year, month, 0).getDate();

			var optionsList = document.getElementById('<%= ddlOwnerBirthDay.ClientID %>').options;
			var daysCount = parseInt(optionsList[optionsList.length - 1].value);

			if (daysCount < maxDay) {
				for (var day = 0; day < (maxDay - daysCount) ; day++) {
					var appendDay = daysCount + day + 1;
					var option = document.createElement('option');
					option.setAttribute('value', appendDay);
					option.innerHTML = appendDay;
					select.appendChild(option);
				}
			} else {
				for (var dayDifference = 0; dayDifference < daysCount - maxDay; dayDifference++) {
					document.getElementById('<%= ddlOwnerBirthDay.ClientID %>').remove(daysCount - dayDifference);
				}
			}
		}

	</script>
</asp:Content>