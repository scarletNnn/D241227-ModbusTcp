using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using NModbus;
using NModbus.Extensions.Enron;

namespace D241227;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private DataAccess _dataAccess;

    public MainWindow()
    {
        InitializeComponent();
        _dataAccess = new DataAccess();
        //InitializeModbus();
    }

    private void InitializeModbus()
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);

                // 读取从站1的寄存器地址100的值

                ushort[] registers = master.ReadHoldingRegisters(1, 0, 6); //读控制卡Modbus_Reg
                bool[] value = master.ReadCoils(1, 1, 1); //读控制卡Modbus_Bit
                ushort[] readInputRegisters = master.ReadInputRegisters(1, 1, 1);
                bool[] readInputs = master.ReadInputs(1, 1, 1); //读控制卡IN_1
                bool[] readInputs2 = master.ReadInputs(1, 10000, 5); //读控制卡OP_0开始的5个点

                master.WriteSingleCoil(1, 2, false); //写控制卡Modbus_Bit
                master.WriteSingleRegister(1, 2, 432); //写控制卡Modbus_Reg

                master.WriteMultipleCoils(1, 1, new bool[] { false, true, true }); //写入多个控制卡Modbus_Bit
                master.WriteMultipleRegisters(1, 1, [555, 666, 777]); //写入多个控制卡Modbus_Reg

                uint[] registers32 = master.ReadHoldingRegisters32(1, 2999, 10); //读控制卡Modbus_Long

                // 读取REG_1到REG_5的值, 并将无符号整数转换为有符号整数
                ushort[] registers3 = master.ReadHoldingRegisters(1, 100, 5);
                short signedValue1 = (short)registers3[0];
                short signedValue2 = (short)registers3[1];
                short signedValue3 = (short)registers3[2];
                short signedValue4 = (short)registers3[3];
                short signedValue5 = (short)registers3[4];

                // 写入负数到寄存器
                short valueToWrite = -4334;
                ushort unsignedValueToWrite = (ushort)valueToWrite; // 将有符号整数转换为无符号整数
                master.WriteSingleRegister(1, 100, unsignedValueToWrite);

                //32位无符号整数
                ushort[] register10 = master.ReadHoldingRegisters(1, 3000, 6);
                uint value10 = (uint)register10[0] + ((uint)register10[1] << 16); //将两个16位寄存器合并为一个32位寄存器
                //32位有符号整数
                ushort[] register11 = master.ReadHoldingRegisters(1, 3006, 6);
                int value11 = (int)register11[0] + ((int)register11[1] << 16); //将两个16位寄存器合并为一个32位寄存器

                //写入32位有符号整数
                int valueToWrite11 = 654321;
                ushort[] unsignedValueToWrite11 = new ushort[2];
                unsignedValueToWrite11[0] = (ushort)valueToWrite11;
                unsignedValueToWrite11[1] = (ushort)(valueToWrite11 >> 16);
                master.WriteMultipleRegisters(1, 3006, unsignedValueToWrite11);

                //读取浮点数
                ushort[] register12 = master.ReadHoldingRegisters(1, 3012, 2);
                float value12 = BitConverter.ToSingle(
                    BitConverter
                        .GetBytes(register12[0])
                        .Concat(BitConverter.GetBytes(register12[1]))
                        .ToArray(),
                    0
                );

                //写入浮点数
                float valueToWrite12 = 123.456f;
                ushort[] unsignedValueToWrite12 = BitConverter
                    .GetBytes(valueToWrite12)
                    .ToUshortArray();
                master.WriteMultipleRegisters(1, 3012, unsignedValueToWrite12);

                // 显示读取到的值
                MessageBox.Show($"registers: {registers[0]}, value:{value[0]}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Modbus通讯错误: {ex.Message}");
        }
    }

    //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    //{
    //    if (sender is TextBox textBox)
    //    {
    //        int index = MainViewModel.Instance.Data.IndexOf(0);
    //        if (index >= 0)
    //        {
    //            if (short.TryParse(textBox.Text, out short newValue))
    //            {
    //                MainViewModel.Instance.Data[index] = newValue;
    //                _dataAccess.WriteRegister(index, newValue);
    //            }
    //            else
    //            {
    //                MessageBox.Show("请输入有效的数字");
    //            }
    //        }
    //    }
    //}

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        FocusManager.SetFocusedElement(this, this);
        //Keyboard.ClearFocus();
    }
}

// Add this extension method to convert byte array to ushort array
public static class ByteArrayExtensions
{
    public static ushort[] ToUshortArray(this byte[] byteArray)
    {
        if (byteArray.Length % 2 != 0)
            throw new ArgumentException("Byte array length must be even.");

        ushort[] ushortArray = new ushort[byteArray.Length / 2];
        for (int i = 0; i < ushortArray.Length; i++)
        {
            ushortArray[i] = BitConverter.ToUInt16(byteArray, i * 2); //将两个字节合并为一个16位无符号整数
        }
        return ushortArray;
    }
}
