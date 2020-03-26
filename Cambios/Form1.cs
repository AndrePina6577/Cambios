namespace Cambios
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Windows.Forms;
    using Cambios.Modelos;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LoadRates();
        }

        private async void LoadRates()
        {
            //bool load;

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://cambiosrafa.azurewebsites.net");

            var response = await client.GetAsync("/api/Rates");

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show(response.ReasonPhrase);
                return;
            }

            var rates = JsonConvert.DeserializeObject<List<Rate>>(result);

            cbOrigem.DataSource = rates;
            cbOrigem.DisplayMember = "Name";

            //Correção bug microsoft de bindar as cb
            cbDestino.BindingContext = new BindingContext();

            cbDestino.DataSource = rates;
            cbDestino.DisplayMember = "Name";

            pgStatus.Value = 100;
        }
    }
}
