/*
=========================================================================================================
  Module      : ターゲットリスト条件クラス(TargetListCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml;
using w2.Domain.User.Helper;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリスト条件クラス
	/// </summary>
	[Serializable]
	public class TargetListCondition : ITargetListCondition
	{
		string m_strDataKbn = null;
		string m_strDataKbnString = null;
		string m_strDataField = null;
		string m_strDataFieldString = null;
		string m_strDataType = null;
		List<Data> m_lValues = new List<Data>();
		string m_strEqualSign = null;
		string m_strEqualSignString = null;
		string m_orderExist = null;
		string m_orderExistString = null;
		string m_fixedPurchaseOrderExist = null;
		string m_fixedPurchaseOrderExistString = null;
        string m_pointExist = null;
		string m_pointExistString = null;
		int m_groupNo = 0;
		string m_conditionType = CONDITION_TYPE_AND;
		string m_groupConditionType = null;
		string _fixedPurchaseKbn = null;

		/// <summary>
		/// 条件削除
		/// </summary>
		/// <param name="conditions">削除する条件</param>
		/// <param name="index">条件のおおもとのIndex</param>
		/// <param name="groupindex">条件のIndex</param>
		public void Remove(TargetListConditionList conditions, int index, int groupindex)
		{
			conditions.TargetConditionList.RemoveAt(index);
		}

		/// <summary>
		/// RepeaterのためのList作成
		/// </summary>
		/// <param name="condition">Listを作成する条件</param>
		/// <returns>リスト型の条件</returns>
		public List<TargetListCondition> MakeBindData(ITargetListCondition condition)
		{
			return new List<TargetListCondition>
			{
				(TargetListCondition)condition
			};
		}

		/// <summary>
		/// AND、OR条件変更
		/// </summary>
		/// <param name="condition">変更した条件</param>
		/// <param name="conditionType">変更したAND、OR条件</param>
		public void ChangeConditionType(ITargetListCondition condition, string conditionType)
		{
			((TargetListCondition)condition).ConditionType = conditionType;
		}

		/// <summary>
		/// AND、OR条件取得
		/// </summary>
		/// <param name="condition">取得する条件</param>
		/// <returns>AND、OR条件</returns>
		public string GetConditionType(ITargetListCondition condition)
		{
			return ((TargetListCondition)condition).ConditionType;
		}

		/// <summary>
		/// グループ番号取得
		/// </summary>
		/// <param name="condition">取得する条件</param>
		/// <returns>グループ番号</returns>
		public int GetGroupNo(ITargetListCondition condition)
		{
			return ((TargetListCondition)condition).GroupNo;
		}

		/// <summary>
		/// 条件からXML作成
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="xtw">書き込むXmlTextWriter</param>
		public void MakeXmlFromCondition(ITargetListCondition condition, XmlTextWriter xtw)
		{
			var tlc = (TargetListCondition)condition;

			xtw.WriteStartElement("Condition");

			xtw.WriteStartElement("DataKbn");
			xtw.WriteString(tlc.DataKbn);
			xtw.WriteEndElement();
			xtw.WriteStartElement("DataKbnString");
			xtw.WriteString(tlc.DataKbnString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("DataField");
			xtw.WriteString(tlc.DataField);
			xtw.WriteEndElement();
			xtw.WriteStartElement("DataFieldString");
			xtw.WriteString(tlc.DataFieldString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("DataType");
			xtw.WriteString(tlc.DataType);
			xtw.WriteEndElement();

			foreach (Data dataValue in tlc.Values)
			{
				xtw.WriteStartElement("Value");
				if (dataValue.Name != dataValue.Value)
				{
					xtw.WriteAttributeString("name", dataValue.Name);
				}
				xtw.WriteString(dataValue.Value);
				xtw.WriteEndElement();
			}

			xtw.WriteStartElement("FixedPurchaseKbn");
			xtw.WriteString(tlc.FixedPurchaseKbn);
			xtw.WriteEndElement();
			xtw.WriteStartElement("EqualSign");
			xtw.WriteString(tlc.EqualSign);
			xtw.WriteEndElement();
			xtw.WriteStartElement("EqualSignString");
			xtw.WriteString(tlc.EqualSignString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("OrderExist");
			xtw.WriteString(tlc.OrderExist);
			xtw.WriteEndElement();
			xtw.WriteStartElement("OrderExistString");
			xtw.WriteString(tlc.OrderExistString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("FixedPurchaseOrderExist");
			xtw.WriteString(tlc.FixedPurchaseOrderExist);
			xtw.WriteEndElement();
			xtw.WriteStartElement("FixedPurchaseOrderExistString");
			xtw.WriteString(tlc.FixedPurchaseOrderExistString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("PointExist");
			xtw.WriteString(tlc.PointExist);
			xtw.WriteEndElement();
			xtw.WriteStartElement("PointExistString");
			xtw.WriteString(tlc.PointExistString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("DmShippingHistoryExist");
			xtw.WriteString(tlc.DmShippingHistoryExist);
			xtw.WriteEndElement();
			xtw.WriteStartElement("DmShippingHistoryExistString");
			xtw.WriteString(tlc.DmShippingHistoryExistString);
			xtw.WriteEndElement();
			xtw.WriteStartElement("FavoriteExist");
			xtw.WriteString(tlc.FavoriteExist);
			xtw.WriteEndElement();
			xtw.WriteStartElement("FavoriteExistString");
			xtw.WriteString(tlc.FavoriteExistString);
			xtw.WriteEndElement();

			xtw.WriteEndElement();
		}

		/// <summary>
		/// データフィールドが正しいかどうか判定
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="settingId">設定ID</param>
		/// <returns>正しければtrue、間違っていればfalse</returns>
		public bool IsExistDataField(ITargetListCondition condition, string settingId)
		{
			return ((TargetListCondition)condition).DataField
				== (Constants.TABLE_USEREXTEND + "." + settingId);
		}

		/// <summary>
		/// ユーザー拡張項目に項目が存在しているか否かを確認
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="uesList">ユーザ拡張項目リスト</param>
		/// <param name="notExistUserExtendSetting">存在していない場合のに格納するDictionary</param>
		public void IsExtendSettingExist(
			ITargetListCondition condition,
			UserExtendSettingList uesList,
			Dictionary<string, string> notExistUserExtendSetting)
		{
			var tlc = (TargetListCondition)condition;
			// ユーザー拡張項目以外は以下のチェックを行わない
			if (tlc.DataField.StartsWith(Constants.TABLE_USEREXTEND) == false) return;

			// このターゲットの項目が正しく最新のユーザー拡張項目設定としてDBに入っているか
			// 直前で消えたりしていないかどうかを確認する
			if (uesList.Items.Exists(item => (Constants.TABLE_USEREXTEND + "." + item.SettingId == tlc.DataField)) == false)
			{
				// 存在していない →エラーにする
				notExistUserExtendSetting.Add(tlc.DataField.Replace(Constants.TABLE_USEREXTEND + ".", ""), tlc.DataFieldString);
			}
		}

		/// <summary>
		/// 検索項目が頒布会コースIDか
		/// </summary>
		/// <returns>頒布会コースIDの検索であればTRUE</returns>
		public bool IsDataFieldSubscriptionBoxCourseId() =>
			this.DataField == $"{Constants.TABLE_ORDER}.{Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID}";

		public const string DATAKBN_USER_INFO = "UserInfo";
		public const string DATAKBN_USER_EXTEND_INFO = "UserExtendInfo";
		public const string DATAKBN_USER_ATTRIBUTE_INFO = "UserAttributeInfo";
		public const string DATAKBN_ORDER_INFO = "OrderInfo";
		/// <summary>選択区分 受注情報(注文拡張項目)</summary>
		public const string DATAKBN_ORDER_EXTEND_INFO = "OrderExtendInfo";
		public const string DATAKBN_ORDER_AGGREGATE = "OrderAggregate";
		public const string DATAKBN_POINT_INFO = "PointInfo";
		public const string DATAKBN_LIMITED_TERM_POINT_INFO = "LimitedTermPointInfo";
		public const string DATAKBN_AGGREGATED_POINT_INFO = "AggregatedPointInfo";
		public const string DATAKBN_CART_INFO = "CartInfo";
		public const string DATAKBN_FIXEDPURCHASE_INFO = "FixedPurchaseInfo";
		/// <summary>選択区分 定期台帳(注文拡張項目)</summary>
		public const string DATAKBN_FIXEDPURCHASE_ORDER_EXTEND_INFO = "FixedPurchaseOrderExtendInfo";
		public const string DATAKBN_SQL_CONDITION = "SqlCondition";
		public const string DATAKBN_COUPON_INFO = "CouponInfo";
		public const string DATAKBN_MAIL_CLICK_INFO = "MailClickInfo";
		public const string DATAKBN_DM_SHIPPING_HISTORY_INFO = "DmShippingHistoryInfo";
		/// <summary>選択区分 'お気に入り'商品情報</summary>
		public const string DATAKBN_FAVORITE_PRODUCT_INFO = "FavoriteProductInfo";

		public const string EQUALSIGN_SELECT_EQUAL = "EqualSelect";
		public const string EQUALSIGN_SELECT_NOT_EQUAL = "NotEqualSelect";

		public const string EQUALSIGN_CHECK_EQUAL = "EqualCheck";
		public const string EQUALSIGN_CHECK_NOT_EQUAL = "NotEqualCheck";

		public const string EQUALSIGN_STRING_EQUAL = "EqualString";
		public const string EQUALSIGN_STRING_NOT_EQUAL = "NotEqualString";
		public const string EQUALSIGN_STRING_CONTAIN = "ContainString";
		public const string EQUALSIGN_STRING_NOT_CONTAIN = "NotContainString";
		public const string EQUALSIGN_STRING_BEGIN_WITH = "BeginWithString";
		public const string EQUALSIGN_STRING_END_WITH = "EndWithString";

		public const string EQUALSIGN_DAYAFTER_EQUAL = "EqualDay";
		public const string EQUALSIGN_DAYAFTER_BIGGER_THAN = "BiggerThanDay";
		public const string EQUALSIGN_DAYAFTER_SMALLER_THAN = "SmallerThanDay";

		public const string EQUALSIGN_NUMBER_EQUAL = "EqualNumber";
		public const string EQUALSIGN_NUMBER_BIGGER_THAN = "BiggerThanNumber";
		public const string EQUALSIGN_NUMBER_SMALLER_THAN = "SmallerThanNumber";

		public const string EQUALSIGN_DATE_EQUAL = "EqualDate";
		public const string EQUALSIGN_DATE_BEFORE_THAN = "BeforeThanDate";
		public const string EQUALSIGN_DATE_AFTER_THAN = "AfterThanDate";
		public const string EQUALSIGN_DATE_MORE_THAN_DAY = "MoreThanDayDate";
		public const string EQUALSIGN_DATE_LESS_THAN_DAY = "LessThanDayDate";
		public const string EQUALSIGN_DATE_LESS_THAN_WEEK = "LessThanWeekDate";
		public const string EQUALSIGN_DATE_LESS_THAN_MONTH = "LessThanMonthDate";
		public const string EQUALSIGN_DATE_LESS_THAN_YEAR = "LessThanYearDate";
		public const string EQUALSIGN_DATE_DAY_BEFORE = "DayBefore";
		public const string EQUALSIGN_DATE_DAY_AFTER = "DayAfter";
		public const string EQUALSIGN_DATE_DAY_BEFORE_FUTURE = "DayBeforeFuture";
		public const string EQUALSIGN_DATE_DAY_AFTER_PAST = "DayBeforePast";
		public const string EQUALSIGN_DATE_DAY_NULL = "DayNull";

		public const string ORDEREXIST_EXIST = "Exist";
		public const string ORDEREXIST_NOTEXIST = "NotExist";

		public const string FIXEDPURCHASEORDEREXIST_EXIST = "Exist";
		public const string FIXEDPURCHASEORDEREXIST_NOTEXIST = "NotExist";

		public const string POINTEXIST_EXIST = "Exist";
		public const string POINTEXIST_NOTEXIST = "NotExist";

		public const string DMSHIPPINGHISTORYINFO_EXIST = "Exist";
		public const string DMSHIPPINGHISTORYINFO_NOTEXIST = "NotExist";

		public const string FAVORITEEXIST_EXIST = "Exist";
		public const string FAVORITEEXIST_NOTEXIST = "NotExist";

		/// <summary>AND条件</summary>
		public const string CONDITION_TYPE_AND = "AND";
		/// <summary>OR条件</summary>
		public const string CONDITION_TYPE_OR = "OR";

		/// <summary>データタイプ：回数</summary>
		public const string DATATYPE_COUNT = "count";
		/// <summary>データタイプ：合計</summary>
		public const string DATATYPE_SUM = "sum";

		/// <summary>データ区分</summary>
		public string DataKbn
		{
			get { return m_strDataKbn; }
			set { m_strDataKbn = value; }
		}
		public string DataKbnString
		{
			get { return m_strDataKbnString; }
			set { m_strDataKbnString = value; }
		}

		/// <summary>定期配送パターン区分</summary>
		public string FixedPurchaseKbn
		{
			get { return _fixedPurchaseKbn; }
			set { _fixedPurchaseKbn = value; }
		}

		/// <summary>データフィールド</summary>
		public string DataField
		{
			get { return m_strDataField; }
			set { m_strDataField = value; }
		}
		public string DataFieldString
		{
			get { return m_strDataFieldString; }
			set { m_strDataFieldString = value; }
		}

		/// <summary>データフィールド</summary>
		public string DataType
		{
			get { return m_strDataType; }
			set { m_strDataType = value; }
		}

		/// <summary>データフィールド</summary>
		public List<Data> Values
		{
			get { return m_lValues; }
		}

		/// <summary>イコールサイン</summary>
		public string EqualSign
		{
			get { return m_strEqualSign; }
			set { m_strEqualSign = value; }
		}
		public string EqualSignString
		{
			get { return m_strEqualSignString; }
			set { m_strEqualSignString = value; }
		}

		/// <summary>注文ありなし</summary>
		public string OrderExist
		{
			get { return m_orderExist; }
			set { m_orderExist = value; }
		}
		public string OrderExistString
		{
			get { return m_orderExistString; }
			set { m_orderExistString = value; }
		}

		/// <summary>定期注文ありなし</summary>
		public string FixedPurchaseOrderExist
		{
			get { return m_fixedPurchaseOrderExist; }
			set { m_fixedPurchaseOrderExist = value; }
		}
		public string FixedPurchaseOrderExistString
		{
			get { return m_fixedPurchaseOrderExistString; }
			set { m_fixedPurchaseOrderExistString = value; }
		}

		/// <summary>ポイントありなし</summary>
		public string PointExist
		{
			get { return m_pointExist; }
			set { m_pointExist = value; }
		}
		public string PointExistString
		{
			get { return m_pointExistString; }
			set { m_pointExistString = value; }
		}

		/// <summary>DM発送履歴ありなし</summary>
		public string DmShippingHistoryExist
		{
			get { return _dmShippingHistoryExist; }
			set { _dmShippingHistoryExist = value; }
		}
		private string _dmShippingHistoryExist = null;
		public string DmShippingHistoryExistString
		{
			get { return _dmShippingHistoryExistString; }
			set { _dmShippingHistoryExistString = value; }
		}
		private string _dmShippingHistoryExistString = null;

		/// <summary>お気に入り商品ありなし</summary>
		public string FavoriteExist
		{
			get { return _favoriteExist; }
			set { _favoriteExist = value; }
		}
		private string _favoriteExist = null;
		/// <summary> お気に入り商品ありなしの名称付き </summary>
		public string FavoriteExistString
		{
			get { return _favoriteExistString; }
			set { _favoriteExistString = value; }
		}
		private string _favoriteExistString = null;

		/// <summary>グループ番号</summary>
		public int GroupNo
		{
			get { return m_groupNo; }
			set { m_groupNo = value; }
		}

		/// <summary>AND、OR条件</summary>
		public string ConditionType
		{
			get { return m_conditionType; }
			set { m_conditionType = value; }
		}

		/// <summary>グループ化されたときのAND、OR条件</summary>
		public string GroupConditionType
		{
			get { return m_groupConditionType; }
			set { m_groupConditionType = value; }
		}


		[Serializable]
		public class Data
		{
			string m_strName = null;
			string m_strValue = null;

			public Data(string strValue)
			{
				m_strName = m_strValue = strValue;
			}

			public Data(string strName, string strValue)
			{
				m_strName = strName;
				m_strValue = strValue;
			}

			public string Name
			{
				get { return m_strName; }
			}

			public string Value
			{
				get { return m_strValue; }
			}

		}
	}
}
