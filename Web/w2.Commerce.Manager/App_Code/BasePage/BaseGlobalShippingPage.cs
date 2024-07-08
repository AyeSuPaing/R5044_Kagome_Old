/*
=========================================================================================================
  Module      : 海外配送基底ページ(BaseGlobalShippingPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Web;
using w2.Domain.DeliveryCompany;

/// <summary>
/// 海外配送基底ページ
/// </summary>
public abstract class BaseGlobalShippingPage : BasePage
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BaseGlobalShippingPage()
	{
	}

	/// <summary>
	/// 参照URL生成
	/// </summary>
	/// <param name="id">海外配送エリアID</param>
	/// <returns>URL</returns>
	protected string CreateDetailUrl(string id)
	{
		var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_CONFIRM);
		uc.AddParam(Constants.REQUEST_KEY_GLOBAL_SHIPPING_AREA_ID, id).AddParam(Constants.REQUEST_KEY_ACTION_STATUS,Constants.ACTION_STATUS_DETAIL);
		return uc.CreateUrl();
	}

	/// <summary>
	/// 編集URL生成
	/// </summary>
	/// <param name="id">海外配送エリアID</param>
	/// <returns>URL</returns>
	protected string CreateEditUrl(string id)
	{
		var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_REGISTER);
		uc.AddParam(Constants.REQUEST_KEY_GLOBAL_SHIPPING_AREA_ID, id).AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE);
		return uc.CreateUrl();
	}

	/// <summary>
	/// 登録確認URL生成
	/// </summary>
	/// <returns>URL</returns>
	public string CreateInsertConfirmUrl()
	{
		var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_CONFIRM);
		uc.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT);
		return uc.CreateUrl();
	}

	/// <summary>
	/// 更新確認URL生成
	/// </summary>
	/// <returns>URL</returns>
	public string CreateUpdateConfirmUrl()
	{
		var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_GLOBAL_SHIPPING_AREA_CONFIRM);
		uc.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE);
		return uc.CreateUrl();
	}

	/// <summary>
	/// 海外配送エリア入力データ
	/// </summary>
	protected GlobalShippingAreaInput KeepingAreaInputData
	{
		get { return Session["keeping_area_input"] == null ? null : ((GlobalShippingAreaInput)Session["keeping_area_input"]); }
		set { Session["keeping_area_input"] = value; }
	}
	/// <summary>配送会社リスト</summary>
	protected DeliveryCompanyModel[] DeliveryCompanyList
	{
		get
		{
			if (m_deliveryComapnyList == null)
			{
				var service = new DeliveryCompanyService();
				m_deliveryComapnyList = service.GetAll();
			}
			return m_deliveryComapnyList;
		}
	}
	private DeliveryCompanyModel[] m_deliveryComapnyList;
}