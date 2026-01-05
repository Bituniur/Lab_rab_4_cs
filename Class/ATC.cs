using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json; // Добавьте этот using
using System.Text.Json.Serialization; // Добавлено для атрибутов

public class ATC
{
	public int ATCId { get; set; }
	public string ATCName { get; set; }
	public string ATCDescription { get; set; }


	public List<Tariff> Tariffs { get; set; } = new List<Tariff>();
	private static readonly List<ATC> _atcs = new List<ATC>(); // Статический список для всех АТС
	private static readonly string DataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DATA");
	private static readonly string FilePath = Path.Combine(DataDirectory, "atcs.json");

	// Статический конструктор для загрузки данных при запуске приложения
	static ATC()
	{
		LoadFromFile();
	}

	// Метод для добавления новой АТС в список и сохранения в файл
	public int Add(string atcName, string atcDescription, IEnumerable<Tariff> tariffs)
	{
		// Генерация ID (простой способ - использовать текущее количество + 1)
		// В реальных приложениях лучше использовать GUID или БД с автоинкрементом
		int newId = _atcs.Count > 0 ? _atcs.Max(a => a.ATCId) + 1 : 1;

		var newATC = new ATC
		{
			ATCId = newId,
			ATCName = atcName,
			ATCDescription = atcDescription,
			Tariffs = tariffs.ToList()
		};

		_atcs.Add(newATC);
		SaveToFile();
		return newId;
	}

	public void AddRegularTariff(double price)
	{
		Tariffs.Add(new Tariff(price, new RegularCostStrategy()));
	}

	public void AddDiscountTariff(double price, double discount)
	{
		Tariffs.Add(new Tariff(price, new DiscountCostStrategy(discount)));
	}

	public IEnumerable<Tariff> GetTariffs() => Tariffs;

	// Метод для получения списка всех АТС
	public static IEnumerable<ATC> Get(int ATCid=0)
	{
		var ret = new List<ATC>();
		ret= _atcs.AsReadOnly().ToList();
		if( ATCid > 0 )
		{
			ret = ret.Where(a=>a.ATCId == ATCid).ToList();
		}

		return ret; // Возвращаем копию списка для безопасности
	}

	public double CalcAverageCost()
	{
		if (Tariffs.Count == 0)
			throw new InvalidOperationException("Нет тарифов для расчёта.");

		double sum = 0;
		foreach (var tariff in Tariffs)
			sum += tariff.GetCostPerMinute();

		return sum / Tariffs.Count;
	}

	// Приватный метод для сохранения списка АТС в файл
	private static void SaveToFile()
	{
		try
		{
			// Создаём папку, если её нет
			if (!Directory.Exists(DataDirectory))
			{
				Directory.CreateDirectory(DataDirectory);
			}

			// Сериализуем список в JSON
			string jsonString = JsonSerializer.Serialize(_atcs, new JsonSerializerOptions { WriteIndented = true });

			// Записываем в файл
			File.WriteAllText(FilePath, jsonString);
		}
		catch (Exception ex)
		{
			// Логирование ошибки. В реальном приложении используйте логгер (NLog, Serilog и т.д.)
			Console.WriteLine($"Ошибка при сохранении в файл: {ex.Message}");
			// Или MessageBox.Show($"Ошибка при сохранении в файл: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	// Приватный метод для загрузки списка АТС из файла
	private static void LoadFromFile()
	{
		try
		{
			if (File.Exists(FilePath))
			{
				string jsonString = File.ReadAllText(FilePath);

				// Десериализуем JSON обратно в список
				var loadedAtcs = JsonSerializer.Deserialize<List<ATC>>(jsonString);

				if (loadedAtcs != null)
				{
					_atcs.AddRange(loadedAtcs);
				}
			}
			else
			{
				// Если файл не существует, создадим пустой список (он уже пустой при инициализации)
				// и создадим папку DATA, если её нет.
				if (!Directory.Exists(DataDirectory))
				{
					Directory.CreateDirectory(DataDirectory);
				}
			}
		}
		catch (Exception ex)
		{
			// Логирование ошибки
			Console.WriteLine($"Ошибка при загрузке из файла: {ex.Message}");
			// Или MessageBox.Show($"Ошибка при загрузке из файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			// В случае ошибки, _atcs останется пустым или с тем, что успело загрузиться до ошибки
		}
	}
	// Метод для добавления тарифа к конкретной АТС (по ID)
	public static void AddRegularTariffToATC(int atcId, double price)
	{
		var atc = _atcs.FirstOrDefault(a => a.ATCId == atcId);
		if (atc != null)
		{
			atc.Tariffs.Add(new Tariff(price, new RegularCostStrategy()));
			SaveToFile(); // Сохраняем при изменении
		}
		else
		{
			throw new InvalidOperationException($"АТС с ID {atcId} не найдена.");
		}
	}

	public static void AddDiscountTariffToATC(int atcId, double price, double discount)
	{
		var atc = _atcs.FirstOrDefault(a => a.ATCId == atcId);
		if (atc != null)
		{
			atc.Tariffs.Add(new Tariff(price, new DiscountCostStrategy(discount)));
			SaveToFile(); // Сохраняем при изменении
		}
		else
		{
			throw new InvalidOperationException($"АТС с ID {atcId} не найдена.");
		}
	}
}