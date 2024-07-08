/*
=========================================================================================================
  Module      : モール設定 (MallConfig.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Common.Util;
using w2.SFTPClientWrapper;

namespace w2.Commerce.MallBatch.StockUpdate.Mall
{
	///**************************************************************************************
	/// <summary>
	/// モールの接続設定
	/// </summary>
	///**************************************************************************************
	public class MallConfig
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MallConfig()
		{
			// プロパティ初期化
			this.Configs = new List<MallConfigTip>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <remarks>モール連携設定情報を保持する</remarks>
		public MallConfig(SqlAccessor sqlAccessor) : this()
		{
			InitMallConfig(sqlAccessor);
		}

		/// <summary>
		/// モール連携設定情報からモール設定をプロパティに保持する
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void InitMallConfig(SqlAccessor sqlAccessor)
		{
			List<MallConfigTip> lMallConfigTipTmp = new List<MallConfigTip>();

			//------------------------------------------------------
			// モール連携設定情報取得
			//------------------------------------------------------
			DataView dvMallCooperationSettings = null;
			using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "GetMallCooperationSettings"))
			{
				dvMallCooperationSettings = sqlStatement.SelectSingleStatement(sqlAccessor);
			}

			//------------------------------------------------------
			// プロパティにセットする
			//------------------------------------------------------
			foreach (DataRowView drvMallCooperationSettings in dvMallCooperationSettings)
			{
				MallConfigTip mallConfigTip = new MallConfigTip();
				mallConfigTip.ShopId = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID]);
				mallConfigTip.MallId = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]);
				mallConfigTip.MallName = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]);
				mallConfigTip.MallKbn = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN]);
				mallConfigTip.FtpHost = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_HOST]);
				mallConfigTip.FtpUserName = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_USER_NAME]);
				mallConfigTip.FtpPassWord = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_PASSWORD]);
				mallConfigTip.FtpUploadDir = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_FTP_UPLOAD_DIR]);
				mallConfigTip.CnvidRtnNInsItemcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_INS_ITEMCSV]);
				mallConfigTip.CnvidRtnNUpdItemcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_UPD_ITEMCSV]);
				mallConfigTip.CnvidRtnNStkItemcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_N_STK_ITEMCSV]);
				mallConfigTip.CnvidRtnVInsItemcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_ITEMCSV]);
				mallConfigTip.CnvidRtnVInsSelectcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_INS_SELECTCSV]);
				mallConfigTip.CnvidRtnVUpdItemcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_UPD_ITEMCSV]);
				mallConfigTip.CnvidRtnVStkSelectcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_V_STK_SELECTCSV]);
				mallConfigTip.CnvidRtnItemcatcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_RTN_ITEMCATCSV]);
				mallConfigTip.RakutenSkuManagementIdOutputFormatForNormal = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_NORMAL]);
				mallConfigTip.RakutenSkuManagementIdOutputFormatForVariation = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_RAKUTEN_SKU_MANAGEMENT_ID_OUTPUT_FORMAT_FOR_VARIATION]);
				mallConfigTip.CnvidYhoNInsDatacsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_DATACSV]);
				mallConfigTip.CnvidYhoNInsStockcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_INS_STOCKCSV]);
				mallConfigTip.CnvidYhoNUpdDatacsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_UPD_DATACSV]);
				mallConfigTip.CnvidYhoNStkDatacsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_N_STK_DATACSV]);
				mallConfigTip.CnvidYhoVInsDatacsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_DATACSV]);
				mallConfigTip.CnvidYhoVInsStockcsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_INS_STOCKCSV]);
				mallConfigTip.CnvidYhoVUpdDatacsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_UPD_DATACSV]);
				mallConfigTip.CnvidYhoVStkDatacsv = StringUtility.ToEmpty(drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_CNVID_YHO_V_STK_DATACSV]);
				mallConfigTip.MaintenanceDateFrom = (drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM] == DBNull.Value) ? null : (DateTime?)drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_FROM];
				mallConfigTip.MaintenanceDateTo = (drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO] == DBNull.Value) ? null : (DateTime?)drvMallCooperationSettings[Constants.FIELD_MALLCOOPERATIONSETTING_MAINTENANCE_DATE_TO];
				mallConfigTip.PathFtpUpload = $"ftp://{mallConfigTip.FtpHost}/{mallConfigTip.FtpUploadDir}";
				mallConfigTip.SFTPSettings = new SFTPSettings
				{
					HostName = mallConfigTip.FtpHost,
					LoginUser = mallConfigTip.FtpUserName,
					LoginPassword = mallConfigTip.FtpPassWord,
					PortNumber = 22,
				};

				// 楽天はSFTPがあるため、URLを変える必要がある
				var useSftp = Constants.MALLCOOPERATION_RAKUTEN_USE_SFTP
					&& (mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN);
				mallConfigTip.IsSftp = useSftp;

				lMallConfigTipTmp.Add(mallConfigTip);
			}

			this.Configs = lMallConfigTipTmp;
		}

		/// <summary>モール設定情報</summary>
		public List<MallConfigTip> Configs { get; set; } 
	}
}
