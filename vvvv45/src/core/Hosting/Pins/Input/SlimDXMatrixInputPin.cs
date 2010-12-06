﻿using System;
using System.Runtime.InteropServices;
using SlimDX;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Hosting.Pins.Input
{
	public class SlimDXMatrixInputPin : DiffPin<Matrix>, IPinUpdater
	{
		protected ITransformIn FTransformIn;
		
		public SlimDXMatrixInputPin(IPluginHost host, InputAttribute attribute)
			: base(host, attribute)
		{
			host.CreateTransformInput(FName, FSliceMode, FVisibility, out FTransformIn);
			base.InitializeInternalPin(FTransformIn);
		}
		
		public override bool IsChanged
		{
			get
			{
				return FTransformIn.PinIsChanged;
			}
		}
		
		unsafe protected void CopyToBuffer(Matrix[] buffer, float* source, int length)
		{
			fixed (Matrix* destination = buffer)
			{
				Matrix* dst = destination;
				Matrix* src = (Matrix*) source;
				
				for (int i = 0; i < length; i++)
					*(dst++) = *(src++);
			}
		}
		
		unsafe public override void Update()
		{
			if (IsChanged)
			{
				int length;
				float* source;
				
				FTransformIn.GetMatrixPointer(out length, out source);
				SliceCount = length;
				
				if (FSliceCount > 0)
					CopyToBuffer(FBuffer, source, length);
			}
			
			base.Update();
		}
	}
}
