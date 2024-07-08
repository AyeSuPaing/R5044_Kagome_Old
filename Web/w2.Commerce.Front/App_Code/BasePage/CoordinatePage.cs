/*
=========================================================================================================
  Module      : コーディネート系基底ページ(CoordinatePage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.Coordinate;

/// <summary>
/// コーディネート系基底ページ
/// </summary>
public class CoordinatePage : BasePage
{
	/// <summary>
	/// コーディネート画面系パラメタ取得
	/// </summary>
	/// <param name="hrRequest">コーディネート一覧・詳細のパラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたDictionary</returns>
	public static Dictionary<string, object> GetParameters(HttpRequest hrRequest)
	{
		var param = new Dictionary<string, object>();

		// 店舗ID
		var strShopId = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_SHOP_ID]);
		if (strShopId == "")
		{
			strShopId = Constants.CONST_DEFAULT_SHOP_ID;
		}
		
		param.Add(Constants.REQUEST_KEY_SHOP_ID, strShopId);
		// ページ番号（ページャ動作時のみもちまわる）
		int iPageNumber;
		if (int.TryParse((string)hrRequest[Constants.REQUEST_KEY_PAGE_NO], out iPageNumber) == false)
		{
			iPageNumber = 1;
		}
		param.Add(Constants.REQUEST_KEY_PAGE_NO, iPageNumber);

		// コーディネートID
		if (param.ContainsKey(Constants.REQUEST_KEY_COORDINATE_ID) == false)
		{
			param.Add(Constants.REQUEST_KEY_COORDINATE_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COORDINATE_ID]));
		}

		// コーディネートカテゴリID
		if (param.ContainsKey(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID) == false)
		{
			param.Add(Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID]));
		}

		// タイトル
		if (param.ContainsKey(Constants.REQUEST_KEY_COORDINATE_KEYWORD) == false)
		{
			param.Add(Constants.REQUEST_KEY_COORDINATE_KEYWORD, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COORDINATE_KEYWORD]));
		}

		// コンテンツタグ
		if (param.ContainsKey(Constants.REQUEST_KEY_CONTENTS_TAG_ID) == false)
		{
			param.Add(Constants.REQUEST_KEY_CONTENTS_TAG_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_CONTENTS_TAG_ID]));
		}

		// リアル店舗ID
		if (param.ContainsKey(Constants.REQUEST_KEY_REAL_SHOP_ID) == false)
		{
			param.Add(Constants.REQUEST_KEY_REAL_SHOP_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_REAL_SHOP_ID]));
		}

		// スタッフID
		if (param.ContainsKey(Constants.REQUEST_KEY_COORDINATE_STAFF_ID) == false)
		{
			param.Add(Constants.REQUEST_KEY_COORDINATE_STAFF_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COORDINATE_STAFF_ID]));
		}

		// オペレータID
		if (param.ContainsKey(Constants.REQUEST_KEY_OPERATOR_ID) == false)
		{
			param.Add(Constants.REQUEST_KEY_OPERATOR_ID, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_OPERATOR_ID]));
		}

		// 身長下限
		if (param.ContainsKey(Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT) == false)
		{
			param.Add(Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT]));
		}

		// 身長上限
		if (param.ContainsKey(Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT) == false)
		{
			param.Add(Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT, StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT]));
		}

		return param;
	}

	/// <summary>
	/// バリエーションがあるか
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>バリエーションがあるか</returns>
	public static bool HasVariation(object product)
	{
		return ProductCommon.HasVariation(product);
	}

	/// <summary>
	/// コーディネート詳細URL作成
	/// </summary>
	/// <param name="coordinate">コーディネート</param>
	/// <returns>商品詳細URL</returns>
	public static string CreateCoordinateDetailUrl(object coordinate)
	{
		return CreateCoordinateDetailUrl((CoordinateModel)coordinate);
	}

	/// <summary>
	/// コーディネート詳細URL作成
	/// </summary>
	/// <param name="model">コーディネートモデル</param>
	/// <returns>コーディネート詳細URL</returns>
	public static string CreateCoordinateDetailUrl(CoordinateModel model)
	{
		if (model == null) return string.Empty;
		var urlCreator = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_FRONT_COORDINATE_DETAIL));
		if (string.IsNullOrEmpty(model.CoordinateId) == false)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_COORDINATE_ID, StringUtility.ToEmpty(model.CoordinateId));
		}
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// コーディネートリストURL作成
	/// </summary>
	/// <param name="paramName">パラム名</param>
	/// <param name="value">値</param>
	/// <returns>コーディネートリストURL</returns>
	public static string CreateCoordinateListUrl(string paramName,string value)
	{
		var urlCreator = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_FRONT_COORDINATE_LIST));
		if (string.IsNullOrEmpty(value) == false) urlCreator.AddParam(paramName, value);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// コーディネート画像URL取得
	/// </summary>
	/// <param name="coordinateId">コーディネートID</param>
	/// <param name="order">順番</param>
	/// <param name="operatorId">オペレータID（プレビュー時指定）</param>
	/// <returns>コーディネート画像URL</returns>
	public static string CreateCoordinateImageUrl(string coordinateId, int order, string operatorId = null)
	{
		// 画像URL作成
		var imageUrl = new StringBuilder();
		var targetDir = (string.IsNullOrEmpty(operatorId) == false) ? Constants.PATH_TEMP + "/" + operatorId + "/" + Constants.PATH_TEMP_COORDINATE : Constants.PATH_COORDINATE;
		imageUrl.Append(Constants.PATH_ROOT).Append(targetDir).Append("/").Append(coordinateId).Append("/").Append(Constants.COORDINATEIMAGE_PREFIX + coordinateId + order + ".jpg");
		if (File.Exists(HttpContext.Current.Server.MapPath(imageUrl.ToString())) == false)
		{
			// 画像無しの場合はNOIMAGE画像
			imageUrl = new StringBuilder();
			imageUrl.Append(Constants.PATH_ROOT).Append(Constants.PATH_COORDINATE).Append("/").Append(Constants.PRODUCTIMAGE_NOIMAGE_PC_LL); 
		}

		// ブラウザキャッシュ対策
		imageUrl.Append("?" + DateTime.Now);

		return imageUrl.ToString();
	}

	/// <summary>
	/// コーディネート一覧遷移URL作成
	/// </summary>
	/// <param name="requestParam">リクエストパラメーター</param>
	/// <returns>作成URL</returns>
	public static string CreateCoordinateListUrl(Dictionary<string, string> requestParam)
	{
		var urlCreator = new UrlCreator(Path.Combine(Constants.PATH_ROOT, Constants.PAGE_FRONT_COORDINATE_LIST));

		AddCoordinateListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_COORDINATE_CATEGORY_ID);
		AddCoordinateListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_COORDINATE_STAFF_ID);
		AddCoordinateListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT);
		AddCoordinateListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT);
		AddCoordinateListUrlParam(urlCreator, requestParam, Constants.REQUEST_KEY_REAL_SHOP_ID);

		// ページ番号は一番最後
		if (requestParam.ContainsKey(Constants.REQUEST_KEY_PAGE_NO))
		{
			var pageNo = (int.Parse(requestParam[Constants.REQUEST_KEY_PAGE_NO]) > 0) ? requestParam[Constants.REQUEST_KEY_PAGE_NO] : "1";
			urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNo);
		}

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// コーディネートURLパラメーター追加
	/// </summary>
	/// <param name="urlCreator">URLクエリ</param>
	/// <param name="requestParam">リクエストパラメーター</param>
	/// <param name="paramName">パラメーター名</param>
	private static void AddCoordinateListUrlParam(
		UrlCreator urlCreator,
		Dictionary<string, string> requestParam,
		string paramName)
	{
		if (string.IsNullOrEmpty(requestParam[paramName]) == false)
		{
			urlCreator.AddParam(paramName, requestParam[paramName]);
		}
	}

	/// <summary>
	/// スタッフ画像パスを取得
	/// </summary>
	/// <param name="staffId">スタッフID</param>
	/// <returns>スタッフ画像パス</returns>
	public static string GetStaffImagePath(string staffId)
	{
		var staffPath = Path.Combine(
			Constants.PATH_ROOT_FRONT_PC,
			Constants.PATH_STAFF,
			staffId + ".jpg");
		var path = (File.Exists(HttpContext.Current.Server.MapPath(staffPath)))
			? staffPath
			: Path.Combine(
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PATH_PRODUCTIMAGES
				+ Constants.PRODUCTIMAGE_NOIMAGE_HEADER
				+ Constants.PRODUCTIMAGE_FOOTER_S);
		return path;
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

	/// <summary>
	/// 店舗を表示するか
	/// </summary>
	/// <param name="model">コーディネイトモデル</param>
	/// <returns>表示するか</returns>
	protected bool ShouldShowRealShop(CoordinateModel model)
	{
		return (String.IsNullOrEmpty(model.RealShopName) == false);
	}

	/// <summary>パラメーター</summary>
	public Dictionary<string, object> Parameters
	{
		get{return GetParameters(this.Request);}
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
		get { return (string)this.Parameters[Constants.REQUEST_KEY_CONTENTS_TAG_ID];}
	}
	/// <summary>身長下限</summary>
	protected string HeightLowerLimit
	{
		get
		{
			int heightLowerLimit;
			int.TryParse((string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_LOWER_LIMIT], out heightLowerLimit);
			var result = (heightLowerLimit == 0) ? null : heightLowerLimit.ToString();
			return result;
		}
	}
	/// <summary>身長上限</summary>
	protected string HeightUpperLimit
	{
		get
		{
			int heightUpperLimit;
			int.TryParse((string)this.Parameters[Constants.REQUEST_KEY_COORDINATE_UPPER_LIMIT], out heightUpperLimit);
			var result = (heightUpperLimit == 0) ? null : heightUpperLimit.ToString();
			return result;
		}
	}
	/// <summary>オペレータID</summary>
	protected string OperatorId
	{
		get { return (string)this.Parameters[Constants.REQUEST_KEY_OPERATOR_ID]; }
	}
}
