/*
=========================================================================================================
  Module      : 注文系プロシージャファサードクラス(OrderProcedureFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Order.Register;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 注文系プロシージャファサードクラス
	/// </summary>
	public class OrderProcedureFacade
	{
		/// <summary>インスタンス</summary>
		private static readonly OrderProcedureFacade m_instance = new OrderProcedureFacade();

		/// <summary>本物のサービスクラスをセットするか</summary>
		private readonly bool m_setRealServices = false;

		/// <summary>サービスの集合（コード簡略化のためHashtableで実装）</summary>
		private readonly Hashtable m_services = new Hashtable();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setRealServices">本物のサービスクラスをセットするか（テストの時にfalse利用想定）</param>
		private OrderProcedureFacade(bool setRealServices = true)
		{
			m_setRealServices = setRealServices;
		}

		#region -GetService サービス取得
		/// <summary>
		/// サービス取得
		/// </summary>
		/// <typeparam name="T1">サービスのインターフェース</typeparam>
		/// <typeparam name="T2">本物のサービスクラス</typeparam>
		/// <returns>サービス</returns>
		private T1 GetService<T1, T2>()
			where T1 : IService
			where T2 : IService, new()
		{
			var key = CreateServicesKey<T1>();
			if (m_services[key] == null)
			{
				if (m_setRealServices) m_services[key] = (T1)(object)new T2(); // いったんobjectキャストしないと変換できない
				else throw new NullReferenceException("オブジェクトがありません：" + typeof(T1));
			}

			return (T1)m_services[key];
		}
		#endregion

		#region -CreateServicesKey サービス取得のためのキー取得
		/// <summary>
		/// サービス取得のためのキー取得
		/// </summary>
		/// <typeparam name="T1">サービスのインターフェース</typeparam>
		/// <returns>キー</returns>
		private string CreateServicesKey<T1>()
		{
			var key = typeof(T1).ToString();
			return key;
		}
		#endregion

		/// <summary>インスタンス</summary>
		public static OrderProcedureFacade Instance
		{
			get { return m_instance; }
		}

		/// <summary>仮注文登録</summary>
		public IOrderPreorderRegister OrderPreorderRegister
		{
			get { return GetService<IOrderPreorderRegister, OrderPreorderRegister>(); }
			set { m_services[CreateServicesKey<IOrderPreorderRegister>()] = value; }
		}

	}
}
