using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_rab_4_cs
{
	public partial class ATCForm : Form
	{
		private readonly int _atcId; // добавляем поле для хранения ID
		private ATC _atc = new ATC();
		private readonly DataGridView _dataGridView;
		private string _atcName = "";
		private string _atcDescription = "";
		public ATCForm(int atcId = 0) // по умолчанию 0 для новых АТС
		{
			_atcId = atcId;
			// Загружаем данные АТС если ID != 0
			if (_atcId != 0)
			{
				LoadATCData();
			}
			InitializeComponent();
			this.FormClosing += AddTariffForm_FormClosing;
			// TextBox'ы
			var txtATCName = new TextBox { Top = 10, Left = 120, Width = 100 };
			var txtDescription = new TextBox { Top = 50, Left = 120 , Width = 100 };

			this.Controls.AddRange(new Control[]
			{
				new Label { Text = "Название:", Top = 10, Left = 10, Width = 100 },
				txtATCName,
				new Label { Text = "Описание:", Top = 50, Left = 10, Width = 100 },
				txtDescription
			});
			

			_dataGridView = new DataGridView
			{
				Height = 80,
				Width = 1000,
				AutoGenerateColumns = false,
				Top = 100
			};

			_dataGridView.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "BasePrice",
				HeaderText = "Базовая цена",
				DataPropertyName = "BasePrice"
			});

			_dataGridView.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "CostPerMinute",
				HeaderText = "Стоимость (руб/мин)",
				DataPropertyName = "CostPerMinute"
			});

			_dataGridView.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Type",
				HeaderText = "Тип тарифа",
				DataPropertyName = "Type"
			});

			this.Controls.Add(_dataGridView);

			

			// TextBox'ы
			var txtPrice = new TextBox { Top = 210, Left = 10, Width = 100 };
			var txtDiscount = new TextBox { Top = 210, Left = 110, Width = 100 };
			// Настройка DataGridView

			// Кнопки
			var btnAddRegular = new Button { Text = "Обычный тариф", Top = 240, Left = 10 };
			var btnAddDiscount = new Button { Text = "Льготный тариф", Top = 240, Left = 90 };
			var btnShowAverage = new Button { Text = "Средняя стоимость", Top = 240, Left = 170 };

			btnAddRegular.Click += (s, e) =>
			{
				 

				if (double.TryParse(txtPrice.Text, out double price))
				{
					try
					{
						_atc.AddRegularTariff(price);
						RefreshGrid();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show("Введите корректную цену.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			};

			btnAddDiscount.Click += (s, e) =>
			{
			 
		 

				if (double.TryParse(txtPrice.Text, out double price) &&
					double.TryParse(txtDiscount.Text, out double discount))
				{
					try
					{
						_atc.AddDiscountTariff(price, discount);
						RefreshGrid();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					MessageBox.Show("Введите корректные цену и скидку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			};

			btnShowAverage.Click += (s, e) =>
			{
			 
				try
				{
					double avg = _atc.CalcAverageCost();
					MessageBox.Show($"Средняя стоимость: {avg:F2} руб/мин", "Результат");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			};





			var btnAddATC = new Button { Text = "Сохранить", Dock = DockStyle.Bottom };
			var btnCloseATC = new Button { Text = "Закрыть", Dock = DockStyle.Bottom };

			btnAddATC.Click += (s, e) =>
			{
				try
				{
					_atcName = txtATCName.Text;
					_atcDescription = txtDescription.Text;
					(new ATC()).Add(_atcName, _atcDescription, _atc.GetTariffs().ToList()); // Передаем список тарифов
					MessageBox.Show("АТС добавлена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

					this.Close();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			};
			btnCloseATC.Click += (s, e) =>
			{
				try
				{
					this.Close();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			};
			this.Controls.AddRange(new Control[]
			{
			btnAddRegular, btnAddDiscount, btnShowAverage,
			txtPrice, txtDiscount,
			new Label { Text = "Цена:", Top = 190, Left = 10 },
			new Label { Text = "Скидка (%):", Top = 190, Left = 110 },
			});

			this.Controls.Add(btnAddATC);
			this.Controls.Add(btnCloseATC);

			this.Text = "Редактирование АТС";


			txtATCName.Text = _atc.ATCName;
			txtDescription.Text = _atc.ATCDescription;
			RefreshGrid();
		}
		private void LoadATCData()
		{
			// Загружаем данные АТС по ID
			_atc = ATC.Get(_atcId).FirstOrDefault();
			//txtPrice.Text = _atc.ATCName;
			//RefreshGrid();
		}
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// DialogForm
			// 
			this.ClientSize = new System.Drawing.Size(500, 350);
			this.Name = "DialogForm";
			this.ResumeLayout(false);

		}
		private class TariffView
		{
			public double BasePrice { get; set; }
			public double CostPerMinute { get; set; }
			public string Type { get; set; }
		}
		private void RefreshGrid()
		{
			var list = new List<TariffView>();
			int i = 1;
			foreach (var tariff in _atc.Tariffs)
			{
				string type = "";
				if (String.IsNullOrEmpty(tariff.TariffType)) // берем если добавляем по типу 
				{
					type = tariff.GetType().Name.Contains("Regular") ? "Обычный" : "Льготный";
				}
				else // берем из файла по названию
				{

					type = tariff.TariffType.Contains("Regular") ? "Обычный" : "Льготный";
				}
				
				list.Add(new TariffView
				{
					BasePrice = tariff.BasePrice,
					CostPerMinute = tariff.GetCostPerMinute(),
					Type = type
				});
				i++;
			}
			_dataGridView.DataSource = list;
		}
		private void AddTariffForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Вызываем RefreshGrid у родительской формы
			(new MainForm()).RefreshGrid();
		}
	}
}