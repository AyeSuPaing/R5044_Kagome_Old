/*
=========================================================================================================
  Module      : 注文配送先モデル作成クラス(CreateModelOrderShipping.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain;

namespace w2.App.Common.Order.Import.OrderImport.CreateModel
{
	/// <summary>
	/// 注文配送先モデル作成
	/// </summary>
	public class CreateModelOrderShipping : CreateModelBase
	{
		/// <summary>
		/// 日付型リスト作成
		/// </summary>
		internal override void CreateListDatetime()
		{
			// なし
		}

		/// <summary>
		/// 日付型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListDatetimeNullable()
		{
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE);
		}

		/// <summary>
		/// Decimal型リスト作成
		/// </summary>
		internal override void CreateListDecimal()
		{
			// なし
		}

		/// <summary>
		/// Decimal型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListDecimalNullable()
		{
			// なし
		}

		/// <summary>
		/// Int型リスト作成
		/// </summary>
		internal override void CreateListInt()
		{
			// なし
		}

		/// <summary>
		/// Int型(Null許可)リスト作成
		/// </summary>
		internal override void CreateListIntNullable()
		{
			this.m_FieldsIntNullable.Add(Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO);
		}
	}
}
