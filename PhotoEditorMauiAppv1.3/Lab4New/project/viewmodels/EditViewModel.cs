using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Image = SixLabors.ImageSharp.Image;
using System.Globalization;
using System.Collections.ObjectModel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Lab4New
{
    public class EditViewModel : INotifyPropertyChanged
    {
        ImageSource myimage;
        Image<Rgba32> editimage;
        int x, y, width, height, countphotos;
        string withandheightcroped;
        int _width, _height;
        public bool Flag { get; set; } = false;
        List<Image> allimages = new();
        
        public ObservableCollection<string> FontSizeOptions { get; } = new() {"1", "2", "3", "4", "5", "6", "7","8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40" } ;
        public ObservableCollection<string> ColorOptions { get; } = new() { "Красный", "Розовый", "Фиолетовый", "Синий", "Бирюзовый", "Зеленый", "Желтый", "Оранжевый", "Коричневый", "Серый", "Белый", "Черный" };
        public Dictionary<string, (byte R, byte G, byte B)> ColorMap { get; } = new Dictionary<string, (byte R, byte G, byte B)>()
        {
            { "Красный", (255, 0, 0) }, { "Розовый", (255, 192, 203) },  
            { "Фиолетовый", (128, 0, 128) }, { "Синий", (0, 0, 255) },     
            { "Бирюзовый", (0, 128, 128) }, { "Зеленый", (0, 128, 0) }, 
            { "Желтый", (255, 255, 0) }, { "Оранжевый", (255, 165, 0) }, 
            { "Коричневый", (165, 42, 42) }, { "Серый", (128, 128, 128) },  
            { "Белый", (255, 255, 255) }, { "Черный", (0, 0, 0) }    
        };

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddPhotoCommand { get; set; }
        public ICommand AddPhotoFromCollageCommand { get; set; }
        public ICommand CreateCollageCommand { get; set; }
        public ICommand LightCommand { get; set; }
        public ICommand ContrastCommand { get; set; }
        public ICommand TurnCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand DeleteAllImagesCommand { get; set; }
        public ICommand SaveAddTextCommand { get; set; }
        public ICommand GetDataCommand { get; set; }
        public ICommand SaveSetPixelCommand { get; set; }
        public ICommand SaveCropCommand { get; set; }

        public EditViewModel()
        {
            CreateCollageCommand = new Command(CreateCollagePhoto);
            AddPhotoFromCollageCommand = new Command(AddPhotoFromCollage);
            AddPhotoCommand = new Command(AddPhoto);
            LightCommand = new Command(LightPhoto);
            ContrastCommand = new Command(ContrastPhoto);
            TurnCommand = new Command(TurnPhoto);
            SaveCropCommand = new Command(CropPhoto);
            SaveAddTextCommand = new Command(AddTextPhoto);
            SaveSetPixelCommand = new Command(SetPixelPhoto);
            DeleteCommand = new Command(DeleteP);
            DeleteAllImagesCommand = new Command(DeleteAllP);
            GetDataCommand = new Command(GetDataPhoto);
            UpdateWithAndHeightCroped();
        }

        private void UpdateWithAndHeightCroped()
        {
            WithAndHeightCroped = (ImageWidthText - Coord_X).ToString() + " " + (ImageHeightText - Coord_Y).ToString();
        }
        async void AddPhoto()
        {
            var result = await FilePicker.PickAsync(OptionsTypeFile());

            if (result != null)
            {
                var fullpath = result.FullPath; //путь к фото оригинал
                string directory = Path.GetDirectoryName(fullpath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullpath);
                string originalExtension = Path.GetExtension(fullpath);
                string newpath = Path.Combine(directory, fileNameWithoutExtension + "MyEditImage" + originalExtension);
                int i = 0;
                while (File.Exists(newpath))
                {
                    newpath = Path.Combine(directory, fileNameWithoutExtension + "MyEditImage" + i.ToString() + originalExtension);
                    i += 1;
                }

                File.Copy(fullpath, newpath, true);
                //работаем с копией
                MyImage = newpath;
                EditImage = PhotoEditor.LoadPhoto(newpath); //загрузили фото
                FullPath = newpath;
                OriginPath = fullpath;
                OriginImage = PhotoEditor.LoadPhoto(OriginPath);
                CountPhotos = 1;
            }
        }
        async void AddPhotoFromCollage()
        {
            var result = await FilePicker.PickAsync(OptionsTypeFile());
            if (result != null)
            {
                var fullpath = result.FullPath; //путь к фото оригинал
                Image new_image = PhotoEditor.LoadPhoto(fullpath);
                AllImages.Add(new_image);
                CountPhotos = AllImages.Count;
            }
        }
        private PickOptions OptionsTypeFile()
        {
            var imageFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "image/png" } },
                { DevicePlatform.iOS, new[] { "public.png" } },
                { DevicePlatform.WinUI, new[] { ".png" } },
                { DevicePlatform.MacCatalyst, new[] { "public.png" } }
            });
            var options = new PickOptions
            {
                PickerTitle = "Выберите изображение",
                FileTypes = imageFileType
            };
            return options;
        }
        async void CreateCollagePhoto()
        {
            if (Flag == false)
            {
                AllImages.Add(EditImage);
                Flag = true;
            }
            if (AllImages.Count <= 1)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Недостаточно фото", "OK");
                return;
            }
            
            Image collage = Collage.CreateCollage(AllImages);
            PhotoEditor.SavePhoto(OriginPath + "collage.png", collage);
            await App.Current.MainPage.DisplayAlert("Уведомление", "Коллаж создан", "OK");
            await App.Current.MainPage.Navigation.PopAsync();
        }
        async void LightPhoto()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Заголовок", "Введите уровень яркости от 0.0 до 5.0", initialValue: "", keyboard: Keyboard.Numeric);
            if (result == "" || result == null) return;
            if (float.TryParse(result, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float brightnessLevel))
            {
                if (brightnessLevel < 0 || brightnessLevel > 5) 
                {
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Уровень яркости должен быть от 0.0 до 5.0", "OK");
                    return;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверный формат числа. Используйте число с плавающей точкой.", "OK");
                return;
            }
            IFilter filter =  new Filter1();
            PhotoEditor.ApplyFilter(EditImage, filter, brightnessLevel);
            PhotoEditor.SavePhoto(FullPath, EditImage);
            MyImage = FullPath;
        }
        async void ContrastPhoto()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Заголовок", "Введите уровень оттенка от 0.0 до 5.0", initialValue: "", keyboard: Keyboard.Numeric);
            if (result == "" || result == null) return;
            if (float.TryParse(result, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float oLevel))
            {
                if (oLevel < 0 && oLevel > 5)
                {
                    await App.Current.MainPage.DisplayAlert("Ошибка", "Уровень оттенка должен быть от 0.0 до 5.0", "OK");
                    return;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверный формат числа. Используйте число с плавающей точкой.", "OK");
                return;
            }
            IFilter filter = new Filter2();
            PhotoEditor.ApplyFilter(EditImage, filter, oLevel);
            PhotoEditor.SavePhoto(FullPath, EditImage);
            MyImage = FullPath;
        }
        public void TurnPhoto()
        {
            int angle = 90;
            PhotoEditor.RotatePhoto(EditImage, angle);
            PhotoEditor.SavePhoto(FullPath, EditImage);
            MyImage = FullPath;
        }
        private void DeleteP()
        {
            File.Copy(OriginPath, FullPath, true);
            EditImage = PhotoEditor.LoadPhoto(FullPath);
            MyImage = FullPath;
        }
        private void DeleteAllP()
        {
            AllImages.Clear();
            CountPhotos = 0;
        }
        async void CropPhoto()
        {
            if (Coord_X < 0 || Coord_X > ImageWidthText || Coord_Y < 0 || Coord_Y > ImageHeightText)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверные координаты для обрезки", "OK");
                return;
            }
            WithAndHeightCroped = (ImageWidthText - Coord_X).ToString() + " " + (ImageHeightText - Coord_Y).ToString();

            if (Width > ImageWidthText - Coord_X || Width <= 0)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверные координаты для обрезки", "OK");
                return;
            }
            if (Height > ImageHeightText - Coord_Y || Height <= 0)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверные координаты для обрезки", "OK");
                return;
            }

            PhotoEditor.CropPhoto(EditImage, Coord_X, Coord_Y, Width, Height);
            PhotoEditor.SavePhoto(FullPath, EditImage);
            MyImage = FullPath;
            ClearFields();
            await App.Current.MainPage.DisplayAlert("Уведомление", "Изображение обрезано", "OK");
            await App.Current.MainPage.Navigation.PopAsync();
        }
        async void AddTextPhoto()
        {
            if (Coord_X < 0 || Coord_X > ImageWidthText || Coord_Y < 0 || Coord_Y > ImageHeightText)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверные координаты для добавления текста", "OK");
                return;
            }
            if (SizeText == null)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Выберите размер текста", "OK");
                return;
            }
            if (ColorText == null)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Выберите цвет текста", "OK");
                return;
            }
            if (Text == null || Text=="")
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Введите текст", "OK");
                return;
            }
            (byte R, byte G, byte B) = ColorMap[ColorText];
            PhotoEditor.AddTextToPhoto((Image<Rgba32>)EditImage, Text, Coord_X, Coord_Y, float.Parse(SizeText), new Rgba32(R, G, B));
            PhotoEditor.SavePhoto(FullPath, EditImage);
            MyImage = FullPath;
            ClearFields();
            await App.Current.MainPage.DisplayAlert("Уведомление", "Текст добавлен", "OK");
            await App.Current.MainPage.Navigation.PopAsync();
        }
        async public void SetPixelPhoto()
        {
            if (Coord_X < 0 || Coord_X > ImageWidthText || Coord_Y < 0 || Coord_Y > ImageHeightText)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Неверные координаты для изменения пикселя", "OK");
                return;
            }
            if (ColorText == null)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Выберите цвет пикселя", "OK");
                return;
            }
            (byte R, byte G, byte B) = ColorMap[ColorText];
            PhotoEditor.ChangePixelColor((Image<Rgba32>)EditImage, Coord_X, Coord_Y, new Rgba32(R, G, B));
            PhotoEditor.SavePhoto(FullPath, EditImage);
            MyImage = FullPath;
            ClearFields();
            await App.Current.MainPage.DisplayAlert("Уведомление", "Пиксель изменен", "OK");
            await App.Current.MainPage.Navigation.PopAsync();
        }
        private void GetDataPhoto()
        {
            ImageWidthText = EditImage.Width;
            ImageHeightText = EditImage.Height;
            WithAndHeight = ImageWidthText.ToString() + " " + ImageHeightText.ToString();
        }
        public bool IsPhotoLoaded { get => MyImage != null; }


        public void ClearFields()
        {
            Text = SizeText = null;
            Coord_X = Coord_Y = 0;
            Height = Width = 0;
        }
        public Image EditImage //изменяемое bitmap
        {
            get => editimage;
            set
            {
                editimage = (Image<Rgba32>)value;
                OnPropertyChanged();
            }
        }
        public List<Image> AllImages
        {
            get => allimages;
            set
            {
                allimages = value;
                OnPropertyChanged();
            }
        }
        public int CountPhotos
        {
            get => countphotos;
            set
            {
                countphotos = value;
                OnPropertyChanged();
            }
        }
        public ImageSource MyImage //для отображения
        {
            get => myimage;
            set
            {
                myimage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPhotoLoaded));
            }
        }
        public string WithAndHeightCroped
        {
            get => withandheightcroped;
            set
            {
                withandheightcroped = value;
                OnPropertyChanged();
            }
        }
        //для работы с сохранением
        public string FullPath{ get; set; } //путь копии
        public string OriginPath { get; set; } 
        public Image OriginImage { get; set; } 
        //для текста
        public string Text { get; set; }
        public string WithAndHeight { get; set; }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }
        public string SizeText { get; set; }
        public string ColorText { get; set; }
        public int Coord_X
        {
            get => x;
            set
            {
                x = value;
                UpdateWithAndHeightCroped();
                OnPropertyChanged();
            }
        }
        public int Coord_Y
        {
            get => y;
            set
            {
                y = value;
                UpdateWithAndHeightCroped();
                OnPropertyChanged();
            }
        }
        public int ImageWidthText
        {
            get => width;
            set
            {
                width = value;
                OnPropertyChanged();
            }
        }
        public int ImageHeightText
        {
            get => height;
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string ? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}