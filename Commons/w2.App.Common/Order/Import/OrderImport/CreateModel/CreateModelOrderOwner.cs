/*
=========================================================================================================
  Module      : 注文者モデル作成クラス(CreateModelOrderOwner.cs)
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
	/// 注文者モデル作成
	/// </summary>
	public class CreateModelOrderOwner : CreateModelBase
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
			this.m_FieldsDatetimeNullable.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH);
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
			// なし
		}

	}
}
