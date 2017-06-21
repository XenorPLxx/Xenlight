using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;

namespace Xenlight
{
    public class Dx11ScreenCapture
    {
        uint numAdapter = 0; // # of graphics card adapter
        uint numOutput = 0; // # of output device (i.e. monitor)
        SharpDX.Direct3D11.Device device;
        Factory1 factory;
        SharpDX.Direct3D11.Texture2DDescription texdes;
        SharpDX.Direct3D11.Texture2D screenTexture;
        Output1 output;
        OutputDuplication duplicatedOutput;
        Resource screenResource = null;
        public SharpDX.DataStream dataStream;
        Surface screenSurface;
        public Dx11ScreenCapture()
        {
            // create device and factory
            device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware);
            factory = new Factory1();
            // creating CPU-accessible texture resource
            texdes = new SharpDX.Direct3D11.Texture2DDescription();
            texdes.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read;
            texdes.BindFlags = SharpDX.Direct3D11.BindFlags.None;
            texdes.Format = Format.B8G8R8A8_UNorm;
            texdes.Height = factory.Adapters1[numAdapter].Outputs[numOutput].Description.DesktopBounds.Height;
            texdes.Width = factory.Adapters1[numAdapter].Outputs[numOutput].Description.DesktopBounds.Width;
            texdes.OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None;
            texdes.MipLevels = 1;
            texdes.ArraySize = 1;
            texdes.SampleDescription.Count = 1;
            texdes.SampleDescription.Quality = 0;
            texdes.Usage = SharpDX.Direct3D11.ResourceUsage.Staging;
            screenTexture = new SharpDX.Direct3D11.Texture2D(device, texdes);

            // duplicate output stuff
            output = new Output1(factory.Adapters1[numAdapter].Outputs[numOutput].NativePointer);
            duplicatedOutput = output.DuplicateOutput(device); 
        }
        public SharpDX.DataStream CaptureScreen()
        {
            // try to get duplicated frame within given time
            try
            {
                OutputDuplicateFrameInformation duplicateFrameInformation;
                duplicatedOutput.AcquireNextFrame(1000, out duplicateFrameInformation, out screenResource);            

            // copy resource into memory that can be accessed by the CPU
            device.ImmediateContext.CopyResource(screenResource.QueryInterface<SharpDX.Direct3D11.Resource>(), screenTexture);
            // cast from texture to surface, so we can access its bytes
            screenSurface = screenTexture.QueryInterface<SharpDX.DXGI.Surface>();

            // map the resource to access it
            screenSurface.Map(MapFlags.Read, out dataStream);

            // seek within the stream and read one byte
            //dataStream.Position = 4;
            //dataStream.ReadByte();

            // free resources
            //dataStream.Close();
            screenSurface.Unmap();
            screenSurface.Dispose();
            screenResource.Dispose();
            duplicatedOutput.ReleaseFrame();
            return dataStream;
            }
            catch (SharpDX.SharpDXException e)
            {
                device = new SharpDX.Direct3D11.Device(SharpDX.Direct3D.DriverType.Hardware);
                factory = new Factory1();
                // creating CPU-accessible texture resource
                texdes = new SharpDX.Direct3D11.Texture2DDescription();
                texdes.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.Read;
                texdes.BindFlags = SharpDX.Direct3D11.BindFlags.None;
                texdes.Format = Format.B8G8R8A8_UNorm;
                texdes.Height = factory.Adapters1[numAdapter].Outputs[numOutput].Description.DesktopBounds.Height;
                texdes.Width = factory.Adapters1[numAdapter].Outputs[numOutput].Description.DesktopBounds.Width;
                texdes.OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None;
                texdes.MipLevels = 1;
                texdes.ArraySize = 1;
                texdes.SampleDescription.Count = 1;
                texdes.SampleDescription.Quality = 0;
                texdes.Usage = SharpDX.Direct3D11.ResourceUsage.Staging;
                screenTexture = new SharpDX.Direct3D11.Texture2D(device, texdes);

                // duplicate output stuff
                output = new Output1(factory.Adapters1[numAdapter].Outputs[numOutput].NativePointer);
                try
                {
                    duplicatedOutput = output.DuplicateOutput(device);
                }
                catch { }
                return CaptureScreen();
            }
        }
    }
}