using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    public ObservableCollection<bool> _DataBool = new();

    public MainViewModel()
    {
        Data = ["0", "0", "1", "0", "0", "0"];
        DataInt = ["0", "0", "0", "0", "0", "0"];
        DataFloat = ["1", "1", "1", "0", "0", "0"];
        DataBool = [false, false, false, false, false, false];
        Alarm = ["1报警", "2报警", "3报警", "4报警", "5报警", "6报警"];
        flag = [false, false, false, false, false, false];

        Task task = new Task(() => Show());
        task.Start();
    }

    List<string> Alarm;

    [ObservableProperty]
    bool _IsGetFocus = true;

    [ObservableProperty]
    string _Message;

    List<bool> flag;

    private void Show()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                if (Instance.DataBool[i] == true && flag[i] == false)
                {
                    Message = Message + "  " + Alarm[i];
                    flag[i] = true;
                }
                else if (Instance.DataBool[i] == false && flag[i] == true)
                {
                    Message = "";

                    for (int t = 0; t < flag.Count; t++)
                    {
                        flag[t] = false;
                    }
                }
            }
            Thread.Sleep(200);
        }
    }

    [RelayCommand]
    void Buttoncmd(string parameter)
    {
        Instance.IsGetFocus = false;
        int index = Convert.ToInt16(parameter);
        var value = !Instance.DataBool[index];
        DataAccess.WriteRegister(index, value);
        Instance.IsGetFocus = true;
    }

    [RelayCommand]
    void MessageChanged()
    {
        DataBool[0] = true;
        var s = Instance.DataBool;
    }
}
