/*
=========================================================================================================
  Module      : 外部APIファサード(ExternalApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.Zcom;

namespace w2.App.Common
{
	/// <summary>
	/// 外部APIファサード
	/// </summary>
	public class ExternalApiFacade
	{
		/// <summary>インスタンス</summary>
		private static readonly ExternalApiFacade m_instance = new ExternalApiFacade();

		/// <summary>本物のサービスクラスをセットするか</summary>
		private readonly bool m_setRealServices = false;

		/// <summary>サービスの集合（コード簡略化のためHashtableで実装）</summary>
		private readonly Hashtable m_services = new Hashtable();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setRealServices">本物のサービスクラスをセットするか（テストの時にfalse利用想定）</param>
		private ExternalApiFacade(bool setRealServices = true)
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
				if (m_setRealServices) m_services[key] = (T1) (object) new T2(); // いったんobjectキャストしないと変換できない
				else throw new NullReferenceException("オブジェクトがありません：" + typeof(T1));
			}

			return (T1) m_services[key];
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

		/// <summary>ZcomAPI連携ファサード生成ファクトリ</summary>
		internal IZcomApiFacadeFactory ZcomApiFacadeFactory
		{
			get { return GetService<IZcomApiFacadeFactory, ZcomApiFacadeFactory>(); }
			set { m_services[CreateServicesKey<IZcomApiFacadeFactory>()] = value; }
		}
		/// <summary>GMOクレジット決済</summary>
		internal IPaymentGmoCredit PaymentGmoCredit
		{
			get { return GetService<IPaymentGmoCredit, PaymentGmoCredit>(); }
			set { m_services[CreateServicesKey<IPaymentGmoCredit>()] = value; }
		}
		/// <summary>AmazonPayCv2APIのファサード</summary>
		internal IAmazonCv2ApiFacade AmazonCv2ApiFacade
		{
			get { return GetService<IAmazonCv2ApiFacade, AmazonCv2ApiFacade>(); }
			set { m_services[CreateServicesKey<IAmazonCv2ApiFacade>()] = value; }
		}
		/// <summary>インスタンス</summary>
		public static ExternalApiFacade Instance
		{
			get { return m_instance; }
		}
	}
}
