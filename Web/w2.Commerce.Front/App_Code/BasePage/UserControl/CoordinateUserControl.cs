/*
=========================================================================================================
  Module      : コーディネート系基底ユーザコントロール(CoordinateUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Web;
using w2.Domain.Coordinate;
using w2.Domain.Coordinate.Helper;

/// <summary>
/// コーディネート系基底ユーザコントロール
/// </summary>
public class CoordinateUserControl : BaseUserControl
{
	/// <summary>
	/// コーディネート一覧情報を取得（キャッシュorDBから）
	/// </summary>
	/// <returns>item1：総件数
	/// item2:コーディネート一覧</returns>
	public Tuple<int, CoordinateListSearchResult[]> GetCoordinateListInfoFromCacheOrDb(string cashName)
	{
		// キャッシュキー作成
		var cacheKey = cashName + string.Join(",", this.RequestParameter.Keys.Select(key => key + "=" + this.RequestParameter[key]).ToArray());

		// キャッシュまたはDBから取得
		var expire = Constants.COORDINATELIST_CACHE_EXPIRE_MINUTES;
		var data = DataCacheManager.GetInstance().GetData(
			cacheKey,
			expire,
			CreateCoordinateListInfoFromDb);
		return data;
	}

	/// <summary>
	/// DBからコーディネート一覧情報作成
	/// </summary>
	/// <returns>item1：総件数
	/// item2:コーディネート一覧</returns>
	public virtual Tuple<int, CoordinateListSearchResult[]> CreateCoordinateListInfoFromDb()
	{
		var searchCondition = new CoordinateListSearchCondition
		{
			Staff = this.StaffId,
			RealShop = this.RealShopId,
			BeginRowNumber = 1,
			EndRowNumber = 1000,
			Category = this.CoordinateCategoryId,
			DisplayKbn = Constants.FLG_COORDINATE_DISPLAY_KBN_PUBLIC,
			HeightUpperLimit = this.HeightUpperLimit,
			HeightLowerLimit = this.HeightLowerLimit,
			DisplayDateKbn = "UNSELECTED",
			ConsiderDisplayDate = "1"
		};

		var list = new CoordinateService().Search(searchCondition).GroupBy(u => u.CoordinateId).Where(u => u.Any())
			.Distinct().Select(u => u.FirstOrDefault()).ToArray();
		var count = new CoordinateService().GetSearchHitCount(searchCondition);
		var info = new Tuple<int, CoordinateListSearchResult[]>(count, list);

		return info;
	}

	/// <summary>
	/// 表示用コーディネートリストを作成
	/// </summary>
	/// <param name="model">モデル</param>
	/// <param name="maxDispCount">表示数</param>
	/// <returns>表示用コーディネートリスト</returns>
	public CoordinateListSearchResult[] CreateCoordinateListForDisplay(CoordinateListSearchResult[] model, int maxDispCount)
	{
		var length = (model.Length > maxDispCount) ? maxDispCount : model.Length;
		var modelDisplays = new CoordinateListSearchResult[length];

		// 表示データ件数分取得
		int dispCounter = 0;
		for (int i = 0; ((i < model.Length) && (dispCounter < maxDispCount)); i++)
		{
			modelDisplays[i] = model[i];
			dispCounter++;
		}

		return modelDisplays;
	}

	/// <summary>
	/// 表示用コーディネートリストを作成
	/// </summary>
	/// <param name="model">モデル</param>
	/// <param name="maxDispCount">表示数</param>
	/// <returns>表示用コーディネートリスト</returns>
	public static CoordinateModel[] CreateCoordinateListForDisplay(CoordinateModel[] model, int maxDispCount)
	{
		var length = (model.Length > maxDispCount) ? maxDispCount : model.Length;
		var modelDisplays = new CoordinateModel[length];

		// 表示データ件数分取得
		int dispCounter = 0;
		for (int i = 0; ((i < model.Length) && (dispCounter < maxDispCount)); i++)
		{
			modelDisplays[i] = model[i];
			dispCounter++;
		}

		return modelDisplays;
	}

	/// <summary>
	/// コーディネート詳細URL作成
	/// </summary>
	/// <param name="product">コーディネートマスタ</param>
	/// <returns>コーディネート詳細URL</returns>
	protected string CreateProductDetailUrl(object product)
	{
		return CreateProductDetailUrl(product, "");
	}

	/// <summary>
	/// コーディネート詳細URL作成
	/// </summary>
	/// <param name="product">コーディネートマスタ</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>コーディネート詳細URL</returns>
	protected string CreateProductDetailUrl(object product, string variationId)
	{
		return ProductCommon.CreateProductDetailUrlUseProductCategoryx(product, variationId, this.BrandId);
	}

	/// <summary>
	/// コーディネートリストURL作成
	/// </summary>
	/// <param name="paramName">パラム名</param>
	/// <param name="value">値</param>
	/// <returns>コーディネートURL</returns>
	public static string CreateCoordinateListUrl(string paramName, string value)
	{
		var urlCreator = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_FRONT_COORDINATE_LIST));
		if (string.IsNullOrEmpty(value) == false) urlCreator.AddParam(paramName, value);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// コーディネートリストURL作成(身長用)
	/// </summary>
	/// <param name="upperLimit">上限</param>
	/// <param name="lowerLimit">下限</param>
	/// <returns>コーディネートURL</returns>
	public static string CreateCoordinateListUrlForHeight(string lowerLimit, string upperLimit)
	{
		var urlCreator = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_FRONT_COORDINATE_LIST));
		if (string.IsNullOrEmpty(upperLimit) == false) urlCreator.AddParam(Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT, upperLimit);
		if (string.IsNullOrEmpty(lowerLimit) == false) urlCreator.AddParam(Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT, lowerLimit);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// スタッフを表示するか
	/// </summary>
	/// <param name="model">コーディネイトモデル</param>
	/// <returns>表示するか</returns>
	protected bool ShouldShowStaff(CoordinateModel model)
	{
		return (String.IsNullOrEmpty(model.StaffName) == false);
	}

	/// <summary>パラメーター</summary>
	public Dictionary<string, object> Parameters
	{
		get { return CoordinatePage.GetParameters(this.Request); }
	}
	/// <summary>リクエストパラメーター</summary>
	public Dictionary<string, string> RequestParameter
	{
		get
		{
			var requestParameter = new Dictionary<string, string>();
			foreach (var requestKey in this.Parameters.Keys)
			{
				requestParameter.Add(requestKey, this.Parameters[requestKey].ToString());
			}
			return requestParameter;
		}
	}
	/// <summary>店舗ID</summary>
	protected string ShopId
	{
		get
		{
			return ((string)this.Parameters[Constants.REQUEST_KEY_SHOP_ID] != null)
				? (string)this.Parameters[Constants.REQUEST_KEY_SHOP_ID]
				: Constants.CONST_DEFAULT_SHOP_ID;
		}
	}
	/// <summary>ページナンバー</summary>
	public int PageNumber
	{
		get { return (int)this.Parameters[Constants.REQUEST_KEY_PAGE_NO]; }
	}
	/// <summary>検索キーワード</summary>
	protected string SearchKeyword
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_KEYWORD]; }
	}
	/// <summary>コーディネートID</summary>
	protected string CoordinateId
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_ID]; }
	}
	/// <summary>スタッフID</summary>
	protected string StaffId
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_STAFF_ID]; }
	}
	/// <summary>コーディネートカテゴリID</summary>
	protected string CoordinateCategoryId
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID]; }
	}
	/// <summary>リアル店舗ID</summary>
	protected string RealShopId
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_REAL_SHOP_ID]; }
	}
	/// <summary>タグID</summary>
	protected string ContentsTagId
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_CONTENTS_TAG_ID]; }
	}
	/// <summary>身長下限</summary>
	protected string HeightLowerLimit
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT]; }
	}
	/// <summary>身長上限</summary>
	protected string HeightUpperLimit
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT]; }
	}
}
