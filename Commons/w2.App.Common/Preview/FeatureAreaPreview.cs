/*
=========================================================================================================
  Module      : 特集エリアプレビュークラス(FeatureAreaPreview.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Text;

namespace w2.App.Common.Preview
{
	/// <summary>
	/// 特集エリアプレビュークラス
	/// </summary>
	public class FeatureAreaPreview : BasePreview
	{
		/// <summary>
		/// 特集エリアプレビュー情報登録
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="datasource">特集エリア情報</param>
		public static void InsertFeatureAreaPreview(string areaId, Hashtable datasource)
		{
			InsertPreview(Constants.FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA, areaId, "", "", "", "", datasource);
		}

		/// <summary>
		/// 特集エリアバナープレビュー情報登録
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="datasource">特集エリアバナー情報</param>
		public static void InsertFeatureAreaBannerPreview(string areaId, Hashtable[] datasource)
		{
			InsertPreview(Constants.FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA_BANNER, areaId, "", "", "", "", datasource);
		}

		/// <summary>
		/// 特集エリアプレビュー情報取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>特集エリアプレビュー情報</returns>
		public static Hashtable GetFeatureAreaPreview(string areaId)
		{
			var result = GetPreview<Hashtable>(Constants.FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA, areaId, "", "", "", "");
			return result;
		}

		/// <summary>
		/// 特集エリアバナープレビュー情報取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>特集エリアプレビュー情報</returns>
		public static Hashtable[] GetFeatureAreaBannerPreview(string areaId)
		{
			var results = GetPreviews<Hashtable[]>(Constants.FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA_BANNER, areaId, "", "", "", "");
			return results;
		}

		/// <summary>
		/// 特集エリアプレビュー用ハッシュ値を生成する
		/// </summary>
		/// <returns>特集エリアプレビュー用ハッシュ値</returns>
		public static string CreateFeatureAreaHash()
		{
			return CreateHash(Constants.FLG_PREVIEW_PREVIEW_KBN_FEATURE_AREA, 32);
		}

		/// <summary>
		/// プレビューページ作成
		/// </summary>
		/// <param name="isThumb">サムネイル作成か</param>
		/// <param name="isPc">PCのプレビューか</param>
		/// <returns>プレビューページ</returns>
		public static string CreatePreviewPage(bool isThumb, bool isPc)
		{
			// プレビューページを作成
			var previewPage = new StringBuilder();
			previewPage.Append("<%@ Register TagPrefix=\"uc\" TagName=\"Preview\" Src=\"~" + (isPc ? "" : "/SmartPhone") + "/Form/PageTemplates/PartsBannerTemplate.ascx.Preview.ascx\" %>");
			previewPage.Append("<%@ Page Language=\"C#\" MasterPageFile=\"~" + (isPc ? "/" : "/SmartPhone/") + (isThumb ? Constants.PAGE_FRONT_DEFAULT_MASTER : Constants.PAGE_FRONT_DEFAULT_PREVIEW) + "\" %>\r\n");
			previewPage.Append("<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"ContentPlaceHolder1\" Runat=\"Server\">\r\n");
			previewPage.Append("<uc:Preview runat=\"server\" IsPreview=\"True\" />\r\n");
			previewPage.Append("</asp:Content>");
			return previewPage.ToString();
		}
	}
}
