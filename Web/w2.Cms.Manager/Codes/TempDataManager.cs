/*
=========================================================================================================
  Module      : TempDataManager(TempDataManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ParamModels.FeaturePageCategory;
using w2.Cms.Manager.ParamModels.News;
using w2.Cms.Manager.ParamModels.ProductGroup;
using w2.Cms.Manager.ParamModels.ProductTagManager;
using w2.Cms.Manager.ParamModels.TagManager;
using w2.Cms.Manager.ParamModels.ProductRanking;
using w2.Domain.CoordinateCategory;
using w2.Domain.FeaturePageCategory;
using w2.Domain.FeaturePage;
using w2.Domain.Staff;
using w2.Domain.MenuAuthority;
using w2.Domain.ProductRanking;
using w2.Domain.ShopOperator;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// TempDataManager
	/// </summary>
	public class TempDataManager
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TempDataManager(HttpSessionStateBase session)
		{
			this.Session = session;
			if (this.TempData == null) this.TempData = new Dictionary<object, KeyValuePair<DateTime, object>>();
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>値</returns>
		private object Get(string key)
		{
			if (this.TempData.ContainsKey(key) == false) return null;
			var kvp = this.TempData[key];
			return kvp.Value;
		}

		/// <summary>
		/// セット
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		private void Set(string key, object value)
		{
			this.TempData[key] = new KeyValuePair<DateTime, object>(DateTime.Now, value);
		}
		/// <summary>店舗管理者</summary>
		public ShopOperatorModel ShopOperator
		{
			get { return (ShopOperatorModel)Get("ShopOperatorModel"); }
			set { Set("ShopOperatorModel", value); }
		}
		/// <summary>スタッフモデル</summary>
		public StaffModel Staff
		{
			get { return (StaffModel)Get("StaffModel"); }
			set { Set("StaffModel", value); }
		}
		/// <summary>コーディネートカテゴリモデル</summary>
		public CoordinateCategoryModel CoordinateCategoryModel
		{
			get { return (CoordinateCategoryModel)Get("CoordinateCategoryModel"); }
			set { Set("CoordinateCategoryModel", value); }
		}
		/// <summary>メニュー権限</summary>
		public MenuAuthorityModel[] MenuAuthority
		{
			get { return (MenuAuthorityModel[])Get("MenuAuthorityModel"); }
			set { Set("MenuAuthorityModel", value);}
		}
		/// <summary>お知らせ一覧パラメータモデル</summary>
		public NewsListParamModel NewsListParamModel
		{
			get { return (NewsListParamModel)Get("NewsListParamModel"); }
			set { Set("NewsListParamModel", value); }
		}
		/// <summary>タグマネージャーパラメータモデル</summary>
		public TagManagerListParamModel TagManagerListParamModel
		{
			get { return (TagManagerListParamModel)Get("TagManagerListParamModel"); }
			set { Set("TagManagerListParamModel", value); }
		}
		/// <summary>商品タグマネージャーパラメータモデル</summary>
		public ProductTagManagerListParamModel ProductTagManagerListParamModel
		{
			get { return (ProductTagManagerListParamModel)Get("ProductTagManagerListParamModel"); }
			set { Set("ProductTagManagerListParamModel", value); }
		}
		/// <summary>マスタ出力設定インプットモデル</summary>
		public MasterExportSettingInput MasterExportSettingInput
		{
			get { return (MasterExportSettingInput)Get("MasterExportSettingInput"); }
			set { Set("MasterExportSettingInput", value); }
		}
		/// <summary>マスタ種別</summary>
		public string MasterType
		{
			get { return (string)Get("MasterType"); }
			set { Set("MasterType", value); }
		}
		/// <summary>商品ランキング一覧パラメータモデル</summary>
		public ProductRankingListParamModel ProductRankingListParamModel
		{
			get { return (ProductRankingListParamModel)Get("ProductRankingListParamModel"); }
			set { Set("ProductRankingListParamModel", value); }
		}
		/// <summary>商品ランキング</summary>
		public ProductRankingModel ProductRanking
		{
			get { return (ProductRankingModel)Get("ProductRankingModel"); }
			set { Set("ProductRankingModel", value); }
		}
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessage
		{
			get { return (string)Get("ErrorMessage"); }
			set { Set("ErrorMessage", value); }
		}
		// ★有効期限切れ制御できていない？
		/// <summary>テンポラリデータ</summary>
		public Dictionary<object, KeyValuePair<DateTime, object>> TempData
		{
			get { return (Dictionary<object, KeyValuePair<DateTime, object>>)this.Session["TempData"]; }
			set { this.Session["TempData"] = value; }
		}
		/// <summary>商品グループリストパラメタ</summary>
		public ProductGroupListParamModel ProductGroupListParamModel
		{
			get { return (ProductGroupListParamModel)Get("ProductGroupListParamModel"); }
			set { Set("ProductGroupListParamModel", value); }
		}
		/// <summary>特集ページ</summary>
		public FeaturePageModel FeaturePageModel
		{
			get { return (FeaturePageModel)Get("FeaturePageModel"); }
			set { Set("FeaturePageModel", value); }
		}
		/// <summary>特集ページカテゴリ</summary>
		public FeaturePageCategoryModel FeaturePageCategoryModel
		{
			get { return (FeaturePageCategoryModel)Get("FeaturePageCategoryModel"); }
			set { Set("FeaturePageCategoryModel", value); }
		}
		/// <summary>特集ページカテゴリ一覧パラメータモデル</summary>
		public FeaturePageCategoryListParamModel FeaturePageCategoryListParamModel
		{
			get { return (FeaturePageCategoryListParamModel)Get("FeaturePageCategoryListParamModel"); }
			set { Set("FeaturePageCategoryListParamModel", value); }
		}
		/// <summary>セッション</summary>
		public HttpSessionStateBase Session { get; private set; }
	}
}