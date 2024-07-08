/*
=========================================================================================================
  Module      : つくーるAPI連携コマンドクラス(UreruAdImportCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Commerce.Batch.ExternalOrderImport.Entity;
using w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd;

namespace w2.Commerce.Batch.ExternalOrderImport.Commands
{
	/// <summary>
	/// つくーるAPI連携コマンドクラス
	/// </summary>
	public class UreruAdImportCommand : CommandBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UreruAdImportCommand()
			: base()
		{
			this.ExecuteResult.AppName = "つくーるAPI連携";
			this.ExecuteResult.MailTemplateId = Constants.CONST_MAIL_ID_URERU_AD_IMPORT_FOR_OPERATOR;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <returns>成功件数/全体件数</returns>
		public override void Exec()
		{
			// 0.機能が有効であるかチェック
			if (Constants.URERU_AD_IMPORT_ENABLED == false) return;

			// 1.最終実行日時を取得
			var lastExec = GetLastExec();

			// 2.指定期間のデータ取得
			var executeDatetime = DateTime.Now;
			var responses = GetResponses(lastExec, executeDatetime);

			// 3.1レコードずつ取り込み。データがなければ7.へ
			var successOrders = ImportOrder(responses);

			// 4.コンビニ後払い、NP後払い、後払い.comの注文があれば与信取得
			var failedAuthOrders = ExecutePayment(successOrders);
			
			// 5.与信取得失敗していたら通知メールをユーザーに送信
			failedAuthOrders.ForEach(failedAuthOrder => failedAuthOrder.SendMailForCvsDefAuthError());
			
			// 6.最終実行日時を最新の受注データの受注日時で更新(受注データがなかった場合は受注日時Toで指定した時間で更新)
			UpdateLastExec(executeDatetime, responses);

			// 7.実行結果取得
			SetResult(responses, successOrders);
		}

		/// <summary>
		/// 最終実行日時を取得
		/// </summary>
		/// <returns>最終実行日時</returns>
		private DateTime GetLastExec()
		{
			var lastExec = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_LASTEXEC_PREFIX + "*");
			if (lastExec.Any())
			{
				var latestDatetime = Path.GetFileName(lastExec.First()).Substring(Constants.FILENAME_LASTEXEC_PREFIX.Length);
				return DateTime.ParseExact(latestDatetime, Constants.FILENAME_LASTEXEC_SUFFIX_DATEFORMAT, null);
			}
			return DateTime.Today;
		}

		/// <summary>
		/// 最終実行日時を更新
		/// </summary>
		/// <param name="executeDatetime">実行時間</param>
		/// <param name="responses">レスポンスデータ</param>
		private void UpdateLastExec(DateTime executeDatetime, UreruAdResponseData responses)
		{
			var latestOrder = (responses.Results != null)
				? responses.Results
					.Where(result => result.Created.HasValue)
					.OrderBy(result => result.Created.Value)
					.LastOrDefault()
				: null;
			var lastExecDate = (latestOrder != null)
				? latestOrder.Created.Value
				: executeDatetime;
			Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_LASTEXEC_PREFIX + "*")
				.ToList()
				.ForEach(File.Delete);
			File.Create(Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				Constants.FILENAME_LASTEXEC_PREFIX + lastExecDate.ToString(Constants.FILENAME_LASTEXEC_SUFFIX_DATEFORMAT)));
		}

		/// <summary>
		/// 指定期間のデータ取得
		/// </summary>
		/// <param name="startDatetime">指定期間(From)</param>
		/// <param name="endDatetime">指定期間(To)</param>
		/// <returns></returns>
		private UreruAdResponseData GetResponses(DateTime startDatetime, DateTime endDatetime)
		{
			var responses = new UreruAdResponseData
			{
				Results = Enumerable.Range(0, ((endDatetime - startDatetime).Days + 1))
					.Select(day =>
					{
						// APIコール用パラメータ作成
						var targetStartDatetime = startDatetime.AddDays(day).AddMinutes(-1);
						var requestData = new UreruAdRequestData
						{
							StartDatetime = targetStartDatetime,
							EndDatetime = (targetStartDatetime.AddDays(1) < endDatetime)
								? targetStartDatetime.AddDays(1)
								: endDatetime
						};

						// APIコール＋JSONデシリアライズ
						var responseData = CallUreruAdApi(requestData);
						return responseData;
					})
					.Where(response => response.Results != null)
					.SelectMany(response => response.Results)
					.ToArray()
			};
			return responses;
		}

		/// <summary>
		/// つくーるAPIコール
		/// </summary>
		/// <param name="requestData">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		private UreruAdResponseData CallUreruAdApi(UreruAdRequestData requestData)
		{
			using(var connector = new HttpApiConnector())
			{
				var response = connector.Do(
						Constants.URERU_AD_IMPORT_API_URL,
						Encoding.UTF8,
						requestData);
				if (response.Contains("result"))
				{
					return JsonConvert.DeserializeObject<UreruAdResponseData>(response);
				}
				return new UreruAdResponseData();
			}
		}

		/// <summary>
		/// つくーる注文データ取込
		/// </summary>
		/// <param name="responseDatas">つくーる注文データ</param>
		/// <returns>取込成功データ</returns>
		private List<OrderImport> ImportOrder(UreruAdResponseData responseDatas)
		{
			if ((responseDatas == null) || (responseDatas.Results == null)) return new List<OrderImport>();

			var successOrders = new List<OrderImport>();
			foreach (var response in responseDatas.Results)
			{
				using (var accessor = new SqlAccessor())
				{
					try
					{
						response.Validate();
						if (response.ExistsDuplicatedOrder) continue;

						accessor.OpenConnection();
						accessor.BeginTransaction();

						// 正常、異常に関わらずINSERT/UPDATE
						var userImport = new UserImport(response, accessor);
						userImport.Import();

						var orderImport = new OrderImport(response, accessor);
						orderImport.Import();

						if (response.ErrorMessage.Any() == false)
						{
							successOrders.Add(orderImport);
						}
						accessor.CommitTransaction();
						Console.WriteLine(string.Format(
							"{0}：{1}",
							response.Id,
							ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTERNAL_IMPORT_STATUS, response.ExternalImportStatus)));
					}
					catch (Exception ex)
					{
						try
						{
							if (accessor.Connection.State == ConnectionState.Open) accessor.RollbackTransaction();
						}
						catch (Exception rollbackException)
						{
							response.ErrorMessage.Add("トランザクションロールバックに失敗");
							ConsoleLogger.WriteError(rollbackException, "トランザクションロールバックに失敗");
						}
						var message = string.Format("つくーる注文ID：{0}のデータでシステムエラーが発生しました。", response.Id);
						response.ErrorMessage.Add(message);
						ConsoleLogger.WriteError(ex, message);
					}
				}
			}

			return successOrders;
		}

		/// <summary>
		/// 後払い決済の与信取得
		/// </summary>
		/// <param name="successOrders">正常取込済み注文</param>
		/// <returns>与信取得失敗の注文</returns>
		private List<OrderImport> ExecutePayment(List<OrderImport> successOrders)
		{
			var failedAuthOrders = new List<OrderImport>();
			if (successOrders == null) return failedAuthOrders;

			foreach (var successOrder in successOrders)
			{
				var order = new OrderService().Get(successOrder.ResponseData.OrderId);
				if (order.IsCanceled) continue;

				var errorMesasge = successOrder.ExecPayment();
				if (string.IsNullOrEmpty(errorMesasge) == false)
				{
					this.ExecuteResult.ErrorLog.Add(string.Format("注文ID：{0}<br />：{1}", successOrder.ResponseData.OrderId, errorMesasge));
					failedAuthOrders.Add(successOrder);
				}
			}
			return failedAuthOrders;
		}

		/// <summary>
		/// 実行結果取得
		/// </summary>
		/// <param name="responses">レスポンスデータ</param>
		/// <param name="successOrders">正常取込済み注文</param>
		private void SetResult(UreruAdResponseData responses, List<OrderImport> successOrders)
		{
			if (responses.Results != null)
			{
				this.ExecuteResult.ErrorLog.AddRange(responses.Results.SelectMany(result => result.ErrorMessage).ToArray());
			}
			
			this.ExecuteResult.ExecuteCount = (responses.Results != null) ? responses.Results.Count() : 0;
			this.ExecuteResult.FailureCount = (responses.Results != null) ? responses.Results.Count(result => result.ErrorMessage.Any()) : 0;
			this.ExecuteResult.SkipCount = (responses.Results != null) ? responses.Results.Count(response => response.ExistsDuplicatedOrder) : 0;
			this.ExecuteResult.SuccessCount = successOrders.Count;
		}
	}
}