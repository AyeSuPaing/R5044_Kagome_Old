/*
=========================================================================================================
  Module      : SEOメタデータ入力クラス(SeoMetadatasInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Input;
using w2.Domain.SeoMetadatas;

/// <summary>
/// SEOメタデータ入力クラス
/// </summary>
public class SeoMetadatasInput : InputBase<SeoMetadatasModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public SeoMetadatasInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public SeoMetadatasInput(SeoMetadatasModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.DataKbn = model.DataKbn;
		this.HtmlTitle = model.HtmlTitle;
		this.MetadataKeywords = model.MetadataKeywords;
		this.MetadataDesc = model.MetadataDesc;
		this.Comment = model.Comment;
		this.DelFlg = model.DelFlg;
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override SeoMetadatasModel CreateModel()
	{
		var model = new SeoMetadatasModel
		{
			ShopId = this.ShopId,
			DataKbn = this.DataKbn,
			HtmlTitle = this.HtmlTitle,
			MetadataKeywords = this.MetadataKeywords,
			MetadataDesc = this.MetadataDesc,
			Comment = this.Comment,
			LastChanged = this.LastChanged,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="registOrModify">登録か変更か（true:登録、false:変更）</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(bool registOrModify)
	{
		var errorMessage = Validator.Validate(registOrModify ? "SeoMetadatasRegist" : "SeoMetadatasModify", this.DataSource);
		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_SHOP_ID] = value; }
	}
	/// <summary>データ区分</summary>
	public string DataKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DATA_KBN]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATA_KBN] = value; }
	}
	/// <summary>タイトル</summary>
	public string HtmlTitle
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_HTML_TITLE]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_HTML_TITLE] = value; }
	}
	/// <summary>キーワード</summary>
	public string MetadataKeywords
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_KEYWORDS]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_KEYWORDS] = value; }
	}
	/// <summary>ディスクリプション</summary>
	public string MetadataDesc
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_DESC]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_METADATA_DESC] = value; }
	}
	/// <summary>コメント</summary>
	public string Comment
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_COMMENT]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_COMMENT] = value; }
	}
	/// <summary>削除フラグ</summary>
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_DEL_FLG] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_SEOMETADATAS_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_SEOMETADATAS_LAST_CHANGED] = value; }
	}
	#endregion
}