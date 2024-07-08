using w2.App.Common.Order.Workflow;
using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using w2.App.CommonTests._Helper;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.OrderWorkflowSetting;
using w2.Domain.TaskSchedule;

namespace w2.App.Common.Order.Workflow.Tests
{
	[TestClass()]
	[Ignore]
	public class WorkflowExecTests : AppTestClassBase
	{
		// DB接続しないと確認できないためコメントアウト
		//[DataTestMethod()]
		//public void ExecOrderWorkflowForScenarioTest()
		//{
		//	Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE = @"C:\inetpub\wwwroot\V5\Commons\w2.CustomerResources\";
		//	Constants.STRING_SQL_CONNECTION = "server=w2DB1;database=_w2DevelopV5.ochiai;uid=sa;pwd=w2Sa";
		//	Constants.PHYSICALDIRPATH_LOGFILE = @"C:\Logs\Tests\";
		//	Constants.LOGGING_PERFORMANCE_SQL_ENABLED = true;

		//	var deptId = "0";
		//	var actionKbn = "ExecScenarioToManual";
		//	var actionMasterId = "32";
		//	var lastCahnged = Constants.FLG_LASTCHANGED_BATCH;

		//	var taskSchedule = new TaskScheduleService().InsertTaskScheduleForExecute(
		//		deptId,
		//		actionKbn,
		//		actionMasterId,
		//		lastCahnged);
		//	var we = new WorkflowExec(
		//		taskSchedule.DeptId,
		//		taskSchedule.ActionKbn,
		//		taskSchedule.ActionMasterId,
		//		taskSchedule.ActionNo,
		//		taskSchedule.ScheduleDate,
		//		taskSchedule.LastChanged,
		//		taskSchedule.ExecuteStatus);
		//	var sm = new OrderWorkflowSettingModel
		//	{
		//		ShopId = "0",
		//		WorkflowKbn = "009",
		//		WorkflowNo = 24,
		//	};

		//	try
		//	{
		//		Parallel.For(
		//			1,
		//			100,
		//			i =>
		//			{
		//				var results = we.As<dynamic>().ExecOrderWorkflowForScenario(sm);
		//			});
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex);
		//		throw;
		//	}
		//}

		// コード修正しないと確認できないためコメントアウト
		//[DataTestMethod()]
		//public void ExecFixedPurchaseActionTest()
		//{
		//	Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE = @"C:\inetpub\wwwroot\V5\Commons\w2.CustomerResources\";
		//	Constants.STRING_SQL_CONNECTION = "server=w2DB1;database=_w2DevelopV5.ochiai;uid=sa;pwd=w2Sa";
		//	Constants.PHYSICALDIRPATH_LOGFILE = @"C:\Logs\Tests\";
		//	Constants.LOGGING_PERFORMANCE_SQL_ENABLED = true;
		//	Constants.PHYSICALDIRPATH_SQL_STATEMENT = @"C:\inetpub\wwwroot\V5\Web\w2.Commerce.Manager\Xml\Db\";

		//	var deptId = "0";
		//	var actionKbn = "ExecScenarioToManual";
		//	var actionMasterId = "32";
		//	var lastChanged = Constants.FLG_LASTCHANGED_BATCH;
		//	var shopId = "0";
		//	var workflowKbn = "009";
		//	var workflowNo = 4; // 定期

		//	var taskSchedule = new TaskScheduleService().InsertTaskScheduleForExecute(
		//		deptId,
		//		actionKbn,
		//		actionMasterId,
		//		lastChanged);
		//	var we = new WorkflowExec(
		//		taskSchedule.DeptId,
		//		taskSchedule.ActionKbn,
		//		taskSchedule.ActionMasterId,
		//		taskSchedule.ActionNo,
		//		taskSchedule.ScheduleDate,
		//		taskSchedule.LastChanged,
		//		taskSchedule.ExecuteStatus);

		//	var ids = new[] { "CSV2018100500002", "CSV2018101600004", "CSV2018101700005", "CSV2018101900001", "CSV2018101900002", "CSV2018101900003", "CSV2018102200001", "CSV2018102500002", "CSVFP2018111900001", "FP2018060700001", "FP2018060700002", "FP2018061300001", "FP2018061500001", "FP2018061800001", "FP2018061800002", "FP2018062100001", "FP2018062200001", "FP2018062200002", "FP2018062200003", "FP2018062200004", "FP2018062200005", "FP2018062300001", "FP2018062300002", "FP2018062300003", "FP2018062300004", "FP2018062300005", "FP2018062300006", "FP2018062300007", "FP2018062500001", "FP2018062600001", "FP2018062800001", "FP2018062800002", "FP2018062800003", "FP2018062800004", "FP2018062900001", "FP2018062900002", "FP2018062900003", "FP2018070100001", "FP2018070100002", "FP2018070100003", "FP2018070200001", "FP2018070400001", "FP2018070400002", "FP2018070400003", "FP2018070400004", "FP2018070400005", "FP2018070400006", "FP2018070400007", "FP2018070400008", "FP2018070600001", "FP2018070900001", "FP2018071400001", "FP2018071500001", "FP2018071900001", "FP2018071900002", "FP2018072700001", "FP2018072800001", "FP2018072900001", "FP2018073000001", "FP2018073000002", "FP2018080300001", "FP2018080600001" };

		//	try
		//	{
		//		var orderToSetting = new WorkflowSetting().GetOrderWorkflowSetting(
		//			shopId,
		//			workflowKbn,
		//			workflowNo.ToString(),
		//			"fixed_purchase");
		//		var ws = new WorkflowSetting(
		//			orderToSetting[0],
		//			deptId,
		//			lastChanged,
		//			WorkflowSetting.WorkflowTypes.FixedPurchase);

		//		Parallel.For(
		//			1,
		//			25,
		//			i =>
		//			{
		//				using (var accessor = new SqlAccessor())
		//				{
		//					accessor.OpenConnection();
		//					accessor.BeginTransaction();

		//					foreach (var id in ids)
		//					{
		//						var results = we.ExecFixedPurchaseAction(
		//							ws,
		//							new WorkflowExecInfo(id.IndexOf(id), id, true, ""),
		//							lastChanged,
		//							true,
		//							DateTime.Now.AddDays(1).Date.ToString(),
		//							DateTime.Now.AddMonths(1).Date.ToString(),
		//							DateTime.Now.AddMonths(2).Date.ToString(),
		//							accessor);
		//					}
		//				}
		//			});
		//	}
		//	catch (Exception ex)
		//	{
		//		FileLogger.WriteError(ex);
		//		throw;
		//	}
		//}
	}
}
