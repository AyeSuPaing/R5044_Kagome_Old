/*
=========================================================================================================
  Module      : 配送先情報一覧ページ(ShippingZoneList.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using w2.Domain.ShopShipping;

public partial class Form_ShippingZone_ShippingZoneList : ShopShippingPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 配送料地帯情報一覧画面表示
			ViewShippingZoneList();
		}
	}

	/// <summary>
	/// 配送料地帯情報一覧表示(DataGridにDataView(配送料地帯情報)を設定)
	/// </summary>
	private void ViewShippingZoneList()
	{
		// 変数宣言
		int iCurrentPageNumber = 1;

		// ページ番号（ページャ動作時のみもちまわる）
		try
		{
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) == "")
			{
			}
			else
			{
				iCurrentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}
		}
		catch
		{
			// 不正ページを指定された場合
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 画面制御
		InitializeComponents();

		// 検索コントロール制御（配送料地帯一覧共通処理）
		SetSearchInfo();

		// 検索情報取得
		var searchKey = StringUtility.ToEmpty(ddlSearchKey.SelectedValue);

		// 配送料地帯一覧
		int totalShippingZoneCounts;	// ページング可能総商品数
		// 配送料地帯データ取得
		var zone = GetShippingZoneListDataView(iCurrentPageNumber, searchKey);
		if (zone.Count != 0)
		{
			totalShippingZoneCounts = int.Parse(zone[0].Row["row_count"].ToString());
			// エラー非表示制御
			trListError.Visible = false;
		}
		else
		{
			totalShippingZoneCounts = 0;
			// エラー表示制御
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);

		}
		// データソースセット
		rList.DataSource = CreateDisplayData(zone);

		// ページャ作成（一覧処理で総件数を取得）
		var strNextUrl = CreateShippingZoneListUrl(searchKey);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalShippingZoneCounts, iCurrentPageNumber, strNextUrl);

		// カレントページをビューステート格納
		ViewState.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber.ToString());

		// データバインド
		DataBind();
	}

	/// <summary>
	/// コンポーネントの初期化
	/// </summary>
	private void InitializeComponents()
	{
		DataView dvShopShippings = GetShopShippingsAll(this.LoginOperatorShopId);
		if (dvShopShippings.Count > 0)
		{
			// 配送料取得をドロップダウンリストに設定
			for (int iCount = 0; iCount < dvShopShippings.Count; iCount++)
			{
				ListItem liTemp = new ListItem();
				liTemp.Value = StringUtility.ToEmpty(
					dvShopShippings[iCount][Constants.FIELD_SHOPSHIPPING_SHIPPING_ID].ToString());	// 配送料設定ID
				liTemp.Text = StringUtility.ToEmpty(
					dvShopShippings[iCount][Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME].ToString());			// 配送種別

				ddlSearchKey.Items.Add(liTemp);
			}
		}
		else
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHIPPINGZONE_SHOP_SHIPPING_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}


	/// <summary>
	/// 配送料地帯一覧データビューを表示分だけ取得
	/// </summary>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <param name="strShippingId">配送料設定ID</param>
	/// <returns>配送料地帯一覧データビュー</returns>
	private DataView GetShippingZoneListDataView(int iPageNumber, string strShippingId)
	{
		// 変数宣言
		DataView dvResult = null;
		string strSearchKey = String.Empty;

		//------------------------------------------------------
		// 初期化
		// strSearchKey: [0]配送料設定ID検索、[99]条件無し
		//------------------------------------------------------
		strSearchKey = "99";
		// 配送料設定ID && 地帯名が存在する場合
		if (strShippingId != "")
		{
			strSearchKey = "0";
		}

		int iBgn = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNumber - 1) + 1;
		int iEnd = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNumber;

		// ステートメントからカテゴリデータ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ShopShippingZone", "GetShippingZoneList"))
		{
			var htInput = new Hashtable
			{
				{ "srch_key", strSearchKey },
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID, strShippingId },
				{ Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID, this.LoginOperatorShopId },
				{ "bgn_row_num", iBgn },
				{ "end_row_num", iEnd },
				{ "min_shipping_zone_no", this.PrefecturesList.Length },
			};

			// SQL発行
			DataSet ds = sqlStatement.SelectStatementWithOC(sqlAccessor, htInput);
			dvResult = ds.Tables["Table"].DefaultView;
		}

		return dvResult;
	}

	/// <summary>
	/// 検索コントロール制御
	/// </summary>
	/// <remarks>
	/// Request内容を検索コントロールに設定
	/// </remarks>
	private void SetSearchInfo()
	{
		try
		{
			ddlSearchKey.SelectedValue = (string)Request[Constants.REQUEST_KEY_SEARCH_KEY];
		}
		catch
		{
			// 不正カテゴリIDを指定された場合
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

	}

	/// <summary>
	/// 配送料地帯一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <returns>配送料地帯一覧遷移URL</returns>
	private string CreateShippingZoneListUrl(string strSearchKey)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_LIST;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SEARCH_KEY + "=" + strSearchKey;

		return strResult;
	}


	/// <summary>
	/// 配送料地帯一覧遷移URL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>配送料地帯一覧遷移URL</returns>
	private string CreateShippingZoneListUrl(string strSearchKey, int iPageNumber)
	{
		string strResult = CreateShippingZoneListUrl(strSearchKey);
		strResult += "&";
		strResult += Constants.REQUEST_KEY_PAGE_NO + "=" + iPageNumber.ToString();

		return strResult;
	}


	/// <summary>
	/// データバインド用配送料地帯詳細URL作成
	/// </summary>
	/// <param name="strShippingId">配送料設定ID</param>
	/// <param name="strShippingZoneNo">配送料地帯区分</param>
	/// <returns>配送料地帯詳細URL作成</returns>
	protected string CreateShippingZoneDetailUrl(string strShippingId, string strShippingZoneNo)
	{
		string strResult = "";
		strResult += Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_CONFIRM;
		strResult += "?";
		strResult += Constants.REQUEST_KEY_SHIPPING_ID + "=" + strShippingId;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_SHIPPING_ZONE_NO + "=" + strShippingZoneNo;
		strResult += "&";
		strResult += Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
		return strResult;
	}


	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// 商品一覧へ
		Response.Redirect(
			CreateShippingZoneListUrl(ddlSearchKey.SelectedValue, 1));
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	/// <summary>
	/// 表示用データ作成
	/// </summary>
	/// <param name="source">データ元</param>
	/// <returns>表示用データ</returns>
	protected List<ShippingZonePrices> CreateDisplayData(DataView source)
	{
		var result = new List<ShippingZonePrices>();
		if (source.Count == 0) return result;

		// 「店舗ID、配送種別ID、地帯区分、配送会社ID」の並び順をセット
		source.Sort = string.Format(
			"{0},{1},{2},{3}",
			Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID,
			Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID,
			Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO,
			Constants.FIELD_SHOPSHIPPINGZONE_DELIVERY_COMPANY_ID);

		var rowCount = 0;
		var beforeKey = string.Empty;
		ShippingZonePrices shippingZone = null;
		foreach (DataRowView row in source)
		{
			rowCount++;
			var shopId = (string)row[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID];
			var shippingId = (string)row[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID];
			var shippingZoneNo = (int)row[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO];

			// 前のレコードの「店舗ID+配送種別ID+地帯区分」と違ったら、配送地帯区分インスタンス作成
			if (beforeKey != shopId + shippingId + shippingZoneNo)
			{
				// 最初のレコードでない場合、結果リストに前のエレメントを追加
				if (rowCount > 1) result.Add(shippingZone);

				// 比較用のキーを変更
				beforeKey = shopId + shippingId + shippingZoneNo;

				// 配送種別地帯区分インスタンス作成
				shippingZone = new ShippingZonePrices
				{
					ShopId = shopId,
					ShippingId = shippingId,
					ShopShippingName = (string)row[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME],
					ShippingZoneNo = shippingZoneNo,
					ShippingZoneName = (string)row[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NAME],
					Zip = (string)row[Constants.FIELD_SHOPSHIPPINGZONE_ZIP],
					DeliveryZonePriceList = new List<ShopShippingZoneModel> { new ShopShippingZoneModel(row) },
				};
			}
			else
			{
				// 配送サービスによるサイズ重量区分の配送料を追加
				if (shippingZone == null) continue;
				shippingZone.DeliveryZonePriceList.Add(new ShopShippingZoneModel(row));
			}
		}

		// 最後のエレメントを結果リストに追加
		result.Add(shippingZone);

		return result;
	}
}

