using QuestPDF.Infrastructure;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Invoice
{

    public partial class MainPage : Page
    {
        private List<InvoiceItem> _invoiceItems = new List<InvoiceItem>();
        private List<TextBox> _namaTextBoxes = new List<TextBox>();
        private List<TextBox> _niTextBoxes = new List<TextBox>();
        private DataTable _dataTable;
        private SQLiteConnection _connection;
        public MainPage()
        {
            InitializeComponent();
            InitializeDatabase();

            QuestPDF.Settings.License = LicenseType.Community; // Pindahkan inisialisasi license ke sini
            txtPembayaran.Items.Add("Fastpay");
            txtPembayaran.Items.Add("Bank");
            txtPembayaran.Items.Add("Dana");
            txtPembayaran.SelectedIndex = 0;
            txtStruk.Items.Add("Tiket Kereta Api");
            txtStruk.Items.Add("Tiket Pesawat");
            txtStruk.Items.Add("Tiket Bus");
            txtStruk.SelectedIndex = 0;
            txtTanggal.SelectedDate = DateTime.Now;
            txtTanggal.Text = DateTime.Today.ToString();
        }

        public class Penumpang
        {
            public string Nama { get; set; }
            public string NIK { get; set; }
            public string TempatDuduk { get; set; }
        }

        public class PenumpangBayi
        {
            public string Nama { get; set; }
        }
        public class InvoiceItem
        {
            public string Struk { get; set; }
            public string Pembayaran { get; set; }
            public String Tanggal { get; set; }
            public int Nomor { get; set; }
            public string NamaKereta { get; set; }
            public string Keberangkatan { get; set; }
            public string Tiba { get; set; }
            public int penumpangDewasa { get; set; }
            public int penumpangDewasaBayi { get; set; }
            public string Nama { get; set; }
            public int Nik { get; set; }
            public string Kursi { get; set; }
            public double satuanDewasa { get; set; }
            public double satuanBayi { get; set; }
            public double Diskon { get; set; }
            public double Admin { get; set; }
            public double hargaTotal { get; set; }
            public string kodePesan { get; set; }
            public List<Penumpang> PenumpangD { get; set; } = new List<Penumpang>();
            public List<PenumpangBayi> PenumpangB { get; set; } = new List<PenumpangBayi>();
        }


        private void InitializeDatabase()
        {
            _connection = new SQLiteConnection("Data Source=InvoiceDB.sqlite;Version=3;");
            if (!File.Exists("InvoiceDB.sqlite"))
            {

                SQLiteConnection.CreateFile("InvoiceDB.sqlite");
                using (var connection = new SQLiteConnection(_connection))
                {
                    connection.Open();
                    string sql = "CREATE TABLE Invoices (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " Struk TEXT, " +
                        "Pembayaran TEXT," +
                        " Tanggal TEXT," +
                        " Nomor INTEGER, " +
                        "namaKereta TEXT, " +
                        "Keberangkatan TEXT," +
                        " Tiba TEXT, " +
                        "penumpangDewasa INTEGER," +
                        " Satuan REAL," +
                        " Diskon REAL, " +
                        "Total REAL," +
                        " kodePesan TEXT)";
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                        
                    }
                }
            }

        }
        private void inisialDBPenumpang()
        {
            _connection = new SQLiteConnection("Data Source=InvoiceDB.sqlite;Version=3;");
            if (!File.Exists("InvoiceDB.sqlite"))
            {
                SQLiteConnection.CreateFile("InvoiceDB.sqlite");
                using (var connection = new SQLiteConnection(_connection))
                {
                    connection.Open();
                    string sql = "CREATE TABLE Invoices (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " Struk TEXT, " +
                        "Pembayaran TEXT," +
                        " Tanggal TEXT," +
                        " Nomor INTEGER, " +
                        "namaKereta TEXT, " +
                        "Keberangkatan TEXT," +
                        " Tiba TEXT, " +
                        "penumpangDewasa INTEGER," +
                        " Satuan REAL," +
                        " Diskon REAL, " +
                        "Total REAL," +
                        " kodePesan TEXT)";
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();

                    }
                }
            }
        }
        private void inisialDBPeunumpangBayi()
        {
            _connection = new SQLiteConnection("Data Source=InvoiceDB.sqlite;Version=3;");
            if (!File.Exists("InvoiceDB.sqlite"))
            {

                SQLiteConnection.CreateFile("InvoiceDB.sqlite");
                using (var connection = new SQLiteConnection(_connection))
                {
                    connection.Open();
                    string sql = "CREATE TABLE Invoices (Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " Struk TEXT, " +
                        "Pembayaran TEXT," +
                        " Tanggal TEXT," +
                        " Nomor INTEGER, " +
                        "namaKereta TEXT, " +
                        "Keberangkatan TEXT," +
                        " Tiba TEXT, " +
                        "penumpangDewasa INTEGER," +
                        " Satuan REAL," +
                        " Diskon REAL, " +
                        "Total REAL," +
                        " kodePesan TEXT)";
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();

                    }
                }
            }

        }

        private void bMaster_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validasi input
                if (string.IsNullOrEmpty(txtStruk.Text) ||
                    string.IsNullOrEmpty(txtPembayaran.Text) ||
                    string.IsNullOrEmpty(txtTanggal.Text) ||
                    string.IsNullOrEmpty(txtNo.Text) ||
                    string.IsNullOrEmpty(txtkereta.Text) ||
                    string.IsNullOrEmpty(txtBerangkat.Text) ||
                    string.IsNullOrEmpty(txtTiba.Text) ||
                    string.IsNullOrEmpty(txtJumlah.Text) ||
                    string.IsNullOrEmpty(txtSatuan.Text) ||
                    string.IsNullOrEmpty(txtDiskon.Text) ||
                    string.IsNullOrEmpty(txtAdmin.Text) ||
                    string.IsNullOrEmpty(txtTotal.Text) ||
                    string.IsNullOrEmpty(txtKode.Text))
                {
                    MessageBox.Show("Harap isi semua kolom!");
                    return;
                }

                // Tambahkan item ke list
                var item = new InvoiceItem
                {
                    Struk = txtStruk.Text,
                    Pembayaran = txtPembayaran.Text,
                    Tanggal = txtTanggal.Text,
                    Nomor = int.Parse(txtNo.Text),
                    NamaKereta = txtkereta.Text,
                    Keberangkatan = txtBerangkat.Text,
                    Tiba = txtTiba.Text,
                    penumpangDewasa = int.Parse(txtJumlah.Text),
                    satuanDewasa = double.Parse(txtSatuan.Text),
                    Diskon = double.Parse(txtDiskon.Text),
                    Admin = double.Parse(txtAdmin.Text),
                    hargaTotal = double.Parse(txtTotal.Text),
                    kodePesan = txtKode.Text,
                    PenumpangD = new List<Penumpang>(),
                    PenumpangB = new List<PenumpangBayi>()
                };
                for(int i = 0; i< _namaTextBoxes.Count; i++ )
                {
                    item.PenumpangD.Add(new Penumpang
                    {
                        Nama = _namaTextBoxes[i].Text,
                        NIK  = _namaTextBoxes[i *2 ].Text,
                        TempatDuduk = _namaTextBoxes[i *2 + 1 ].Text
                    });
                }
                if (dynamicFormContainer2 != null && dynamicFormContainer2.Children.Count > 0)
                {
                    var babyGrid = (Grid)dynamicFormContainer2.Children[0];
                    foreach (var child in babyGrid.Children)
                    {
                        if (child is TextBox txtNamaBayi && txtNamaBayi.Tag != null)
                        {
                            item.PenumpangB.Add(new PenumpangBayi
                            {
                                Nama = txtNamaBayi.Text
                            });
                        }
                    }
                }

                _invoiceItems.Add(item);
                RefreshDataGrid();
                SaveToDatabase(item);
                GeneratePDF(item);
                ClearInputFields();
                MessageBox.Show("Data berhasil disimpan dan PDF telah dibuat!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshDataGrid()
        {
            dgItems.ItemsSource = null;
            dgItems.ItemsSource = _invoiceItems;
        }

        private void ClearInputFields()
        {
            txtStruk.Text = "";
            txtPembayaran.Text = "";
            txtTanggal.Text = "";
            txtNo.Text = "";
            txtkereta.Text = "";
            txtBerangkat.Text = "";
            txtTiba.Text = "";
            txtJumlah.Text = "";
            txtSatuan.Text = "";
            txtDiskon.Text = "";
            txtTotal.Text = "";
            txtKode.Text = "";
        }

        private void SaveToDatabase(InvoiceItem item)
        {
            using (var connection = new SQLiteConnection("Data Source=InvoiceDB.sqlite;Version=3;"))
            {
                connection.Open();
                string sql = @"INSERT INTO Invoices 
                            (Struk, Pembayaran, Tanggal, Nomor, namaKereta, Keberangkatan, Tiba, 
                             penumpangDewasa, Satuan, Diskon, Total, kodePesan) 
                             VALUES 
                            (@Struk, @Pembayaran, @Tanggal, @Nomor, @namaKereta, @Keberangkatan, @Tiba, 
                             @penumpangDewasa, @satuanDewasa, @Diskon, @Total, @kodePesan)";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Struk", item.Struk);
                    command.Parameters.AddWithValue("@Pembayaran", item.Pembayaran);
                    command.Parameters.AddWithValue("@Tanggal", item.Tanggal);
                    command.Parameters.AddWithValue("@Nomor", item.Nomor);
                    command.Parameters.AddWithValue("@namaKereta", item.NamaKereta);
                    command.Parameters.AddWithValue("@Keberangkatan", item.Keberangkatan);
                    command.Parameters.AddWithValue("@Tiba", item.Tiba);
                    command.Parameters.AddWithValue("@penumpangDewasa", item.penumpangDewasa);
                    command.Parameters.AddWithValue("@satuanDewasa", item.satuanDewasa);
                    command.Parameters.AddWithValue("@Diskon", item.Diskon);
                    command.Parameters.AddWithValue("@Total", item.hargaTotal);
                    command.Parameters.AddWithValue("@kodePesan", item.kodePesan);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void GeneratePDF(InvoiceItem item)
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, $"Tiket_Kereta_{item.Nomor}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A5); // Ukuran lebih cocok untuk tiket
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Header dengan logo dan informasi perusahaan
                        page.Header()
                            .Column(col =>
                            {
                                col.Item().AlignCenter().Text("TIKET KERETA API").Bold().FontSize(14);
                                col.Item().AlignCenter().Text("CV. SUMBER SELAMAT").FontSize(10);
                                col.Item().PaddingBottom(5).LineHorizontal(1);
                            });

                        // Konten utama
                        page.Content()
                            .PaddingVertical(10)
                            .Column(column =>
                            {
                                // Informasi tiket
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Nomor Tiket:").SemiBold();
                                    row.RelativeItem().Text(item.Struk);
                                });

                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Tanggal:");
                                    row.RelativeItem().Text(item.Tanggal);
                                });

                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Kereta:");
                                    row.RelativeItem().Text(item.NamaKereta);
                                });

                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Keberangkatan:");
                                    row.RelativeItem().Text($"{item.Keberangkatan} → {item.Tiba}");
                                });

                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("penumpangDewasa:");
                                    row.RelativeItem().Text(item.penumpangDewasa.ToString());
                                });

                                // Garis pemisah
                                column.Item().PaddingVertical(5).LineHorizontal(0.5f);

                                // Detail pembayaran
                                column.Item().Text("Rincian Harga:").FontSize(11).Bold();
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Harga Satuan:");
                                    row.RelativeItem().Text(item.satuanDewasa.ToString("C"));
                                });
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Diskon:");
                                    row.RelativeItem().Text(item.Diskon.ToString("C"));
                                });
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Total:");
                                    row.RelativeItem().Text(item.hargaTotal.ToString("C")).Bold();
                                });

                                // Barcode (placeholder)
                                column.Item().PaddingTop(10).AlignCenter().Text($"kodePesan: {item.kodePesan}").FontFamily("Courier New");
                            });

                        // Footer
                        page.Footer()
                            .AlignCenter()
                            .Text("Terima kasih telah menggunakan layanan kami")
                            .FontSize(8);
                    });
                })
                .GeneratePdf(filePath);

                MessageBox.Show($"PDF berhasil disimpan di:\n{filePath}", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuat PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       

        private void bReport_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CrudPage());
        }
        private void bLogout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtJumlah_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Bersihkan form sebelumnya
            dynamicFormContainer.Children.Clear();
            _namaTextBoxes.Clear();
            _niTextBoxes.Clear();

            // Coba parsing jumlah penumpangDewasa
            if (int.TryParse(txtJumlah.Text, out int jumlahpenumpangDewasa) && jumlahpenumpangDewasa > 0)
            {
                // Grid utama untuk menampung header dan form
                var mainGrid = new Grid();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Kolom Nama
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(15) }); // Spacer
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Kolom NIK
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(15) }); // Spacer
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Kolom Tempat

                // Tambahkan header
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Baris header
                for (int i = 0; i < jumlahpenumpangDewasa; i++)
                {
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Baris data
                }

                // Header Nama
                var headerNama = new TextBlock
                {
                    Text = "Nama",
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Grid.SetRow(headerNama, 0);
                Grid.SetColumn(headerNama, 0);

                // Header NIK
                var headerNIK = new TextBlock
                {
                    Text = "NIK",
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Grid.SetRow(headerNIK, 0);
                Grid.SetColumn(headerNIK, 2);
                var headerTempat = new TextBlock
                {
                    Text = "Tempat Duduk",
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Grid.SetRow(headerTempat, 0);
                Grid.SetColumn(headerTempat, 4);

                mainGrid.Children.Add(headerNama);
                mainGrid.Children.Add(headerNIK);
                mainGrid.Children.Add(headerTempat);

                // Buat form dinamis sesuai jumlah penumpangDewasa
                for (int i = 1; i <= jumlahpenumpangDewasa; i++)
                {
                    // TextBox untuk Nama
                    var txtNama = new TextBox
                    {
                        Width = 100,
                        Height = 20,
                        Margin = new Thickness(0, 0, 0, 10),
                        Tag = i
                    };
                    Grid.SetRow(txtNama, i);
                    Grid.SetColumn(txtNama, 0);
                    _namaTextBoxes.Add(txtNama);

                    // TextBox untuk NIK
                    var txtNIK = new TextBox
                    {
                        Width = 100,
                        Height = 20,
                        Margin = new Thickness(0, 0, 0, 10),
                        Tag = i
                    };
                    Grid.SetRow(txtNIK, i);
                    Grid.SetColumn(txtNIK, 2);
                    _niTextBoxes.Add(txtNIK);

                    // TextBox untuk NIK
                    var txtTempat = new TextBox
                    {
                        Width = 100,
                        Height = 20,
                        Margin = new Thickness(0, 0, 0, 10),
                        Tag = i
                    };
                    Grid.SetRow(txtTempat, i);
                    Grid.SetColumn(txtTempat, 4);
                    _niTextBoxes.Add(txtTempat);

                    mainGrid.Children.Add(txtNama);
                    mainGrid.Children.Add(txtNIK);
                    mainGrid.Children.Add(txtTempat);
                }

                // Tambahkan ke container utama
                dynamicFormContainer.Children.Add(mainGrid);
            }
        }

        private void txtjumlahBayi_TextChanged(object sender, TextChangedEventArgs e)
        {
            dynamicFormContainer2.Children.Clear();
            _namaTextBoxes.Clear();

            // Coba parsing jumlah penumpangDewasa
            if (int.TryParse(txtjumlahBayi.Text, out int jumlahpenumpangBayi) && jumlahpenumpangBayi > 0)
            {
                // Grid utama untuk menampung header dan form
                var mainGrid = new Grid();
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Kolom Nama

                // Tambahkan header
                mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Baris header
                for (int i = 0; i < jumlahpenumpangBayi; i++)
                {
                    mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Baris data
                }

                var headerNama = new TextBlock
                {
                    Text = "Nama",
                    Margin = new Thickness(0, 0, 0, 5)
                };
                Grid.SetRow(headerNama, 0);
                Grid.SetColumn(headerNama, 0);

                for (int i = 1; i <= jumlahpenumpangBayi; i++)
                {
                    var txtNamaBayi = new TextBox
                    {
                        Width = 100,
                        Height = 20,
                        Margin = new Thickness(0, 0, 0, 10),
                        Tag = i
                    };
                    Grid.SetRow(txtNamaBayi, i);
                    Grid.SetColumn(txtNamaBayi, 0);
                    _namaTextBoxes.Add(txtNamaBayi);
                    mainGrid.Children.Add(txtNamaBayi);
                }
                dynamicFormContainer2.Children.Add(mainGrid);
            }

        }
    }
}
