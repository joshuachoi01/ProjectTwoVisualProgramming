using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectTwo
{
    public partial class Form1 : Form
    {
        //lists instead of arrays for easier use
        List<string> itemNames = new List<string>();
        List<int> itemQuantities = new List<int>();
        List<string> itemCategories = new List<string>();

        string filePath = "grocery.txt"; //name of file

        public Form1()
        {
            InitializeComponent();
            LoadItemsFromFile();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnClearList_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Are you sure you want to clear the entire list?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                lstItems.Items.Clear();
                tbInput.Clear();
                itemNames.Clear();
                itemQuantities.Clear();
                itemCategories.Clear();

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(""); // Clear the file
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string input = tbInput.Text;

            if (TryParseInput(input, out string name, out int quantity, out string category))
            {
                itemNames.Add(name);
                itemQuantities.Add(quantity);
                itemCategories.Add(category);

                string formatted = $"{category} - {name} (Qty: {quantity})";
                lstItems.Items.Add(formatted);

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine($"{name},{quantity},{category}");
                }

                tbInput.Clear();
            }
            else
            {
                MessageBox.Show("Please enter item as: Name, Quantity, Category"); //Error Message
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstItems.SelectedIndex >= 0)
            {
                string selected = lstItems.SelectedItem.ToString();
                lstItems.Items.RemoveAt(lstItems.SelectedIndex);

                List<string> updatedLines = new List<string>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (FormatFromLine(line) != selected)
                        {
                            updatedLines.Add(line);
                        }
                    }
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (string line in updatedLines)
                    {
                        writer.WriteLine(line);
                    }
                }

                itemNames.Clear();
                itemQuantities.Clear();
                itemCategories.Clear();
                lstItems.Items.Clear();
                LoadItemsFromFile();
            }
            else
            {
                MessageBox.Show("Please select an item to remove.");
            }
        }

        //methods 
        private bool TryParseInput(string input, out string name, out int quantity, out string category) //tryparse and other text processing
        {
            name = "";
            quantity = 0;
            category = "";

            string[] parts = input.Split(','); //using split and trim to get items.
            if (parts.Length != 3) return false;

            name = parts[0].Trim();
            category = parts[2].Trim();

            return int.TryParse(parts[1].Trim(), out quantity);
        }
        private string FormatFromLine(string line) //formatting the line
        {
            if (TryParseInput(line, out string name, out int quantity, out string category))
            {
                return $"{category} - {name} (Qty: {quantity})";
            }
            return "";
        }
        private void LoadItemsFromFile() //loading the file
        {
            if (!File.Exists(filePath)) return;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (TryParseInput(line, out string name, out int quantity, out string category))
                    {
                        itemNames.Add(name);
                        itemQuantities.Add(quantity);
                        itemCategories.Add(category);
                        lstItems.Items.Add($"{category} - {name} (Qty: {quantity})");
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cmbItems_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.PerformClick(); // triggers the Add Item button click
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
