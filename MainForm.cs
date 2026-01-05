using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_rab_4_cs
{
	public partial class MainForm : Form
	{
		private readonly ATC _atc= new ATC();
		private readonly DataGridView _dataGridView;
		public MainForm()
		{
			InitializeComponent();
			_dataGridView = new DataGridView
			{
				Height = 80,
				Width = 1000,
				AutoGenerateColumns = false,
				Top = 40
			};

			_dataGridView.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "ATCId",
				HeaderText = "ID",
				DataPropertyName = "ATCId",
				Visible = false // делаем колонку с ID невидимой, но доступной для получения значения
			});

			_dataGridView.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "ATCName",
				HeaderText = "Название",
				DataPropertyName = "ATCName"
			});

			_dataGridView.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "ATCDescription",
				HeaderText = "Описание ",
				DataPropertyName = "ATCDescription"
			});
			_dataGridView.CellClick += (s, e) =>
			{
				// Выделяем строку по одиночному клику
				if (e.RowIndex >= 0)
				{
					_dataGridView.ClearSelection();
					_dataGridView.Rows[e.RowIndex].Selected = true;
					_dataGridView.CurrentCell = _dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
				}
			};
			_dataGridView.CellDoubleClick += (s, e) =>
			{
				// Выделяем строку по одиночному клику
				if (e.RowIndex >= 0)
				{
					_dataGridView.ClearSelection();
					_dataGridView.Rows[e.RowIndex].Selected = true;
					_dataGridView.CurrentCell = _dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
					if (_dataGridView.SelectedRows.Count > 0)
					{
						var selectedRowIndex = _dataGridView.SelectedRows[0].Index;
						// Получаем ID из выделенной строки (предполагается, что в первой колонке у вас ID)
						var atcId = Convert.ToInt32( _dataGridView.Rows[selectedRowIndex].Cells[0].Value); // или нужный индекс колонки

						using (var ATCForm = new ATCForm(atcId)) // передаем ID в конструктор
						{
							ATCForm.ShowDialog();
						}
					}
					else
					{
						MessageBox.Show("Выберите строку для редактирования", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			};
 
			_dataGridView.ReadOnly=true;
			this.Controls.Add(_dataGridView);
			RefreshGrid();

			var btnAddATC = new Button { Text = "Добавить", Top = 10 };

			btnAddATC.Click += (s, e) =>
			{
				// Открытие второй формы как модальное окно
				using (var ATCForm = new ATCForm())
				{
					ATCForm.ShowDialog();
				}
			};

			this.Controls.AddRange(new Control[]
			{
				btnAddATC
			});

			this.Text = "АТС — Управление тарифами";
			this.Size = new System.Drawing.Size(1200, 600);
		}
		

		public void RefreshGrid()
		{
			var list = new List<ATCView>();
			int i = 1;
			foreach (var atc in ATC.Get())
			{

				list.Add(new ATCView
				{
					ATCId = atc.ATCId, 
					ATCName = atc.ATCName,
					ATCDescription = atc.ATCDescription
				});
				i++;
			}
			_dataGridView.DataSource = list;
		}
		private class ATCView
		{
			public int ATCId { get; set; }
			public string ATCName { get; set; }
			public string ATCDescription { get; set; }
			 
		}
	}

}
