<%--
=========================================================================================================
  Module      : Top (Top.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="~/Form/ScoringSale/Top.aspx.cs" Inherits="Form_ScoringSale_Top" %>
<%@ Register TagPrefix="uc" TagName="ScoringSaleTopPage" Src="~/SmartPhone/Form/Common/ScoringSale/ScoringSaleTopPage.ascx" %>
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<meta charset="UTF-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title></title>
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/base.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/reset.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/user.css" />
	<link rel="stylesheet" href="<%= Constants.PATH_ROOT %>SmartPhone/Css/ScoringSale/scoringsale.css" />
	<link rel ="preconnect" href="https://fonts.googleapis.com" />
	<link rel ="preconnect" href="https://fonts.gstatic.com" crossorigin />
	<link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&display=swap" rel="stylesheet" />
	<script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.11.1.min.js"></script>
</head>
<body>
	<form onsubmit="return (document.getElementById('__EVENTVALIDATION') != null);" runat="server">
		<div id="Wrap">
			<div class="wrapBottom">
				<div class="wrapTop">
					<div id="Contents">
						<div id="scoringsale" class="scoringsale colorPattern-1">
							<div class="scoringsale_inner">
								<div class="scoringsale_item">
									<uc:ScoringSaleTopPage ID="ScoringSaleTopPage1" runat="server" />
								</div>
							</div>
						</div>
					</div>
				</div>
				<!--wrapTop-->
			</div>
			<!--wrapBottom-->
		</div>
		<!--Wrap-->
	</form>
</body>
</html>
