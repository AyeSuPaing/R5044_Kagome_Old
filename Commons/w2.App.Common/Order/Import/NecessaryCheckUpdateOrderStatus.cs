/*
=========================================================================================================
  Module      : 必須チェック(受注ステータス更新)クラス(NecessaryCheckUpdateOrderStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Import.OrderImport.NecessaryCheck
{
	/// <summary>
	/// 必須チェック(受注ステータス更新)
	/// </summary>
	public class NecessaryCheckUpdateOrderStatus : NecessaryCheckBase
	{
		/// <summary>
		/// 必須項目リスト作成
		/// </summary>
		internal override void CreateListNecessary()
		{
			this.m_FieldsNecessary.Add(Constants.FIELD_ORDER_ORDER_ID);
		}
	}
}
