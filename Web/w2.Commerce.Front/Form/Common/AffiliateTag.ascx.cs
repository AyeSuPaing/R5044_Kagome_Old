/*
=========================================================================================================
  Module      : アフィリエイトタグ (AffiliateTag.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.App.Common.Affiliate;
using w2.App.Common.Global.Region;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// アフィリエイトタグ
/// </summary>
public partial class Form_Common_AffiliateTag : BaseUserControl
{
	#region ラップ済みコントロール宣言
	WrappedRepeater WrAffiliateTag
	{
		get { return GetWrappedControl<WrappedRepeater>("rAffiliateTag"); }
	}
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Init(object sender, EventArgs e)
	{
		this.Page.LoadComplete += PageLoadComplete;
	}

	/// <summary>
	/// ページロード完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void PageLoadComplete(object sender, EventArgs e)
	{
		if ((Constants.W2MP_MULTIPURPOSE_AFFILIATE_OPTION_ENABLED == false)
			|| this.IsAlreadyDisplayed
			|| (Constants.TAG_OUTPUT_PRODUCT_ENV_ONLY
				&& (Constants.SETTING_PRODUCTION_ENVIRONMENT == false))) return;

		var device = this.IsPc
			? Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_PC
			: Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_SP;

		var affiliateCooperationSessionDate = new AffiliateCooperationSessionDate
		{
			AdvCodeNow = (string)Session[Constants.SESSION_KEY_ADVCODE_NOW],
			CartList = SessionManager.CartList,
			LoginUser = this.LoginUser,
			RegionModel = RegionManager.GetInstance().Region
		};

		var AffiliateTagList = new AffiliateTagManager().GetAffiliateTag(
			Request.Url.AbsolutePath,
			this.Location,
			device,
			this.AllTag,
			this.Datas,
			affiliateCooperationSessionDate);

		//ロギング
		foreach (var affiliateTag in AffiliateTagList)
		{
			if (string.IsNullOrEmpty(affiliateTag.Content) || (affiliateTag.Logging == false)) continue;

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("AffiliateCoopLog", "InsertAffiliateCoopLog"))
			{
				var ht = new Hashtable();
				ht.Add(
					Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN,
					Constants.FLG_AFFILIATECOOPLOG_AFFILIATE_KBN_PC); // アフィリエイト区分(PC)
				ht.Add(Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID, affiliateTag.LogKey); // マスタID(注文ID)
				ht.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1, affiliateTag.Content); // 成果報告内容
				ht.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2, affiliateTag.AffiliateName);
				ht.Add(
					Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA4,
					this.Location + " : " + Request.Url.AbsolutePath);
				ht.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA5, device);
				ht.Add(
					Constants.FIELD_AFFILIATECOOPLOG_COOP_STATUS,
					Constants.FLG_AFFILIATECOOPLOG_COOP_STATUS_WAIT);
				ht.Add(Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11, affiliateTag.AffiliateId);

				statement.ExecStatementWithOC(accessor, ht);
			}
		}

		// データバインド
		this.WrAffiliateTag.DataSource = AffiliateTagList.Select(t => t.Content).ToList();
		this.WrAffiliateTag.DataBind();
	}

	/// <remarks>外部から設定可能</remarks>
	public object Datas { get; set; }
	/// <remarks>共通出力タグフラグ</remarks>
	public bool AllTag { get; set; }
	/// <remarks>出力箇所</remarks>
	public string Location { get; set; }
	/// <summary>表示フラグ</summary>
	public bool IsAlreadyDisplayed { get; set; }
}