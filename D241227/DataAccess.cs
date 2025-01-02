using System.Net.Sockets;
using System.Windows;
using NModbus;

namespace D241227;

public class DataAccess
{
    public DataAccess()
    {
        Task task = new Task(() => InitializeModbus());
        task.Start();
    }

    void InitializeModbus()
    {
        try
        {
            while (true)
            {
                if (MainViewModel.Instance.IsGetFocus)
                {
                    using (TcpClient client = new TcpClient("127.0.0.1", 502))
                    {
                        var factory = new ModbusFactory();
                        var master = factory.CreateMaster(client);

                        //Modbus_Bit
                        bool[] value = master.ReadCoils(1, 0, 5);
                        for (int i = 0; i < value.Length; i++)
                        {
                            MainViewModel.Instance.DataBool[i] = value[i];
                        }

                        //Modbus_Reg
                        ushort[] registers3 = master.ReadHoldingRegisters(1, 100, 5);
                        for (int i = 0; i < registers3.Length; i++)
                        {
                            MainViewModel.Instance.Data[i] = ((short)registers3[i]).ToString();
                        }

                        //Modbus_Long
                        ushort[] register11 = master.ReadHoldingRegisters(1, 3000, 10);
                        for (int i = 0; i < register11.Length / 2; i++)
                        {
                            int value32 =
                                (int)register11[i * 2] + ((int)register11[i * 2 + 1] << 16);
                            MainViewModel.Instance.DataInt[i] = value32.ToString();
                        }

                        //Modbus_Float
                        ushort[] register12 = master.ReadHoldingRegisters(1, 6000, 10);
                        byte[] byteArray = register12.SelectMany(BitConverter.GetBytes).ToArray();
                        for (int i = 0; i < register12.Length / 2; i++)
                        {
                            float value32 = BitConverter.ToSingle(byteArray, i * 4);
                            MainViewModel.Instance.DataFloat[i] = value32.ToString();
                        }

                        Thread.Sleep(50);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    //Modbus_Reg
    public static void WriteRegister(int index, short value)
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);

                ushort unsignedValueToWrite = (ushort)value;
                master.WriteSingleRegister(1, (ushort)(100 + index), unsignedValueToWrite);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Modbus通讯错误: {ex.Message}");
        }
    }

    //Modbus_Long
    public static void WriteRegister(int index, int value)
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);

                ushort[] unsignedValueToWrite = new ushort[2];
                unsignedValueToWrite[0] = (ushort)value;
                unsignedValueToWrite[1] = (ushort)(value >> 16);
                master.WriteMultipleRegisters(1, (ushort)(3000 + index), unsignedValueToWrite);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Modbus通讯错误: {ex.Message}");
        }
    }

    //Modbus_Float
    public static void WriteRegister(int index, float value)
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);

                ushort[] unsignedValueToWrite = BitConverter.GetBytes(value).ToUshortArray();
                master.WriteMultipleRegisters(1, (ushort)(6000 + index), unsignedValueToWrite);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Modbus通讯错误: {ex.Message}");
        }
    }

    //Modbus_Bit
    public static void WriteRegister(int index, bool value)
    {
        try
        {
            using (TcpClient client = new TcpClient("127.0.0.1", 502))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);

                master.WriteSingleCoil(1, (ushort)(index), value);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Modbus通讯错误: {ex.Message}");
        }
    }
}
