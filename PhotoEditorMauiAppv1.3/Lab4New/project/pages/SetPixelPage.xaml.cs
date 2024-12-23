namespace Lab4New;

public partial class SetPixelPage : ContentPage
{
    private EditViewModel _editViewModel;
    public SetPixelPage(EditViewModel editViewModel)
    {
        InitializeComponent();
        _editViewModel = editViewModel;
        BindingContext = _editViewModel;
    }
    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue != null)
        {
            var entry = sender as Entry;

            // Оставляем только цифры
            string filteredText = new string(e.NewTextValue.Where(char.IsDigit).ToArray());

            // Если текст изменился после фильтрации, обновляем поле ввода
            if (e.NewTextValue != filteredText)
            {
                entry.Text = filteredText;
            }
        }
    }
}


