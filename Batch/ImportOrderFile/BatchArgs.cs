/*
=========================================================================================================
  Module      : バッチ引数クラス(BatchArgs.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.IO;

namespace w2.Commerce.Batch.ImportOrderFile
{
	/// <summary>
	/// バッチ引数クラス
	/// </summary>
	public class BatchArgs
	{
		/// <summary>引数</summary>
		private string[] m_args = new string[] { };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private BatchArgs() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="args"></param>
		public BatchArgs(string[] args)
			: this()
		{
			if ((args == null) || (args.Length < 5))
			{
				this.ImportFilePath = "";
				this.ImportFileType = "";
				this.ExecExternalShipmentEntry = false;
				this.ExecuteOperatorName = "batch";
				this.OrderFileSettingValue = "";
				this.MailTemplateID = "";
				this.m_args = new string[] { };
			}
			else if (args.Length == 5)
			{
				this.ImportFilePath = args[0];
				this.ImportFileType = args[1];
				this.ExecExternalShipmentEntry = (args[2] == "1");
				this.ExecuteOperatorName = args[3];
				this.OrderFileSettingValue = args[4];
				this.MailTemplateID = "";
				this.m_args = args;
			}
			else
			{
				this.ImportFilePath = args[0];
				this.ImportFileType = args[1];
				this.ExecExternalShipmentEntry = (args[2] == "1");
				this.ExecuteOperatorName = args[3];
				this.OrderFileSettingValue = args[4];
				this.MailTemplateID = args[5];
				this.m_args = args;
			}
		}

		/// <summary>
		/// 引数チェックしてNGなら例外を投げる
		/// </summary>
		public void ThrowExceptionIfBatchArgsAreIllegal()
		{
			if (string.IsNullOrEmpty(this.ImportFilePath)
				|| string.IsNullOrEmpty(this.ImportFileType)
				|| string.IsNullOrEmpty(this.ExecuteOperatorName)
				|| string.IsNullOrEmpty(this.OrderFileSettingValue))
			{
				throw new BatchArgsException("指定されていない必須項目があります。");
			}

			if (File.Exists(this.ImportFilePath) == false)
			{
				throw new BatchArgsException("指定された取り込み対象のファイルが存在しません。");
			}

			switch (this.ImportFileType)
			{
				case Constants.KBN_ORDERFILE_SHIPPING_NO_LINK:
				case Constants.KBN_ORDERFILE_ECAT2000LINK:
				case Constants.KBN_B2_RAKUTEN_INCL_LINK:
				case Constants.KBN_ORDERFILE_PAYMENT_DATA:
				case Constants.KBN_ORDERFILE_IMPORT_ORDER:
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS:
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_PRICE_BY_TAX_RATE:
				case Constants.KBN_B2_RAKUTEN_INCL_LINK_CLOUD:
				case Constants.KBN_ORDERFILE_CANCEL_FIXEDPURCHASE:
				case Constants.KBN_ORDERFILE_ORDER_EXTEND:
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT:
				case Constants.KBN_ORDERFILE_IMPORT_PAYMENT_DEPOSIT_DSK:
				case Constants.KBN_ORDERFILE_SHIPPING_DATA:
				case Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK:
					break;

				default:
					throw new BatchArgsException(
						string.Format(
							"取り込みファイルの種別「{0}」を認識できません。",
							this.ImportFileType));
			}
		}

		#region +CreateArgsInfoMessage 引数情報メッセージ生成
		/// <summary>
		/// 引数情報メッセージ生成
		/// </summary>
		/// <returns>引数情報のメッセージ</returns>
		public string CreateArgsInfoMessage()
		{
			var msg = string.Format("■バッチ引数情報\r\n"
				+ "第一引数：取り込み対象のファイルのフルパス:指定値→{0}\r\n"
				+ "第二引数：取り込みファイルの種別:指定値→{1}\r\n"
				+ "第三引数：出荷情報登録連携するかしないか:指定値→{2}\r\n"
				+ "第四引数：実行オペレーター名:指定値→{3}\r\n"
				+ "第五引数：注文ファイル設定値→{4}\r\n"
				+ "第六引数：指定メールテンプレートID:指定値→{5}\r\n",
				this.ImportFilePath,
				this.ImportFileType,
				this.ExecExternalShipmentEntry,
				this.ExecuteOperatorName,
				this.OrderFileSettingValue,
				this.MailTemplateID);
			return msg;
		}
		#endregion

		/// <summary>
		/// ファイルタイプ名取得
		/// </summary>
		/// <returns>ファイルタイプ名</returns>
		public string GetFileTypeName()
		{
			switch (this.ImportFileType)
			{
				case Constants.KBN_ORDERFILE_SHIPPING_NO_LINK:
					return "w2Commerce標準配送伝票紐付けデータ";

				case Constants.KBN_ORDERFILE_ECAT2000LINK:
					return "e - cat2000紐づけデータ";

				case Constants.KBN_B2_RAKUTEN_INCL_LINK:
					return "B2配送伝票紐付けデータ";

				case Constants.KBN_B2_RAKUTEN_INCL_LINK_CLOUD:
					return "B2配送伝票紐付けデータ";

				case Constants.KBN_ORDERFILE_PAYMENT_DATA:
					return "入金データ";

				case Constants.KBN_ORDERFILE_IMPORT_ORDER:
					return "注文データ";

				case Constants.KBN_ORDERFILE_IMPORT_ORDER_PRICE_BY_TAX_RATE:
					return "税率毎価格情報データ";

				case Constants.KBN_ORDERFILE_ORDER_EXTEND:
					return "注文拡張ステータス更新データ";

				case Constants.KBN_ORDERFILE_IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT:
					return "２回目未入金者取込";

				case Constants.KBN_ORDERFILE_IMPORT_PAYMENT_DEPOSIT_DSK:
					return "DSK入金データ取込";

				case Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK:
					return "宅配通配送実績紐付けデータ";

				case Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS:
					return "受注情報ステータスの一括更新";

				default:
					return "不明な形式";
			}
		}

		/// <summary>
		/// 取り込みファイルの種別
		/// ECAT2000LINK          ：e-cat2000紐づけデータ
		/// SHIPPING_NO_LINK      ：w2Commerce標準配送伝票紐付けデータ
		/// B2_RAKUTEN_INCL_LINK  ：B2配送伝票紐付けデータ
		/// PAYMENT_DATA          ：入金データ
		/// IMPORT_ORDER          ：注文データ
		/// IMPORT_ORDER_PRICE_BY_TAX_RATE          ：税率毎価格情報データ
		/// IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT ：２回目未入金者取込
		/// IMPORT_PAYMENT_DEPOSIT_DSK           ：DSK入金データ取込"
		/// IMPORT_ORDER_STATUS    :受注情報ステータス更新
		/// </summary>
		public string ImportFileType { get; set; }
		/// <summary>取込対象ファイルのフルパス</summary>
		public string ImportFilePath { get; set; }
		/// <summary>
		/// 出荷情報登録連携フラグ
		/// True：連携する
		/// False：連携しない
		/// </summary>
		public bool ExecExternalShipmentEntry { get; set; }
		/// <summary>実行オペレータ名</summary>
		public string ExecuteOperatorName { get; set; }
		/// <summary>注文ファイル設定値</summary>
		public string OrderFileSettingValue { get; set; }
		/// <summary>メールテンプレートID</summary>
		public string MailTemplateID { get; set; }
	}
}
