<%--
=========================================================================================================
  Module      : ブランドHTML出力コントローラ(BodyProductBrandHtml.ascx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Form/Common/Product/BodyProductBrandHtml.ascx.cs" Inherits="Form_Common_Product_BodyProductBrandHtml" %>
<%--

下記のタグはファイル情報保持用です。削除しないでください。
タイトルタグはカスタムパーツのみ利用します。
<%@ Page Title="無名のパーツ" %>
<%@ FileInfo LastChanged="最終更新者" %>

--%>
<script runat="server">
public new void Page_Init(Object sender, EventArgs e)
{
base.Page_Init(sender, e);

<%-- ▽編集可能領域：プロパティ設定▽ --%>

//------------------------------------------------------
// フォルダ名を指定します。
// 指定されたフォルダ名を元に下記HTMLを自動で読み込みます。
// ROOT/Contents/Html/Brand/フォルダ名/ブランドID.html
// 
// 例）
// フォルダ名「Left」ブランド「tst」の場合
// ROOT/Contents/Html/Brand/Left/tst.html
//------------------------------------------------------
this.FolderName = "Left";

<%-- △編集可能領域△ --%>
}
</script>

<%-- html別ファイル領域※ブランド毎に所定のhtmlファイルを所定のディレクトリに設置する事で自動で読み込まれます。ファイルが存在しない場合は、default.htmlが読み込まれます。--%>
<div id="divBrandHtml" runat="server"></div>