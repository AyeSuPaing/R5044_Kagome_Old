/*
=========================================================================================================
  Module      : 新着情報入力クラス (NewsInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Input;
using w2.Domain.News;

/// <summary>
/// 新着情報マスタ入力クラス
/// </summary>
public class NewsInput : InputBase<NewsModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public NewsInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public NewsInput(NewsModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.NewsId = model.NewsId;
		this.DisplayDateFrom = model.DisplayDateFrom.ToString();
		this.NewsTextKbn = model.NewsTextKbn;
		this.NewsText = model.NewsText;
		this.NewsTextKbnMobile = model.NewsTextKbnMobile;
		this.NewsTextMobile = model.NewsTextMobile;
		this.DispFlg = model.DispFlg;
		this.MobileDispFlg = model.MobileDispFlg;
		this.DisplayOrder = model.DisplayOrder.ToString();
		this.ValidFlg = model.ValidFlg;
		this.DelFlg = model.DelFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.BrandId = model.BrandId;
		this.DisplayDateTo = (model.DisplayDateTo.HasValue) ? model.DisplayDateTo.ToString() : null;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override NewsModel CreateModel()
	{
		var model = new NewsModel
		{
			ShopId = this.ShopId,
			NewsId = this.NewsId,
			DisplayDateFrom = DateTime.Parse(this.DisplayDateFrom),
			NewsTextKbn = this.NewsTextKbn,
			NewsText = this.NewsText,
			NewsTextKbnMobile = this.NewsTextKbnMobile,
			NewsTextMobile = this.NewsTextMobile,
			DispFlg = this.DispFlg,
			MobileDispFlg = this.MobileDispFlg,
			DisplayOrder = int.Parse(this.DisplayOrder),
			ValidFlg = this.ValidFlg,
			DelFlg = this.DelFlg,
			LastChanged = this.LastChanged,
			BrandId = this.BrandId,
			DisplayDateTo = (this.DisplayDateTo != null) ? DateTime.Parse(this.DisplayDateTo) : (DateTime?)null,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string validatorName)
	{
		// 入力チェック
		var errorMessages = new StringBuilder(Validator.Validate(validatorName, this.DataSource));

		if ((this.DisplayDateTo != null)
			&& (Validator.IsDate(this.DisplayDateFrom))
			&& (Validator.IsDate(this.DisplayDateTo))
			&& (Validator.CheckDateRange(this.DisplayDateFrom, this.DisplayDateTo) == false))
		{
			errorMessages.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_DATERANGE).Replace("@@ 1 @@", "表示日付"));
		}

		return errorMessages.ToString();
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_NEWS_SHOP_ID] = value; }
	}
	/// <summary>新着ID</summary>
	public string NewsId
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_ID]; }
		set { this.DataSource[Constants.FIELD_NEWS_NEWS_ID] = value; }
	}
	/// <summary>表示日付（From）</summary>
	public string DisplayDateFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_FROM]; }
		set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_FROM] = value; }
	}
	/// <summary>本文区分</summary>
	public string NewsTextKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN]; }
		set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN] = value; }
	}
	/// <summary>本文</summary>
	public string NewsText
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT]; }
		set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT] = value; }
	}
	/// <summary>モバイル本文区分</summary>
	public string NewsTextKbnMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN_MOBILE]; }
		set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_KBN_MOBILE] = value; }
	}
	/// <summary>モバイル本文</summary>
	public string NewsTextMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_MOBILE]; }
		set { this.DataSource[Constants.FIELD_NEWS_NEWS_TEXT_MOBILE] = value; }
	}
	/// <summary>表示フラグ</summary>
	public string DispFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DISP_FLG]; }
		set { this.DataSource[Constants.FIELD_NEWS_DISP_FLG] = value; }
	}
	/// <summary>モバイル表示フラグ</summary>
	public string MobileDispFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_MOBILE_DISP_FLG]; }
		set { this.DataSource[Constants.FIELD_NEWS_MOBILE_DISP_FLG] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_ORDER] = value; }
	}
	/// <summary>有効フラグ</summary>
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_NEWS_VALID_FLG] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_NEWS_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_NEWS_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_NEWS_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_NEWS_LAST_CHANGED] = value; }
	}
	/// <summary>ブランドID</summary>
	public string BrandId
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_BRAND_ID]; }
		set { this.DataSource[Constants.FIELD_NEWS_BRAND_ID] = value; }
	}
	/// <summary>表示日付（To）</summary>
	public string DisplayDateTo
	{
		get { return (string)this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO]; }
		set { this.DataSource[Constants.FIELD_NEWS_DISPLAY_DATE_TO] = value; }
	}
	#endregion
}
