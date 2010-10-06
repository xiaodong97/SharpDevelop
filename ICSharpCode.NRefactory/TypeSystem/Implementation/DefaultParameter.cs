﻿// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation for IParameter.
	/// </summary>
	public sealed class DefaultParameter : AbstractFreezable, IParameter, ISupportsInterning
	{
		string name = string.Empty;
		ITypeReference type = SharedTypes.UnknownType;
		IList<IAttribute> attributes;
		IConstantValue defaultValue;
		DomRegion region;
		byte flags;
		
		protected override void FreezeInternal()
		{
			type.Freeze();
			attributes = FreezeList(attributes);
			if (defaultValue != null)
				defaultValue.Freeze();
			base.FreezeInternal();
		}
		
		[ContractInvariantMethod]
		void ObjectInvariant()
		{
			Contract.Invariant(type != null);
			Contract.Invariant(name != null);
		}
		
		public string Name {
			get { return name; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				Contract.EndContractBlock();
				CheckBeforeMutation();
				name = value;
			}
		}
		
		public ITypeReference Type {
			get { return type; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				Contract.EndContractBlock();
				CheckBeforeMutation();
				type = value;
			}
		}
		
		public IList<IAttribute> Attributes {
			get {
				if (attributes == null)
					attributes = new List<IAttribute>();
				return attributes;
			}
		}
		
		public IConstantValue DefaultValue {
			get { return defaultValue; }
			set {
				CheckBeforeMutation();
				defaultValue = value;
			}
		}
		
		public DomRegion Region {
			get { return region; }
			set {
				CheckBeforeMutation();
				region = value;
			}
		}
		
		bool HasFlag(byte flag)
		{
			return (this.flags & flag) != 0;
		}
		void SetFlag(byte flag, bool value)
		{
			CheckBeforeMutation();
			if (value)
				this.flags |= flag;
			else
				this.flags &= unchecked((byte)~flag);
		}
		
		public bool IsRef {
			get { return HasFlag(1); }
			set { SetFlag(1, value); }
		}
		
		public bool IsOut {
			get { return HasFlag(2); }
			set { SetFlag(2, value); }
		}
		
		public bool IsParams {
			get { return HasFlag(4); }
			set { SetFlag(4, value); }
		}
		
		public bool IsOptional {
			get { return this.DefaultValue != null; }
		}
		
		void ISupportsInterning.PrepareForInterning(IInterningProvider provider)
		{
			CheckBeforeMutation();
			name = provider.InternString(name);
			type = provider.InternObject(type);
			attributes = provider.InternObjectList(attributes);
			defaultValue = provider.InternObject(defaultValue);
		}
		
		int ISupportsInterning.GetHashCodeForInterning()
		{
			return type.GetHashCode() ^ (attributes != null ? attributes.GetHashCode() : 0) ^ (defaultValue != null ? defaultValue.GetHashCode() : 0);
		}
		
		bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
		{
			DefaultParameter p = other as DefaultParameter;
			return p != null && type == p.type && attributes == p.attributes
				&& defaultValue == p.defaultValue && region == p.region && flags == p.flags;
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			if (IsRef)
				b.Append("ref ");
			if (IsOut)
				b.Append("out ");
			if (IsParams)
				b.Append("params ");
			b.Append(name);
			b.Append(':');
			b.Append(type.ToString());
			if (defaultValue != null) {
				b.Append(" = ");
				b.Append(defaultValue.ToString());
			}
			return b.ToString();
		}
	}
}
