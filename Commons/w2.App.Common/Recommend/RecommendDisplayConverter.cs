/*
=========================================================================================================
  Module      : レコメンド表示変換クラス(RecommendDisplayConverter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Web;
using w2.Domain.Recommend;

namespace w2.App.Common.Recommend
{
	/// <summary>
	/// レコメンド表示タグ置換クラス
	/// </summary>
	[Serializable]
	public class RecommendDisplayConverter
	{
		#region 定数
		/// <summary>レコメンド商品投入ボタン画像URLタグ</summary>
		public const string TAG_ADD_ITEM_BUTTONIMAGE_URL = "@@ add_item_button_image_url @@";
		/// <summary>レコメンド商品投入ボタンリンクタグ</summary>
		public const string TAG_ADD_ITEM_LINK = "@@ add_item_link @@";
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="recommend">レコメンド設定</param>
		/// <param name="addItemLinkUniqueID">レコメンド商品投入リンクボタンのユニークID</param>
		/// <param name="isSmartPhone">スマートフォンか？</param>
		public RecommendDisplayConverter(RecommendModel recommend, string addItemLinkUniqueID, bool isSmartPhone)
		{
			this.Recommend = recommend;
			this.AddItemLinkUniqueID = addItemLinkUniqueID;
			this.IsSmartPhone = isSmartPhone;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// レコメンド表示変換
		/// </summary>
		/// <returns>タグ置換及びエンコード後のレコメンド表示</returns>
		public string ExecAndGetRecommendHtml()
		{
			// レコメンド表示取得
			var recommendDisplay =
				(this.IsSmartPhone == false)
				? this.Recommend.RecommendDisplayPc
				: this.Recommend.RecommendDisplaySp;
			var isDisplayText =
				(this.IsSmartPhone == false)
				? (this.Recommend.RecommendDisplayKbnPc == Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_PC_TEXT)
				: (this.Recommend.RecommendDisplayKbnSp == Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_KBN_SP_TEXT);

			// タグ置換
			recommendDisplay = ReplaceRecommendDisplayTag(recommendDisplay);

			// TEXT or HTMLで返す
			return isDisplayText ? HtmlSanitizer.HtmlEncode(recommendDisplay) : recommendDisplay;
		}

		/// <summary>
		/// レコメンド表示タグ置換
		/// </summary>
		/// <param name="recommendDisplay">レコメンド表示</param>
		/// <returns>タグ置換後のレコメンド表示</returns>
		private string ReplaceRecommendDisplayTag(string recommendDisplay)
		{
			// レコメンド商品投入リンク置換
			var addItemLink
				= string.Format("javascript:__doPostBack('{0}','');", this.AddItemLinkUniqueID);
			recommendDisplay = recommendDisplay.Replace(TAG_ADD_ITEM_LINK, addItemLink);

			// ボタン画像ファイルパス置換
			var buttonImageType = 
				(this.IsSmartPhone == false)
				? ButtonImageType.AddItemPc
				: ButtonImageType.AddItemSp;
			var buttonImageOperator = new RecommendButtonImageOperator(buttonImageType);
			var filePath = buttonImageOperator.GetRecommendButtonImageFilePath(this.Recommend.RecommendId);
			recommendDisplay = recommendDisplay.Replace(TAG_ADD_ITEM_BUTTONIMAGE_URL, filePath);

			return recommendDisplay;
		}
		#endregion

		#region プロパティ
		/// <summary>レコメンド設定</summary>
		private RecommendModel Recommend { get; set; }
		/// <summary>レコメンド商品投入リンクボタンのユニークID</summary>
		private string AddItemLinkUniqueID { get; set; }
		/// <summary>スマートフォンか？</summary>
		private bool IsSmartPhone { get; set; }
		#endregion
	}
}