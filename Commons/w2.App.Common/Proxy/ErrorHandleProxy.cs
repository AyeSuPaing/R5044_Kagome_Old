/*
=========================================================================================================
  Module      : エラーハンドルプロキシ(ErrorHandleProxy.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using System.Text;

namespace w2.App.Common.Proxy
{
	class ErrorHandleProxy : RealProxy
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="binder">ユーザイベントバインダ</param>
		/// <param name="type">型情報</param>
		public ErrorHandleProxy(ContextBoundObject binder, Type type)
			: base(type)
		{
			this.Context = binder;
		}

		/// <summary>
		/// メソッド呼び出し
		/// </summary>
		/// <param name="callMessage">メッセージ</param>
		/// <returns>Returnメッセージ</returns>
		public override IMessage Invoke(IMessage callMessage)
		{
			ReturnMessage returnMessage = null;

			// メソッド呼び出し命令を格納
			IMethodCallMessage callMethod = (IMethodCallMessage)callMessage;

			// コンストラクタの呼び出し命令を格納
			IConstructionCallMessage callConst = callMethod as IConstructionCallMessage;

			if (callConst != null)
			{
				// コンストラクタの要求がある場合はコンストラクタを実行
				RealProxy rp = RemotingServices.GetRealProxy(this.Context);
				rp.InitializeServerObject(callConst);
				MarshalByRefObject tp = this.GetTransparentProxy() as MarshalByRefObject;
				returnMessage = (ReturnMessage)EnterpriseServicesHelper.CreateConstructionReturnMessage(callConst, tp);
			}
			else
			{
				returnMessage = InvokeMethod(callMethod);
			}

			return returnMessage;
		}

		/// <summary>
		/// メソッド実行
		/// </summary>
		/// <param name="callMethod">メソッド呼び出しメッセージ</param>
		/// <returns>Returnメッセージ</returns>
		private ReturnMessage InvokeMethod(IMethodCallMessage callMethod)
		{
			ReturnMessage returnMessage = null;

			// メソッドの呼び出しの場合はメソッドを実行
			returnMessage = RemotingServices.ExecuteMessage(this.Context, callMethod) as ReturnMessage;

			// メソッドをキャンセルする属性が付与されている場合、
			// ログを落として例外を無かった事にする。
			if (HasExceptionCancelerAttribute(callMethod.MethodBase))
			{
				if (returnMessage.Exception != null)
				{
					w2.Common.Logger.FileLogger.WriteError("プラグイン処理中にエラーが発生しました。", returnMessage.Exception);

					returnMessage = new ReturnMessage(null, callMethod);
				}
			}
			return returnMessage;
		}

		/// <summary>
		/// 例外キャンセル属性を持っているか
		/// </summary>
		/// <param name="method">メソッド</param>
		/// <returns>所持フラグ</returns>
		private bool HasExceptionCancelerAttribute(MethodBase method)
		{
			// 付与されている属性の一覧取得
			List<object> attributes = method.GetCustomAttributes(true).ToList();
			return attributes.Find(attr => attr.GetType() == typeof(ExceptionCancelerAttribute)) != null;
		}

		/// <summary>透過オブジェクト情報</summary>
		private ContextBoundObject Context { get; set; }
	}
}
