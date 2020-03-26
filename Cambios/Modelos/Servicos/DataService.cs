namespace Cambios.Modelos.Servicos
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.IO;

    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        private DialogService dialogService;

        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Rates.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source="+ path);
                connection.Open();

                string sqlcommand = "create table if not exists Rate(RateId int, Code varchar(5), TaxRate real, Name varchar(250))";

                command = new SQLiteCommand (sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
            }
        }

        public void SaveDate(List<Rate> Rates)
        {
            try
            {
                foreach (var rate in Rates)
                {
                    string sql = string.Format($"insert into Rate (RateId, Code, TaxRate, Name) values ({rate.RateId}, '{rate.Code}', '{rate.TaxRate}', '{rate.Name}')");

                    command = new SQLiteCommand(sql, connection);
                    command.ExecuteNonQuery();
                }

                connection.Close();

            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
            }
        }

        public List<Rate> GetData()
        {
            List<Rate> Rates = new List<Rate>();

            try
            {
                string sql = "select RateId, Code, TaxRate, Name from Rate";

                command = new SQLiteCommand(sql, connection);

                //Lê cada registo
                SQLiteDataReader cin = command.ExecuteReader();

                while (cin.Read())
                {
                    Rates.Add(new Rate
                    {
                        RateId = (int)cin["RateId"],
                        Code = (string)cin["Code"],
                        TaxRate = (double)cin["TaxRate"],
                        Name = (string)cin["Name"]
                    });
                }

                connection.Close();

                return Rates;
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
                return null;
            }
        }

        public void DeleteData()
        {
            try
            {
                string sql = "Delete from Rate";

                command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                dialogService.ShowMessage("Erro", ex.Message);
            }
        }
    }
}
