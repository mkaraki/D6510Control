using System.IO.Ports;
using System.Text.Json;

namespace D6510Control
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var appdir = AppDomain.CurrentDomain.BaseDirectory;
			var configPath = Path.Combine(appdir, "config.json");

			Config? config = null;

			using (var sr = new StreamReader(configPath))
			{
				var json = sr.ReadToEnd();
				config = JsonSerializer.Deserialize<Config>(json);
			}

			if (config == null)
			{
				Console.Error.WriteLine("Failed to load config");
				Environment.Exit(1);
			}

			if (config.MonitorId < 0 || config.MonitorId > 99)
			{
				Console.Error.WriteLine("Invalid monitor ID");
				Environment.Exit(4);
			}

			if (args.Length == 0)
			{
				Console.Error.WriteLine("No command specified");
				Environment.Exit(2);
			}

			SerialPort port = new SerialPort(config.ComPort,
				115200, Parity.None, 8, StopBits.One);

			var serialMonitorId = config.MonitorId.ToString("D2");

			try
			{
				port.Open();

				switch (args[0])
				{
					case "standby":
						SendCmdAndCr(port, $"8{serialMonitorId}{(char)0x73}{(char)0x21}000");
						break;

					case "src-vga":
						SendCmdAndCr(port, $"8{serialMonitorId}{(char)0x73}{(char)0x22}000");
						break;

					case "src-hdmi":
						SendCmdAndCr(port, $"8{serialMonitorId}{(char)0x73}{(char)0x22}001");
						break;

					case "src-computer":
						SendCmdAndCr(port, $"8{serialMonitorId}{(char)0x73}{(char)0x22}002");
						break;

					case "src-dvi":
						SendCmdAndCr(port, $"8{serialMonitorId}{(char)0x73}{(char)0x22}003");
						break;

					case "src-dp":
						SendCmdAndCr(port, $"8{serialMonitorId}{(char)0x73}{(char)0x22}004");
						break;

					default:
						Console.Error.WriteLine("Unknown command");
						Environment.Exit(3);
						break;
				}
			}
			finally
			{
				if (port.IsOpen)
				{
					port.Close();
				}

				port.Dispose();
			}

		}

		private static void SendCmdAndCr(SerialPort port, string command)
		{
			port.Write(command + "\r");
			Console.WriteLine(command);
		}
	}
}
