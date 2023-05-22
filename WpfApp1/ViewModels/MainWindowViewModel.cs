using iTextSharp.text.pdf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.ViewModels.Base;
using Xceed.Wpf.Toolkit;

namespace WpfApp1.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly List<StrokeCollection> _desks;

        public MainWindowViewModel()
        {
             _desks = new List<StrokeCollection>();

            _inkCanvasStrokes = new StrokeCollection();

            Desks.Add(inkCanvas1.Strokes.Clone());

            #region Commands



            #endregion
        }

        #region Properties

        #region IncCanvasStrokes

        private StrokeCollection _inkCanvasStrokes;

        public StrokeCollection InkCanvasStrokes
        {
            get => _inkCanvasStrokes;
            set => Set(ref _inkCanvasStrokes, value);
        }

        #endregion

        #endregion

        #region Commands

        public ICommand SaveCanvas { get; private set; }

        private void OnSaveCanvasExecuted(object p) 
        {
            InkCanvas inkCanvas = p as InkCanvas;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "pdf files|*.pdf";
            SFD.ShowDialog();

            // https://stackoverflow.com/questions/21411878/saving-a-canvas-to-png-c-sharp-wpf
            if (SFD.FileName == "") return;
            var stream = new FileStream(SFD.FileName, FileMode.Append, FileAccess.Write, FileShare.None);

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            document.SetMargins(10, 10, 10, 10);

            PdfWriter.GetInstance(document, stream);
            document.Open();

            StrokeCollection temp = InkCanvasStrokes;

            for (int i = 0; i < Desks.Count; i++)
            {
                inkCanvas.Strokes = Desks[i];
                Rect bounds = VisualTreeHelper.GetDescendantBounds(inkCanvas);
                double dpi = 96d;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

                DrawingVisual dv = new DrawingVisual();

                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(inkCanvas1);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);

                MemoryStream fs = new MemoryStream();
                JpegBitmapEncoder encoder1 = new JpegBitmapEncoder();
                encoder1.Frames.Add(BitmapFrame.Create(rtb));
                encoder1.Save(fs);
                byte[] tArr = fs.ToArray();

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(tArr);

                image.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                image.ScaleToFit(document.PageSize.Width - 10, document.PageSize.Height - 10);

                document.Add(image);
                document.NewPage();
                fs.Close();
                rtb.Clear();
            }

            inkCanvas1.Strokes = temp;

            document.Close();
        }

        private ICommand ClearCanvas { get; private set; }

        public void OnClearCanvaseExecuted(object p)
        {
            InkCanvasStrokes.Clear();
        }


        #endregion

        /*
        // Отменить дейсвтие
        public void Undo(object sender, RoutedEventArgs e)
        {
            EraserButton.IsChecked = false;
            EraserStroke.IsChecked = false;
            if (inkCanvas1.Strokes.Count > 0) inkCanvas1.Strokes.RemoveAt(inkCanvas1.Strokes.Count - 1);
        }
        // Кнопка вернуть
        public void Redo(object sender, RoutedEventArgs e)
        {
            EraserButton.IsChecked = false;
            EraserStroke.IsChecked = false;
            try
            {
                inkCanvas1.Strokes.Add(Desks[DeskNumber][inkCanvas1.Strokes.Count]);
            }
            catch { MessageBox.Show("Нечего возвращать"); }
        }

        // Очистка доски
        private void ClearCanvas(object sender, RoutedEventArgs e) { inkCanvas1.Strokes.Clear(); }


        // Сохраняем свое творчество в pdf используя iTextSharp из NuGet пакетов
        private void SaveCanvas(object sender, RoutedEventArgs e)
        {

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Filter = "pdf files|*.pdf";
            SFD.ShowDialog();

            // https://stackoverflow.com/questions/21411878/saving-a-canvas-to-png-c-sharp-wpf
            if (SFD.FileName == "") return;
            var stream = new FileStream(SFD.FileName, FileMode.Append, FileAccess.Write, FileShare.None);

            iTextSharp.text.Document document = new iTextSharp.text.Document();
            document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
            document.SetMargins(10, 10, 10, 10);

            PdfWriter.GetInstance(document, stream);
            document.Open();

            StrokeCollection temp = inkCanvas1.Strokes;

            for (int i = 0; i < Desks.Count; i++)
            {
                inkCanvas1.Strokes = Desks[i];
                Rect bounds = VisualTreeHelper.GetDescendantBounds(inkCanvas1);
                double dpi = 96d;

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, PixelFormats.Default);

                DrawingVisual dv = new DrawingVisual();

                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(inkCanvas1);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }

                rtb.Render(dv);

                MemoryStream fs = new MemoryStream();
                JpegBitmapEncoder encoder1 = new JpegBitmapEncoder();
                encoder1.Frames.Add(BitmapFrame.Create(rtb));
                encoder1.Save(fs);
                byte[] tArr = fs.ToArray();

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(tArr);

                image.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                image.ScaleToFit(document.PageSize.Width - 10, document.PageSize.Height - 10);

                document.Add(image);
                document.NewPage();
                fs.Close();
                rtb.Clear();
            }

            inkCanvas1.Strokes = temp;

            document.Close();

        }

        // Меняем толщину кисти
        private void NumericLimit(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                inkCanvas1.DefaultDrawingAttributes.Width = (int)InkWidth.Value;
                inkCanvas1.DefaultDrawingAttributes.Height = (int)InkWidth.Value;
                inkCanvas1.EraserShape = new RectangleStylusShape((int)InkWidth.Value, (int)InkWidth.Value);
            }
            catch { InkWidth.Value = 1; }

        }
        // Выбираем ластик
        private void ChoiceEraser(object sender, RoutedEventArgs e)
        {

            *//*
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.DefaultDrawingAttributes.Color = Color.FromArgb(255, 255, 255, 255);
            inkCanvas1.UseCustomCursor = true;
            *//*
            if (EraserButton.IsChecked == true)
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint;
            else
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }
        private void ChoiceEraserBytStroke(object sender, RoutedEventArgs e)
        {
            if (EraserStroke.IsChecked == true)
                inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByStroke;
            else
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        // Выбираем кисть
        private void ChoicePen(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            inkCanvas1.DefaultDrawingAttributes.Color = clr;
            inkCanvas1.UseCustomCursor = false;
        }
        // Выбираем "Выбрать"
        private void ChoiceSelect(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Select;
            inkCanvas1.UseCustomCursor = false;
        }
        // Временный объект
        private void ChoiceTemp(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.GestureOnly;
            inkCanvas1.DefaultDrawingAttributes.Color = clr;
            inkCanvas1.UseCustomCursor = false;
        }

        // Отслеживаем координаты мыши
        private void MouseMove1(object sender, MouseEventArgs e)
        {
            textBlock1.Text = "X = " + Convert.ToInt32(e.GetPosition(inkCanvas1).X).ToString() + "\nY = " + Convert.ToInt32(e.GetPosition(inkCanvas1).Y).ToString();
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (e.GetPosition(inkCanvas1).X > inkCanvas1.Width - 15) inkCanvas1.Width += 20;
                if (e.GetPosition(inkCanvas1).Y > inkCanvas1.Height - 15) inkCanvas1.Height += 20;

            }

        }
        // Раскрываем colorCanvas для выбора цвета
        private void ColorPicker(object sender, RoutedEventArgs e) { colorPicker1.Visibility = Visibility; }
        // При потери фокуса у ColorPicker смена цвета кисти
        private void PickerHide(object sender, MouseEventArgs e)
        {

            colorPicker1.Visibility = Visibility.Hidden;

            mcolor.A = colorPicker1.A;
            mcolor.R = colorPicker1.R;
            mcolor.G = colorPicker1.G;
            mcolor.B = colorPicker1.B;

            clr = Color.FromArgb(mcolor.A, mcolor.R, mcolor.G, mcolor.B);

            inkCanvas1.DefaultDrawingAttributes.Color = clr;
        }
        // Клонируем inkCanvas1 в temp для UnDo/ReDo
        private void MouseLeftButtonUp1(object sender, MouseButtonEventArgs e)
        {
            if (EraserStroke.IsChecked == false & EraserButton.IsChecked == false)
            {
                Desks[DeskNumber] = inkCanvas1.Strokes.Clone();
            }
        }
        // Меняем доску на следующую
        private void nextDesk(object sender, RoutedEventArgs e)
        {
            DeskNumber++;
            if (DeskNumber <= Desks.Count - 1)
            {
                Desks[DeskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
            else
            {

                Desks[DeskNumber - 1] = inkCanvas1.Strokes.Clone();
                inkCanvas1.Strokes.Clear();
                Desks.Add(inkCanvas1.Strokes.Clone());
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
        }
        // Меняем доску на предыдущую
        private void previousDesk(object sender, RoutedEventArgs e)
        {
            if (DeskNumber > 0)
            {
                Desks[DeskNumber] = inkCanvas1.Strokes.Clone();
                DeskNumber--;
                inkCanvas1.Strokes = Desks[DeskNumber].Clone();
                CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
            }
            else
            {
                MessageBox.Show("Last desk");
            }
        }
        // Добавляем новую доску
        private void addDesk(object sender, RoutedEventArgs e)
        {
            Desks.Add(inkCanvas1.Strokes.Clone());
        }

        private void SaveDesks(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "udf files|*.udf";
            saveFileDialog.ShowDialog();
            string[] desks = new string[Desks.Count];
            StrokeCollectionConverter converter = new StrokeCollectionConverter();

            if (saveFileDialog.FileName != "")
            {
                for (int i = 0; i < Desks.Count; i++)
                {
                    desks[i] = converter.ConvertToString(Desks[i]);
                }
                string json = JsonConvert.SerializeObject(desks);
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }
        private void LoadDesks(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "udf files|*.udf";
            openFileDialog.ShowDialog();

            StrokeCollectionConverter converter = new StrokeCollectionConverter();
            Desks.Clear();
            string readText = File.ReadAllText(openFileDialog.FileName);
            string[] desks = JsonConvert.DeserializeObject<string[]>(readText);
            foreach (var i in desks)
            {
                Desks.Add((StrokeCollection)converter.ConvertFromString(i));
            }
            DeskNumber = 0;
            inkCanvas1.Strokes = Desks[DeskNumber].Clone();
            CurrentDesk.Text = $"{(DeskNumber + 1).ToString()} / {Desks.Count}";
        }

        private void ChoiceHand(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        private void ReturnINK(object sender, KeyEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void onOffFXAA(object sender, RoutedEventArgs e)
        {

            if (FXAA.IsChecked == true)
                inkCanvas1.DefaultDrawingAttributes.FitToCurve = true;
            else
                inkCanvas1.DefaultDrawingAttributes.FitToCurve = false;

        }

        private void onOffHighlither(object sender, RoutedEventArgs e)
        {
            if (Highlither.IsChecked == true)
                inkCanvas1.DefaultDrawingAttributes.IsHighlighter = true;
            else
                inkCanvas1.DefaultDrawingAttributes.IsHighlighter = false;
        }*/

    }

}
