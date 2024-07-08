/*
=========================================================================================================
  Module      : 特別配送先情報確認ページ処理(ShippingZoneConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Common.Extensions;
using w2.Common.Web;
using w2.Domain.ShopShipping;

public partial class Form_ShippingZone_ShippingZoneConfirm : ShopShippingPage
{
	protected Hashtable m_htParam = new Hashtable();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト取得＆ビューステート格納
			//------------------------------------------------------
			string strActionStatus = Request[Constants.REQUEST_KEY_ACTION_STATUS];
			ViewState.Add(Constants.REQUEST_KEY_ACTION_STATUS, strActionStatus);

			//------------------------------------------------------
			// 画面制御
			//------------------------------------------------------
			InitializeComponents( strActionStatus );

			//------------------------------------------------------
			// 画面設定処理
			//------------------------------------------------------
			// 登録・コピー登録・更新画面確認？
			if ((strActionStatus == Constants.ACTION_STATUS_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
				|| (strActionStatus == Constants.ACTION_STATUS_UPDATE))
			{
				//------------------------------------------------------
				// 処理区分チェック
				//------------------------------------------------------
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				m_htParam = this.ShippingZoneInSession;
			}
				// 詳細表示？
			else if (strActionStatus == Constants.ACTION_STATUS_DETAIL)
			{
				// 配送料設定ID,配送料地帯区分取得
				string strShippingId = Request[Constants.REQUEST_KEY_SHIPPING_ID];
				string strShippingZoneNo = Request[Constants.REQUEST_KEY_SHIPPING_ZONE_NO];

				DataRow dr = null;
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatements = new SqlStatement("ShopShippingZone", "GetShippingZone"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID, this.LoginOperatorShopId);
					htInput.Add(Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ID, strShippingId);
					htInput.Add(Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO, strShippingZoneNo);

					DataSet ds = sqlStatements.SelectStatementWithOC( sqlAccessor, htInput);

					// 該当データが有りの場合
					if (ds.Tables["Table"].DefaultView.Count != 0)
					{
						dr = ds.Tables["Table"].Rows[0];
						m_htParam.Add(
							DELIVERY_ZONE_PRICES,
							ds.Tables["Table"].DefaultView.Cast<DataRowView>()
								.Select(row => new ShopShippingZoneModel(row)).ToList());
					}
						// 該当データ無しの場合
					else
					{
						// エラーページへ
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);

					}
				}

				// Hashtabe格納
				foreach(DataColumn dc in dr.Table.Columns)
				{
					m_htParam.Add(dc.ColumnName, dr[dc.ColumnName]);
				}
			}
				// それ以外の場合
			else
			{
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// VIewStateにも格納しておく
			this.ShippingZoneInViewState = m_htParam;

			// 配送不可エリアが設定されている配送会社名を取得してカンマで結合
			var unavailableShippingCompanyNames =
				((List<ShopShippingZoneModel>)m_htParam[DELIVERY_ZONE_PRICES])
					.Where(item => item.UnavailableShippingAreaFlg == Constants.FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_VALID)
					.Select(item => GetDeliveryCompanyName(item.DeliveryCompanyId))
					.JoinToStringWithSeparator(",");
			lNotDelivaryCompanyNames.Text = HtmlSanitizer.HtmlEncode(unavailableShippingCompanyNames);

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents(string strActionStatus)
	{
		// 新規・コピー新規？
		if (strActionStatus == Constants.ACTION_STATUS_INSERT ||
			strActionStatus == Constants.ACTION_STATUS_COPY_INSERT)
		{
			btnInsertTop.Visible = true;
			btnInsertBottom.Visible = true;
			trDetail.Visible = false;
			trConfirm.Visible = true;
		}
			// 更新？
		else if(strActionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			btnUpdateTop.Visible = true;
			btnUpdateBottom.Visible = true;
			trDetail.Visible = false;
			trConfirm.Visible = true;
		}
			// 詳細
		else if(strActionStatus == Constants.ACTION_STATUS_DETAIL)
		{
			btnEditTop.Visible = true;
			btnEditBottom.Visible = true;
			btnCopyInsertTop.Visible = true;
			btnCopyInsertBottom.Visible = true;
			btnDeleteTop.Visible = true;
			btnDeleteBottom.Visible = true;
			trDateChanged.Visible = true;
			trLastChanged.Visible = true;
			trDetail.Visible = true;
		}
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 配送料地帯情報をそのままセッションへセット
		this.ShippingZoneInSession = this.ShippingZoneInViewState;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 配送料地帯情報をそのままセッションへセット
		this.ShippingZoneInSession = this.ShippingZoneInViewState;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_REGISTER + "?" +
			Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 配送料地帯情報取得
		var inputData = this.ShippingZoneInViewState;

		// 削除
		var service = new ShopShippingService();
		service.DeleteShopShippingZoneByShippingIdAndZone(
			(string)inputData[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID],
			(string)inputData[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID],
			inputData[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO].ToString());

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.ShopShipping).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_LIST );
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

		// 配送料地帯情報取得
			var inputData = this.ShippingZoneInViewState;

			// 情報登録
			var service = new ShopShippingService();
			var deliveryZones = (List<ShopShippingZoneModel>)inputData[DELIVERY_ZONE_PRICES];
			deliveryZones.ForEach(model => service.InsertShopShippingZone(model, accessor));

			// トランザクション確定
			accessor.CommitTransaction();
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.ShopShipping).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect( Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_LIST );
	}


	/// <summary>
	/// 更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			var inputData = this.ShippingZoneInViewState;

			// 一旦元の情報削除
			var service = new ShopShippingService();
			service.DeleteShopShippingZoneByShippingIdAndZone(
				(string)inputData[Constants.FIELD_SHOPSHIPPINGZONE_SHOP_ID],
				(string)inputData[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID + "_old"],
				(string)inputData[Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO + "_old"],
				accessor);

			// 変更情報を登録
			var deliveryZones = (List<ShopShippingZoneModel>)inputData[DELIVERY_ZONE_PRICES];
			deliveryZones.ForEach(model => service.InsertShopShippingZone(model, accessor));

			// トランザクション確定
			accessor.CommitTransaction();
		}

		// フロント系サイトを最新情報へ更新
		RefreshFileManagerProvider.GetInstance(RefreshFileType.ShopShipping).CreateUpdateRefreshFile();

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_ZONE_LIST);
	}
}
