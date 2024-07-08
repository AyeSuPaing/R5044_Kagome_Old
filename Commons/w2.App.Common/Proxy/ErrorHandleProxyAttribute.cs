/*
=========================================================================================================
  Module      : エラーハンドルプロキシ属性(ErrorHandleProxyAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace w2.App.Common.Proxy
{
	/// <summary>
	/// エラー処理を操作する為のプロキシ
	/// </summary>
	/// <remarks>
	/// ContextBoundObjectを継承したクラスに対して付与出来る属性。
	/// ExceptionCancelerAttributeを対象のクラスのメソッドに付与する事で、
	/// 例外発生時に例外のログを自動的に落とし、例外を無かった事にする事が出来る。
	/// </remarks>
	/// <example>
	/// <para>[ErrorHandleProxy()]</para>
	/// <para>class Sample : ContextBoundObject</para>
	/// <para>    [ExceptionCanceler]</para>
	/// <para>    public void SampleMethod()</para>
	/// </example>
	[AttributeUsage(AttributeTargets.Class)]
	class ErrorHandleProxyAttribute : ProxyAttribute
	{
		/// <summary>
		/// インスタンス生成
		/// </summary>
		/// <param name="serverType">型情報</param>
		/// <returns>透過オブジェクト</returns>
		public override MarshalByRefObject CreateInstance(Type serverType)
		{
			MarshalByRefObject target = (MarshalByRefObject)base.CreateInstance(serverType);

			ErrorHandleProxy proxy = new ErrorHandleProxy((ContextBoundObject)target, serverType);
			return (MarshalByRefObject)proxy.GetTransparentProxy();
		}
	}

	/// <summary>
	/// 例外をキャンセルする
	/// </summary>
	/// <remarks>
	/// 戻り値が無いvoidのメソッドのみで利用するべき
	/// </remarks>
	public class ExceptionCancelerAttribute : Attribute
	{
		// 属性の存在が分かれば良い //
	}
}
