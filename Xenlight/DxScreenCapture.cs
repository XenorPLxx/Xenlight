//-----------------------------------------------------------------------
// <copyright file="DxScreenCapture.cs" company="Xenlight Dawid Karczewski">
//     Xenlight Dawid Karczewski.
// </copyright>
//-----------------------------------------------------------------------

namespace Xenlight
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms;
    using Microsoft.DirectX.Direct3D;
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1642:ConstructorSummaryDocumentationMustBeginWithStandardText", Justification = "Reviewed.")]

    /// <summary>
    /// Klasa odpowiadająca za przechwytywanie obrazu przez DirectX.
    /// </summary>
    public class DxScreenCapture : Form
    {
        /// <summary>
        /// Urzadzenie DX.
        /// </summary>
        private Device d;

        /// <summary>
        /// Powierzchnia DX.
        /// </summary>
        private Surface s;

        /// <summary>
        /// Konstruktor klasy.
        /// </summary>
        public DxScreenCapture()
        {
            PresentParameters present_params = new PresentParameters();
            present_params.Windowed = true;
            present_params.SwapEffect = SwapEffect.Discard;
            this.d = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, present_params);
            this.s = this.d.CreateOffscreenPlainSurface(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
        }

        /// <summary>
        /// Właściwość łapiąca obraz.
        /// </summary>
        /// <returns>Złapaną powierzchnię.</returns>
        public Surface CaptureScreen()
        {
            try
            {
                this.s.Dispose();
                this.s = this.d.CreateOffscreenPlainSurface(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
                this.d.GetFrontBufferData(0, this.s);
                //s = this.d.GetBackBuffer(0, 0, BackBufferType.Mono);
            }
            catch { }
            return this.s;
        }
    }
}
