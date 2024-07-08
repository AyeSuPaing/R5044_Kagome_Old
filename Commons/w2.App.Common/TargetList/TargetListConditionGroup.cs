/*
=========================================================================================================
  Module      : ターゲットグループリスト条件クラス(TargetListConditionGroup.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using w2.Domain.User.Helper;

namespace w2.App.Common.TargetList
{
	/// <summary>
	/// ターゲットリストグループ条件クラス
	/// </summary>
	[Serializable]
	public class TargetListConditionGroup : ITargetListCondition
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TargetListConditionGroup()
		{
			this.TargetGroup = new List<TargetListCondition>();
		}

		/// <summary>
		/// 条件削除
		/// </summary>
		/// <param name="conditions">削除する条件</param>
		/// <param name="index">条件のおおもとのIndex</param>
		/// <param name="groupIndex">条件のIndex</param>
		public void Remove(TargetListConditionList conditions, int index, int groupIndex)
		{
			var group = ((TargetListConditionGroup)(conditions.TargetConditionList[index])).TargetGroup;
			group.RemoveAt(groupIndex);
			if (group.Count == 1)
			{
				group[0].GroupNo = 0;
				group[0].GroupConditionType = null;
				conditions.TargetConditionList.Insert(index, group[0]);
				conditions.TargetConditionList.RemoveAt(index);
			}
		}

		/// <summary>
		/// RepeaterのためのList作成
		/// </summary>
		/// <param name="condition">Listを作成する条件</param>
		/// <returns>リスト型の条件</returns>
		public List<TargetListCondition> MakeBindData(ITargetListCondition condition)
		{
			var conditionList = (TargetListConditionGroup)condition;
			return conditionList.TargetGroup;
		}

		/// <summary>
		/// AND、OR条件変更
		/// </summary>
		/// <param name="conditions">変更した条件</param>
		/// <param name="conditionType">変更したAND、OR条件</param>
		public void ChangeConditionType(ITargetListCondition conditions, string conditionType)
		{
			foreach (var condition in ((TargetListConditionGroup)conditions).TargetGroup)
			{
				condition.ChangeConditionType(condition, conditionType);
				condition.GroupConditionType = (conditionType == TargetListCondition.CONDITION_TYPE_AND)
					? TargetListCondition.CONDITION_TYPE_OR
					: TargetListCondition.CONDITION_TYPE_AND;
			}
		}

		/// <summary>
		/// AND、OR条件取得
		/// </summary>
		/// <param name="condition">取得する条件</param>
		/// <returns>AND、OR条件</returns>
		public string GetConditionType(ITargetListCondition condition)
		{
			return ((TargetListConditionGroup)condition).TargetGroup[0].ConditionType;
		}

		/// <summary>
		/// グループ番号取得
		/// </summary>
		/// <param name="condition">取得する条件</param>
		/// <returns>グループ番号</returns>
		public int GetGroupNo(ITargetListCondition condition)
		{
			return ((TargetListConditionGroup)condition).TargetGroup[0].GroupNo;
		}

		/// <summary>
		/// 条件からXML作成
		/// </summary>
		/// <param name="conditions">条件</param>
		/// <param name="xtw">書き込むXmlTextWriter</param>
		public void MakeXmlFromCondition(ITargetListCondition conditions, XmlTextWriter xtw)
		{
			xtw.WriteStartElement("GroupCondition");
			xtw.WriteAttributeString("type", ((TargetListConditionGroup)conditions).TargetGroup.Last().GroupConditionType);

			foreach (var condition in ((TargetListConditionGroup)conditions).TargetGroup)
			{
				condition.MakeXmlFromCondition(condition, xtw);
			}
			xtw.WriteEndElement();	// GroupCondition
		}

		/// <summary>
		/// データフィールドが正しいかどうか判定
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="settingId">設定ID</param>
		/// <returns>正しければtrue、間違っていればfalse</returns>
		public bool IsExistDataField(ITargetListCondition condition, string settingId)
		{
			return ((TargetListConditionGroup)condition).TargetGroup.Exists(
				x => x.DataField == (Constants.TABLE_USEREXTEND + "." + settingId));
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
			foreach (var tlc in ((TargetListConditionGroup)condition).TargetGroup)
			{
				tlc.IsExtendSettingExist(tlc, uesList, notExistUserExtendSetting);
			}
		}

		/// <summary>条件格納用リストプロパティ</summary>
		public List<TargetListCondition> TargetGroup { set; get; }
	}
}
