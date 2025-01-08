using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace D241227;

public class TextBoxBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.GotFocus += AssociatedObject_GotFocus;
        AssociatedObject.LostFocus += AssociatedObject_LostFocus;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
        AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
    }

    private void AssociatedObject_LostFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        var index = Convert.ToInt16(textBox.Tag);
        if (index < 10)
        {
            var value = Convert.ToInt16(textBox.Text);
            DataAccess.WriteRegister(index, value);
        }
        else if (10 <= index && index < 20)
        {
            var indexInt = (index - 10) * 2;
            var value = Convert.ToInt32(textBox.Text);
            DataAccess.WriteRegister(indexInt, value);
        }
        else if (20 <= index && index < 30)
        {
            var indexInt = (index - 20) * 2;
            var value = Convert.ToSingle(textBox.Text);
            DataAccess.WriteRegister(indexInt, value);
        }

        //if (short.TryParse(textBox.Text, out short newValue))
        //{
        //    MainViewModel.Instance.Data[index] = newValue;
        //    DataAccess.WriteRegister(index, newValue);
        //}

        MainViewModel.Instance.IsGetFocus = true;
    }

    private void AssociatedObject_GotFocus(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.IsGetFocus = false;
    }
}
