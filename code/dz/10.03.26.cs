using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Collections.Generic;

namespace UniversalMdiEditorFramework
{
    #region Задание 1: Универсальный каркас

    // Интерфейс для всех документов
    public interface IDocument
    {
        string FileName { get; set; }
        bool IsModified { get; }
        void CreateNew();
        void Open(string filePath);
        void Save();
        void SaveAs(string filePath);
        void Print();
        void Close();
    }

    // Базовый абстрактный класс документа
    public abstract class BaseDocument : IDocument
    {
        public string FileName { get; set; } = "Новый документ";
        public bool IsModified { get; protected set; }

        public abstract void CreateNew();
        public abstract void Open(string filePath);
        public abstract void Save();
        public abstract void SaveAs(string filePath);
        public abstract void Print();
        public abstract void Close();
    }

    // Абстрактная форма редактора
    public abstract class EditorForm : Form
    {
        protected IDocument document;
        protected OpenFileDialog openDialog;
        protected SaveFileDialog saveDialog;

        public string FileName 
        { 
            get => document.FileName; 
            set => document.FileName = value; 
        }

        public bool IsModified => document.IsModified;

        protected EditorForm(string title, IDocument doc)
        {
            document = doc;
            Text = title;
            IsMdiChild = true;
            WindowState = FormWindowState.Maximized;
            openDialog = new OpenFileDialog();
            saveDialog = new SaveFileDialog();
            UpdateTitle();
        }

        public virtual void CreateNew() 
        {
            document.CreateNew();
            OnContentChanged();
            UpdateTitle();
        }

        public virtual void Open(string filePath)
        {
            document.Open(filePath);
            OnContentChanged();
            UpdateTitle();
        }

        public virtual void Save()
        {
            OnSaveContent();
            document.Save();
            UpdateTitle();
        }

        public virtual void SaveAs(string filePath)
        {
            OnSaveContent();
            document.SaveAs(filePath);
            UpdateTitle();
        }

        public void Print() => document.Print();

        public virtual void Close()
        {
            if (IsModified)
            {
                var result = MessageBox.Show("Сохранить изменения?", "Закрытие", 
                    MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) Save();
                else if (result == DialogResult.Cancel) return;
            }
            Dispose();
        }

        protected virtual void UpdateTitle() => 
            Text = $"{Path.GetFileName(FileName)} {(IsModified ? "*" : "")}";

        // Абстрактные методы для реализации в наследниках
        protected abstract void OnContentChanged();
        protected abstract void OnSaveContent();
    }

    // Фабрика документов
    public static class DocumentFactory
    {
        public static IDocument CreateDocument(string type)
        {
            return type.ToLower() switch
            {
                "text" => new TextDocument(),
                "image" => new ImageDocument(),
                _ => throw new ArgumentException($"Неизвестный тип документа: {type}")
            };
        }
    }

    // Главная MDI форма
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            Text = "Универсальный MDI Редактор";
        }

        private void СоздатьToolStripMenuItem_Click(object sender, ToolStripMenuItem item)
        {
            string type = item.Tag?.ToString() ?? "text";
            var doc = DocumentFactory.CreateDocument(type);
            EditorForm editor = CreateEditor(type, doc);
            editor.MdiParent = this;
            editor.Show();
            editor.CreateNew();
        }

        private EditorForm CreateEditor(string type, IDocument doc)
        {
            return type.ToLower() switch
            {
                "text" => new TextEditorForm(doc),
                "image" => new ImageEditorForm(doc),
                _ => throw new ArgumentException($"Неизвестный тип редактора: {type}")
            };
        }

        private void ФайлОткрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var active = ActiveMdiChild as EditorForm;
            if (active != null)
            {
                OpenFileDialog dlg = new OpenFileDialog() { Filter = "Все файлы (*.*)|*.*" };
                if (dlg.ShowDialog() == DialogResult.OK)
                    active.Open(dlg.FileName);
            }
        }

        private void ФайлСохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (ActiveMdiChild as EditorForm)?.Save();
        }

        private void ФайлСохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (ActiveMdiChild as EditorForm)?.SaveFileAs();
        }

        private void ФайлПечатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (ActiveMdiChild as EditorForm)?.Print();
        }

        private void ФайлЗакрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (ActiveMdiChild as EditorForm)?.Close();
        }

        private void ФайлЗакрытьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form child in MdiChildren)
                (child as EditorForm)?.Close();
        }

        private void ОкноКаскадомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void ОкноПлиткойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }
    }

    #endregion

    #region Задание 2: Текстовый редактор

    public class TextDocument : BaseDocument
    {
        private string content = "";

        public override void CreateNew() => content = "";
        public override void Open(string filePath) => content = File.ReadAllText(filePath);
        public override void Save() => File.WriteAllText(FileName, content);
        public override void SaveAs(string filePath) 
        { 
            File.WriteAllText(filePath, content); 
            FileName = filePath; 
        }
        public override void Print()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) => e.Graphics.DrawString(content, new Font("Arial", 10), Brushes.Black, e.MarginBounds);
            pd.Print();
        }
        public override void Close() { }
        internal string Content => content;
        internal void SetContent(string newContent) { content = newContent; IsModified = true; }
    }

    public class TextEditorForm : EditorForm
    {
        private RichTextBox richTextBox;

        public TextEditorForm(IDocument doc) : base("Текстовый редактор", doc)
        {
            richTextBox = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 10) };
            Controls.Add(richTextBox);
            richTextBox.TextChanged += (s, e) => 
            {
                ((TextDocument)document).SetContent(richTextBox.Text);
                UpdateTitle();
            };
        }

        protected override void OnContentChanged() => richTextBox.Text = ((TextDocument)document).Content;
        protected override void OnSaveContent() => ((TextDocument)document).SetContent(richTextBox.Text);
    }

    #endregion

    #region Задание 3: Графический редактор

    public class ImageDocument : BaseDocument
    {
        private Image image;

        public override void CreateNew() => image = new Bitmap(800, 600);
        public override void Open(string filePath)
        {
            image?.Dispose();
            image = Image.FromFile(filePath);
            FileName = filePath;
        }
        public override void Save() => SaveImage(image, FileName);
        public override void SaveAs(string filePath) 
        { 
            SaveImage(image, filePath); 
            FileName = filePath; 
        }
        public override void Print()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) => e.Graphics.DrawImage(image, e.MarginBounds);
            pd.Print();
        }
        public override void Close() => image?.Dispose();
        internal Image Image => image;
        internal void SetImage(Image newImage) 
        { 
            image?.Dispose(); 
            image = newImage; 
            IsModified = true; 
        }

        private static void SaveImage(Image img, string path)
        {
            ImageFormat format = GetImageFormat(path);
            img.Save(path, format);
        }

        private static ImageFormat GetImageFormat(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext switch
            {
                ".png" => ImageFormat.Png,
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                _ => ImageFormat.Png
            };
        }
    }

    public class ImageEditorForm : EditorForm
    {
        private PictureBox pictureBox;
        private ToolStrip toolStrip;
        private ContextMenuStrip toolMenu;
        private Image tempImage;

        public ImageEditorForm(IDocument doc) : base("Графический редактор", doc)
        {
            pictureBox = new PictureBox 
            { 
                Dock = DockStyle.Fill, 
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(pictureBox);

            // Палитра инструментов
            toolStrip = new ToolStrip { Dock = DockStyle.Top };
            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripButton("✏️ Карандаш", null, DrawTool_Click),
                new ToolStripButton("🗑️ Очистить", null, ClearTool_Click),
                new ToolStripButton("🔄 Поворот", null, RotateTool_Click),
                new ToolStripButton("⚫ Инверсия", null, InvertTool_Click),
                new ToolStripSeparator(),
                new ToolStripComboBox("Формат", new[] { "PNG", "JPG", "BMP" })
            });
            Controls.Add(toolStrip);

            // Контекстное меню
            toolMenu = new ContextMenuStrip();
            toolMenu.Items.Add("Выделить область", null, SelectArea_Click);
            pictureBox.ContextMenuStrip = toolMenu;

            pictureBox.MouseClick += PictureBox_MouseClick;
        }

        private void DrawTool_Click(object sender, EventArgs e)
        {
            // Простая реализация рисования (можно расширить)
            using (Graphics g = Graphics.FromImage(((ImageDocument)document).Image))
            {
                g.FillEllipse(Brushes.Red, 50, 50, 20, 20);
                pictureBox.Invalidate();
            }
            OnContentChanged();
        }

        private void ClearTool_Click(object sender, EventArgs e)
        {
            ((ImageDocument)document).SetImage(new Bitmap(800, 600));
            OnContentChanged();
        }

        private void RotateTool_Click(object sender, EventArgs e)
        {
            ImageDocument imgDoc = (ImageDocument)document;
            tempImage = new Bitmap(imgDoc.Image.Width, imgDoc.Image.Height);
            using (Graphics g = Graphics.FromImage(tempImage))
            {
                g.TranslateTransform(imgDoc.Image.Width / 2f, imgDoc.Image.Height / 2f);
                g.RotateTransform(90);
                g.DrawImage(imgDoc.Image, -imgDoc.Image.Width / 2f, -imgDoc.Image.Height / 2f);
            }
            imgDoc.SetImage(tempImage);
            OnContentChanged();
        }

        private void InvertTool_Click(object sender, EventArgs e)
        {
            ImageDocument imgDoc = (ImageDocument)document;
            tempImage = new Bitmap(imgDoc.Image.Width, imgDoc.Image.Height);
            using (Graphics g = Graphics.FromImage(tempImage))
            {
                ImageAttributes attr = new ImageAttributes();
                attr.SetColorMatrix(new ColorMatrix(new float[][] {
                    new float[] {-1, 0, 0, 0, 1},
                    new float[] {0, -1, 0, 0, 1},
                    new float[] {0, 0, -1, 0, 1},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {1, 1, 1, 1, 1}
                }));
                g.DrawImage(imgDoc.Image, new Rectangle(0, 0, tempImage.Width, tempImage.Height),
                    0, 0, imgDoc.Image.Width, imgDoc.Image.Height, GraphicsUnit.Pixel, attr);
            }
            imgDoc.SetImage(tempImage);
            OnContentChanged();
        }

        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Простое рисование по клику
            using (Graphics g = Graphics.FromImage(((ImageDocument)document).Image))
            {
                g.FillEllipse(Brushes.Blue, e.X - 5, e.Y - 5, 10, 10);
            }
            OnContentChanged();
        }

        private void SelectArea_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Функция выделения области (реализация по заданию)");
        }

        public override void SaveAs(string filePath)
        {
            saveDialog.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|BMP (*.bmp)|*.bmp|GIF (*.gif)|*.gif";
            if (saveDialog.ShowDialog() == DialogResult.OK)
                base.SaveAs(saveDialog.FileName);
        }

        protected override void OnContentChanged()
        {
            pictureBox.Image?.Dispose();
            pictureBox.Image = new Bitmap(((ImageDocument)document).Image);
        }

        protected override void OnSaveContent() { /* Изображение уже обновлено */ }
    }

    #endregion

    // Генератор формы MainForm
    partial class MainForm
    {
        private MenuStrip menuStrip;
        private ToolStripMenuItem файлToolStripMenuItem;
        private ToolStripMenuItem открытьToolStripMenuItem;
        private ToolStripMenuItem сохранитьToolStripMenuItem;
        private ToolStripMenuItem сохранитьКакToolStripMenuItem;
        private ToolStripMenuItem печатьToolStripMenuItem;
        private ToolStripMenuItem закрытьToolStripMenuItem;
        private ToolStripMenuItem закрытьВсеToolStripMenuItem;
        private ToolStripMenuItem новыйToolStripMenuItem;
        private ToolStripMenuItem новыйТекстToolStripMenuItem;
        private ToolStripMenuItem новыйИзображениеToolStripMenuItem;
        private ToolStripMenuItem окноToolStripMenuItem;
        private ToolStripMenuItem каскадомToolStripMenuItem;
        private ToolStripMenuItem плиткойToolStripMenuItem;

        private void InitializeComponent()
        {
            // Создание меню
            menuStrip = new MenuStrip();
            
            файлToolStripMenuItem = new ToolStripMenuItem("Файл");
            новыйToolStripMenuItem = new ToolStripMenuItem("Новый");
            новыйТекстToolStripMenuItem = new ToolStripMenuItem("Текстовый");
            новыйТекстToolStripMenuItem.Tag = "text";
            новыйИзображениеToolStripMenuItem = new ToolStripMenuItem("Изображение");
            новыйИзображениеToolStripMenuItem.Tag = "image";
            открытьToolStripMenuItem = new ToolStripMenuItem("Открыть");
            сохранитьToolStripMenuItem = new ToolStripMenuItem("Сохранить");
            сохранитьКакToolStripMenuItem = new ToolStripMenuItem("Сохранить как...");
            печатьToolStripMenuItem = new ToolStripMenuItem("Печать");
            закрытьToolStripMenuItem = new ToolStripMenuItem("Закрыть");
            закрытьВсеToolStripMenuItem = new ToolStripMenuItem("Закрыть все");

            окноToolStripMenuItem = new ToolStripMenuItem("Окно");
            каскадомToolStripMenuItem = new ToolStripMenuItem("Каскадом");
            плиткойToolStripMenuItem = new ToolStripMenuItem("Плиткой");

            // События
            новыйТекстToolStripMenuItem.Click += СоздатьToolStripMenuItem_Click;
            новыйИзображениеToolStripMenuItem.Click += СоздатьToolStripMenuItem_Click;
            открытьToolStripMenuItem.Click += ФайлОткрытьToolStripMenuItem_Click;
            сохранитьToolStripMenuItem.Click += ФайлСохранитьToolStripMenuItem_Click;
            сохранитьКакToolStripMenuItem.Click += ФайлСохранитьКакToolStripMenuItem_Click;
            печатьToolStripMenuItem.Click += ФайлПечатьToolStripMenuItem_Click;
            закрытьToolStripMenuItem.Click += ФайлЗакрытьToolStripMenuItem_Click;
            закрытьВсеToolStripMenuItem.Click += ФайлЗакрытьВсеToolStripMenuItem_Click;
            каскадомToolStripMenuItem.Click += ОкноКаскадомToolStripMenuItem_Click;
            плиткойToolStripMenuItem.Click += ОкноПлиткойToolStripMenuItem_Click;

            // Иерархия меню
            новыйToolStripMenuItem.DropDownItems.AddRange(new[] { новыйТекстToolStripMenuItem, новыйИзображениеToolStripMenuItem });
            файлToolStripMenuItem.DropDownItems.AddRange(new[] { 
                новыйToolStripMenuItem, new ToolStripSeparator(),
                открытьToolStripMenuItem, сохранитьToolStripMenuItem, сохранитьКакToolStripMenuItem, 
                печатьToolStripMenuItem, закрытьToolStripMenuItem, закрытьВсеToolStripMenuItem 
            });
            окноToolStripMenuItem.DropDownItems.AddRange(new[] { каскадомToolStripMenuItem, плиткойToolStripMenuItem });

            menuStrip.Items.AddRange(new[] { файлToolStripMenuItem, окноToolStripMenuItem });
            Controls.Add(menuStrip);
        }
    }

    // Запуск приложения
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}