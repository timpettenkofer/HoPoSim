using Prism.Commands;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HoPoSim.Presentation.Helpers
{
    public class BitmapRenderer
    {
        public BitmapRenderer(Func<string> fileNameDelegate)
        {
            _fileNameDelegate = fileNameDelegate;
            ExportImageCommand = new DelegateCommand<UIElement>(this.ExportImage);
        }

        Func<string> _fileNameDelegate;
        public DelegateCommand<UIElement> ExportImageCommand { get; protected set; }


        public void ExportImage(UIElement uiElement)
        {
            if (uiElement == null)
                return;

            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "png files (*.png)|*.png",
                RestoreDirectory = true,
                FileName = (_fileNameDelegate() ?? "Capture") + "_" + DateTime.Now.ToString("dd-MM-yyyy")
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ConvertToBitmapSource(uiElement, saveFileDialog.FileName);
            }
        }


        private static void ConvertToBitmapSource(UIElement element, string filename)
        {
            //var target = new RenderTargetBitmap(
            //    (int)element.RenderSize.Width, (int)element.RenderSize.Height,
            //    96, 96, PixelFormats.Pbgra32);
            //target.Render(element);
            var target = RenderVisual(element, 192, 192);
            var encoder = new PngBitmapEncoder();
            var outputFrame = BitmapFrame.Create(target);
            encoder.Frames.Add(outputFrame);

            using (var file = File.OpenWrite(filename))
            {
                encoder.Save(file);
            }
        }

        private static RenderTargetBitmap RenderVisual(UIElement elt, double dpiX, double dpiY)
        {
            //PresentationSource source = PresentationSource.FromVisual(elt);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(elt.RenderSize.Width * dpiX / 96.0),
                  (int)(elt.RenderSize.Height * dpiX / 96.0), dpiX, dpiY, PixelFormats.Default);

            //Rect bounds = VisualTreeHelper.GetDescendantBounds(elt);
            //RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
            //                                                (int)(bounds.Height * dpiY / 96.0),
            //                                                dpiX,
            //                                                dpiY,
            //                                                PixelFormats.Default);


            VisualBrush sourceBrush = new VisualBrush(elt);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0),
                      new Point(elt.RenderSize.Width, elt.RenderSize.Height)));
            }
            rtb.Render(drawingVisual);

            return rtb;
        }
    }
}
