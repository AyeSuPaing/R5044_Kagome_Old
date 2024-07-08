/*
=========================================================================================================
  Module      : 海外配送料編集ページ(GlobalShippingPostageEdit.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.GlobalShipping;
using w2.Domain.ShopShipping;

/// <summary>
/// 海外配送料編集ページ
/// </summary>
public partial class Form_Global_Shipping_Postage_Edit : BaseGlobalShippingPage
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponents();
		}
	}
	#endregion

	#region #btnBackToListTop_Click 戻るクリック
	/// <summary>
	/// 戻るクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackToListTop_Click(object sender, EventArgs e)
	{
		var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_CONFIRM);
		uc.AddParam(Constants.REQUEST_KEY_SHIPPING_ID, this.KeepingShippingId).AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);
		Response.Redirect(uc.CreateUrl());
	}
	#endregion

	#region #btnClearGlobalPostage_Click 料金表クリアボタンクリック
	/// <summary>
	/// 料金表クリアボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnClearGlobalPostage_Click(object sender, EventArgs e)
	{
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				var sv = new GlobalShippingService();
				sv.DeleteGlobalShippingPostageByShippingDeliveryCompany(
				base.LoginOperatorShopId,
				this.KeepingShippingId,
				this.KeepingDeliveryCompanyId,
				accessor);

				// 更新日と最終更新者を更新
				new ShopShippingService().UpdateShopShippingDateChangedAndLastChanged(
					this.LoginOperatorShopId,
					this.KeepingShippingId,
					this.LoginOperatorName,
					accessor);

				accessor.CommitTransaction();
			}
			catch (Exception)
			{
				accessor.RollbackTransaction();
			}
		}

		var logmsg = string.Format(
			"オペレーター：{0}が、配送種別：{1}の配送サービス：{2}の海外配送料金表をクリアしました。",
			base.LoginOperatorName,
			this.KeepingShippingId,
			this.KeepingDeliveryCompanyId);
		FileLogger.WriteInfo(logmsg);

		// 海外配送エリア再度表示
		this.ControlGlobalShipping(this.LoginOperatorShopId, this.KeepingShippingId, this.KeepingDeliveryCompanyId);
	}
	#endregion

	#region #btnCopyGlobalPostage_Click 料金表コピーボタンクリック
	/// <summary>
	/// 料金表コピーボタンクリック
	/// </summary>
	/// <param name="sender">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void btnCopyGlobalPostage_Click(object sender, EventArgs e)
	{
		var fromShipping = ddlShipping.SelectedValue;
		var fromDelivery = ddlDelivery.SelectedValue;
		var toShipping = this.KeepingShippingId;
		var toDelivery = this.KeepingDeliveryCompanyId;

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				var sv = new GlobalShippingService();
				sv.CopyGlobalShippingPostageByShippingDeliveryCompany(
					base.LoginOperatorShopId,
					fromShipping,
					fromDelivery,
					toShipping,
					toDelivery,
					base.LoginOperatorName,
					accessor);

				// 更新日と最終更新者を更新
				new ShopShippingService().UpdateShopShippingDateChangedAndLastChanged(
					this.LoginOperatorShopId,
					this.KeepingShippingId,
					this.LoginOperatorName,
					accessor);

				accessor.CommitTransaction();
			}
			catch (Exception)
			{
				accessor.RollbackTransaction();
			}
		}

		var logmsg = string.Format(
			"オペレーター：{0}が、配送種別：{1}の配送サービス：{2}の海外配送料金表を基に配送種別：{3}の配送サービス：{4}へコピーしました。",
			base.LoginOperatorName,
			fromShipping,
			fromDelivery,
			toShipping,
			toDelivery);
		FileLogger.WriteInfo(logmsg);

		// 海外配送エリア再度表示
		this.ControlGlobalShipping(this.LoginOperatorShopId, this.KeepingShippingId, this.KeepingDeliveryCompanyId);
	}
	#endregion

	#region #repGlobalShippingAreaPostage_ItemCommand 料金表リピーターイベント
	/// <summary>
	/// 料金表リピーターイベント
	/// </summary>
	/// <param name="source">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void repGlobalShippingAreaPostage_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "DelGlobalPostage")
		{
			this.DeletePostage(e);
		}
		else if (e.CommandName == "ChangePostage")
		{
			this.ChangePostage(e);
		}
	}
	#endregion

	#region #repGlobalShippingArea_ItemCommand 海外配送エリアリピーターイベント
	/// <summary>
	/// 海外配送エリアリピーターイベント
	/// </summary>
	/// <param name="source">イベント発生オブジェクト</param>
	/// <param name="e">イベント引数</param>
	protected void repGlobalShippingArea_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "AddAreaPostage")
		{
			this.AddAreaPostage(e);
		}
	}
	#endregion

	/// <summary>
	/// 海外配送料金表制御
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	private void ControlGlobalShipping(string shopId, string shippingId, string deliveryCompanyId)
	{
		// 海外配送エリアの一覧を取得
		var sv = new GlobalShippingService();
		var area = sv.GetValidGlobalShippingArea();
		var areaPostage = area
			.Select(a =>
			{
				var rtn = new GlobalPostageMap(a.DataSource);
				rtn.Postage = sv.GetAreaPostageByShippingDeliveryCompany(
					shopId,
					shippingId,
					a.GlobalShippingAreaId,
					deliveryCompanyId);
				rtn.AreaCount = area.Length;
				return rtn;
			})
			.ToArray();
		this.repGlobalShippingArea.DataSource = areaPostage;
		this.repGlobalShippingArea.DataBind();

		this.rAnchor.DataSource = areaPostage;
		this.rAnchor.DataBind();

	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		var shippingsv = new ShopShippingService();
		var shipping = shippingsv.Get(this.LoginOperatorShopId, this.KeepingShippingId);

		// 該当データ無しの場合、エラーページへ遷移
		if ((shipping == null)
			|| (shipping.CompanyList.FirstOrDefault(c => (c.DeliveryCompanyId == this.KeepingDeliveryCompanyId)) == null))
		{
			RedirectToErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL));
		}

		this.KeepingShippingName = shipping.ShopShippingName;
		this.KeepingDeliveryCompanyName = this.DeliveryCompanyList
			.First(c => (c.DeliveryCompanyId == this.KeepingDeliveryCompanyId)).DeliveryCompanyName;

		// 送料表
		this.ControlGlobalShipping(base.LoginOperatorShopId, this.KeepingShippingId, this.KeepingDeliveryCompanyId);

		// 配送種別の一覧を取得
		var shippings = shippingsv.GetAll(base.LoginOperatorShopId);
		this.ddlShipping.Items.AddRange(shippings.Select(s => new ListItem(s.ShopShippingName, s.ShippingId)).ToArray());
		this.ddlShipping.DataBind();

		// 配送サービスプルダウン設定
		SetDeliveryListItems(shippings.First(s => (s.ShippingId == ddlShipping.Items[0].Value)));

		this.DataBind();
	}

	/// <summary>
	/// 料金削除
	/// </summary>
	/// <param name="e">リピータイベント引数</param>
	private void DeletePostage(RepeaterCommandEventArgs e)
	{
		var seq = e.CommandArgument.ToString();
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				var sv = new GlobalShippingService();
				sv.DeleteGlobalShippingPostage(int.Parse(seq), accessor);

				// 更新日と最終更新者を更新
				new ShopShippingService().UpdateShopShippingDateChangedAndLastChanged(
					this.LoginOperatorShopId,
					this.KeepingShippingId,
					this.LoginOperatorName,
					accessor);

				accessor.CommitTransaction();
			}
			catch (Exception)
			{
				accessor.RollbackTransaction();
			}
		}

		var logmsg = string.Format(
			"オペレーター：{0}が、配送種別：{1}の配送サービス：{2}のシーケンス番号：{3}の料金表を削除しました。",
			base.LoginOperatorName,
			this.KeepingShippingId,
			this.KeepingDeliveryCompanyId,
			seq);
		FileLogger.WriteInfo(logmsg);

		this.ControlGlobalShipping(base.LoginOperatorShopId, this.KeepingShippingId, this.KeepingDeliveryCompanyId);
	}

	/// <summary>
	/// 料金変更
	/// </summary>
	/// <param name="e">リピーターイベント引数</param>
	private void ChangePostage(RepeaterCommandEventArgs e)
	{
		var seq = e.CommandArgument.ToString();
		var sv = new GlobalShippingService();
		var tbChangePostage = e.Item.FindControl("tbChangePostage");
		var lt = (Literal)e.Item.FindControl("ltMsg");
		var postageText = ((TextBox)tbChangePostage).Text;
		decimal postage = 0;

		var input = new Hashtable();
		input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE, postageText);
		var validMsg = Validator.Validate("GlobalShippingPostageModify", input);

		if (string.IsNullOrEmpty(validMsg) == false)
		{
			lt.Text = validMsg;
			return;
		}

		decimal.TryParse(postageText, out postage);

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				sv.ChangeGlobalShippingPostage(int.Parse(seq), postage, base.LoginOperatorName, accessor);

				// 更新日と最終更新者を更新
				new ShopShippingService().UpdateShopShippingDateChangedAndLastChanged(
					this.LoginOperatorShopId,
					this.KeepingShippingId,
					this.LoginOperatorName,
					accessor);

				accessor.CommitTransaction();
			}
			catch (Exception)
			{
				accessor.RollbackTransaction();
			}
		}

		lt.Text = "更新しました";

		var logmsg = string.Format(
			"オペレーター：{0}が、配送種別：{1}の配送サービス：{2}のシーケンス番号：{3}の料金表を変更しました。",
			base.LoginOperatorName,
			this.KeepingShippingId,
			this.KeepingDeliveryCompanyId,
			seq);
		FileLogger.WriteInfo(logmsg);
	}

	/// <summary>
	/// 料金追加
	/// </summary>
	/// <param name="e">リピーターイベント引数</param>
	protected void AddAreaPostage(RepeaterCommandEventArgs e)
	{
		var areaId = e.CommandArgument.ToString();
		var tbGramGt = (TextBox)e.Item.FindControl("tbWeightGramGreaterThanOrEqualTo");
		var tbGramLt = (TextBox)e.Item.FindControl("tbWeightGramLessThan");
		var tbPostage = (TextBox)e.Item.FindControl("tbAddAreaPostage");
		var lt = (Literal)e.Item.FindControl("ltMsg");

		decimal postage = 0;
		int gramGt = 0;
		int gramLt = 0;

		var input = new Hashtable();
		input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE, tbPostage.Text);
		input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_GREATER_THAN_OR_EQUAL_TO, tbGramGt.Text);
		input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_WEIGHT_GRAM_LESS_THAN, tbGramLt.Text);
		var validMsg = Validator.Validate("GlobalShippingPostageRegist", input);

		if (string.IsNullOrEmpty(validMsg) == false)
		{
			lt.Text = validMsg;
			return;
		}

		decimal.TryParse(tbPostage.Text, out postage);
		int.TryParse(tbGramGt.Text, out gramGt);
		int.TryParse(tbGramLt.Text, out gramLt);

		if (gramGt > gramLt)
		{
			lt.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GLOBAL_SHIPPING_POSTAGE_RANGE_ERROR);
			return;
		}

		var model = new GlobalShippingPostageModel();
		model.ShopId = this.LoginOperatorShopId;
		model.ShippingId = this.KeepingShippingId;
		model.DeliveryCompanyId = this.KeepingDeliveryCompanyId;
		model.GlobalShippingAreaId = areaId;
		model.WeightGramGreaterThanOrEqualTo = gramGt;
		model.WeightGramLessThan = gramLt;
		model.GlobalShippingPostage = postage;
		model.LastChanged = this.LoginOperatorName;

		var sv = new GlobalShippingService();
		var isDup = sv.DuplicationWeightRange(model);

		if (isDup)
		{
			lt.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_GLOBAL_SHIPPING_POSTAGE_DUPLICATE_ERROR);
			return;
		}

		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				// 登録
				sv.RegisterGlobalShippingPostage(model, accessor);

				// 更新日と最終更新者を更新
				new ShopShippingService().UpdateShopShippingDateChangedAndLastChanged(
					this.LoginOperatorShopId,
					this.KeepingShippingId,
					this.LoginOperatorName,
					accessor);

				accessor.CommitTransaction();
			}
			catch (Exception)
			{
				accessor.RollbackTransaction();
			}
		}

		var logmsg = string.Format(
			"オペレーター：{0}が、配送種別：{1}の配送サービス：{2}の海外配送エリア：{3}、重量：{4}～{5}、海外配送料金：{6}を追加しました。",
			base.LoginOperatorName,
			this.KeepingShippingId,
			this.KeepingDeliveryCompanyId,
			model.GlobalShippingAreaId,
			model.WeightGramGreaterThanOrEqualTo,
			model.WeightGramLessThan,
			model.GlobalShippingPostage);
		FileLogger.WriteInfo(logmsg);

		// 海外配送エリア再度表示
		this.ControlGlobalShipping(this.LoginOperatorShopId, this.KeepingShippingId, this.KeepingDeliveryCompanyId);

	}

	/// <summary>
	/// 海外用の金額表示制御
	/// </summary>
	/// <param name="val">対象値</param>
	/// <returns>制御した値</returns>
	protected string GlobalPriceDisplayControlToTextbox(decimal val)
	{
		return val.ToPriceString();
	}

	/// <summary>
	/// 配送種別変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShipping_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// 配送サービスの選択肢設定
		SetDeliveryListItems(new ShopShippingService().Get(this.LoginOperatorShopId, ddlShipping.SelectedValue));
	}

	/// <summary>
	/// 配送サービスの選択肢設定
	/// </summary>
	/// <param name="shipping">配送種別情報</param>
	private void SetDeliveryListItems(ShopShippingModel shipping)
	{
		ddlDelivery.Items.Clear();
		var deliveryList = shipping.CompanyList
			.GroupBy(c => c.DeliveryCompanyId)
			.Select(grp => grp.First())
			.ToList();
		ddlDelivery.Items.AddRange(
			this.DeliveryCompanyList
				.Where(c => (deliveryList.Any(item => item.DeliveryCompanyId == c.DeliveryCompanyId)))
				.Select(d => new ListItem(d.DeliveryCompanyName, d.DeliveryCompanyId)).ToArray());
		ddlDelivery.DataBind();
	}

	/// <summary>配送種別ID</summary>
	protected string KeepingShippingId
	{
		get { return Request[Constants.REQUEST_KEY_SHIPPING_ID]; }
	}
	/// <summary>配送会社ID</summary>
	protected string KeepingDeliveryCompanyId
	{
		get { return Request[Constants.REQUEST_KEY_DELIVERY_COMPANY_ID]; }
	}
	/// <summary>配送種別名（ViewState保持）</summary>
	protected string KeepingShippingName
	{
		get { return StringUtility.ToEmpty(ViewState["keeping_shipping_name"]); }
		set { ViewState["keeping_shipping_name"] = value; }
	}
	/// <summary>配送会社名（ViewState保持）</summary>
	protected string KeepingDeliveryCompanyName
	{
		get { return StringUtility.ToEmpty(ViewState["keeping_delivery_company_name"]); }
		set { ViewState["keeping_delivery_company_name"] = value; }
	}

	/// <summary>
	/// 海外配送料金表マップ
	/// </summary>
	[Serializable]
	public class GlobalPostageMap : GlobalShippingAreaModel
	{
		/// <summary>コンストラクタ</summary>
		public GlobalPostageMap() : base() { }
		/// <summary>コンストラクタ</summary>
		/// <param name="source">データソース</param>
		public GlobalPostageMap(DataRowView source) : base(source) { }
		/// <summary>コンストラクタ</summary>
		/// <param name="source">データソース</param>
		public GlobalPostageMap(Hashtable source) : base(source) { }

		/// <summary>料金表モデル</summary>
		public GlobalShippingPostageModel[] Postage { get; set; }
		/// <summary>配送エリア件数</summary>
		public int AreaCount { get; set; }
	}
}

