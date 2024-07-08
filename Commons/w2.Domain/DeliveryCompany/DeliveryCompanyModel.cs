/*
=========================================================================================================
  Module      : 配送会社マスタモデル (DeliveryCompanyModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.DeliveryCompany
{
	/// <summary>
	/// 配送会社マスタモデル
	/// </summary>
	[Serializable]
	public partial class DeliveryCompanyModel : ModelBase<DeliveryCompanyModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DeliveryCompanyModel()
		{
			this.ShippingTimeSetFlg = Constants.FLG_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG_INVALID;
			this.DisplayOrder = 1;
			this.DeliveryCompanyMailSizeLimit = 999;
			this.DeliveryLeadTimeSetFlg = Constants.FLG_DELIVERYCOMPANY_LEAD_TIME_SET_FLG_INVALID;
			this.ShippingLeadTimeDefault = Constants.FLG_SHIPPING_LEAD_TIME_DEFAULT;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DeliveryCompanyModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public DeliveryCompanyModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
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
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DISPLAY_ORDER] = value; }
		}
		/// <summary>配送サイズ上限</summary>
		public int DeliveryCompanyMailSizeLimit
		{
			get { return (int)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_SIZE_LIMIT]; }
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
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_DATE_CHANGED]; }
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
		public int ShippingLeadTimeDefault
		{
			get { return (int)this.DataSource[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_LEAD_TIME_DEFAULT]; }
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
}
