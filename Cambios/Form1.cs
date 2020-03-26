namespace Cambios
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Cambios.Modelos;
    using Cambios.Modelos.Servicos;
    using System.Threading.Tasks;

    public partial class Form1 : Form
    {
        #region Atributos
        private List<Rate> Rates;

        private NetworkService networkService;

        private ApiService apiService;

        private DialogService dialogService;

        private DataService dataService;
        #endregion
        public Form1()
        {
            InitializeComponent();

            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();

            LoadRates();
        }

        private async void LoadRates()
        {
            bool load;

            lblResultado.Text = "A atualizar taxas...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccess)
            {
                LoadLocalrates();
                load = false;
            }
            else
            {
                await LoadApiRates();
                load = true;
            }

            if (Rates.Count == 0)
            {
                lblResultado.Text = "Não há ligação à Internet\ne não foram previamente carregadas as taxas de conversão\nTente mais tarde.";
                return;
            }

            cbOrigem.DataSource = Rates;
            cbOrigem.DisplayMember = "Name";

            //Correção bug microsoft de bindar as combo Boxes
            cbDestino.BindingContext = new BindingContext();

            cbDestino.DataSource = Rates;
            cbDestino.DisplayMember = "Name";

            btnConverter.Enabled = true;
            btnTroca.Enabled = true;

            lblResultado.Text = "Taxas atualizadas";

            if (load)
            {
                lblStatus.Text = string.Format($"Taxas carregadas da Internet em {DateTime.Now:F}");
            }
            else
            {
                lblStatus.Text = string.Format("Taxas carregadas da base de dados.");
            }

            pgStatus.Value = 100;   
        }

        private void LoadLocalrates()
        {
            Rates = dataService.GetData();
        }

        private async Task LoadApiRates()
        {
            var response = await apiService.GetRates("https://cambiosrafa.azurewebsites.net", "/api/Rates");

            Rates = (List<Rate>)response.Result;

            dataService.DeleteData();
            dataService.SaveDate(Rates);
        }

        private void btnConverter_Click(object sender, EventArgs e)
        {
            Converter();
        }

        private void Converter()
        {
            if (string.IsNullOrEmpty(tbValor.Text))
            {
                dialogService.ShowMessage("Erro", "Insira um valor a converter");
                return;
            }

            decimal valor;
            if (!decimal.TryParse(tbValor.Text, out valor))
            {
                dialogService.ShowMessage("Erro de conversão", "Valor terá que ser numérico");
                return;
            }

            if (cbOrigem.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem que escolher uma moeda a converter.");
                return;
            }

            if (cbDestino.SelectedItem == null)
            {
                dialogService.ShowMessage("Erro", "Tem que escolher uma moeda de destino para converter.");
                return;
            }

            var taxaOrigem = (Rate)cbOrigem.SelectedItem;
            var taxaDestino = (Rate)cbDestino.SelectedItem;

            var valorConvertido = valor / (decimal)taxaOrigem.TaxRate * (decimal)taxaDestino.TaxRate;

            lblResultado.Text = string.Format($"{taxaOrigem.Code} {valor:C2} = {taxaDestino.Code} {valorConvertido:C2}");
        }
        private void btnTroca_Click(object sender, EventArgs e)
        {
            Troca();
        }

        private void Troca()
        {
            var aux = cbOrigem.SelectedItem;

            cbOrigem.SelectedItem = cbDestino.SelectedItem;
            cbDestino.SelectedItem = aux;

            Converter();
        }
    }
}
