
// Класс тарифа (Контекст стратегии)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization; // Добавлено для атрибутов
public class Tariff
{
	public double BasePrice { get; set; }

	private readonly ICostStrategy _strategy;
	public string TariffType { get; set; } // "Regular" или "Discount"
	public double Discount { get; set; } // Для Discount тарифа

	// Свойство только для получения стоимости, не сериализуется
	[JsonIgnore]
	public double CostPerMinute
	{
		get
		{
			var strategy = GetStrategy();
			return strategy?.Calculate(BasePrice) ?? BasePrice;
		}
	}

	// Конструктор для десериализации
	public Tariff() {
		_strategy = GetStrategy();
	}

	// Конструктор для создания тарифа с конкретной стратегией
	public Tariff(double basePrice, ICostStrategy strategy)
	{
	 
		if (basePrice < 0)
			throw new ArgumentException("Цена не может быть отрицательной.");
		BasePrice = basePrice;
		TariffType = TariffType = strategy is RegularCostStrategy ? "Regular" : "Discount";
		_strategy = strategy;
	}

	// Вспомогательный метод для получения стратегии

	public double GetCostPerMinute() => _strategy.Calculate(BasePrice);
	private ICostStrategy GetStrategy()
	{
		ICostStrategy ret = null; // Инициализируем переменную

		switch (TariffType)
		{
			case "Regular": 
			ret = new RegularCostStrategy();
				break;
			case "Discount":
				ret = new DiscountCostStrategy(Discount);
				break;
			default:
				ret = new RegularCostStrategy();
				break;
		} 
		return ret;
	}
}
