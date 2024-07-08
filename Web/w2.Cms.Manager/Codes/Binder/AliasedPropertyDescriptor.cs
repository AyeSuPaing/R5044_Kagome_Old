/*
=========================================================================================================
  Module      : AliasedPropertyDescriptor(AliasedPropertyDescriptor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.ComponentModel;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// AliasedPropertyDescriptor
	/// </summary>
	internal sealed class AliasedPropertyDescriptor : PropertyDescriptor
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="alias">エイリアス</param>
		/// <param name="inner">内部PropertyDescriptor</param>
		public AliasedPropertyDescriptor(string alias, PropertyDescriptor inner)
			: base(alias, null)
		{
			this.Inner = inner;
		}

		/// <summary>
		/// CanResetValue
		/// </summary>
		/// <param name="component"></param>
		/// <returns>可能か</returns>
		public override bool CanResetValue(object component)
		{
			return this.Inner.CanResetValue(component);
		}

		/// <summary>
		/// GetValue
		/// </summary>
		/// <param name="component">component</param>
		/// <returns>value</returns>
		public override object GetValue(object component)
		{
			return this.Inner.GetValue(component);
		}

		/// <summary>
		/// ResetValue
		/// </summary>
		/// <param name="component">component</param>
		public override void ResetValue(object component)
		{
			this.Inner.ResetValue(component);
		}

		/// <summary>
		/// SetValue
		/// </summary>
		/// <param name="component">component</param>
		/// <param name="value">value</param>
		public override void SetValue(object component, object value)
		{
			this.Inner.SetValue(component, value);
		}

		/// <summary>
		/// ShouldSerializeValue
		/// </summary>
		/// <param name="component">component</param>
		/// <returns>必須か</returns>
		public override bool ShouldSerializeValue(object component)
		{
			return this.Inner.ShouldSerializeValue(component);
		}

		/// <summary>IsReadOnly</summary>
		public override bool IsReadOnly
		{
			get { return this.Inner.IsReadOnly; }
		}
		/// <summary>ComponentType</summary>
		public override Type ComponentType
		{
			get { return this.Inner.ComponentType; }
		}
		/// <summary>PropertyType</summary>
		public override Type PropertyType
		{
			get { return this.Inner.PropertyType; }
		}
		/// <summary>内部PropertyDescriptor</summary>
		public PropertyDescriptor Inner { get; private set; }
	}
}