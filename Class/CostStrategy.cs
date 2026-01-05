using System;
using System.Collections.Generic;

// Интерфейс стратегии расчёта стоимости
public interface ICostStrategy
{
    double Calculate(double basePrice);
}

// Обычная стратегия (без скидки)
public class RegularCostStrategy : ICostStrategy
{
    public double Calculate(double basePrice) => basePrice;
}

// Стратегия со скидкой
public class DiscountCostStrategy : ICostStrategy
{
    private readonly double _discount;
	public double Discount { get; set; } // Свойство для хранения скидки


	public DiscountCostStrategy(double discount)
    {
        if (discount < 0 || discount > 100)
            throw new ArgumentException("Скидка должна быть в диапазоне [0..100].");
        _discount = discount / 100.0; // Переводим % в долю
    }

    public double Calculate(double basePrice) => basePrice * (1.0 - _discount);
}


