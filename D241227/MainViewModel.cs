using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace D241227;

public partial class MainViewModel : ObservableObject
{
    public static MainViewModel Instance { get; set; } = new();

    [ObservableProperty]
    ObservableCollection<string> _Data = new();

    [ObservableProperty]
    ObservableCollection<string> _DataInt = new();

    [ObservableProperty]
    ObservableCollection<string> _DataFloat = new();

    [ObservableProperty]
    ObservableCollection<bool> _DataBool = new();

    public MainViewModel()
    {
        Data = ["0", "0", "0", "0", "0", "0"];
        DataInt = ["0", "0", "0", "0", "0", "0"];
        DataFloat = ["0", "0", "0", "0", "0", "0"];
        DataBool = [false, false, false, false, false, false];
    }

    [ObservableProperty]
    bool _IsGetFocus = true;

    [RelayCommand]
    void Buttoncmd(string parameter)
    {
        Instance.IsGetFocus = false;
        int index = Convert.ToInt16(parameter);
        var value = !Instance.DataBool[index];
        DataAccess.WriteRegister(index, value);
        Instance.IsGetFocus = true;
    }
}
