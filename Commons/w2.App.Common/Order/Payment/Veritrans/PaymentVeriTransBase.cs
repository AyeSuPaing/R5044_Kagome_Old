/*
=========================================================================================================
  Module      : ベリトランスベースクラス(PaymentVeriTransBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using jp.veritrans.tercerog.mdk;
using log4net.Config;

namespace w2.App.Common.Order.Payment.Veritrans
{
	/// <summary>
	/// ベリトランスベースクラス
	/// </summary>
	public abstract class PaymentVeriTransBase
	{
		/// <summary>ベリトランス決済種別</summary>
		protected virtual VeriTransConst.VeritransPaymentKbn VeritransPaymentType { get { return VeriTransConst.VeritransPaymentKbn.Credit; } }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected PaymentVeriTransBase()
		{
			var veriTransConfigPath = GetVeriTransConfigPath();
			MdkContext.Configure(new FileInfo(veriTransConfigPath));

			var logRootDir = Constants.PHYSICALDIRPATH_LOGFILE;
			Environment.SetEnvironmentVariable(VeriTransConst.VERITRANS_LOG_OUT_DIR_ENV, logRootDir);

			var veriTransLogConfigPath = Constants.PAYMENT_VERITRANS4G_LOG4G_CONFIG_PATH + VeriTransConst.LOG_CONFIG_FILE_NAME;
			XmlConfigurator.Configure(new FileInfo(veriTransLogConfigPath));
		}

		/// <summary>
		/// Get VeriTrans config path
		/// </summary>
		/// <returns>VeriTrans config path</returns>
		private string GetVeriTransConfigPath()
		{
			switch (this.VeritransPaymentType)
			{
				case VeriTransConst.VeritransPaymentKbn.Credit:
					return Constants.PAYMENT_VERITRANS4G_MDK_CONFIG_PATH + VeriTransConst.VERITRANS_CREDIT_CONFIG_FILE_NAME;

				case VeriTransConst.VeritransPaymentKbn.CvsDef:
					return Constants.PAYMENT_VERITRANS4G_MDK_CONFIG_PATH + VeriTransConst.VERITRANS_CVS_DEF_CONFIG_FILE_NAME;

				case VeriTransConst.VeritransPaymentKbn.Paypay:
					return Constants.PAYMENT_VERITRANS4G_MDK_CONFIG_PATH + VeriTransConst.VERITRANS_PAYPAY_CONFIG_FILE_NAME;

				default:
					throw new ArgumentOutOfRangeException(nameof(this.VeritransPaymentType), this.VeritransPaymentType, null);
			}
		}

		/// <summary>
		/// JPO 支払情報作成
		/// </summary>
		/// <param name="creditInstallmentsCode">支払回数(コード)</param>
		/// <returns>JPO 支払情報</returns>
		/// <remarks>ボーナス払い(21)は非対応</remarks>
		protected string CreateJpo(string creditInstallmentsCode)
		{
			switch (creditInstallmentsCode)
			{
				// 一括
				case VeriTransConst.CREDIT_INSTALLMENTS_ONE_TIME_PAYMENT:
					return "10";

				// リボ払い
				case VeriTransConst.CREDIT_INSTALLMENTS_REVOLVING_PAYMENT:
					return "80";

				// 支払い回数
				default:
					return "61C" + creditInstallmentsCode;
			}
		}
	}
}
