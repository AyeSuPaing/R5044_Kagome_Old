/*
=========================================================================================================
  Module      : 条件のインターフェース(ITargetListCondition.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Xml;
using w2.Domain.User.Helper;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリスト条件インターフェース
	/// </summary>
	public interface ITargetListCondition
	{
		/// <summary>
		/// 条件削除
		/// </summary>
		/// <param name="conditions">削除する条件</param>
		/// <param name="index">条件のおおもとのIndex</param>
		/// <param name="groupIndex">条件のIndex</param>
		void Remove(TargetListConditionList conditions, int index, int groupIndex);

		/// <summary>
		/// RepeaterのためのList作成
		/// </summary>
		/// <param name="condition">Listを作成する条件</param>
		/// <returns>リスト型の条件</returns>
		List<TargetListCondition> MakeBindData(ITargetListCondition condition);

		/// <summary>
		/// AND、OR条件変更
		/// </summary>
		/// <param name="condition">変更した条件</param>
		/// <param name="conditionType">変更したAND、OR条件</param>
		void ChangeConditionType(ITargetListCondition condition, string conditionType);

		/// <summary>
		/// AND、OR条件取得
		/// </summary>
		/// <param name="condition">取得する条件</param>
		/// <returns>AND、OR条件</returns>
		string GetConditionType(ITargetListCondition condition);

		/// <summary>
		/// グループ番号取得
		/// </summary>
		/// <param name="condition">取得する条件</param>
		/// <returns>グループ番号</returns>
		int GetGroupNo(ITargetListCondition condition);

		/// <summary>
		/// 条件からXML作成
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="xtw">書き込むXmlTextWriter</param>
		void MakeXmlFromCondition(ITargetListCondition condition, XmlTextWriter xtw);

		/// <summary>
		/// データフィールドが正しいかどうか判定
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="settingId">設定ID</param>
		/// <returns>正しければtrue、間違っていればfalse</returns>
		bool IsExistDataField(ITargetListCondition condition, string settingId);

		/// <summary>
		/// ユーザー拡張項目に項目が存在しているか否かを確認
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="uesList">ユーザ拡張項目リスト</param>
		/// <param name="notExistUserExtendSetting">存在していない場合のに格納するDictionary</param>
		void IsExtendSettingExist(
			ITargetListCondition condition,
			UserExtendSettingList uesList,
			Dictionary<string, string> notExistUserExtendSetting);
	}
}
