/*
=========================================================================================================
  Module      : 配送会社入力クラス (DeliveryCompanyInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.DeliveryCompany;

/// <summary>
/// 配送会社マスタ入力クラス
/// </summary>
[Serializable]
public class DeliveryCompanyInput : InputBase<DeliveryCompanyModel>
{
	#region constant
	/// <summary>
	/// 列挙体：入力チェック
	/// </summary>
	public enum EnumDeliveryCompanyInputValidationKbn
	{
		/// <summary>新規登録</summary>
		DeliveryCompanyRegist = 0,
		/// <summary>更新</summary>
		DeliveryCompanyModify = 1,
	}

	private const string CHECK_DUPLICATION = "CHECK_DUPLICATION";	// 重複チェック
	#endregion

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public DeliveryCompanyInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public DeliveryCompanyInput(DeliveryCompanyModel model)
		: this()
	{
		this.DeliveryCompanyId = model.DeliveryCompanyId;
		this.DeliveryCompanyName = model.DeliveryCompanyName;
		this.DeliveryCompanyTypeCreditcard = model.DeliveryCompanyTypeCreditcard;
		this.DeliveryCompanyMailSizeLimit = model.DeliveryCompanyMailSizeLimit.ToString();
		this.ShippingTimeSetFlg = model.ShippingTimeSetFlg;
		this.ShippingTimeId1 = model.ShippingTimeId1;
		this.ShippingTimeMessage1 = model.ShippingTimeMessage1;
		this.ShippingTimeId2 = model.ShippingTimeId2;
		this.ShippingTimeMessage2 = model.ShippingTimeMessage2;
		this.ShippingTimeId3 = model.ShippingTimeId3;
		this.ShippingTimeMessage3 = model.ShippingTimeMessage3;
		this.ShippingTimeId4 = model.ShippingTimeId4;
		this.ShippingTimeMessage4 = model.ShippingTimeMessage4;
		this.ShippingTimeId5 = model.ShippingTimeId5;
		this.ShippingTimeMessage5 = model.ShippingTimeMessage5;
		this.ShippingTimeId6 = model.ShippingTimeId6;
		this.ShippingTimeMessage6 = model.ShippingTimeMessage6;
		this.ShippingTimeId7 = model.ShippingTimeId7;
		this.ShippingTimeMessage7 = model.ShippingTimeMessage7;
		this.ShippingTimeId8 = model.ShippingTimeId8;
		this.ShippingTimeMessage8 = model.ShippingTimeMessage8;
		this.ShippingTimeId9 = model.ShippingTimeId9;
		this.ShippingTimeMessage9 = model.ShippingTimeMessage9;
		this.ShippingTimeId10 = model.ShippingTimeId10;
		this.ShippingTimeMessage10 = model.ShippingTimeMessage10;
		this.DisplayOrder = model.DisplayOrder.ToString();
		this.DateCreated = model.DateCreated.ToString();
		this.DateChanged = model.DateChanged.ToString();
		this.LastChanged = model.LastChanged;
		this.DeliveryLeadTimeSetFlg = model.DeliveryLeadTimeSetFlg;
		this.ShippingLeadTimeDefault = StringUtility.ToEmpty(model.ShippingLeadTimeDefault);
		this.DeliveryCompanyTypePostPayment = model.DeliveryCompanyTypePostPayment;
		this.DeliveryCompanyTypePostNpPayment = model.DeliveryCompanyTypePostNpPayment;
		this.DeliveryCompanyTypeGooddeal = model.DeliveryCompanyTypeGooddeal;
		this.DeliveryCompanyDeadlineTime = model.DeliveryCompanyDeadlineTime;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override DeliveryCompanyModel CreateModel()
	{
		var model = new DeliveryCompanyModel
		{
			DeliveryCompanyId = this.DeliveryCompanyId,
			DeliveryCompanyName = this.DeliveryCompanyName,
			DeliveryCompanyTypeCreditcard = this.DeliveryCompanyTypeCreditcard,
			ShippingTimeSetFlg = this.ShippingTimeSetFlg,
			DisplayOrder = int.Parse(this.DisplayOrder),
			DeliveryCompanyMailSizeLimit = int.Parse(this.DeliveryCompanyMailSizeLimit),
			ShippingTimeId1 = this.ShippingTimeId1,
			ShippingTimeMessage1 = this.ShippingTimeMessage1,
			ShippingTimeMatching1 = this.ShippingTimeMatching1,
			ShippingTimeId2 = this.ShippingTimeId2,
			ShippingTimeMessage2 = this.ShippingTimeMessage2,
			ShippingTimeMatching2 = this.ShippingTimeMatching2,
			ShippingTimeId3 = this.ShippingTimeId3,
			ShippingTimeMessage3 = this.ShippingTimeMessage3,
			ShippingTimeMatching3 = this.ShippingTimeMatching3,
			ShippingTimeId4 = this.ShippingTimeId4,
			ShippingTimeMessage4 = this.ShippingTimeMessage4,
			ShippingTimeMatching4 = this.ShippingTimeMatching4,
			ShippingTimeId5 = this.ShippingTimeId5,
			ShippingTimeMessage5 = this.ShippingTimeMessage5,
			ShippingTimeMatching5 = this.ShippingTimeMatching5,
			ShippingTimeId6 = this.ShippingTimeId6,
			ShippingTimeMessage6 = this.ShippingTimeMessage6,
			ShippingTimeMatching6 = this.ShippingTimeMatching6,
			ShippingTimeId7 = this.ShippingTimeId7,
			ShippingTimeMessage7 = this.ShippingTimeMessage7,
			ShippingTimeMatching7 = this.ShippingTimeMatching7,
			ShippingTimeId8 = this.ShippingTimeId8,
			ShippingTimeMessage8 = this.ShippingTimeMessage8,
			ShippingTimeMatching8 = this.ShippingTimeMatching8,
			ShippingTimeId9 = this.ShippingTimeId9,
			ShippingTimeMessage9 = this.ShippingTimeMessage9,
			ShippingTimeMatching9 = this.ShippingTimeMatching9,
			ShippingTimeId10 = this.ShippingTimeId10,
			ShippingTimeMessage10 = this.ShippingTimeMessage10,
			ShippingTimeMatching10 = this.ShippingTimeMatching10,
			LastChanged = this.LastChanged,
			DeliveryLeadTimeSetFlg = this.DeliveryLeadTimeSetFlg,
			ShippingLeadTimeDefault = Int32.Parse(this.ShippingLeadTimeDefault),
			DeliveryCompanyTypePostPayment = this.DeliveryCompanyTypePostPayment,
			DeliveryCompanyTypePostNpPayment = this.DeliveryCompanyTypePostNpPayment,
			DeliveryCompanyTypeGooddeal = this.DeliveryCompanyTypeGooddeal,
			DeliveryCompanyDeadlineTime = this.DeliveryCompanyDeadlineTime ?? string.Empty,
			DeliveryCompanyTypeGmoAtokaraPayment = this.DeliveryCompanyTypeGmoAtokaraPayment,
		};
		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="validateKbn">ユーザー入力チェック列挙体</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(EnumDeliveryCompanyInputValidationKbn validateKbn)
	{
		var errorMessage = string.Empty;

		switch (validateKbn)
		{
			case EnumDeliveryCompanyInputValidationKbn.DeliveryCompanyRegist:
				errorMessage = Validator.Validate("DeliveryCompanyRegist", this.DataSource);
				break;

			case EnumDeliveryCompanyInputValidationKbn.DeliveryCompanyModify:
				errorMessage = Validator.Validate("DeliveryCompanyModify", this.DataSource);
				break;
		}

		// 配送時間帯チェック
		errorMessage += CheckRequiredShippingTimeIdAndShippingTimeMessage();

		return errorMessage;
	}

	/// <summary>
	/// 配送時間帯チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckRequiredShippingTimeIdAndShippingTimeMessage()
	{
		if (((this.ShippingTimeId1 == string.Empty) ^ (this.ShippingTimeMessage1 == string.Empty))
			|| ((this.ShippingTimeId2 == string.Empty) ^ (this.ShippingTimeMessage2 == string.Empty))
			|| ((this.ShippingTimeId3 == string.Empty) ^ (this.ShippingTimeMessage3 == string.Empty))
			|| ((this.ShippingTimeId4 == string.Empty) ^ (this.ShippingTimeMessage4 == string.Empty))
			|| ((this.ShippingTimeId5 == string.Empty) ^ (this.ShippingTimeMessage5 == string.Empty))
			|| ((this.ShippingTimeId6 == string.Empty) ^ (this.ShippingTimeMessage6 == string.Empty))
			|| ((this.ShippingTimeId7 == string.Empty) ^ (this.ShippingTimeMessage7 == string.Empty))
			|| ((this.ShippingTimeId8 == string.Empty) ^ (this.ShippingTimeMessage8 == string.Empty))
			|| ((this.ShippingTimeId9 == string.Empty) ^ (this.ShippingTimeMessage9 == string.Empty))
			|| ((this.ShippingTimeId10 == string.Empty) ^ (this.ShippingTimeMessage10 == string.Empty)))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_DELIVERY_COMPANY_TIME_INPUT_ERROR);
		}

		return string.Empty;
	}
	#endregion

	#region プロパティ
	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID] = value; }
	}
	/// <summary>配送会社名</summary>
	public string DeliveryCompanyName
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME] = value; }
	}
	/// <summary>出荷連携配送会社(クレジットカード)</summary>
	public string DeliveryCompanyTypeCreditcard
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_CREDITCARD]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_CREDITCARD] = value; }
	}
	/// <summary>出荷連携配送会社(後払い)</summary>
	public string DeliveryCompanyTypePostPayment
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_PAYMENT]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_PAYMENT] = value; }
	}
	/// <summary>出荷連携配送会社(Gooddeal)</summary>
	public string DeliveryCompanyTypeGooddeal
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GOODDEAL]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GOODDEAL] = value; }
	}
	/// <summary>配送サイズ上限</summary>
	public string DeliveryCompanyMailSizeLimit
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_SIZE_LIMIT]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_SIZE_LIMIT] = value; }
	}
	/// <summary>配送希望時間帯設定可能フラグ</summary>
	public string ShippingTimeSetFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG] = value; }
	}
	/// <summary>配送希望時間帯ID1</summary>
	public string ShippingTimeId1
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID1]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID1] = value; }
	}
	/// <summary>配送希望時間帯文言1</summary>
	public string ShippingTimeMessage1
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE1]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE1] = value; }
	}
	/// <summary>配送希望時間帯ID2</summary>
	public string ShippingTimeId2
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID2]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID2] = value; }
	}
	/// <summary>配送希望時間帯文言2</summary>
	public string ShippingTimeMessage2
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE2]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE2] = value; }
	}
	/// <summary>配送希望時間帯ID3</summary>
	public string ShippingTimeId3
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID3]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID3] = value; }
	}
	/// <summary>配送希望時間帯文言3</summary>
	public string ShippingTimeMessage3
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE3]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE3] = value; }
	}
	/// <summary>配送希望時間帯ID4</summary>
	public string ShippingTimeId4
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID4]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID4] = value; }
	}
	/// <summary>配送希望時間帯文言4</summary>
	public string ShippingTimeMessage4
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE4]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE4] = value; }
	}
	/// <summary>配送希望時間帯ID5</summary>
	public string ShippingTimeId5
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID5]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID5] = value; }
	}
	/// <summary>配送希望時間帯文言5</summary>
	public string ShippingTimeMessage5
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE5]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE5] = value; }
	}
	/// <summary>配送希望時間帯ID6</summary>
	public string ShippingTimeId6
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID6]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID6] = value; }
	}
	/// <summary>配送希望時間帯文言6</summary>
	public string ShippingTimeMessage6
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE6]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE6] = value; }
	}
	/// <summary>配送希望時間帯ID7</summary>
	public string ShippingTimeId7
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID7]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID7] = value; }
	}
	/// <summary>配送希望時間帯文言7</summary>
	public string ShippingTimeMessage7
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE7]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE7] = value; }
	}
	/// <summary>配送希望時間帯ID8</summary>
	public string ShippingTimeId8
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID8]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID8] = value; }
	}
	/// <summary>配送希望時間帯文言8</summary>
	public string ShippingTimeMessage8
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE8]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE8] = value; }
	}
	/// <summary>配送希望時間帯ID9</summary>
	public string ShippingTimeId9
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID9]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID9] = value; }
	}
	/// <summary>配送希望時間帯文言9</summary>
	public string ShippingTimeMessage9
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE9]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE9] = value; }
	}
	/// <summary>配送希望時間帯ID10</summary>
	public string ShippingTimeId10
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID10]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_ID10] = value; }
	}
	/// <summary>配送希望時間帯文言10</summary>
	public string ShippingTimeMessage10
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE10]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MESSAGE10] = value; }
	}
	/// <summary>表示順</summary>
	public string DisplayOrder
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DISPLAY_ORDER]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DISPLAY_ORDER] = value; }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_LAST_CHANGED] = value; }
	}
	/// <summary>リードタイム設定可能フラグ</summary>
	public string DeliveryLeadTimeSetFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG] = value; }
	}
	/// <summary>基本配送リードタイム</summary>
	public string ShippingLeadTimeDefault
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_LEAD_TIME_DEFAULT]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_LEAD_TIME_DEFAULT] = value; }
	}
	/// <summary> 配送希望時間帯マッチング1 </summary>
	public string ShippingTimeMatching1
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING1]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING1] = value; }
	}
	/// <summary> 配送希望時間帯マッチング2 </summary>
	public string ShippingTimeMatching2
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING2]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING2] = value; }
	}
	/// <summary> 配送希望時間帯マッチング3 </summary>
	public string ShippingTimeMatching3
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING3]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING3] = value; }
	}
	/// <summary> 配送希望時間帯マッチング4 </summary>
	public string ShippingTimeMatching4
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING4]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING4] = value; }
	}
	/// <summary> 配送希望時間帯マッチング5 </summary>
	public string ShippingTimeMatching5
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING5]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING5] = value; }
	}
	/// <summary> 配送希望時間帯マッチング6 </summary>
	public string ShippingTimeMatching6
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING6]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING6] = value; }
	}
	/// <summary> 配送希望時間帯マッチング7 </summary>
	public string ShippingTimeMatching7
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING7]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING7] = value; }
	}
	/// <summary> 配送希望時間帯マッチング8 </summary>
	public string ShippingTimeMatching8
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING8]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING8] = value; }
	}
	/// <summary> 配送希望時間帯マッチング9 </summary>
	public string ShippingTimeMatching9
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING9]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING9] = value; }
	}
	/// <summary> 配送希望時間帯マッチング10 </summary>
	public string ShippingTimeMatching10
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING10]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_MATCHING10] = value; }
	}
	/// <summary>出荷連携配送会社(NP後払い)</summary>
	public string DeliveryCompanyTypePostNpPayment
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_NP_PAYMENT]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_POST_NP_PAYMENT] = value; }
	}
	/// <summary>当日出荷締め時間</summary>
	public string DeliveryCompanyDeadlineTime
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_DEADLINE_TIME]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_DEADLINE_TIME] = value; }
	}
	/// <summary>出荷連携配送会社(GMOアトカラ)</summary>
	public string DeliveryCompanyTypeGmoAtokaraPayment
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GMO_ATOKARA_PAYMENT]; }
		set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_TYPE_GMO_ATOKARA_PAYMENT] = value; }
	}
	#endregion
}
