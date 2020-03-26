namespace Cambios
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Windows.Forms;
    using Cambios.Modelos;
    using Cambios.Modelos.Servicos;
    using System.Threading.Tasks;

    public partial class Form1 : Form
    {
        #region Atributos
        private NetworkService networkService;

        private ApiService apiService;
        #endregion
        public List<Rate> Rates { get; set; } = new List<Rate>();
        public Form1()
        {
            InitializeComponent();

            networkService = new NetworkService();
            apiService = new ApiService();
            LoadRates();
        }

        private async void LoadRates()
        {
            //bool load;

            lblResultado.Text = "A atualizar taxas...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                MessageBox.Show(connection.Message);
                return;
            }
            else
            {
                await LoadApiRates();
            }

            cbOrigem.DataSource = Rates;
            cbOrigem.DisplayMember = "Name";

            //Correção bug microsoft de bindar as cb
            cbDestino.BindingContext = new BindingContext();

            cbDestino.DataSource = Rates;
            cbDestino.DisplayMember = "Name";

            pgStatus.Value = 100;

            lblResultado.Text = "Taxas carregadas";
        }

        private async Task LoadApiRates()
        {
            var response = await apiService.GetRates("https://cambiosrafa.azurewebsites.net", "/api/Rates");

            Rates = (List<Rate>)response.Result;
        }
    }
}
